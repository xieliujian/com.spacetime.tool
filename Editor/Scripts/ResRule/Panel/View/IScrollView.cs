using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal abstract class IScrollView : TreeView
    {
        protected static IList<TreeViewItem> s_NullDataList = new List<TreeViewItem>(0);

        TreeViewItem m_Root = new TreeViewItem { id = 0, depth = -1, displayName = "root" };
        
        protected ResRuleManagerAsset m_RuleManager;
        protected List<ResRuleData> m_RuleDatas;
        protected HashSet<string> m_IncludeAssetPathSet = null;

        public Type resImporterType { get; private set; }
        public string filter { get; private set; }

        public IScrollView(Type resImporterType, string filter, TreeViewState state, MultiColumnHeader multiColumnHeader)
            : base(state, multiColumnHeader)
        {
            this.resImporterType = resImporterType;
            this.filter = filter;
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        public void SetInfo(ResRuleManagerAsset ruleMgr, ResRuleData ruleData, HashSet<string> includeAssetPathSet = null)
        {
            if (ruleData == null)
            {
                m_RuleManager = ruleMgr;
                m_RuleDatas = null;
            }
            else
            {
                m_RuleManager = ruleMgr;
                m_RuleDatas = new List<ResRuleData>();
                m_RuleDatas.Add(ruleData);
            }

            m_IncludeAssetPathSet = includeAssetPathSet;
            Reload();
        }
        
        public void SetInfo(ResRuleManagerAsset ruleMgr, List<ResRuleData> ruleDatas, HashSet<string> includeAssetPathSet = null)
        {
            m_RuleManager = ruleMgr;
            m_RuleDatas = ruleDatas;
            
            m_IncludeAssetPathSet = includeAssetPathSet;
            Reload();
        }

        protected override TreeViewItem BuildRoot() { return m_Root; }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (m_RuleManager == null)
            {
                return s_NullDataList;
            }
            
            if (root != null && root.children != null)
            {
                root.children.Clear();
            }

            string projectName = EditorFileUtils.GetProjectName();
            List<string> assetPathList = new List<string>();
            
            string[] guids = AssetDatabase.FindAssets(filter, GetRuleDataPaths());
            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (m_IncludeAssetPathSet != null && !m_IncludeAssetPathSet.Contains(assetPath))
                {
                    continue;
                }
                
                if (projectName == "scene" &&
                    (assetPath.StartsWith("Assets/scene/LookDev") || assetPath.StartsWith("Assets/scene/common/level_layout/")))
                {
                    continue;
                }
                
                if (projectName == "character" &&
                    (assetPath.StartsWith("Assets/character/zout/")))
                {
                    continue;
                }

                if (ResRuleHelper.CheckPathIsInToolDirectory(assetPath))
                {
                    continue;
                }
                
                if (m_RuleDatas != null && !IsInRuleDataPaths(assetPath))
                {
                    continue;
                }
                
                assetPathList.Add(assetPath);
            }

            for (int i = 0; i < assetPathList.Count; i++)
            {
                TryAddRes(root, i + 1, AssetDatabase.AssetPathToGUID(assetPathList[i]));
            }

            if (rootItem.children == null || rootItem.children.Count <= 0)
            {
                return s_NullDataList;
            }

            SetupDepthsFromParentsAndChildren(root);
            SortItems(rootItem.children);
            return rootItem.children;
        }

        protected abstract bool TryAddRes(TreeViewItem root, int id, string resGUID);

        protected abstract void SortItems(List<TreeViewItem> children);

        /// <summary>
        /// 双击选中
        /// </summary>
        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            var assetObject = AssetDatabase.LoadAssetAtPath(item.displayName, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetObject;
            EditorGUIUtility.PingObject(assetObject);
        }

        protected sealed override void RowGUI(RowGUIArgs args)
        {
            if (ViewItemModule.S.isDerty)
            {
                ViewItemModule.S.isDerty = false;
                Reload();
            }

            OnRowGUI(args);
        }

        protected abstract void OnRowGUI(RowGUIArgs args);
        
        protected string[] GetRuleDataPaths()
        {
            if (m_RuleDatas == null || m_RuleDatas.Count <= 0)
            {
                return ResRuleDefine.CHECK_NORMAL_DIR_S;
            }
            
            List<string> dataPaths = new List<string>();
            
            foreach (ResRuleData data in m_RuleDatas)
            {
                foreach (string path in data.assetPathList)
                {
                    if (!dataPaths.Contains(path))
                    {
                        dataPaths.Add(path);
                    }
                }
            }

            return dataPaths.ToArray();
        }

        protected bool IsInRuleDataPaths(string assetPath)
        {
            if (m_RuleDatas == null)
            {
                return true;
            }

            for (int i = 0; i < m_RuleDatas.Count; ++i)
            {
                if (m_RuleDatas[i].IsContainsPath(assetPath))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
