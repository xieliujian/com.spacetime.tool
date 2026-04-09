using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using ST.Core;

namespace ST.Tool
{
    internal partial class ResRulePanel
    {
        partial class Style
        {
            public GUILayoutOption TitleSmallLabelWidth = GUILayout.Width(60);
            public GUILayoutOption TitleLabelWidth = GUILayout.Width(120);
            public GUILayoutOption TitleLargeLabelWidth = GUILayout.Width(200);
            public GUILayoutOption MiniBtnWidth = GUILayout.Width(20);

            public GUIStyle IconButton = new GUIStyle("IconButton") { alignment = TextAnchor.MiddleCenter };

            // 文本类型
            public GUIContent RuleNameTitle = EditorGUIUtility.TrTextContent("Rule Name:", "规则名字");
            public GUIContent KeyWordTitle = EditorGUIUtility.TrTextContent("Key Word:", "匹配正则表达，需全路径匹配");
            public GUIContent ExcludeWordTitle = EditorGUIUtility.TrTextContent("Exclude Word:", "排除关键字, 空格分隔");
            public GUIContent ExtraRuleTitle = EditorGUIUtility.TrTextContent("Extra Rule:", "额外的规则处理");
            public GUIContent ActionPathsTitle = EditorGUIUtility.TrTextContent("影响路径:", "规则影响的路径列表");
            public GUIContent ExcludeActionPathsTitle = EditorGUIUtility.TrTextContent("排除路径:", "规则影响的排除路径列表");
            public GUIContent ParameterTitle = EditorGUIUtility.TrTextContent("Extra Rule Param:", "额外的规则处理的参数");
            public GUIContent ExecuteRuleBtn = EditorGUIUtility.TrTextContent("执行规则", "执行规则");
            public GUIContent RefreshRuleBtn = EditorGUIUtility.TrTextContent("刷新规则", "刷新规则");
            public GUIContent PullUpRuleBtn = EditorGUIUtility.TrTextContent("↑", "上移规则");
            public GUIContent PushDownRuleBtn = EditorGUIUtility.TrTextContent("↓", "下移规则");

            public GUIContent OverrideModelOptimizeGameObjectsToggle = EditorGUIUtility.TrTextContent("OverrideOptimizeGameObjects", "允许规则设置OptimizeGameObjects");
            public GUIContent OverrideAnimationOptimizeToggle = EditorGUIUtility.TrTextContent("OverrideAnimationCompress", "允许规则设置AnimationCompress");
            public GUIContent OverrideImportBlendShapesToggle = EditorGUIUtility.TrTextContent("OverrideImportBlendShapes", "允许规则设置ImportBlendShapes");
            public GUIContent OverrideResampleCurvesToggle = EditorGUIUtility.TrTextContent("OverrideResampleCurves", "允许规则设置ResampleCurves");

            public GUIContent OverrideNPOTToggle = EditorGUIUtility.TrTextContent("OverrideNPOT", "允许规则设置NPOT");

            public GUIContent OverrideTypeToggle = EditorGUIUtility.TrTextContent("OverrideType", "覆盖选项");

            // 图标类型
            public GUIContent AddPathBtn = EditorGUIUtility.TrIconContent("d_Toolbar Plus", "添加一条新的规则影响路径");
            public GUIContent MinusPathBtn = EditorGUIUtility.TrIconContent("d_Toolbar Minus", "删除路径");
            public GUIContent RuleMenuBtn = EditorGUIUtility.TrIconContent("pane options", "规则菜单");
            public GUIContent SelectFolderBtn = EditorGUIUtility.TrIconContent("d_SettingsIcon", "选择规则影响路径");
        }

        Vector3 m_ViewPos;
        Dictionary<string, int> m_RuleNameCountMap = new Dictionary<string, int>();
        Dictionary<string, int> m_KeyWordAddPathCountMap = new Dictionary<string, int>();

