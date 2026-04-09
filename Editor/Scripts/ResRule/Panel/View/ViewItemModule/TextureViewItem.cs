using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

using Object = UnityEngine.Object;

namespace ST.Tool
{
    internal class TextureViewItem : TreeViewItemBase
    {
        public string Path;
        public long CompressSize;
        public long Size;
        public TextureImporterFormat WindeowFormat;
        public TextureImporterFormat IOSFormat;
        public TextureImporterFormat AndroidFormat;
        public TextureImporterType TextureType;
        public TextureImporterAlphaSource AlphaSource;
        public bool sRGB;
        public bool Read_Write;
        public bool CenerateMipMaps;
        public TextureWrapMode WrapMode;

        public TextureImporter textureImporter;

        static MethodInfo s_TextureUtilGetStorageMemorySizeLongMethodInfo = null;

        internal static TextureViewItem Create(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                return null;
            }

            TextureViewItem tvd = new TextureViewItem();
            tvd.displayName = assetPath;

            tvd.textureImporter = importer;
            tvd.Path = assetPath;
            tvd.CompressSize = GetFileSize(assetPath);
            tvd.Size = GetTexSize(importer);
            tvd.WindeowFormat = importer.GetPlatformTextureSettings("Standalone").format;
            tvd.IOSFormat = importer.GetPlatformTextureSettings("iPhone").format;
            tvd.AndroidFormat = importer.GetPlatformTextureSettings("Android").format;
            tvd.TextureType = importer.textureType;
            tvd.AlphaSource = importer.alphaSource;
            tvd.sRGB = importer.sRGBTexture;
            tvd.Read_Write = importer.isReadable;
            tvd.CenerateMipMaps = importer.mipmapEnabled;
            tvd.WrapMode = importer.wrapMode;

            return tvd;
        }

        protected override bool IsIdentical()
        {
            return ResRuleHelper.IsEqual(ruleData, textureImporter);
        }

        static long GetFileSize(string assetPath)
        {
            if (s_TextureUtilGetStorageMemorySizeLongMethodInfo == null)
            {
                var type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
                //var type = Types.GetType ("UnityEditor.TextureUtil", "UnityEditor.dll");
                s_TextureUtilGetStorageMemorySizeLongMethodInfo = type.GetMethod ("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            }
            
            Texture t = AssetDatabase.LoadAssetAtPath<Object>(assetPath) as Texture;

            if (t == null)
            {
                return 0;
            }

            
            return (long)s_TextureUtilGetStorageMemorySizeLongMethodInfo.Invoke(null,new object[]{ t });
            //return Profiler.GetRuntimeMemorySizeLong(t) / 2;
        }

        static int GetTexSize(TextureImporter ai)
        {
#if UNITY_ANDROID
            string platform = "android";
#elif UNITY_IPHONE
            string platform = "ios";
#else
            string platform = "standalone";
#endif
            if (ai == null)
            {
                return 0;
            }
            
            TextureImporterPlatformSettings settings = ai.GetPlatformTextureSettings(platform);
            Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(ai.assetPath);
            
            if (tex == null)
            {
                return 0;
            }

            return Mathf.Max(tex.height, tex.width);
        }
    }
}
