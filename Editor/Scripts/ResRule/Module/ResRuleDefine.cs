namespace ST.Tool
{
    internal static class ResRuleDefine
    {
        /// <summary> 默认检查文件路径数组 </summary>
        public readonly static string[] CHECK_NORMAL_DIR_S = new string[] { CHECK_NORMAL_DIR };

        /// <summary> 默认检查文件路径 </summary>
        public const string CHECK_NORMAL_DIR = "Assets";

        /// <summary> 规则工具路径 </summary>
        public const string RULE_TOOL_DIR = "Assets/LingRen/Script/ResTools";

        /// <summary> 规则项目路径 </summary>
        public const string RULE_PROJECT_DIR = "Assets/LingRen/Script/ResTools/EditorConfig/Projects";

        /// <summary> 模板文件路径 </summary>
        public const string RULE_TEMPLATE_DIR = "Assets/LingRen/Script/ResTools/EditorConfig/ResTemplate";
    }
}
