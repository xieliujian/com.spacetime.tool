using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    public partial class ResRuleData
    {
        [SerializeField] string ruleAssetsPath;

        AssetImporter m_Importer;
        Editor m_Editor;
        Type m_ResImporterType;

        public Type resImporterType
        {
            get
            {
                if (m_ResImporterType == null)
                {
                    var assetImporter = AssetImporter.GetAtPath(ruleAssetsPath);

                    if (assetImporter != null)
                    {
                        m_ResImporterType = assetImporter.GetType();
                    }
                    else
                    {
                        m_ResImporterType = null;
                    }
                }

                return m_ResImporterType;
            }
        }

        /// <summary> 资源信息 </summary>
        public AssetImporter importer { get { TryInitEditorData(); return m_Importer; } }
        /// <summary> 编辑器绘制组件 </summary>
        public Editor editor { get { TryInitEditorData(); return m_Editor; } }

        /// <summary>
        /// 根据规则类型模板创建规则
        /// </summary>
        public static ResRuleData Create(Type resImporterType, string projectAssetDir)
        {
            if (string.IsNullOrEmpty(projectAssetDir))
            {
                return null;
            }

            ResRuleData data = new ResRuleData();
            data.ruleName = string.Format("New {0} Rule", resImporterType);

            string templateAssetPath = ResRuleManagerAsset.GetTemplatePath(resImporterType);

            if (string.IsNullOrEmpty(templateAssetPath))
            {
                return null;
            }

            data.ruleAssetsPath = Path.Combine(projectAssetDir, NewFileName(resImporterType, templateAssetPath));

            if (!File.Exists(data.ruleAssetsPath))
            {
                File.Copy(templateAssetPath, data.ruleAssetsPath, false);
                AssetDatabase.Refresh();
                Debug.LogFormat("Create rule Finish.\noldPath:{0}\nnewPath:{1}", templateAssetPath, data.ruleAssetsPath);
            }

            return data;
        }
        
        public static ResRuleData Create(Type resImporterType, string projectAssetDir, ResRuleData templateRuleData)
        {
            if (string.IsNullOrEmpty(projectAssetDir))
            {
                return null;
            }

            ResRuleData data = templateRuleData.Clone();
            data.ruleName = data.ruleName + " Copy";
            data.keyWord = data.keyWord + " Copy";

            string templateAssetPath = ResRuleManagerAsset.GetTemplatePath(resImporterType);

            if (string.IsNullOrEmpty(templateAssetPath))
            {
                return null;
            }

            data.ruleAssetsPath = Path.Combine(projectAssetDir, NewFileName(resImporterType, templateAssetPath));

            if (!File.Exists(data.ruleAssetsPath))
            {
                File.Copy(templateAssetPath, data.ruleAssetsPath, false);
                AssetDatabase.Refresh();
                
                AssetImporter templateAI = AssetImporter.GetAtPath(templateRuleData.ruleAssetsPath);
                AssetImporter ai = AssetImporter.GetAtPath(data.ruleAssetsPath);
                EditorUtility.CopySerialized(templateAI, ai);
                ai.SaveAndReimport();

                Debug.LogFormat("Create rule Finish.\noldPath:{0}\nnewPath:{1}", templateAssetPath, data.ruleAssetsPath);
            }

            return data;
        }

        /// <summary>
        /// 删除对应的资源文件
        /// </summary>
        internal void DeleteRuleFile()
        {
            File.Delete(ruleAssetsPath);
            File.Delete(ruleAssetsPath + ".meta");
            AssetDatabase.Refresh();
            Debug.LogFormat("LR.ResCheck:ResRuleData:DeleteRuleFile. delete finish, path:{0}", ruleAssetsPath);
        }

        /// <summary>
        /// 尝试初始化编辑器下数据
        /// </summary>
        void TryInitEditorData()
        {
            if (m_Importer == null || m_Editor == null)
            {
                m_Importer = AssetImporter.GetAtPath(ruleAssetsPath);
                m_Editor = Editor.CreateEditor(m_Importer);
            }
        }

        /// <summary>
        /// 创建一个新文件名
        /// </summary>
        static string NewFileName(Type resImporterType, string tempLateAssetPath)
        {
            string timeStr = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            string extension = Path.GetExtension(tempLateAssetPath);
            string fileName = string.Format("{0}-{1}{2}", resImporterType.Name, timeStr, extension);
            return fileName;
        }
    }
}
