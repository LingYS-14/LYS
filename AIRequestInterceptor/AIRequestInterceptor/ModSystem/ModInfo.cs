using System.Collections.Generic;

namespace AIRequestInterceptor.ModSystem
{
    public class ModInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string Author { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public int Priority { get; set; } = 0;

        // 【修复点 1】：把 Rules 改名为 Injections，与项目其他代码保持一致！
        public List<InjectionRule> Injections { get; set; } = new List<InjectionRule>();
        public override string ToString()
        {
            return Name; // 这里返回你要显示的名称
        }
    }
}