        /// <summary>
        /// 绘制菜单区域
        /// </summary>
        void DrawRuleGUI()
        {
            if (m_RuleMgrAsset == null)
            {
                GUILayout.Label("plasses click File->为当前工程创建规则管理.", "ErrorLabel", GUILayout.Height(m_RuleAreaHeight));
                return;
            }

            if (m_RuleMgrAsset.ruleDataList == null)
            {
                m_RuleMgrAsset.ruleDataList = new List<ResRuleData>();
            }

            m_ViewPos = EditorGUILayout.BeginScrollView(m_ViewPos, GUILayout.Height(m_RuleAreaHeight));

            int drawCount = 0;

            for (int i = 0; i < m_RuleMgrAsset.ruleDataList.Count; i++)
            {
                if (DrawRuleDataCell(i, m_RuleMgrAsset.ruleDataList[i]))
                {
                    drawCount++;
                }
            }

            if (drawCount <= 0)
            {
                GUILayout.Label("plasses click File->New Rule.", "ErrorLabel", GUILayout.Height(m_RuleAreaHeight));
            }

            EditorGUILayout.EndScrollView();

            TryRefreshRuleNameCountMap();    // 尝试刷新规则名数量信息
        }

        /// <summary>
        /// 绘制规则内容
        /// </summary>
        bool DrawRuleDataCell(int index, ResRuleData resRuleData)
        {
            int removeRuleIndex = -1;     // 要删除的规则index

            if (resRuleData == null || resRuleData.resImporterType != m_SelectedView.resImporterType)
            {
                return false;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                bool isShowArea = DrawRuleDataCellTitle(index, resRuleData);

                // 绘制规则元素内容
                if (isShowArea)
                {
                    EditorGUI.indentLevel++;
                    DrawRuleDataCellData(index, resRuleData);
                    DrawAssetImporter(index, resRuleData);
                    EditorGUI.indentLevel--;
                }
            }

            // 删除规则
            if (removeRuleIndex >= 0)
            {
                m_RuleMgrAsset.RemoveRuleAt(removeRuleIndex);
                removeRuleIndex = -1;
            }

            return true;
        }

        /// <summary>
        /// 绘制资源信息
        /// </summary>
        static void DrawAssetImporter(int index, ResRuleData resRuleData)
        {
            if (resRuleData == null || resRuleData.editor == null)
            {
                return;
            }

            if (ResEditorGUI.TitleToggleBtn(resRuleData.resImporterType.ToString(), "OnInspectorGUI" + index))
            {
                // 开始检查变更
                EditorGUI.BeginChangeCheck();

                // 绘制GUI
                resRuleData.editor.OnInspectorGUI();

                if (resRuleData.importer != null && EditorGUI.EndChangeCheck())
                {
                    // 变更后保存数据
                    resRuleData.importer.SaveAndReimport();
                }
            }
        }

        /// <summary>
        /// 绘制规则标题
        /// </summary>
        bool DrawRuleDataCellTitle(int index, ResRuleData resRuleData)
        {
            bool isShowArea = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                // 绘制规则元素标题
                GetRelicTitleInfo(resRuleData, out string ruleTitleName, out Color color);
                GUI.color = color;
                isShowArea = ResEditorGUI.TitleToggleBtn(ruleTitleName + " " + System.IO.Path.GetFileName(resRuleData.importer.assetPath), string.Format("RuleCell{0}", index));
                GUI.color = Color.white;

                EditorGUILayout.LabelField("禁用规则:", GUILayout.Width(60));
                resRuleData.isDisable = EditorGUILayout.Toggle(resRuleData.isDisable, GUILayout.Width(30));

                if (GUILayout.Button(m_Style.RefreshRuleBtn, m_Style.TitleLabelWidth))
                {
                    m_SelectedView.SetInfo(m_RuleMgrAsset, resRuleData);
                }

                if (GUILayout.Button(m_Style.ExecuteRuleBtn, m_Style.TitleLabelWidth))
                {
                    m_RuleMgrAsset.ExecuteRuleAt(index);
                }

                if (GUILayout.Button(m_Style.PullUpRuleBtn, m_Style.MiniBtnWidth))
                {
                    m_RuleMgrAsset.PullUpRule(index);
                }

                if (GUILayout.Button(m_Style.PushDownRuleBtn, m_Style.MiniBtnWidth))
                {
                    m_RuleMgrAsset.PushDownRule(index);
                }

                Rect ruleBtnRect = GUILayoutUtility.GetRect(m_Style.RuleMenuBtn, m_Style.IconButton, m_Style.MiniBtnWidth);
                if (GUI.Button(ruleBtnRect, m_Style.RuleMenuBtn, m_Style.IconButton))
                {
                    MenuCommand command = new MenuCommand(this, index);
                    EditorUtility.DisplayPopupMenu(new Rect(ruleBtnRect.x, ruleBtnRect.y + ruleBtnRect.height, 0f, 0f), "CONTEXT/RuleDataMenu", command);
                }
            }

