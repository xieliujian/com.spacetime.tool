using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using ST.Core;

namespace ST.Tool
{
    /// <summary>
    /// 材质贴图替换编辑器窗口：指定源贴图与目标贴图后，
    /// 批量替换工程内所有文件对源贴图的GUID 引用。
    /// </summary>
    public class MaterialTexReplaceEditor : EditorWindow
    {
        /// <summary>
        /// 打开材质贴图替换编辑器窗口，菜单位于 <c>SpaceTime/Tool/材质贴图替换编辑器</c>。
        /// </summary>
        [MenuItem("SpaceTime/Tool/材质贴图替换编辑器", false, 300)]
        static void OnOpen()
        {
            MaterialTexReplaceEditor window = ScriptableObject.CreateInstance<MaterialTexReplaceEditor>();
            window.titleContent = new GUIContent("材质贴图替换编辑器");
            window.Show();
        }

        /// <summary>
        /// 需要被替换的源贴图。
        /// </summary>
        Texture m_SrcTex;

        /// <summary>
        /// 替换后使用的目标贴图。
        /// </summary>
        Texture m_DstTex;

        /// <summary>
        /// 绘制源贴图，目标贴图字段与替换按钮。
        /// </summary>
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            m_SrcTex = EditorGUILayout.ObjectField("源贴图", m_SrcTex, typeof(Texture), false) as Texture;
            m_DstTex = EditorGUILayout.ObjectField("目标贴图", m_DstTex, typeof(Texture), false) as Texture;

            if (GUILayout.Button("替换贴图"))
            {
                ReplaceAllTexture();
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 获取工程内所有材质资产。
        /// </summary>
        public static List<Material> GetAllMaterials()
        {
            List<Material> mats = new List<Material>();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in assetPaths)
            {
                if (!assetPath.EndsWith(".mat"))
                {
                    continue;
                }

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (mat != null)
                {
                    mats.Add(mat);
                }
            }

            return mats;
        }

        /// <summary>
        /// 遍历所有材质文件，将文件中源贴图的GUID替换为目标贴图的GUID并保存。
        /// </summary>
        void ReplaceAllTexture()
        {
            if (m_SrcTex == null || m_DstTex == null)
                return;

            var allmat = GetAllMaterials();
            if (allmat == null)
                return;

            int count = allmat.Count;
            if (count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                var mat = allmat[i];

                EditorUtility.DisplayProgressBar("ReplaceAllTexture", $"ReplaceAllTexture {i}/{count}", (float)(i) / count);

                var matPath = AssetDatabase.GetAssetPath(mat);
                var shader = mat.shader;
                if (shader == null)
                    continue;

                var srcTexPath = AssetDatabase.GetAssetPath(m_SrcTex);
                var dstTexPath = AssetDatabase.GetAssetPath(m_DstTex);
                var srcTexGUID = AssetDatabase.GUIDFromAssetPath(srcTexPath);
                var dstTexGUID = AssetDatabase.GUIDFromAssetPath(dstTexPath);

                var absMatPath = EditorUtils.AssetsPath2ABSPath(matPath);
                var text = File.ReadAllText(absMatPath);
                if (!text.Contains(srcTexGUID.ToString()))
                    continue;

                text = text.Replace(srcTexGUID.ToString(), dstTexGUID.ToString());

                File.WriteAllText(absMatPath, text);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
    }
}