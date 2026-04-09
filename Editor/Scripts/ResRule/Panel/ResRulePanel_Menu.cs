using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ResRulePanel
    {
        partial class Style
        {
            public GUIContent FileMenuName = EditorGUIUtility.TrTextContent("File");
            public GUIContent EditMenuName = EditorGUIUtility.TrTextContent("Edit");
        }

        ResRuleMgrSelectField m_ResRuleMgrSelectField = new ResRuleMgrSelectField();
        ResRuleManagerAsset m_RuleMgrAsset;

        /// <summary>
        /// 绘制菜单区域
        /// </summary>
        void DrawMenuGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                Rect fileRect = GUILayoutUtility.GetRect(m_Style.FileMenuName, EditorStyles.toolbarButton, GUILayout.Width(50));
                if (EditorGUI.DropdownButton(fileRect, m_Style.FileMenuName, FocusType.Passive, EditorStyles.toolbarButton))
                {
                    MenuCommand command = new MenuCommand(this);
                    EditorUtility.DisplayPopupMenu(new Rect(fileRect.x, fileRect.y + fileRect.height, 0f, 0f), "CONTEXT/Menu/File", command);
                }

                Rect editRect = GUILayoutUtility.GetRect(m_Style.EditMenuName, EditorStyles.toolbarButton, GUILayout.Width(50));
                if (EditorGUI.DropdownButton(editRect, m_Style.EditMenuName, FocusType.Passive, EditorStyles.toolbarButton))
                {
                    MenuCommand command = new MenuCommand(this);
                    EditorUtility.DisplayPopupMenu(new Rect(editRect.x, editRect.y + editRect.height, 0f, 0f), "CONTEXT/Menu/Editor", command);
                }

                GUILayout.FlexibleSpace();
                ShowResRuleManagerSelectGUI();
            }
        }

        /// <summary>
        /// 绘制规则管理选择
        /// </summary>
        void ShowResRuleManagerSelectGUI()
        {
            var newMgr = m_ResRuleMgrSelectField.ShowGUI();

            if (m_RuleMgrAsset != newMgr)
            {
                m_RuleMgrAsset = newMgr;

                if (m_SelectedView != null)
                {
                    m_SelectedView.SetInfo(m_RuleMgrAsset, (ResRuleData)null);
                }
            }
        }

        #region 为当前工程创建规则管理
        [MenuItem("CONTEXT/Menu/File/为当前工程创建规则管理", false, 0)]
        static void CreateProject(MenuCommand item)
        {
            CreateCurProjectRuleManagerAsset();

            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel != null && resRulePanel.m_ResRuleMgrSelectField != null)
            {
                resRulePanel.m_ResRuleMgrSelectField.RefreshData();
            }
        }

        [MenuItem("CONTEXT/Menu/File/为当前工程创建规则管理", true)]
        static bool CreateProjectCheck(MenuCommand item)
        {
            string projectName = EditorFileUtils.GetProjectName();
            string projectAssetPath = Path.Combine(ResRuleDefine.RULE_PROJECT_DIR, projectName, projectName + ".asset");
            return AssetDatabase.LoadAssetAtPath<ResRuleManagerAsset>(projectAssetPath) == null;
        }
        #endregion

        [MenuItem("CONTEXT/Menu/File/Save", false, 1)]
        static void SaveProject(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;
            
            if (resRulePanel != null && resRulePanel.m_RuleMgrAsset != null)
            {
                EditorUtility.SetDirty(resRulePanel.m_RuleMgrAsset);
                AssetDatabase.SaveAssets();
            }
        }

        #region CONTEXT/Menu/File/New Rule
        [MenuItem("CONTEXT/Menu/File/New Rule", false, 20)]
        static void CreateRule(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel != null && resRulePanel.m_RuleMgrAsset != null && resRulePanel.m_SelectedView != null)
            {
                resRulePanel.m_RuleMgrAsset.CreateRuleData(resRulePanel.m_SelectedView.resImporterType);
                EditorUtility.SetDirty(resRulePanel.m_RuleMgrAsset);
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("CONTEXT/Menu/File/New Rule", true)]
        static bool NewRuleCheck(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;
            return resRulePanel != null && resRulePanel.m_RuleMgrAsset != null;
        }
        #endregion

        [MenuItem("CONTEXT/Menu/File/退出", false, 90)]
        static void ExitWindows(MenuCommand item)
        {
            GetWindow<ResRulePanel>().Close();
        }


        [MenuItem("CONTEXT/Menu/Editor/执行所有规则")]
        static void ExecuteRuleAll(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel != null && resRulePanel.m_RuleMgrAsset != null)
            {
                resRulePanel.m_RuleMgrAsset.ExecuteRuleAll();
            }
        }

        [MenuItem("CONTEXT/Menu/Editor/检查所有资源")]
        static void CheckResAll(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;

            if (resRulePanel != null && resRulePanel.m_RuleMgrAsset != null)
            {
                var checkFilePath = resRulePanel.m_RuleMgrAsset.CheckResAll();
                string tips = "资源检查完成, 结果:" + checkFilePath;
                resRulePanel.ShowNotification(new GUIContent(tips));
                Debug.Log(tips);
            }
        }
        
        [MenuItem("CONTEXT/Menu/Editor/检查所有资源-场景")]
        static void CheckResAllScene(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;
            string buildSceneTextPath = "H:\\seres\\trunk\\client\\output\\default\\data\\standalone\\builds_scene.txt";
            HashSet<string> sceneAssetPathSet = GetAssetPathSet(buildSceneTextPath);

            if (resRulePanel != null && resRulePanel.m_RuleMgrAsset != null)
            {
                var checkFilePath = resRulePanel.m_RuleMgrAsset.CheckResAll(sceneAssetPathSet);
                string tips = "资源检查完成, 结果:" + checkFilePath;
                resRulePanel.ShowNotification(new GUIContent(tips));
                Debug.Log(tips);
            }
        }

        [MenuItem("CONTEXT/Menu/Editor/打开查询工具...")]
        static void OpenResSearchPanel(MenuCommand item)
        {
            ResCheckPanel.GetWindow();
        }

        [MenuItem("CONTEXT/Menu/Editor/执行所有规则", true)]
        [MenuItem("CONTEXT/Menu/Editor/检查所有资源", true)]
        [MenuItem("CONTEXT/Menu/Editor/打开查询工具...", true)]
        static bool SplatCheck(MenuCommand item)
        {
            var resRulePanel = item.context as ResRulePanel;
            return resRulePanel != null && resRulePanel.m_RuleMgrAsset != null;
        }

        /// <summary>
        /// 为当前项目创建规则工程
        /// </summary>
        static void CreateCurProjectRuleManagerAsset()
        {
            string projectName = EditorFileUtils.GetProjectName();
            string projectAssetDir = Path.Combine(ResRuleDefine.RULE_PROJECT_DIR, projectName);

            if (!Directory.Exists(projectAssetDir))
            {
                Directory.CreateDirectory(projectAssetDir);
            }

            string projectAssetPath = Path.Combine(projectAssetDir, projectName + ".asset");

            var config = ScriptableObject.CreateInstance<ResRuleManagerAsset>();
            AssetDatabase.CreateAsset(config, projectAssetPath);
            AssetDatabase.Refresh();
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
