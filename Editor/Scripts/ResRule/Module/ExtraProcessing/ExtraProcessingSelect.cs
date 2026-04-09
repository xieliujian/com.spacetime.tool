using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ST.Tool
{
    public static class ExtraProcessingSelect
    {
#if !UNITY_2020
        static List<Type> m_TempTypeList = new List<Type>();
#endif
        static Dictionary<Type, string[]> m_OptionMap = new Dictionary<Type, string[]>();

        public static string ShowGUI(Type type, string curSelected)
        {

            if (m_OptionMap.Count <= 0)
            {
                GetToolsInfo(m_OptionMap);
            }

            if (!m_OptionMap.TryGetValue(type, out string[] options))
            {
                return string.Empty;
            }

            int curIndex = FindIndex(curSelected, options);
            int newIndex = EditorGUILayout.Popup(curIndex, options);
            string ep = curSelected;

            if (curIndex != newIndex)
            {
                ep = options[newIndex];
            }

            return ep;
        }

        static int FindIndex(string extraProcessing, string[] options)
        {
            if (string.IsNullOrEmpty(extraProcessing) || options == null)
            {
                return -1;
            }

            for (int i = 0; i < options.Length; i++)
            {
                if (extraProcessing == options[i])
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 获取所有工具信息
        /// </summary>
        static void GetToolsInfo(Dictionary<Type, string[]> optionsMap)
        {
            if (optionsMap == null)
            {
                return;
            }


            Dictionary<Type, List<string>> m_TempMap = new Dictionary<Type, List<string>>();
            optionsMap.Clear();
#if !UNITY_2020
            typeof(ExtraProcessingBase<>).GetGenericImpAll(m_TempTypeList);

            foreach (Type type in m_TempTypeList)
#else
            foreach (Type type in TypeCache.GetTypesDerivedFrom(typeof(ExtraProcessingBase<>)))
#endif
            {
                PropertyInfo property = type.GetProperty("instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                MethodInfo getMethod = property.GetGetMethod();
                IExtraProcess extraProcessing = (IExtraProcess)getMethod.Invoke(null, null);
                var importerType = extraProcessing.GetImporterType();
                var extraProcessingName = extraProcessing.GetName();

                if (!m_TempMap.ContainsKey(importerType))
                {
                    m_TempMap[importerType] = new List<string>();
                }

                Assert.IsFalse(m_TempMap[importerType].Contains(extraProcessingName), string.Format("该工具重名:{0}, type={1}", extraProcessingName, extraProcessing.GetType()));
                m_TempMap[importerType].Add(extraProcessingName);
            }

            foreach (var kv in m_TempMap)
            {
                var count = kv.Value.Count;

                if (!optionsMap.ContainsKey(kv.Key))
                {
                    optionsMap[kv.Key] = kv.Value.ToArray();
                }
            }
        }

        internal static IExtraProcess GetExtraProcessing(Type importerType, string extraProcessingName)
        {
            if (string.IsNullOrEmpty(extraProcessingName))
            {
                return null;
            }

#if !UNITY_2020
            typeof(ExtraProcessingBase<>).GetGenericImpAll(m_TempTypeList);

            foreach (Type type in m_TempTypeList)
#else
            foreach (Type type in TypeCache.GetTypesDerivedFrom(typeof(ExtraProcessingBase<>)))
#endif
            {
                PropertyInfo property = type.GetProperty("instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                MethodInfo getMethod = property.GetGetMethod();
                IExtraProcess extraProcessing = (IExtraProcess)getMethod.Invoke(null, null);

                if (importerType == extraProcessing.GetImporterType() && extraProcessingName == extraProcessing.GetName())
                {
                    return extraProcessing;
                }
            }

            return null;
        }
    }
}
