using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ST.Tool
{
    /// <summary>
    /// 资源工具编辑器 GUI 辅助（从 LRGUI 迁移，去 LR 前缀）。
    /// </summary>
    public static class ResEditorGUI
    {
        /// <summary>绘制可折叠按钮，展开/收起状态持久化到 EditorPrefs。</summary>
        public static bool TitleToggleBtn(string label, string key = "TitleToggleBtn")
        {
            bool state = EditorPrefs.GetBool(key, false);
            string titleText = string.Format("{0} {1}", state ? " \u25BC" : " \u25BA", label);
            if (GUILayout.Button(titleText, "IN TitleText")) state = !state;
            if (GUI.changed) EditorPrefs.SetBool(key, state);
            return state;
        }

        /// <summary>绘制带前缀的可折叠按钮。</summary>
        public static bool TitleToggleBtn(string label, string prefixStr, string key = "TitleToggleBtn")
        {
            bool state = EditorPrefs.GetBool(key, false);
            string titleText = string.Format("{0}{1} {2}", prefixStr, state ? " \u25BC" : " \u25BA", label);
            if (GUILayout.Button(titleText, "IN TitleText")) state = !state;
            if (GUI.changed) EditorPrefs.SetBool(key, state);
            return state;
        }

        /// <summary>将 UnityEngine.Object 列表转为 GUIContent 数组（使用 .name 字段）。</summary>
        public static GUIContent[] ToGUIContents<T>(this IList<T> objectList) where T : Object
        {
            if (objectList == null || objectList.Count <= 0) return null;
            var guiContents = new GUIContent[objectList.Count];
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i] != null)
                    guiContents[i] = new GUIContent(objectList[i].name);
            }
            return guiContents;
        }
    }
}
