using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(ScenePlayerRoam))]
    public class ScenePlayerRoamEditor : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            GUILayout.Label("_______________________________________ע��_____________________________________________");
            GUILayout.Label("���� Ctrl + L��������������������棬�ٴΰ��� Ctrl + L �������Լ������������");
            GUILayout.Label("________________________________________________________________________________________");

            base.OnInspectorGUI();
        }
    }
}


