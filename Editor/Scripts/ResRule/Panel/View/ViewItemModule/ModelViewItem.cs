using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    internal class ModelViewItem : TreeViewItemBase
    {
        public string Path;
        public ModelImporter importer;

        public ModelImporterMeshCompression MeshCompression;
        public bool Read_Write;
        public bool OptimizeMesh;
        public ModelImporterNormals Normal;
        public bool UVS;
        public long MeshCount;
        public long VertexCount;
        public long TriCount;
        public int SkinCount;
        public int BoneCount;
        public ModelImporterAnimationType AnimationType;
        public bool OptimizeGameObjects;
        public ModelImporterAnimationCompression AnimCompression;
        public float AnimationClipLength;
        public bool IsLoop;
        public long AnimationClipSize;


        internal static ModelViewItem Create(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (importer == null)
            {
                return null;
            }

            GetOtherInfo(assetPath, out long meshCount1, out long vertexCount1, out long triCount1, out int skinCount1);
            GetAnimationInfo(importer, out float clipLength1, out bool isLoop1);

            ModelViewItem tvd = new ModelViewItem();
            tvd.displayName = assetPath;
            tvd.Path = assetPath;
            tvd.importer = importer;

            tvd.MeshCompression = importer.meshCompression;
            tvd.Read_Write = importer.isReadable;
            tvd.OptimizeMesh = importer.optimizeMeshPolygons && importer.optimizeMeshVertices;
            tvd.Normal = importer.importNormals;
            tvd.UVS = importer.generateSecondaryUV;
            tvd.MeshCount = meshCount1;
            tvd.VertexCount = vertexCount1;
            tvd.TriCount = triCount1;
            tvd.SkinCount = skinCount1;
            tvd.BoneCount = importer.transformPaths.Length;
            tvd.AnimationType = importer.animationType;
            tvd.OptimizeGameObjects = importer.optimizeGameObjects;
            tvd.AnimCompression = importer.animationCompression;
            tvd.AnimationClipLength = clipLength1;
            tvd.IsLoop = isLoop1;
            tvd.AnimationClipSize = EditorFileUtils.GetResSize<AnimationClip>(assetPath);

            return tvd;
        }

        protected override bool IsIdentical()
        {
            return ResRuleHelper.IsEqual(ruleData, importer);
        }


        /// <summary>
        /// 获取动画信息
        /// </summary>
        static void GetAnimationInfo(ModelImporter importer, out float clipLength, out bool isLoop)
        {
            clipLength = 0;
            isLoop = false;

            if (importer == null || importer.clipAnimations == null || importer.clipAnimations.Length <= 0)
            {
                return;
            }

            var clip = importer.clipAnimations[0];

            if (clip != null)
            {
                isLoop = clip.loopTime;
            }

            if (importer.importedTakeInfos == null || importer.importedTakeInfos.Length <= 0)
            {
                return;
            }

            clipLength = importer.importedTakeInfos[0].bakeStopTime - importer.importedTakeInfos[0].bakeStartTime;
        }

        /// <summary>
        /// 获取模型相关信息
        /// </summary>
        static void GetOtherInfo(string assetPath, out long meshCount, out long vertexCount, out long triCount, out int skinCount)
        {
            vertexCount = 0;
            meshCount = 0;
            triCount = 0;
            skinCount = 0;

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (obj == null)
            {
                return;
            }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (skinnedMeshRenderers != null)
            {
                skinCount = skinnedMeshRenderers.Length;

                foreach (var smr in skinnedMeshRenderers)
                {
                    vertexCount += smr.sharedMesh.vertexCount;
                    triCount += smr.sharedMesh.triangles.Length / 3;
                    meshCount++;
                }
            }

            MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>(true);

            if (filters == null)
            {
                return;
            }

            for (int j = 0; j < filters.Length; j++)
            {
                MeshFilter f = filters[j];
                vertexCount += f.sharedMesh.vertexCount;
                triCount += f.sharedMesh.triangles.Length / 3;
                meshCount++;
            }

            //Debug.LogWarning("总共Mesh=" + meshCount + "   总共顶点=" + vertexCount + "   总共三角形=" + triCount);
        }
    }
}
