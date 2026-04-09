using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    public enum TextureImporterOverrideType
    {
        nothing = 0,

        textureType  = 1 << 0,
        textureShape = 1 << 1,

        sRGBTexture         = 1 << 2,
        alphaSource         = 1 << 3,
        alphaIsTransparency = 1 << 4,
        ignorePngGamma      = 1 << 5,

        spriteImportMode    = 1 << 6,
        spritePixelsPerUnit = 1 << 7,

        isReadable          = 1 << 8,
        streamingMipmaps    = 1 << 9,
        mipmapEnabled       = 1 << 10,

        wrapMode    = 1 << 11,
        filterMode  = 1 << 12,
        anisoLevel  = 1 << 13,

        maxTextureSize = 1 << 14,
        textureCompression = 1 << 15,

        platformSettings_Overridden         = 1 << 16,
        platformSettings_MaxTextureSize     = 1 << 17,
        platformSettings_ResizeAlgorithm    = 1 << 18,
        platformSettings_Format             = 1 << 19,
        platformSettings_CompressionQuality = 1 << 20, 

        everything = ~0,
    };

    [System.Serializable]
    public class RuleDataModelData
    {
        [SerializeField] 
        public bool isOverrideOptimizeGameObjects = false;

        [SerializeField] 
        public bool isOverrideAnimationCompress = true;
        
        [SerializeField] 
        public bool isOverrideImportBlendShapes = true;
        
        [SerializeField] 
        public bool isOverrideResampleCurves = true;

        public RuleDataModelData Clone()
        {
            RuleDataModelData obj = new RuleDataModelData();
            obj.isOverrideOptimizeGameObjects = isOverrideOptimizeGameObjects;
            obj.isOverrideAnimationCompress = isOverrideAnimationCompress;
            obj.isOverrideImportBlendShapes = isOverrideImportBlendShapes;
            obj.isOverrideResampleCurves = isOverrideResampleCurves;
            return obj;
        }
    }
    
    [System.Serializable]
    public class RuleDataTextureData
    {
        [SerializeField]
        public bool isOverrideNPOT = false;

        [SerializeField]
        public TextureImporterOverrideType overrideType = TextureImporterOverrideType.everything;

        public RuleDataTextureData Clone()
        {
            RuleDataTextureData obj = new RuleDataTextureData();
            obj.isOverrideNPOT = isOverrideNPOT;
            obj.overrideType = overrideType;
            return obj;
        }
    }

    [System.Serializable]
    public partial class ResRuleData
    {
        [SerializeField]
        string m_KeyWord = ".*";
        [SerializeField]
        List<string> m_ExcludeWordList = new List<string>();
        [SerializeField]
        string m_RuleName = string.Empty;
        [SerializeField]
        List<string> m_AssetPathList = new List<string>();
        [SerializeField]
        List<string> m_ExcludeAssetPathList = new List<string>();
        [SerializeField]
        List<string> m_ParameterList = new List<string>();
        [SerializeField]
        string m_ExtraProcessName;
        [SerializeField]
        bool m_IsDisable = false;
        [SerializeField]
        RuleDataModelData m_ModelData = new RuleDataModelData();
        [SerializeField]
        RuleDataTextureData m_TextureData = new RuleDataTextureData();

        Regex m_Regex;
        IExtraProcess m_ExtraProcess;
        HashSet<string> m_ParameterSet;

        public string keyWord
        {
            get { return m_KeyWord; }
            set
            {
                if (m_KeyWord != value)
                {
                    m_KeyWord = value;
                    m_Regex = null;     // 修改keyWorld时需要更新正则表达式，这里置空在第一次使用时重新生成
                }
            }
        }

        public List<string> excludeWordList
        {
            get { return m_ExcludeWordList; }
            set { m_ExcludeWordList = value; }
        }

        public string ruleName
        {
            get { return m_RuleName; }
            set { m_RuleName = value; }
        }

        public List<string> assetPathList
        {
            get { return m_AssetPathList; }
            set { m_AssetPathList = value; }
        }

        public List<string> excludeAssetPathList
        {
            get { return m_ExcludeAssetPathList; }
            set { m_ExcludeAssetPathList = value; }
        }
        
        public List<string> parameterList
        {
            get { return m_ParameterList; }
            set
            {
                m_ParameterList = value;

                if (m_ParameterSet == null)
                {
                    m_ParameterSet = new HashSet<string>();
                }
                
                m_ParameterSet.Clear();

                foreach (var p in m_ParameterList)
                {
                    m_ParameterSet.Add(p);
                }
            }
        }

        public HashSet<string> parameterSet
        {
            get
            {
                if (m_ParameterSet == null || m_ParameterSet.Count != m_ParameterList.Count)
                {
                    m_ParameterSet = new HashSet<string>(m_ParameterList);
                }

                return m_ParameterSet;
            }
        }

        public string extraProcessName
        {
            get { return m_ExtraProcessName; }
            set { m_ExtraProcessName = value; }
        }

        public bool isDisable
        {
            get { return m_IsDisable; }
            set { m_IsDisable = value; }
        }

        public RuleDataModelData modelData
        {
            get { return m_ModelData; }
        }
        
        public RuleDataTextureData textureData
        {
            get { return m_TextureData; }
        }

        public ResRuleData Clone()
        {
            ResRuleData obj = new ResRuleData();

            obj.m_KeyWord = m_KeyWord;
            obj.m_ExcludeWordList.AddRange(m_ExcludeWordList);
            obj.ruleName = m_RuleName;
            obj.m_AssetPathList.AddRange(m_AssetPathList);
            obj.m_ExcludeAssetPathList.AddRange(m_ExcludeAssetPathList);
            obj.m_ParameterList.AddRange(m_ParameterList);
            obj.m_ExtraProcessName = m_ExtraProcessName + "";
            obj.m_IsDisable = m_IsDisable;
            obj.m_ModelData = m_ModelData.Clone();
            obj.m_TextureData = m_TextureData.Clone();
            
            obj.m_Regex = m_Regex;
            obj.m_ExtraProcess = m_ExtraProcess;

            if (m_ParameterSet != null)
            {
                obj.m_ParameterSet = new HashSet<string>();
                foreach (var s in m_ParameterSet)
                {
                    obj.m_ParameterSet.Add(s);
                }
            }
            
            return obj;
        }

        private ResRuleData() { }

        /// <summary>
        /// 执行规则
        /// </summary>
        internal void Execute(bool isShowBar = false)
        {
            var resObj = AssetDatabase.LoadAssetAtPath(ruleAssetsPath, typeof(UnityEngine.Object));
            if (resObj == null)
            {
                Debug.LogError("ResRuleData Execute Error " + ruleAssetsPath);
                return;
            }

            var filter = string.Format("t:{0}", resObj.GetType().Name);
            string[] guids = AssetDatabase.FindAssets(filter, assetPathList.ToArray());

            if (guids == null || guids.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (!IsContainsPath(assetPath))
                {
                    continue;
                }

                if (isShowBar)
                {
                    EditorUtility.DisplayProgressBar("更改中....", assetPath, i * 1f / guids.Length);
                }

                AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                if (!ResRuleHelper.IsEqual(this, ai))
                {
                    AssetDatabase.ImportAsset(assetPath);
                }
            }

            if (isShowBar)
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 执行额外处理
        /// </summary>
        internal void ExecuteOtherChanged(AssetImporter assetImporter)
        {
            if (m_ExtraProcess == null || m_ExtraProcess.GetName() != extraProcessName)
            {
                m_ExtraProcess = ExtraProcessingSelect.GetExtraProcessing(m_ResImporterType, extraProcessName);
            }

            if (m_ExtraProcess != null)
            {
                m_ExtraProcess.OnExecute(this, assetImporter);
            }
        }

        internal bool IsOtherEqual(AssetImporter assetImporter)
        {
            var extraProcess = GetExtraProcess();

            if (extraProcess != null)
            {
                return extraProcess.OnEqual(this, assetImporter);
            }

            return true;
        }

        internal IExtraProcess GetExtraProcess()
        {
            if (m_ExtraProcess == null || m_ExtraProcess.GetName() != extraProcessName)
            {
                m_ExtraProcess = ExtraProcessingSelect.GetExtraProcessing(m_ResImporterType, extraProcessName);
            }
            
            return m_ExtraProcess;
        }

        /// <summary>
        /// 判断资源是否在规则路径下
        /// </summary>
        internal bool IsContainsPath(string resAssetFilePath)
        {
            foreach (var assetDirPath in excludeAssetPathList)
            {
                if (!string.IsNullOrEmpty(assetDirPath) && resAssetFilePath.StartsWith(assetDirPath))
                {
                    return false;
                }
            }
            
            foreach (var excludeWorld in m_ExcludeWordList)
            {
                if (!string.IsNullOrEmpty(excludeWorld) && resAssetFilePath.Contains(excludeWorld))
                {
                    return false;
                }
            }

            foreach (var assetDirPath in assetPathList)
            {
                if (!string.IsNullOrEmpty(assetDirPath) && resAssetFilePath.StartsWith(assetDirPath))
                {
                    if (m_Regex == null)
                    {
                        m_Regex = new Regex(m_KeyWord);
                    }

                    Match m = m_Regex.Match(resAssetFilePath);
                    if (m.Value == resAssetFilePath)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
