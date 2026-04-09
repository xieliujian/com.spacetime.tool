using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ResRulePanel : EditorWindow
    {
        internal static ResRulePanel GetWindow()
        {
            var window = GetWindow<ResRulePanel>();
            window.titleContent = new GUIContent("Resources Rule Panel");
            window.m_RuleAreaHeight = window.position.height / 3f;
            window.Focus();
            window.Repaint();
            return window;
        }

        ResTypeSelectField m_ResTypeSelectField = new ResTypeSelectField();
        IScrollView m_SelectedView;
        Style m_Style;

        void OnGUI()
        {
            if (m_Style == null)
            {
                m_Style = new Style();
            }

            DrawMenuGUI();
            DrawResTypeGUI();
            DrawRuleGUI();
            ShowViewGUI();
        }

        /// <summary>
        /// 绘制资源类型选择GUI
        /// </summary>
        void DrawResTypeGUI()
        {
            var newView = m_ResTypeSelectField.ShowGUI(true);

            if (newView != m_SelectedView || m_SelectedView == null)
            {
                m_SelectedView = newView;
            }
        }
    }
}
