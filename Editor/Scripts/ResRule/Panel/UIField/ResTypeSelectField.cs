using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal class ResTypeSelectField
    {
       GUIContent m_SearchContent = EditorGUIUtility.TrTextContent("Search", "SearchField");

        GUIContent[] m_OptionContents = new GUIContent[]
        {
            EditorGUIUtility.TrTextContent("Texture", "Texture"),
            EditorGUIUtility.TrTextContent("Model", "Model"),
            EditorGUIUtility.TrTextContent("Audio", "Audio"),
        };

        IScrollView[] m_OptionDatas;

        int m_SelectedIndex = 0;

        public IScrollView activeView { get; private set; }
        public SearchField searchField { get; private set; }

        /// <summary>
        /// 绘制资源类型选择GUI
        /// </summary>
        public IScrollView ShowGUI(bool tableBtnStyle = false)
        {
            if (searchField == null)
            {
                searchField = new SearchField();
            }

            if (m_OptionDatas == null)
            {
                m_OptionDatas = new IScrollView[]
                {
                    TextureScrollView.Get(),
                    ModelScrollView.Get(),
                    AudioScrollView.Get(),
                };
            }

            int newIndex = 0;

            // 绘制不同风格的选择方式
            if (tableBtnStyle)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                newIndex = GUILayout.Toolbar(m_SelectedIndex, m_OptionContents, EditorStyles.toolbarButton);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("ResType:");
                newIndex = EditorGUILayout.Popup(m_SelectedIndex, m_OptionContents);
            }

            // 设置事件和当前选择
            if (m_SelectedIndex != newIndex || activeView == null)
            {
                // 去除旧的事件
                if (activeView != null)
                {
                    searchField.downOrUpArrowKeyPressed -= activeView.SetFocusAndEnsureSelectedItem;
                }

                m_SelectedIndex = newIndex;
                int index = Mathf.Clamp(m_SelectedIndex, 0, m_OptionDatas.Length - 1);
                activeView = m_OptionDatas[index];

                // 添加新的事件
                if (activeView != null)
                {
                    searchField.downOrUpArrowKeyPressed -= activeView.SetFocusAndEnsureSelectedItem;
                    searchField.downOrUpArrowKeyPressed += activeView.SetFocusAndEnsureSelectedItem;
                }
            }

            return activeView;
        }

        /// <summary>
        /// 绘制搜索框
        /// </summary>
        public void ShowSearchFieldGUI()
        {
            if (activeView != null && searchField != null)
            {
                Rect rect = GUILayoutUtility.GetRect(m_SearchContent, EditorStyles.label, GUILayout.MinWidth(200));
                activeView.searchString = searchField.OnGUI(rect, activeView.searchString);
            }
        }
    }
}
