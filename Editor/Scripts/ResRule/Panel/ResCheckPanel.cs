using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ResCheckPanel : EditorWindow
    {
        internal static ResCheckPanel GetWindow()
        {
            var window = GetWindow<ResCheckPanel>();
            window.titleContent = new GUIContent("Resources Check Panel");
            window.Focus();
            window.Repaint();
            return window;
        }

        bool isRefreshView = false;

        ResRuleMgrSelectField m_RuleMgrSelectField = new ResRuleMgrSelectField();
        ResRuleSelectField m_ResRuleDataSelectField = new ResRuleSelectField();
        ResTypeSelectField m_ResTypeSelectField = new ResTypeSelectField();

        IScrollView m_SelectedView;
        ResRuleManagerAsset m_RuleMgrAsset;
        List<ResRuleData> m_MaskRuleDataList;

        void OnGUI()
        {
            ShowToolbar();
            ShowView();
        }

        /// <summary>
        /// 绘制工具栏
        /// </summary>
        void ShowToolbar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                ShowResRuleManagerSelectGUI();
                DrawResTypeGUI();
                ShowResRuleSelectGUI();
                GUILayout.FlexibleSpace();
                DrawSearchFieldGUI();

                TryRefreshView();
            }
        }

        /// <summary>
        /// 绘制数据显示
        /// </summary>
        void ShowView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, this.position.height, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            rect.height -= rect.y;
            m_SelectedView.OnGUI(rect);
        }

        /// <summary>
        /// 尝试刷新数据显示（更新筛选结果）
        /// </summary>
        void TryRefreshView()
        {
            if (!isRefreshView)
            {
                return;
            }

            isRefreshView = false;
            m_ResRuleDataSelectField.RefreshData(m_RuleMgrAsset, m_SelectedView.resImporterType);

            //HashSet<string> includeAssetPaths = GetAssetPathSet("H:\\seres\\trunk\\client\\output\\default\\data\\standalone\\builds_scene.txt");
            HashSet<string> includeAssetPaths = null;

			// 没有规则显示所有数据
            if (m_MaskRuleDataList == null || m_MaskRuleDataList.Count == 0)
            {
                m_SelectedView.SetInfo(m_RuleMgrAsset, (ResRuleData)null, includeAssetPaths);
            }
            else
            {
                m_SelectedView.SetInfo(m_RuleMgrAsset, m_MaskRuleDataList, includeAssetPaths);
            }
        }

        /// <summary>
        /// 绘制规则管理选择
        /// </summary>
        void ShowResRuleManagerSelectGUI()
        {
            var newMgr = m_RuleMgrSelectField.ShowGUI();

            if (m_RuleMgrAsset != newMgr)
            {
                m_RuleMgrAsset = newMgr;
                isRefreshView = true;
            }
        }

        /// <summary>
        /// 绘制资源类型选择GUI
        /// </summary>
        void DrawResTypeGUI()
        {
            var newView = m_ResTypeSelectField.ShowGUI();

            if (newView != m_SelectedView || m_SelectedView == null)
            {
                m_SelectedView = newView;
                isRefreshView = true;
            }
        }

        /// <summary>
        /// 绘制规则选择
        /// </summary>
        void ShowResRuleSelectGUI()
        {
            if (m_ResRuleDataSelectField.ShowGUI(m_RuleMgrAsset, m_SelectedView.resImporterType, out List<ResRuleData> newDataList))
            {
                m_MaskRuleDataList = newDataList;
                isRefreshView = true;
            }

            if (GUILayout.Button("Refresh"))
            {
                isRefreshView = true;
            }
        }

        /// <summary>
        /// 绘制搜索框
        /// </summary>
        void DrawSearchFieldGUI()
        {
            m_ResTypeSelectField.ShowSearchFieldGUI();
        }

        static HashSet<string> GetAssetPathSet(string buildTextPath)
        {
            HashSet<string> assetPathSet = new HashSet<string>();

            foreach (var line in File.ReadAllLines(buildTextPath))
            {
                if (line.StartsWith("\t"))
                {
                    string assetPath = line.Substring(1, line.Length - 1);
                    assetPathSet.Add(assetPath);
                }
            }

            return assetPathSet;
        }
    }
}
