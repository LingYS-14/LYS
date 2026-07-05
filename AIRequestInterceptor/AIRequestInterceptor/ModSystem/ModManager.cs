using AIRequestInterceptor.ModSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AIRequestInterceptor.ModSystem
{
    /// <summary>
    /// 负责加载、管理、合并所有 Mod 配置
    /// </summary>
    public static class ModManager
    {
        // 当前激活的 Mod 配置（合并后的最终结果）
        public static ActiveModProfile ActiveProfile { get; private set; } = new ActiveModProfile();

        // 所有已加载的 Mod 文件
        public static List<ModInfo> AllMods { get; private set; } = new List<ModInfo>();

        // 冲突列表（用于 UI 显示）
        public static List<ModConflict> Conflicts { get; private set; } = new List<ModConflict>();

        /// <summary>
        /// 从 Mods 文件夹加载所有 Mod
        /// </summary>
        public static void LoadAllMods(string modsFolder = null)
        {
            // 1. 获取正确路径
            string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string folder = modsFolder ?? Path.Combine(exePath, "Mods");

            System.Diagnostics.Debug.WriteLine($"[ModManager] 尝试加载 Mod 文件夹: {folder}");

            if (!Directory.Exists(folder))
            {
                System.Diagnostics.Debug.WriteLine($"[ModManager] 文件夹不存在，创建: {folder}");
                Directory.CreateDirectory(folder);
                return;
            }

            // 2. 清空旧数据
            AllMods.Clear();
            Conflicts.Clear();

            // 3. 详细遍历并验证
            foreach (var file in Directory.GetFiles(folder, "*.json"))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"[ModManager] 尝试加载: {file}");

                    // 使用 UTF8 避免编码问题
                    string json = File.ReadAllText(file, Encoding.UTF8);

                    // 验证 JSON 格式
                    if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith("{"))
                    {
                        System.Diagnostics.Debug.WriteLine($"[ModManager] 跳过无效文件: {file} (内容为空或不是JSON)");
                        continue;
                    }

                    // 【修复点 1】：这里改成反序列化为 ModInfo ！！
                    ModInfo mod = Newtonsoft.Json.JsonConvert.DeserializeObject<ModInfo>(json);

                    if (mod != null && !string.IsNullOrEmpty(mod.Id))
                    {
                        AllMods.Add(mod);
                        System.Diagnostics.Debug.WriteLine($"[ModManager] 成功加载 Mod: {mod.Name} (ID: {mod.Id})");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ModManager] 跳过无效 Mod (ID为空): {file}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ModManager] 加载失败 {file}: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"[ModManager] 总共加载 {AllMods.Count} 个 Mod");

            // 【修复点 2】：删除了 CheckConflicts() 调用，避免找不到方法的报错
        }

        /// <summary>
        /// 检测已启用 Mod 之间的冲突
        /// </summary>
        public static void DetectConflicts()
        {
            Conflicts.Clear();

            // 按类型分组检测冲突
            var enabledMods = AllMods.Where(m => m.IsEnabled).ToList();

            // 检测 Parameter 冲突 (同一个 key 被多个 Mod 修改)
            var paramGroups = enabledMods
                .SelectMany(m => m.Injections
                    .Where(i => i.Type == "Parameter")
                    .Select(i => new { Mod = m, Key = i.Key }))
                .GroupBy(x => x.Key)
                .Where(g => g.Count() > 1);

            foreach (var group in paramGroups)
            {
                Conflicts.Add(new ModConflict
                {
                    ConflictType = "Parameter",
                    Target = group.Key,
                    ModNames = group.Select(x => x.Mod.Name).ToList(),
                    ResolutionOptions = group.Select(x => x.Mod.Name).ToList()
                });
            }

            // 检测 Database 冲突 (同一个 category 被多个 Mod 修改)
            var dbGroups = enabledMods
                .SelectMany(m => m.Injections
                    .Where(i => i.Type == "Database")
                    .Select(i => new { Mod = m, Category = i.Category }))
                .GroupBy(x => x.Category)
                .Where(g => g.Count() > 1);

            foreach (var group in dbGroups)
            {
                Conflicts.Add(new ModConflict
                {
                    ConflictType = "Database",
                    Target = group.Key,
                    ModNames = group.Select(x => x.Mod.Name).ToList(),
                    ResolutionOptions = group.Select(x => x.Mod.Name).ToList()
                });
            }
        }

        /// <summary>
        /// 生成最终合并配置
        /// </summary>
        public static void GenerateActiveProfile()
        {
            ActiveProfile = new ActiveModProfile();

            // 按优先级排序 (高优先级的 Mod 优先处理)
            var enabledMods = AllMods
                .Where(m => m.IsEnabled)
                .OrderByDescending(m => m.Priority)
                .ToList();

            foreach (var mod in enabledMods)
            {
                foreach (var injection in mod.Injections)
                {
                    switch (injection.Type)
                    {
                        case "SystemPromptOverride":
                            ActiveProfile.SystemPromptOverrides.Add(injection);
                            break;
                        case "Parameter":
                            ActiveProfile.Parameters[injection.Key] = injection.Value;
                            break;
                        case "Database":
                            ActiveProfile.DatabaseInjections.Add(injection);
                            break;
                        case "NpcPersona":
                            ActiveProfile.NpcPersonas.Add(injection);
                            break;
                        case "DeductionContext":
                            ActiveProfile.DeductionContexts.Add(injection);
                            break;
                        case "ToolInjection":
                            ActiveProfile.ToolInjections.Add(injection);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 保存 Mod 启用状态
        /// </summary>
        public static void SaveModStates()
        {
            // 实际项目中，可以将启用状态保存到配置文件
            // 例如: File.WriteAllText("mod_state.json", JsonConvert.SerializeObject(AllMods));
        }
    }

    /// <summary>
    /// 代表一个合并后的最终 Mod 配置
    /// </summary>
    public class ActiveModProfile
    {
        public List<InjectionRule> SystemPromptOverrides { get; set; } = new List<InjectionRule>();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public List<InjectionRule> DatabaseInjections { get; set; } = new List<InjectionRule>();
        public List<InjectionRule> NpcPersonas { get; set; } = new List<InjectionRule>();
        public List<InjectionRule> DeductionContexts { get; set; } = new List<InjectionRule>();
        public List<InjectionRule> ToolInjections { get; set; } = new List<InjectionRule>();
    }

    /// <summary>
    /// 代表一个 Mod 冲突
    /// </summary>
    public class ModConflict
    {
        public string ConflictType { get; set; } // "Parameter", "Database" 等
        public string Target { get; set; } // 冲突的目标 (如 "temperature", "装备")
        public List<string> ModNames { get; set; } = new List<string>();
        public List<string> ResolutionOptions { get; set; } = new List<string>();
    }
}