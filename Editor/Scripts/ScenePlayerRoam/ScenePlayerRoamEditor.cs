using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// <see cref="ScenePlayerRoam"/> 的自定义 Inspector：显示操作提示说明。
    /// </summary>
    [CustomEditor(typeof(ScenePlayerRoam))]
    public class ScenePlayerRoamEditor : Editor
    {
        /// <summary>
        /// 绘制提示文本并渲染默认 Inspector 字段。
        /// </summary>
        public override void OnInspectorGUI()
        {
            GUILayout.Label("_______________________________________注意_____________________________________________");
            GUILayout.Label("按下 Ctrl + L键，可以屏蔽摄像机跟随，再次按下 Ctrl + L 键，可以继续摄像机跟随");
            GUILayout.Label("________________________________________________________________________________________");

            base.OnInspectorGUI();
        }
    }
}