            return isShowArea;
        }

        /// <summary>
        /// 绘制资源标题
        /// </summary>
        void DrawRuleDataCellData(int index, ResRuleData resRuleData)
        {
            if (resRuleData == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            // 绘制 规则名
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_Style.RuleNameTitle, m_Style.TitleLabelWidth);
                GUI.color = GetRuleNameColor(resRuleData.ruleName);
                resRuleData.ruleName = EditorGUILayout.TextField(resRuleData.ruleName);
                GUI.color = Color.white;
            }

            // 绘制 关键字
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_Style.KeyWordTitle, m_Style.TitleLabelWidth);
                resRuleData.keyWord = string.IsNullOrEmpty(resRuleData.keyWord) ? ".*" : resRuleData.keyWord;
                resRuleData.keyWord = EditorGUILayout.TextField(resRuleData.keyWord);
            }

            // 排除关键字
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_Style.ExcludeWordTitle, m_Style.TitleLabelWidth);
                resRuleData.excludeWordList = EditorFileUtils.String2StringList(
                    EditorGUILayout.TextField(EditorFileUtils.StringList2String(resRuleData.excludeWordList, " ")), ' ');
            }

            // 绘制额外处理信息
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_Style.ExtraRuleTitle, m_Style.TitleLabelWidth);
                resRuleData.extraProcessName = ExtraProcessingSelect.ShowGUI(m_SelectedView.resImporterType, resRuleData.extraProcessName);
            }
            
            // 额外参数
            resRuleData.parameterList = DrawRuleStringList(resRuleData.parameterList, m_Style.ParameterTitle);

            // 绘制规则影响路径信息
            DrawRuleActionPaths(resRuleData.assetPathList, m_Style.ActionPathsTitle, 1);
            DrawRuleActionPaths(resRuleData.excludeAssetPathList, m_Style.ExcludeActionPathsTitle, 0);

            DrawExtraData(resRuleData);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_RuleMgrAsset);
            }
        }

        void DrawExtraData(ResRuleData resRuleData)
        {
            if (resRuleData.importer is ModelImporter)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(m_Style.OverrideModelOptimizeGameObjectsToggle, m_Style.TitleLargeLabelWidth);
                    resRuleData.modelData.isOverrideOptimizeGameObjects =
                        EditorGUILayout.Toggle(resRuleData.modelData.isOverrideOptimizeGameObjects);

                    EditorGUILayout.LabelField(m_Style.OverrideAnimationOptimizeToggle, m_Style.TitleLargeLabelWidth);
                    resRuleData.modelData.isOverrideAnimationCompress =
                        EditorGUILayout.Toggle(resRuleData.modelData.isOverrideAnimationCompress);

                    EditorGUILayout.LabelField(m_Style.OverrideImportBlendShapesToggle, m_Style.TitleLargeLabelWidth);
                    resRuleData.modelData.isOverrideImportBlendShapes =
                        EditorGUILayout.Toggle(resRuleData.modelData.isOverrideImportBlendShapes);
                    
                    EditorGUILayout.LabelField(m_Style.OverrideResampleCurvesToggle, m_Style.TitleLargeLabelWidth);
                    resRuleData.modelData.isOverrideResampleCurves =
                        EditorGUILayout.Toggle(resRuleData.modelData.isOverrideResampleCurves);
                }
            }
            
            if (resRuleData.importer is TextureImporter)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(m_Style.OverrideNPOTToggle, m_Style.TitleLargeLabelWidth);
                    resRuleData.textureData.isOverrideNPOT = EditorGUILayout.Toggle(resRuleData.textureData.isOverrideNPOT);
                }

				using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(m_Style.OverrideTypeToggle, m_Style.TitleLargeLabelWidth);
                    resRuleData.textureData.overrideType = (TextureImporterOverrideType)EditorGUILayout.EnumFlagsField(resRuleData.textureData.overrideType);
				}
            }
        }

        /// <summary>
        /// 绘制规则影响路径信息
        /// </summary>
        /// <param name="resRuleData"></param>
        void DrawRuleActionPaths(List<string> assetPathList, GUIContent title, int minCount)
        {
            int removePathIndex = -1;

            // 添加一个路径
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(title);
                if (GUILayout.Button(m_Style.AddPathBtn, m_Style.IconButton, m_Style.MiniBtnWidth))
                {
                    assetPathList.Add(string.Empty);
                }
            }
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            // 文件路径
            for (int i = 0; i < assetPathList.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.color = GetAssetPathColor(assetPathList[i]);
                    EditorGUILayout.TextField(assetPathList[i]);
                    GUI.color = Color.white;

                    if (GUILayout.Button(m_Style.SelectFolderBtn, m_Style.IconButton, m_Style.MiniBtnWidth))
                    {
                        var assetPath = SelectPath(assetPathList);

                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            assetPathList[i] = Path.GetDirectoryName(assetPath).Replace("\\", "/") + "/";
                        }

                        GUI.FocusControl(string.Empty);         // 取消输入框焦点
                    }

                    EditorGUI.BeginDisabledGroup(assetPathList.Count <= minCount);
                    if (GUILayout.Button(m_Style.MinusPathBtn, m_Style.IconButton, m_Style.MiniBtnWidth))
                    {
                        removePathIndex = i;
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (removePathIndex >= 0)
            {
                assetPathList.RemoveAt(removePathIndex);
                removePathIndex = -1;
            }
        }
        
        /// <summary>
        /// 绘制规则字符串列表信息
        /// </summary>
        /// <param name="resRuleData"></param>
        void DrawRuleStringPaths(List<string> strList, GUIContent title, int minCount)
        {
            int removePathIndex = -1;

            // 添加一个路径
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(title);
                if (GUILayout.Button(m_Style.AddPathBtn, m_Style.IconButton, m_Style.MiniBtnWidth))
                {
                    strList.Add(string.Empty);
                }
            }
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            // 文件路径
            for (int i = 0; i < strList.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    strList[i] = EditorGUILayout.TextField(strList[i]);
                    EditorGUI.BeginDisabledGroup(strList.Count <= minCount);
                    if (GUILayout.Button(m_Style.MinusPathBtn, m_Style.IconButton, m_Style.MiniBtnWidth))
                    {
                        removePathIndex = i;
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (removePathIndex >= 0)
            {
                strList.RemoveAt(removePathIndex);
                removePathIndex = -1;
            }
        }

        List<string> DrawRuleStringList(List<string> strList, GUIContent title)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(title, m_Style.TitleLabelWidth);
                string lastStr = EditorFileUtils.StringList2String(strList, " ");
                string newStr = EditorGUILayout.TextField(lastStr);

                if (lastStr != newStr)
                {
                    return EditorFileUtils.String2StringList(newStr, ' ');
                }
            }

            return strList;
        }

        /// <summary>
        /// 获取规则标题信息
        /// </summary>
        void GetRelicTitleInfo(ResRuleData ruleData, out string ruleTitleName, out Color titleColor)
        {
            var errorInfo = CheckRuleInfo(ruleData);
            var isError = !string.IsNullOrEmpty(errorInfo);
            ruleTitleName = isError ? errorInfo : ruleData.ruleName;
            titleColor = isError ? Color.red : Color.white;
        }

        /// <summary>
        /// 获取规则标题名(带重名判定)
        /// </summary>
        string CheckRuleInfo(ResRuleData ruleData)
        {
            if (ruleData == null)
            {
                return "Null Data";
            }

            if (string.IsNullOrEmpty(ruleData.ruleName))
            {
                return "未设置规则名";
            }

            m_RuleNameCountMap.TryGetValue(ruleData.ruleName, out int count);

            if (count > 1)
            {
                return string.Format("规则名重复", ruleData.ruleName);
            }

            if (string.IsNullOrEmpty(ruleData.keyWord))
            {
                return "未设置匹配正则表达式";
            }

            if (ruleData.assetPathList == null || ruleData.assetPathList.Count <= 0)
            {
                return "未设置作用路径";
            }

            foreach (var assetPath in ruleData.assetPathList)
            {
                if (string.IsNullOrEmpty(assetPath) || !Directory.Exists(assetPath))
                {
                    return "有无效的路径";
                }
            }

            // TODO
            foreach (var assetPath in ruleData.assetPathList)
            {
                string keyWordAndPath = string.Format("{0}{1}", ruleData.keyWord, assetPath);

                if (m_KeyWordAddPathCountMap.TryGetValue(keyWordAndPath, out int useCount) && useCount > 1)
                {
                    return "关键字+路径有重复使用";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取路径颜色
        /// </summary>
        Color GetAssetPathColor(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || !Directory.Exists(assetPath))
            {
                return Color.red;
            }

            return Color.white;
        }

        /// <summary>
        /// 获取规则名颜色
        /// </summary>
        Color GetRuleNameColor(string ruleName)
        {
            if (string.IsNullOrEmpty(ruleName))
            {
                return Color.red;
            }

            if (m_RuleNameCountMap.TryGetValue(ruleName, out int count))
            {
                return count == 1 ? Color.white : Color.red;
            }

            return Color.red;
        }

        /// <summary>
        /// 更新规则名次数信息
        /// </summary>
        void TryRefreshRuleNameCountMap()
        {
            if (GUI.changed || m_RuleNameCountMap.Count <= 0 || m_KeyWordAddPathCountMap.Count <= 0)
            {
                RefreshRuleNameCountMap();
            }
        }

        /// <summary>
        /// 更新规则名次数信息 TODO
        /// </summary>
        void RefreshRuleNameCountMap()
        {
            m_RuleNameCountMap.Clear();
            m_KeyWordAddPathCountMap.Clear();

            foreach (var ruleData in m_RuleMgrAsset.ruleDataList)
            {
                if (ruleData == null || ruleData.resImporterType != m_SelectedView.resImporterType)
                {
                    continue;
                }

                // 更新规则名称使用次数
                if (!m_RuleNameCountMap.ContainsKey(ruleData.ruleName))
                {
                    m_RuleNameCountMap[ruleData.ruleName] = 0;
                }

                m_RuleNameCountMap[ruleData.ruleName]++;

                if (ruleData.assetPathList == null)
                {
                    continue;
                }

                // 更新路径和正则表达式使用次数
                foreach (var assetPath in ruleData.assetPathList)
                {
                    string keyWordAndPath = string.Format("{0}{1}", ruleData.keyWord, assetPath);

                    if (!m_KeyWordAddPathCountMap.ContainsKey(keyWordAndPath))
                    {
                        m_KeyWordAddPathCountMap[keyWordAndPath] = 0;
                    }

                    m_KeyWordAddPathCountMap[keyWordAndPath]++;
                }
            }
        }

        /// <summary>
        /// 选择路径
        /// </summary>
        string SelectPath(List<string> assetPathList)
        {
            var folderPath = EditorUtility.OpenFolderPanel("选择目录", "Assets", string.Empty);

            if (!string.IsNullOrEmpty(folderPath) && folderPath.IndexOf("Assets") >= 0)
            {
                var assetsPath = EditorUtils.ABSPath2AssetsPath(folderPath) + "/";

                if (assetPathList.Contains(assetsPath))
                {
                    ShowNotification(new GUIContent("规则内已包含该路径"));
                    return string.Empty;
                }

                return assetsPath;
            }

            ShowNotification(new GUIContent("不能选择 assets 以外的目录"));
            return string.Empty;
        }
    }
}
