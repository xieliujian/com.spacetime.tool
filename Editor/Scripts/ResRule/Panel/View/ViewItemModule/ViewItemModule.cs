using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace ST.Tool
{
    internal class ViewItemModule
    {
        static ViewItemModule m_S;
        internal static ViewItemModule S
        {
            get
            {
                if (m_S == null)
                {
                    m_S = new ViewItemModule();
                }

                return m_S;
            }
        }

        Dictionary<string, TreeViewItem> m_DataMap = new Dictionary<string, TreeViewItem>();
        TreeViewItem m_LastData;
        string m_LastUIID;

        internal bool isDerty = false;

        internal T GetData<T>(string guid) where T : TreeViewItem
        {
            if (!string.IsNullOrEmpty(m_LastUIID) && guid == m_LastUIID && m_LastData != null)
            {
                return m_LastData as T;
            }

            Type t = typeof(T);
            TreeViewItem result;

            if (!m_DataMap.TryGetValue(guid, out result) || result == null || result.GetType() != t)
            {
                //Debug.LogError("11111111111111111");
                result = CreateData<T>(guid);
                m_DataMap[guid] = result;
            }

            m_LastUIID = guid;
            m_LastData = result;

            //Debug.LogError("22222222222222222222");
            return result as T;
        }

        internal void Clear()
        {
            m_LastData = null;
            m_LastUIID = string.Empty;
            m_DataMap.Clear();
        }

        /// <summary>
        /// 通过路径删除缓存数据
        /// </summary>
        internal void RemoveDataByAssetPath(string[] assetPaths)
        {
            if (assetPaths == null || assetPaths.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < assetPaths.Length; i++)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPaths[i]);

                if (m_DataMap.Remove(guid))
                {
                    //Debug.Log("delete " + assetPaths[i]);
                    isDerty = true;
                }
            }
        }

        T CreateData<T>(string guid) where T : TreeViewItem
        {
            Type type = typeof(T);

            if (type == typeof(TextureViewItem))
            {
                return TextureViewItem.Create(guid) as T;
            }
            else if (type == typeof(AudioViewItem))
            {
                return AudioViewItem.Create(guid) as T;
            }
            else if (type == typeof(ModelViewItem))
            {
                return ModelViewItem.Create(guid) as T;
            }

            return default(T);
        }
    }
}
