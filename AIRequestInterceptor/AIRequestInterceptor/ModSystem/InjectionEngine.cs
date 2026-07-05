using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRequestInterceptor.ModSystem;

namespace AIRequestInterceptor.ModSystem
{
    /// <summary>
    /// Mod 注入引擎：负责将激活的 Mod 配置应用到 AI 请求的 JSON 中
    /// </summary>
    public static class InjectionEngine
    {
        /// <summary>
        /// 应用 Mod 配置到请求体
        /// </summary>
        public static string ApplyModProfile(string originalJson, ActiveModProfile profile)
        {
            try
            {
                JObject root = JObject.Parse(originalJson);

                // 1. 注入 SystemPromptOverride (通杀所有请求)
                if (profile.SystemPromptOverrides.Count > 0 && root["system_prompt_override"] != null)
                {
                    string currentOverride = root["system_prompt_override"].ToString();
                    foreach (var rule in profile.SystemPromptOverrides)
                    {
                        if (rule.Action.Equals("Append", StringComparison.OrdinalIgnoreCase))
                        {
                            currentOverride += rule.Content;
                        }
                    }
                    root["system_prompt_override"] = currentOverride;
                }

                // 2. 注入 Parameter (修改 temperature 等顶层参数)
                if (profile.Parameters.Count > 0)
                {
                    foreach (var kvp in profile.Parameters)
                    {
                        string valStr = kvp.Value?.ToString() ?? "";  // ✅ 加上 .ToString()

                        // 🌟 核心修复：智能识别并转换数据类型 🌟
                        // 1. 尝试解析为整数 (如 max_tokens: 2048)
                        if (long.TryParse(valStr, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out long longVal))
                        {
                            root[kvp.Key] = longVal;
                        }
                        // 2. 尝试解析为浮点数 (如 temperature: 1.05, top_p: 0.9)
                        else if (double.TryParse(valStr, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out double doubleVal))
                        {
                            root[kvp.Key] = doubleVal;
                        }
                        // 3. 尝试解析为布尔值 (如 stream: true)
                        else if (bool.TryParse(valStr, out bool boolVal))
                        {
                            root[kvp.Key] = boolVal;
                        }
                        // 4. 如果都不是，则作为普通字符串处理 (如 model: "deepseek-chat")
                        else
                        {
                            root[kvp.Key] = valStr;
                        }
                    }
                }

                // 3. 注入 Messages 内容 (Database, NpcPersona, DeductionContext)
                if (root["messages"] is JArray messages)
                {
                    // 遍历所有 message
                    foreach (var msg in messages)
                    {
                        string role = msg["role"]?.ToString();
                        string content = msg["content"]?.ToString();

                        if (string.IsNullOrEmpty(content)) continue;

                        // 3.1 处理 system 消息 (注入 Database)
                        if (role == "system" && profile.DatabaseInjections.Count > 0)
                        {
                            content = ApplyDatabaseInjections(content, profile.DatabaseInjections);
                        }

                        // 3.2 处理 user 消息 (注入 NpcPersona 和 DeductionContext)
                        if (role == "user")
                        {
                            if (profile.NpcPersonas.Count > 0)
                            {
                                content = ApplyNpcPersonaInjections(content, profile.NpcPersonas);
                            }
                            if (profile.DeductionContexts.Count > 0)
                            {
                                content = ApplyDeductionContextInjections(content, profile.DeductionContexts);
                            }
                        }

                        msg["content"] = content;
                    }
                }

                return root.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InjectionEngine] 解析或注入失败: {ex.Message}");
                return originalJson; // 失败则返回原始 JSON，保证游戏不崩溃
            }
        }

        /// <summary>
        /// 处理 Database 注入 (针对聊天请求的 CSV 数据库)
        /// </summary>
        private static string ApplyDatabaseInjections(string content, List<InjectionRule> injections)
        {
            foreach (var rule in injections)
            {
                // 查找类似 "## 装备" 或 "# 装备" 的标题
                string headerPattern = $"## {rule.Category}";
                int headerIndex = content.IndexOf(headerPattern, StringComparison.OrdinalIgnoreCase);

                if (headerIndex == -1)
                {
                    headerPattern = $"# {rule.Category}";
                    headerIndex = content.IndexOf(headerPattern, StringComparison.OrdinalIgnoreCase);
                }

                if (headerIndex != -1)
                {
                    if (rule.Position == "start")
                    {
                        // 插入到标题下一行
                        int insertPos = content.IndexOf('\n', headerIndex) + 1;
                        if (insertPos > 0)
                            content = content.Insert(insertPos, rule.Content + "\n");
                    }
                    else // "end"
                    {
                        // 找到下一个 ## 标题的前面，或者字符串末尾
                        int nextHeaderIndex = content.IndexOf("\n## ", headerIndex + headerPattern.Length);
                        if (nextHeaderIndex == -1)
                        {
                            content += "\n" + rule.Content;
                        }
                        else
                        {
                            content = content.Insert(nextHeaderIndex, "\n" + rule.Content);
                        }
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// 处理 NpcPersona 注入 (匹配 NPC 名字并追加设定)
        /// </summary>
        private static string ApplyNpcPersonaInjections(string content, List<InjectionRule> injections)
        {
            foreach (var rule in injections)
            {
                if (!string.IsNullOrEmpty(rule.Match) && content.Contains(rule.Match))
                {
                    // 简单策略：直接在 user 消息末尾追加
                    // 高级策略可以寻找特定 NPC 区块的末尾，这里为了稳定性采用末尾追加
                    content += $"\n\n【{rule.Match} 的隐藏设定】{rule.Content}";
                }
            }
            return content;
        }

        /// <summary>
        /// 处理 DeductionContext 注入 (在推演/任务请求的 user 消息末尾追加)
        /// </summary>
        private static string ApplyDeductionContextInjections(string content, List<InjectionRule> injections)
        {
            StringBuilder sb = new StringBuilder(content);
            sb.Append("\n\n# Mod 额外世界设定 (系统强制注入)\n");
            foreach (var rule in injections)
            {
                sb.AppendLine(rule.Content);
            }
            return sb.ToString();
        }
    }
}