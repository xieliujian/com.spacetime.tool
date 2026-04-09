using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ResRulePanel : EditorWindow
    {
        bool m_Dragging = false;
        float m_RuleAreaHeight;

        /// <summary>
        /// 绘制资源列表
        /// </summary>
        void ShowViewGUI()
        {
            DrawDragArea(this, ref m_Dragging, ref m_RuleAreaHeight);

            Rect rect = GUILayoutUtility.GetRect(0, this.position.height, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            rect.y = rect.y + 3;
            rect.height = this.position.height - rect.y;
            m_SelectedView.OnGUI(rect);
        }

        /// <summary>
        /// 绘制拖拽区域
        /// </summary>
        static void DrawDragArea(EditorWindow eWindow, ref bool dragging, ref float height)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y += rect.height;
            rect.height = 3;

            // 改变光标
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);

            var curEvent = Event.current;

            if (curEvent == null)
            {
                return;
            }

            switch (curEvent.rawType)
            {
                case EventType.MouseDown:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        dragging = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (dragging)
                    {
                        height += Event.current.delta.y;
                        eWindow.Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (dragging)
                    {
                        dragging = false;
                    }
                    break;
            }
        }
    }
}
