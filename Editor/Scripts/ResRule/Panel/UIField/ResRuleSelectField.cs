using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal class ResRuleSelectField
    {
        int m_Mask = -1;
        string[] m_OptionGUIContents;
        List<ResRuleData> m_DataAllList = new List<ResRuleData>();
        List<ResRuleData> m_MaskDataList = new List<ResRuleData>();

        /// <summary>
        /// 绘制规则选择
        /// </summary>
        public bool ShowGUI(ResRuleManagerAsset ruleMgr, Type importerType, out List<ResRuleData> maskDataList)
        {
            maskDataList = m_MaskDataList;
            var change = false;

            if (m_DataAllList.Count <= 0)
            {
                RefreshData(ruleMgr, importerType);
            }

            int newMaxk = 0;

            if (m_OptionGUIContents.Length == 0)
            {
                newMaxk = -1;
            }
            else
            {
                GUILayout.Label("Rule:");
                newMaxk = EditorGUILayout.MaskField(m_Mask, m_OptionGUIContents);
            }

            if (newMaxk != m_Mask)
            {
                m_Mask = newMaxk;
                change = true;
                RefreshMaskDataList();
            }

            return change;
        }

        /// <summary>
        /// 刷新所有规则管理数据
        /// </summary>
        public void RefreshData(ResRuleManagerAsset ruleMgr, Type importerType)
        {
            if (ruleMgr == null)
            {
                return;
            }

            ruleMgr.GetRuleDataList(importerType, m_DataAllList);
            m_OptionGUIContents = ToGUIContents(m_DataAllList);
        }

        /// <summary>
        /// 刷新选中数据
        /// </summary>
        void RefreshMaskDataList()
        {
            m_MaskDataList.Clear();

            for (int i = 0; i < m_DataAllList.Count; i++)
            {
                if (((m_Mask >> i) & 1) == 1)
                {
                    m_MaskDataList.Add(m_DataAllList[i]);
                }
            }
        }

        /// <summary>
        /// 获取UI显示用数据
        /// </summary>
        string[] ToGUIContents(List<ResRuleData> dataList)
        {
            if (dataList == null)
            {
                return null;
            }

            string[] result = new string[dataList.Count];

            for (int i = 0; i < dataList.Count; i++)
            {
                var data = dataList.Get(i);
                result[i] = data.ruleName;
            }

            return result;
        }
    }
}
