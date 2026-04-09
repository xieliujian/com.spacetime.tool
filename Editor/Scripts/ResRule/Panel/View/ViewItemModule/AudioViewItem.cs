using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace ST.Tool
{
    internal class AudioViewItem : TreeViewItemBase
    {
        public string Path;
        public AudioImporter importer;

        public bool ForceToMono;
        public bool LoadInBackground;
        public AudioClipLoadType LoadType;
        public AudioCompressionFormat WindeowFormat;
        public AudioCompressionFormat IOSFormat;
        public AudioCompressionFormat AndroidFormat;
        public float Quality;
        public AudioSampleRateSetting SamplreRateSetting;
        public long MemorySize;                                 // 内存占用大小
        public float TimeLanght;                                // 时长


        internal static AudioViewItem Create(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;

            if (importer == null)
            {
                return null;
            }

            AudioViewItem tvd = new AudioViewItem();
            tvd.displayName = assetPath;
            tvd.Path = assetPath;
            tvd.importer = importer;
            tvd.ForceToMono = importer.forceToMono;
            tvd.LoadInBackground = importer.loadInBackground;
            tvd.LoadType = importer.GetOverrideSampleSettings("Standalone").loadType;
            tvd.WindeowFormat = importer.GetOverrideSampleSettings("Standalone").compressionFormat;
            tvd.IOSFormat = importer.GetOverrideSampleSettings("iPhone").compressionFormat;
            tvd.AndroidFormat = importer.GetOverrideSampleSettings("Android").compressionFormat;
            tvd.Quality = importer.GetOverrideSampleSettings("Standalone").quality;
            tvd.SamplreRateSetting = importer.GetOverrideSampleSettings("Standalone").sampleRateSetting;
            var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            tvd.MemorySize = GetAudioClipMemorySize(assetPath);
            tvd.TimeLanght = audioClip != null ? audioClip.length : 0;

            return tvd;
        }

        protected override bool IsIdentical()
        {
            return ResRuleHelper.IsEqual(ruleData, importer);
        }

        static long GetAudioClipMemorySize(string assetPath)
        {
            AudioImporter ai = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            
            if (ai == null)
            {
                return 0;
            }
            
            var so = new SerializedObject(ai);
            SerializedProperty sp = so.FindProperty("m_PreviewData.m_CompSize");
            return sp.longValue;
        }
    }
}
