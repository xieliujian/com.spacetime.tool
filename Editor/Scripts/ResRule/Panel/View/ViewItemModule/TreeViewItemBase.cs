using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

using Object = UnityEngine.Object;

namespace ST.Tool
{
    internal abstract class TreeViewItemBase : TreeViewItem
    {
        public ResRuleData ruleData { get; private set; }

        public void RefreshRuleData<T>(ResRuleManagerAsset ruleManager) where T : AssetImporter
        {
            ruleData = GetRuleData<T>(ruleManager, displayName);
        }

        /// <summary>
        /// 获取规则名
        /// </summary>
        public string GetRuleName(out Color ruleNameColor)
        {
            string result;

            if (ruleData != null)
            {
                result = ruleData.ruleName;
                ruleNameColor = IsIdentical() ? Color.white : Color.red;
            }
            else
            {
                result = "Null";
                ruleNameColor = Color.red;
            }

            return result;
        }

        protected abstract bool IsIdentical();

        static long GetFileSize(string assetPath)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (obj == null)
            {
                return 0;
            }

            return Profiler.GetRuntimeMemorySizeLong(obj) / 2;
        }

        /// <summary>
        /// 获取规则信息
        /// </summary>
        static ResRuleData GetRuleData<T>(ResRuleManagerAsset ruleManager, string assetPath) where T : AssetImporter
        {
            var manage = ruleManager;

            if (manage == null)
            {
                manage = ResRuleManagerAsset.GetCurProjectResRuleManagerAsset();
            }

            if (manage == null)
            {
                return null;
            }

            return manage.GetRuleData(typeof(T), assetPath);
        }
    }
}
