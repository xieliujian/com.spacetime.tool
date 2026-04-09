using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal class ResRuleMgrSelectField //: ScriptableSingleton<ResRuleManagerAssetUtility>
    {
        int m_SelectedIndex = -1;
        GUIContent[] m_OptionGUIContents;
        List<ResRuleManagerAsset> m_RuleMgrAssetAll;

        List<ResRuleManagerAsset> dataList
        {
            get
            {
                if (m_RuleMgrAssetAll == null)
                {
                    m_RuleMgrAssetAll = new List<ResRuleManagerAsset>();
                }

                return m_RuleMgrAssetAll;
            }
        }

        /// <summary>
        /// 获取当前的规则管理器
        /// </summary>
        public ResRuleManagerAsset GetActiveRuleMgr()
        {
            if (m_SelectedIndex < 0 || dataList == null || dataList.Count <= 0 || m_SelectedIndex >= dataList.Count)
            {
                return null;
            }

            return dataList[m_SelectedIndex];
        }

        /// <summary>
        /// 绘制资源管理选择UI
        /// </summary>
        public ResRuleManagerAsset ShowGUI()
        {
            if (dataList.Count <= 0)
            {
                RefreshData();
            }

            if (dataList.Count <= 0)
            {
                GUILayout.Label("Rule manager asset is not exist!  ", "ErrorLabel");
                return null;
            }

            GUILayout.Label("RuleMgr:");
            int newIndex = EditorGUILayout.Popup(m_SelectedIndex, m_OptionGUIContents);

            if (m_SelectedIndex != newIndex)
            {
                m_SelectedIndex = Mathf.Clamp(newIndex, 0, dataList.Count - 1);
            }

            return GetActiveRuleMgr();
        }

        /// <summary>
        /// 刷新所有规则管理数据
        /// </summary>
        public void RefreshData()
        {
            ResRuleManagerAsset.GetResRuleManagerAssetAll(dataList);
            m_OptionGUIContents = dataList.ToGUIContents();
            TryInitSelectedIndex();
        }

        /// <summary>
        /// 尝试初始化默认选择index
        /// </summary>
        void TryInitSelectedIndex()
        {
            // 已有选择时不再处理
            if (m_SelectedIndex >= 0 || dataList.Count <= 0)
            {
                return;
            }

            var projectName = EditorFileUtils.GetProjectName();

            for (int i = 0; i < dataList.Count; i++)
            {
                // 优先选择当前工程规则管理信息
                if (projectName == dataList[i].name)
                {
                    m_SelectedIndex = i;
                    return;
                }
            }

            m_SelectedIndex = 0;
        }
    }
}
