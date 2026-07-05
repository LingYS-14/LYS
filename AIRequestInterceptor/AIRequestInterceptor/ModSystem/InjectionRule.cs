namespace AIRequestInterceptor.ModSystem
{
    /// <summary>
    /// 定义单条注入规则的数据结构
    /// </summary>
    public class InjectionRule
    {
        // 【新增修复点】：添加 Type 属性，解决项目其他代码调用 rule.Type 报错的问题！
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Match { get; set; } = string.Empty;
        public string Action { get; set; } = "Append";
        public string Position { get; set; } = "end";
        public string Content { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}