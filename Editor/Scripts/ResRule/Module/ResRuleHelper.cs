using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    internal static class ResRuleHelper
    {
        static List<string> s_PathTempList = new List<string>();

        /// <summary>
        /// 获取规则列表内所有路径
        /// </summary>
        public static string[] GetRulePathsAll(List<ResRuleData> ruleDataList)
        {
            s_PathTempList.Clear();

            if (ruleDataList == null)
            {
                return null;
            }

            for (int i = 0; i < ruleDataList.Count; i++)
            {
                var ruleData = ruleDataList[i];

                if (ruleData != null)
                {
                    s_PathTempList.AddRange(ruleData.assetPathList);
                }
            }

            var result = s_PathTempList.ToArray();
            s_PathTempList.Clear();
            return result;
        }

        /// <summary>
        /// 判定资源是否在工具目录
        /// </summary>
        public static bool CheckPathIsInToolDirectory(string assetPath)
        {
            if (assetPath.StartsWith(ResRuleDefine.RULE_TOOL_DIR))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 对比两个资源属性是否一致
        /// </summary>
        public static bool IsEqual(ResRuleData ruleData, AssetImporter assetImporter)
        {
            if (ruleData == null || ruleData.importer == null || assetImporter == null)
            {
                return false;
            }

            var typeTemplate = ruleData.importer.GetType();
            var typeImporter = assetImporter.GetType();

            // 不同类型的资源不做对比
            if (typeTemplate != typeImporter)
            {
                return false;
            }

            if (typeTemplate == typeof(TextureImporter))
            {
                return CCTextureImporter.IsEqual(ruleData, ruleData.importer as TextureImporter, assetImporter as TextureImporter) && ruleData.IsOtherEqual(assetImporter);
            }
            else if (typeTemplate == typeof(AudioImporter))
            {
                return CCAudioImporter.IsEqual(ruleData, ruleData.importer as AudioImporter, assetImporter as AudioImporter) && ruleData.IsOtherEqual(assetImporter);
            }
            else if (typeTemplate == typeof(ModelImporter))
            {
                return CCModelImporter.IsEqual(ruleData, ruleData.importer as ModelImporter, assetImporter as ModelImporter, ruleData.GetExtraProcess()) && ruleData.IsOtherEqual(assetImporter);
            }

            Debug.LogErrorFormat("RuleHelper:Equals. Unprocessed resource type.  type={0}", typeImporter);
            return false;
        }

        /// <summary>
        /// 对拷两个资源属性信息
        /// </summary>
        public static bool Copy(ResRuleData ruleData, AssetImporter source, AssetImporter dest)
        {
            if (ruleData == null || source == null || dest == null)
            {
                return false;
            }
            
            var sourceType = source.GetType();
            var destType = dest.GetType();

            // 不同类型的资源不做拷贝
            if (sourceType != destType)
            {
                return false;
            }

            if (sourceType == typeof(TextureImporter))
            {
                return CCTextureImporter.Copy(ruleData, source as TextureImporter, dest as TextureImporter);
            }
            else if (sourceType == typeof(AudioImporter))
            {
                return CCAudioImporter.Copy(ruleData, source as AudioImporter, dest as AudioImporter);
            }
            else if (sourceType == typeof(ModelImporter))
            {
                return CCModelImporter.Copy(ruleData, source as ModelImporter, dest as ModelImporter);
            }
            
            return false;
        }
    }
}
