using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ResRuleManagerAsset
    {
        StringBuilder m_TempStringBuilder = new StringBuilder();

        /// <summary>
        /// 获取规则模板文件路径
        /// </summary>
        public static string GetTemplatePath(Type resImporterType)
        {
            if (resImporterType == typeof(AudioImporter))
            {
                return Path.Combine(ResRuleDefine.RULE_TEMPLATE_DIR, "template.mp3");
            }
            else if (resImporterType == typeof(TextureImporter))
            {
                return Path.Combine(ResRuleDefine.RULE_TEMPLATE_DIR, "template.png");
            }
            else if (resImporterType == typeof(ModelImporter))
            {
                return Path.Combine(ResRuleDefine.RULE_TEMPLATE_DIR, "template.FBX");
            }

            Debug.LogError("未处理的资源类型:" + resImporterType);
            return string.Empty;
        }

        /// <summary>
        /// 获取当前项目的资源管理数据
        /// </summary>
        public static ResRuleManagerAsset GetCurProjectResRuleManagerAsset()
        {
            var projectName = EditorFileUtils.GetProjectName();
            string[] guids = AssetDatabase.FindAssets("t:ResRuleManagerAsset " + projectName, new string[] { ResRuleDefine.RULE_PROJECT_DIR });

            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var assetFileName = Path.GetFileNameWithoutExtension(assetPath);

                if (string.IsNullOrEmpty(assetPath) || projectName != Path.GetFileNameWithoutExtension(assetPath))
                {
                    continue;
                }

                var resRuleManagerAsset = AssetDatabase.LoadAssetAtPath<ResRuleManagerAsset>(assetPath);

                if (resRuleManagerAsset != null)
                {
                    return resRuleManagerAsset;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有规则管理数据
        /// </summary>
        public static void GetResRuleManagerAssetAll(List<ResRuleManagerAsset> resultList)
        {
            if (resultList == null)
            {
                return;
            }

            resultList.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ResRuleManagerAsset", new string[] { ResRuleDefine.RULE_PROJECT_DIR });

            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                var resRuleManagerAsset = AssetDatabase.LoadAssetAtPath<ResRuleManagerAsset>(assetPath);

                if (resRuleManagerAsset != null)
                {
                    resultList.Add(resRuleManagerAsset);
                }
            }
        }

        /// <summary>
        /// 检查所有资源信息
        /// </summary>
        public string CheckResAll(HashSet<string> includeAssets = null)
        {
            return CheckRes("t:audioclip t:model t:texture", includeAssets);
        }

        public string CheckRes(string filter, HashSet<string> includeAssets = null)
        {
            m_TempStringBuilder.Length = 0;
            m_TempStringBuilder.Append("规则名,资源类型,错误信息,资源路径\n");

            string[] guids = AssetDatabase.FindAssets(filter, ResRuleDefine.CHECK_NORMAL_DIR_S);

            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // 资源在工具目录跳过
                if (ResRuleHelper.CheckPathIsInToolDirectory(assetPath))
                {
                    continue;
                }

                if (includeAssets != null && !includeAssets.Contains(assetPath))
                {
                    continue;
                }

                CheckResByGUID(assetPath, out string errorInfo, out Type importerType, out string ruleName);
                m_TempStringBuilder.AppendFormat("{0},{1},{2},{3}\n", ruleName, importerType.Name, errorInfo, assetPath);
            }

            string checkInfo = m_TempStringBuilder.ToString();
            Debug.Log(checkInfo);
            return SaveCSVFile(checkInfo);
        }

        /// <summary>
        /// 判断资源应用信息
        /// </summary>
        void CheckResByGUID(string assetPath, out string errorInfo, out Type importerType, out string ruleName)
        {
            errorInfo = "资源和规则匹配";
            importerType = typeof(Type);
            ruleName = "未匹配";

            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var resImporter = AssetImporter.GetAtPath(assetPath);
            importerType = resImporter.GetType();
            var fileName = Path.GetFileName(assetPath);

            int ruleCount = 0;
            ResRuleData lastRuleData = null;

            string matchRuleNameStr = string.Empty;

            foreach (var ruleData in ruleDataList)
            {
                if (ruleData.resImporterType != importerType)
                {
                    continue;
                }

                // 资源不在规则下 或不匹配规则
                if (!ruleData.IsContainsPath(assetPath))
                {
                    continue;
                }

                ruleName = ruleData.ruleName;
                ruleCount++;

                if (ruleCount == 1)
                {
                    lastRuleData = ruleData;
                }

                matchRuleNameStr = matchRuleNameStr + "; " + ruleName;
            }

            if (ruleCount < 1)
            {
                errorInfo = "资源未匹配任何规则";
            }
            else if (ruleCount > 1)
            {
                errorInfo = "资源匹配了多个规则";
                Debug.LogErrorFormat("{0} match multi rules: {1}", assetPath, matchRuleNameStr);
            }
            else
            {
                if (!ResRuleHelper.IsEqual(lastRuleData, resImporter))
                {
                    errorInfo = "资源信息和规则不匹配";
                }
            }
        }

        string SaveCSVFile(string content)
        {
            string fileName = string.Format("CheckLog/RecCheck_{0}.csv", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
            var mgrAssetPath = AssetDatabase.GetAssetPath(this);
            var dirName = Path.GetDirectoryName(mgrAssetPath);
            string path = Path.Combine(dirName, fileName);

            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // 生成文本
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            sw.Write(content);
            sw.Close();

            // 资源刷新
            AssetDatabase.Refresh();
            return path;
        }
    }
}
