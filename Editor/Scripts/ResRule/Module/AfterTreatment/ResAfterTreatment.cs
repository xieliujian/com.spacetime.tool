using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    /// <summary>
    /// 注意：自定义的AssetPostprocessor应该以dll的方式调用，可以确保即使脚本有编译错误，也可以始终执行它们
    /// </summary>
    public class ResAfterTreatment : AssetPostprocessor
    {
        private static bool s_Enable = true;
        private static int s_CurFrameCount = -1;
        private static HashSet<string> s_CurFrameImportedSet = new HashSet<string>();

        public static void SetEnable(bool isEnable)
        {
            s_Enable = isEnable;
        }
        
        /// <summary>
        /// 模型导入之前调用
        /// </summary>
        void OnPreprocessModel()
        {
            TryCleanCurFrameImportedSet();
            Preprocess();
        }

        /// <summary>
        /// 纹理导入之前调用，针对入到的纹理进行设置
        /// </summary>
        void OnPreprocessTexture()
        {
            TryCleanCurFrameImportedSet();
            Preprocess();
        }

        /// <summary>
        /// 导入声音之前调用
        /// </summary>
        void OnPreprocessAudio()
        {
            TryCleanCurFrameImportedSet();
            Preprocess();
        }

        /// <summary>
        /// 导入Animation之前调用
        /// </summary>
        void OnPreprocessAnimation()
        {
            TryCleanCurFrameImportedSet();
            Preprocess();
        }

        /// <summary>
        /// 处理导入的资源信息
        /// </summary>
        void Preprocess()
        {
            if (!s_Enable)
            {
                return;
            }

            s_CurFrameImportedSet.Add(this.assetPath);
            
            // 工具目录资源不处理
            if (ResRuleHelper.CheckPathIsInToolDirectory(this.assetPath))
            {
                return;
            }

            // 不是工程下的目录不处理
            if (!this.assetPath.StartsWith(ResRuleDefine.CHECK_NORMAL_DIR))
            {
                return;
            }

            var module = ResRuleManagerAsset.GetCurProjectResRuleManagerAsset();

            if (module == null)
            {
                return;
            }

            Type importerType = assetImporter.GetType();
            var ruleData = module.GetRuleData(importerType, this.assetPath);

            if (ruleData == null || ruleData.importer == null)
            {
                return;
            }

            // 拷贝信息
            ResRuleHelper.Copy(ruleData, ruleData.importer, assetImporter);

            // 执行额外的处理
            ruleData.ExecuteOtherChanged(assetImporter);
        }

        /// <summary>
        /// 所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            TryCleanCurFrameImportedSet();
            
            if (!s_Enable)
            {
                return;
            }
            
            var module = ResRuleManagerAsset.GetCurProjectResRuleManagerAsset();
            
            // 移动后重新执行导入
            foreach (string str in movedAssets)
            {
                AssetImporter ai = AssetImporter.GetAtPath(str);
                if (module == null || ai == null)
                {
                    continue;
                }
                
                Type importerType = ai.GetType();
                var ruleData = module.GetRuleData(importerType, str);

                if (ruleData != null && ruleData.importer != null && !s_CurFrameImportedSet.Contains(str))
                {
                    s_CurFrameImportedSet.Add(str);
                    AssetDatabase.ImportAsset(str);
                }
            }
            
            // 新导入后重新执行导入
            foreach (string str in importedAsset)
            {
                AssetImporter ai = AssetImporter.GetAtPath(str);
                if (module == null || ai == null)
                {
                    continue;
                }
                
                Type importerType = ai.GetType();
                var ruleData = module.GetRuleData(importerType, str);

                if (ruleData != null && ruleData.importer != null && !s_CurFrameImportedSet.Contains(str))
                {
                    s_CurFrameImportedSet.Add(str);
                    AssetDatabase.ImportAsset(str);
                }
            }

            // 资源改变后删除缓存的显示数据
            ViewItemModule.S.RemoveDataByAssetPath(importedAsset);
            ViewItemModule.S.RemoveDataByAssetPath(deletedAssets);
            ViewItemModule.S.RemoveDataByAssetPath(movedAssets);
            ViewItemModule.S.RemoveDataByAssetPath(movedFromAssetPaths);
        }

        static void TryCleanCurFrameImportedSet()
        {
            if (Time.frameCount != s_CurFrameCount)
            {
                s_CurFrameImportedSet.Clear();
                s_CurFrameCount = Time.frameCount;
            }
        }
    }
}
