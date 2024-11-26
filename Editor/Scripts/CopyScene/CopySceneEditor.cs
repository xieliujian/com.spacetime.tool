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
        [MenuItem("SpaceTime/Tool/���Ƴ���", false, 300)]
        public static void OnOpen()
        {
            CopySceneEditor window = ScriptableObject.CreateInstance<CopySceneEditor>();
            window.titleContent = new GUIContent("���Ƴ���");
            window.Show();
        }

        /// <summary>
        /// Դ·��
        /// </summary>
        string m_SrcPath;

        /// <summary>
        /// Ŀ��·��
        /// </summary>
        string m_DstPath;

        /// <summary>
        /// Ŀ�곡������
        /// </summary>
        string m_DstSceneName;

        /// <summary>
        /// ��ѯ��Ŀ¼
        /// </summary>
        string m_FindRootDir;

        private void OnGUI()
        {
            m_FindRootDir = Application.dataPath + "/scene/";

            // 1.
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Դ·��", GUILayout.Width(50f));
                m_SrcPath = GUILayout.TextField(m_SrcPath);

                if (GUILayout.Button("���", GUILayout.Width(50f)))
                {
                    m_SrcPath = EditorUtility.OpenFolderPanel("Դ·��", m_FindRootDir, "");
                }
            }
            GUILayout.EndHorizontal();

            // 2.
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Ŀ��·��", GUILayout.Width(50f));
                m_DstPath = GUILayout.TextField(m_DstPath);

                if (GUILayout.Button("���", GUILayout.Width(50f)))
                {
                    m_DstPath = EditorUtility.OpenFolderPanel("Ŀ��·��", m_FindRootDir, "");
                }
            }
            GUILayout.EndHorizontal();

            // 3.
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Ŀ�곡�����֣����֣�", GUILayout.Width(150f));
                m_DstSceneName = GUILayout.TextField(m_DstSceneName);
            }
            GUILayout.EndHorizontal();

            // .
            if (GUILayout.Button("����"))
            {
                CopyScene.Copy(m_SrcPath, m_DstPath, m_DstSceneName);
            }
        }
    }
}

