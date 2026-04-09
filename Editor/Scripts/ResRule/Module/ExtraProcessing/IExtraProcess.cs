using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    /// <summary>
    /// 资源额外处理接口
    /// </summary>
    public interface IExtraProcess
    {
        Type GetImporterType();
        string GetName();
        string GetDesc();
        void OnExecute(ResRuleData ruleData, AssetImporter assetImporter);
        bool OnEqual(ResRuleData ruleData, AssetImporter assetImporter);
        bool GetIsOverride(string key);
    }

    /// <summary>
    /// 资源额外处理接口
    /// </summary>
    public abstract class ExtraProcessingBase<T> : ScriptableSingleton<T>, IExtraProcess where T : ExtraProcessingBase<T>
    {
        public abstract Type GetImporterType();
        public abstract string GetName();
        public abstract string GetDesc();
        public abstract void OnExecute(ResRuleData ruleData, AssetImporter assetImporter);
        public abstract bool OnEqual(ResRuleData ruleData, AssetImporter assetImporter);
        public abstract bool GetIsOverride(string key);
    }
}
