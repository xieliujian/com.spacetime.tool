using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ResRulePanel
    {
        #region 执行规则
        [MenuItem("CONTEXT/RuleDataMenu/执行规则", false, 0)]
        static void OnExecuteRule(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel != null && resRulePanel.m_RuleMgrAsset != null)
            {
                var ruleIndex = item.userData;
                resRulePanel.m_RuleMgrAsset.ExecuteRuleAt(ruleIndex);
            }
        }

        [MenuItem("CONTEXT/RuleDataMenu/执行规则", true)]
        static bool OnExecuteRuleCheck(MenuCommand item)
        {
            return RuleIsExist(item);
        }
        #endregion

        #region 删除规则
        [MenuItem("CONTEXT/RuleDataMenu/删除规则", false, 40)]
        static void OnRemoveRule(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel == null || resRulePanel.m_RuleMgrAsset == null)
            {
                return;
            }

            var ruleIndex = item.userData;
            resRulePanel.m_RuleMgrAsset.RemoveRuleAt(ruleIndex);
            EditorUtility.SetDirty(resRulePanel.m_RuleMgrAsset);
        }

        [MenuItem("CONTEXT/RuleDataMenu/删除规则", true)]
        static bool OnRemoveRuleCheck(MenuCommand item)
        {
            return RuleIsExist(item);
        }
        #endregion

        #region 正则表达式帮助
        [MenuItem("CONTEXT/RuleDataMenu/正则表达式帮助", false, 60)]
        static void OnRegexHelper(MenuCommand item)
        {
            Application.OpenURL("https://www.runoob.com/csharp/csharp-regular-expressions.html");
        }
        #endregion

        #region 刷新统计信息
        [MenuItem("CONTEXT/RuleDataMenu/刷新统计信息", false, 1)]
        static void OnRefreshRuleInfo(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel == null || resRulePanel.m_SelectedView == null)
            {
                return;
            }

            var ruleIndex = item.userData;
            var ruleData = resRulePanel.GetRuleDataAt(ruleIndex);

            if (ruleData != null)
            {
                resRulePanel.m_SelectedView.SetInfo(resRulePanel.m_RuleMgrAsset, ruleData);
            }
        }

        [MenuItem("CONTEXT/RuleDataMenu/刷新统计信息", true)]
        static bool OnRefreshRuleInfoCheck(MenuCommand item)
        {
            return RuleIsExist(item);
        }
        #endregion

        #region 复制当前规则
        [MenuItem("CONTEXT/RuleDataMenu/复制当前规则", false, 20)]
        static void OnNullFunction(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel == null || resRulePanel.m_SelectedView == null)
            {
                return;
            }

            var ruleIndex = item.userData;
            var ruleData = resRulePanel.GetRuleDataAt(ruleIndex);

            if (ruleData != null)
            {
                resRulePanel.m_RuleMgrAsset.CreateRuleData(resRulePanel.m_SelectedView.resImporterType, ruleData);
                EditorUtility.SetDirty(resRulePanel.m_RuleMgrAsset);
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("CONTEXT/RuleDataMenu/复制当前规则", true)]
        static bool OnNullFunctionCheck(MenuCommand item)
        {
            return RuleIsExist(item);
        }
        #endregion

        /// <summary>
        /// 规则是否存在
        /// </summary>
        static bool RuleIsExist(MenuCommand item)
        {
            if (item == null)
            {
                return false;
            }

            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel == null)
            {
                return false;
            }

            var ruleIndex = item.userData;
            return resRulePanel.GetRuleDataAt(ruleIndex) != null;
        }

        /// <summary>
        /// 获取规则数据
        /// </summary>
        ResRuleData GetRuleDataAt(int ruleIndex)
        {
            if (m_RuleMgrAsset == null)
            {
                return null;
            }

            return m_RuleMgrAsset.ruleDataList.Get(ruleIndex);
        }
    }
}
