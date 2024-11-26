using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class CopySceneEditor : EditorWindow
    {
        /// <summary>
        /// 
        /// </summary>
        [MenuItem("SpaceTime/Tool/复制场景", false, 300)]
        public static void OnOpen()
        {
            CopySceneEditor window = ScriptableObject.CreateInstance<CopySceneEditor>();
            window.titleContent = new GUIContent("复制场景");
            window.Show();
        }

        /// <summary>
        /// 源路径
        /// </summary>
        string m_SrcPath;

        /// <summary>
        /// 目标路径
        /// </summary>
        string m_DstPath;

        /// <summary>
        /// 目标场景名字
        /// </summary>
        string m_DstSceneName;

        /// <summary>
        /// 查询根目录
        /// </summary>
        string m_FindRootDir;

        private void OnGUI()
        {
            m_FindRootDir = Application.dataPath + "/scene/";

            // 1.
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("源路径", GUILayout.Width(50f));
                m_SrcPath = GUILayout.TextField(m_SrcPath);

                if (GUILayout.Button("浏览", GUILayout.Width(50f)))
                {
                    m_SrcPath = EditorUtility.OpenFolderPanel("源路径", m_FindRootDir, "");
                }
            }
            GUILayout.EndHorizontal();

            // 2.
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("目标路径", GUILayout.Width(50f));
                m_DstPath = GUILayout.TextField(m_DstPath);

                if (GUILayout.Button("浏览", GUILayout.Width(50f)))
                {
                    m_DstPath = EditorUtility.OpenFolderPanel("目标路径", m_FindRootDir, "");
                }
            }
            GUILayout.EndHorizontal();

            // 3.
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("目标场景名字（数字）", GUILayout.Width(150f));
                m_DstSceneName = GUILayout.TextField(m_DstSceneName);
            }
            GUILayout.EndHorizontal();

            // .
            if (GUILayout.Button("复制"))
            {
                CopyScene.Copy(m_SrcPath, m_DstPath, m_DstSceneName);
            }
        }
    }
}

