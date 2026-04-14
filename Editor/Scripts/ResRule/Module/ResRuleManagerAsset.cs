using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    [CreateAssetMenu(menuName = "SpaceTime/ResRule/CreateResRuleManagerAsset")]
    internal partial class ResRuleManagerAsset : ScriptableObject
    {
        public List<ResRuleData> ruleDataList;

        /// <summary>
        /// 执行规则
        /// </summary>
        internal void ExecuteRuleAt(int ruleIndex)
        {
            if (ruleDataList == null)
            {
                return;
            }

            var ruleData = ruleDataList.Get(ruleIndex);

            if (ruleData != null)
            {
                ruleData.Execute(true);
            }
        }

        /// <summary>
        /// 执行全部规则
        /// </summary>
        internal void ExecuteRuleAll()
        {
            if (ruleDataList == null)
            {
                return;
            }

            for (int i = 0; i < ruleDataList.Count; i++)
            {
                var ruleData = ruleDataList.Get(i);

                if (ruleData != null)
                {
                    ruleData.Execute(false);
                }
            }
        }

        /// <summary>
        /// 创建规则
        /// </summary>
        internal bool CreateRuleData(Type resImporterType)
        {
            string projectName = EditorFileUtils.GetProjectName();
            string projectAssetDir = Path.Combine(ResRuleDefine.RULE_PROJECT_DIR, projectName);
            var resRuleData = ResRuleData.Create(resImporterType, projectAssetDir);

            if (resRuleData != null)
            {
                ruleDataList.Add(resRuleData);
                return true;
            }

            return false;
        }
        
        internal bool CreateRuleData(Type resImporterType, ResRuleData templateRuleData)
        {
            string projectName = EditorFileUtils.GetProjectName();
            string projectAssetDir = Path.Combine(ResRuleDefine.RULE_PROJECT_DIR, projectName);
            var resRuleData = ResRuleData.Create(resImporterType, projectAssetDir, templateRuleData);

            if (resRuleData != null)
            {
                ruleDataList.Add(resRuleData);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="ruleIndex"></param>
        internal void RemoveRuleAt(int ruleIndex)
        {
            if(ruleIndex < 0 || ruleDataList == null || ruleIndex >= ruleDataList.Count)
            {
                return;
            }

            var ruleData = ruleDataList[ruleIndex];
            ruleDataList.RemoveAt(ruleIndex);

            if (ruleData != null)
            {
                ruleData.DeleteRuleFile();
            }

            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// 根据资源路径获取对应规则
        /// </summary>
        /// <param name="importerType"> 资源类型 </param>
        /// <param name="resAssetFilePath"> 资源路径 </param>
        internal ResRuleData GetRuleData(Type importerType, string resAssetFilePath)
        {
            if (string.IsNullOrEmpty(resAssetFilePath))
            {
                return null;
            }

            if (ruleDataList == null)
            {
                return null;
            }

            for (int i = 0; i < ruleDataList.Count; i++)
            {
                var ruleData = ruleDataList.Get(i);

                if (ruleData == null || ruleData.isDisable)
                {
                    continue;
                }

                // 资源类型不匹配
                if (ruleData.resImporterType != importerType)
                {
                    continue;
                }

                // 该资源不在规则控制范围内
                if (!ruleData.IsContainsPath(resAssetFilePath))
                {
                    continue;
                }

                return ruleData;
            }

            return null;
        }

        /// <summary>
        /// 根据类型获取所有有规则
        /// </summary>
        internal void GetRuleDataList(Type importerType, List<ResRuleData> resultList)
        {
            if (resultList == null)
            {
                return;
            }

            resultList.Clear();

            if (ruleDataList == null)
            {
                return;
            }

            for (int i = 0; i < ruleDataList.Count; i++)
            {
                var ruleData = ruleDataList.Get(i);

                // 资源类型不匹配
                if (ruleData == null || ruleData.resImporterType != importerType)
                {
                    continue;
                }

                resultList.Add(ruleData);
            }
        }

        internal void PullUpRule(int index)
        {
            if (ruleDataList == null || index <= 0 || index >= ruleDataList.Count)
            {
                return;
            }

            ResRuleData preData = ruleDataList[index - 1];
            ResRuleData curData = ruleDataList[index];

            ruleDataList[index - 1] = curData;
            ruleDataList[index] = preData;

            EditorUtility.SetDirty(this);
        }

        internal void PushDownRule(int index)
        {
            if (ruleDataList == null || index < 0 || index >= ruleDataList.Count - 1)
            {
                return;
            }

            ResRuleData lastData = ruleDataList[index + 1];
            ResRuleData curData = ruleDataList[index];

            ruleDataList[index + 1] = curData;
            ruleDataList[index] = lastData;

            EditorUtility.SetDirty(this);
        }
    }
}
