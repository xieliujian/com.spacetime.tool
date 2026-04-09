using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    public static class CCTextureImporter
    {
        static TextureImporterSettings s_TiSetting = new TextureImporterSettings();

        static TextureImporterSettings s_TiSetting1 = new TextureImporterSettings();
        static TextureImporterSettings s_TiSetting2 = new TextureImporterSettings();

        static TextureImporterSettings s_TiSettingSrc = new TextureImporterSettings();
        static TextureImporterSettings s_TiSettingDst = new TextureImporterSettings();

        /// <summary>
        /// 对拷图片资源信息
        /// </summary>
        public static bool Copy(ResRuleData ruleData, TextureImporter source, TextureImporter dest)
        {
            if (ruleData == null || source == null || dest == null)
            {
                return false;
            }

            source.ReadTextureSettings(s_TiSettingSrc);
            dest.ReadTextureSettings(s_TiSettingDst);

            s_TiSettingDst.spriteMeshType = s_TiSettingSrc.spriteMeshType;
            s_TiSettingDst.spriteGenerateFallbackPhysicsShape = s_TiSettingSrc.spriteGenerateFallbackPhysicsShape;
            
            TextureImporterPlatformSettings dstDefaultSetting = dest.GetPlatformTextureSettings("DefaultTexturePlatform");
            TextureImporterCompression dstTextureCompression = dstDefaultSetting.textureCompression;
            
            dest.SetTextureSettings(s_TiSettingDst);
            
            var overrideType = ruleData.textureData.overrideType;

            if ((overrideType & TextureImporterOverrideType.textureType) != 0)
            {
                dest.textureType = source.textureType;
            }
            
            if ((overrideType & TextureImporterOverrideType.textureShape) != 0)
            {
                dest.textureShape = source.textureShape;
            }

            if ((overrideType & TextureImporterOverrideType.sRGBTexture) != 0)
            {
                dest.sRGBTexture = source.sRGBTexture;
            }

            if ((overrideType & TextureImporterOverrideType.alphaSource) != 0)
            {
                dest.alphaSource = source.alphaSource;
            }

            if ((overrideType & TextureImporterOverrideType.isReadable) != 0)
            {
                dest.isReadable = source.isReadable;
            }

            if ((overrideType & TextureImporterOverrideType.alphaIsTransparency) != 0)
            {
                dest.alphaIsTransparency = source.alphaIsTransparency;
            }

            if ((overrideType & TextureImporterOverrideType.ignorePngGamma) != 0)
            {
                dest.ignorePngGamma = source.ignorePngGamma;
            }

            if ((overrideType & TextureImporterOverrideType.spriteImportMode) != 0)
            {
                dest.spriteImportMode = source.spriteImportMode;
            }

            if ((overrideType & TextureImporterOverrideType.spritePixelsPerUnit) != 0)
            {
                dest.spritePixelsPerUnit = source.spritePixelsPerUnit;
            }

            if ((overrideType & TextureImporterOverrideType.isReadable) != 0)
            {
                dest.isReadable = source.isReadable;
            }

            if ((overrideType & TextureImporterOverrideType.streamingMipmaps) != 0)
            {
                dest.streamingMipmaps = source.streamingMipmaps;
                dest.streamingMipmapsPriority = source.streamingMipmapsPriority;
            }

            if ((overrideType & TextureImporterOverrideType.mipmapEnabled) != 0)
            {
                dest.mipmapEnabled = source.mipmapEnabled;
            }

            if ((overrideType & TextureImporterOverrideType.wrapMode) != 0)
            {
                dest.wrapMode = source.wrapMode;
            }

            if ((overrideType & TextureImporterOverrideType.filterMode) != 0)
            {
                dest.filterMode = source.filterMode;
            }

            if ((overrideType & TextureImporterOverrideType.anisoLevel) != 0)
            {
                dest.anisoLevel = source.anisoLevel;
            }

            if ((overrideType & TextureImporterOverrideType.maxTextureSize) != 0)
            {
                dest.maxTextureSize = source.maxTextureSize;
            }

            if ((overrideType & TextureImporterOverrideType.textureCompression) != 0)
            {
                dest.textureCompression = source.textureCompression;
            }

            if (ruleData.textureData.isOverrideNPOT)
            {
                dest.npotScale = source.npotScale;
            }

            SetPlatformSettings(source, dest, "Standalone", overrideType);
            SetPlatformSettings(source, dest, "iPhone", overrideType);
            SetPlatformSettings(source, dest, "Android", overrideType);
            
            dstDefaultSetting = dest.GetPlatformTextureSettings("DefaultTexturePlatform");
            dstDefaultSetting.textureCompression = dstTextureCompression;
            dest.SetPlatformTextureSettings(dstDefaultSetting);

            return true;
        }

        /// <summary>
        /// 图片资源对比
        /// </summary>
        public static bool IsEqual(ResRuleData ruleData, TextureImporter data1, TextureImporter data2)
        {
            if (ruleData == null || data1 == null || data2 == null)
            {
                return false;
            }
            
            var overrideType = ruleData.textureData.overrideType;
            data1.ReadTextureSettings(s_TiSetting1);
            data2.ReadTextureSettings(s_TiSetting2);

            bool value = true;
            
            value = value && (s_TiSettingDst.spriteMeshType == s_TiSettingSrc.spriteMeshType);
            value = value && (s_TiSettingDst.spriteGenerateFallbackPhysicsShape == s_TiSettingSrc.spriteGenerateFallbackPhysicsShape);
            
            if ((overrideType & TextureImporterOverrideType.textureType) != 0)
            {
                value = value && (data1.textureType == data2.textureType);
            }
            
            if ((overrideType & TextureImporterOverrideType.textureShape) != 0)
            {
                value = value && (data1.textureShape == data2.textureShape);
            }

            if ((overrideType & TextureImporterOverrideType.sRGBTexture) != 0)
            {
                value = value && (data1.sRGBTexture == data2.sRGBTexture);
            }

            if ((overrideType & TextureImporterOverrideType.alphaSource) != 0)
            {
                value = value && (data1.alphaSource == data2.alphaSource);
            }

            if ((overrideType & TextureImporterOverrideType.isReadable) != 0)
            {
                value = value && (data1.isReadable == data2.isReadable);
            }

            if ((overrideType & TextureImporterOverrideType.alphaIsTransparency) != 0)
            {
                value = value && (data1.alphaIsTransparency == data2.alphaIsTransparency);
            }

            if ((overrideType & TextureImporterOverrideType.ignorePngGamma) != 0)
            {
                value = value && (data1.ignorePngGamma == data2.ignorePngGamma);
            }

            if ((overrideType & TextureImporterOverrideType.spriteImportMode) != 0)
            {
                value = value && (data1.spriteImportMode == data2.spriteImportMode);
            }

            if ((overrideType & TextureImporterOverrideType.spritePixelsPerUnit) != 0)
            {
                value = value && (data1.spritePixelsPerUnit == data2.spritePixelsPerUnit);
            }

            if ((overrideType & TextureImporterOverrideType.isReadable) != 0)
            {
                value = value && (data1.isReadable == data2.isReadable);
            }

            if ((overrideType & TextureImporterOverrideType.streamingMipmaps) != 0)
            {
                value = value && (data1.streamingMipmaps == data2.streamingMipmaps);
                value = value && (data1.streamingMipmapsPriority == data2.streamingMipmapsPriority);
            }

            if ((overrideType & TextureImporterOverrideType.mipmapEnabled) != 0)
            {
                value = value && (data1.mipmapEnabled == data2.mipmapEnabled);
            }

            if ((overrideType & TextureImporterOverrideType.wrapMode) != 0)
            {
                value = value && (data1.wrapMode == data2.wrapMode);
            }

            if ((overrideType & TextureImporterOverrideType.filterMode) != 0)
            {
                value = value && (data1.filterMode == data2.filterMode);
            }

            if ((overrideType & TextureImporterOverrideType.anisoLevel) != 0)
            {
                value = value && (data1.anisoLevel == data2.anisoLevel);
            }

            if ((overrideType & TextureImporterOverrideType.maxTextureSize) != 0)
            {
                value = value && (data1.maxTextureSize == data2.maxTextureSize);
            }

            if ((overrideType & TextureImporterOverrideType.textureCompression) != 0)
            {
                //value = value && (data1.textureCompression == data2.textureCompression);
            }

            if (ruleData.textureData.isOverrideNPOT)
            {
                value = value && (data1.npotScale == data2.npotScale);
            }

            value = value && EqualsPlatformSettings(data2, data1, "Standalone", overrideType);
            value = value && EqualsPlatformSettings(data2, data1, "iPhone", overrideType);
            value = value && EqualsPlatformSettings(data2, data1, "Android", overrideType);

            return value;
        }

        /// <summary>
        /// 设置图片不同平台信息
        /// </summary>
        static void SetPlatformSettings(TextureImporter source, TextureImporter dest, string platformName, TextureImporterOverrideType overrideType)
        {
            TextureImporterPlatformSettings sourceSetting = source.GetPlatformTextureSettings(platformName);
            TextureImporterPlatformSettings destSetting = dest.GetPlatformTextureSettings(platformName);

            if ((overrideType & TextureImporterOverrideType.platformSettings_MaxTextureSize) != 0)
            {
                bool isMobile = platformName == "iPhone" || platformName == "Android";
                int mobileMaxSize = CCTextureImporterUserData.GetPropInt(dest, CCTextureImporterUserData.PROP_MOBILE_MAX_SIZE);

                if (isMobile && mobileMaxSize > 0)
                {
                    destSetting.maxTextureSize = mobileMaxSize;
                }
                else
                {
                    destSetting.maxTextureSize = sourceSetting.maxTextureSize;
                }
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_ResizeAlgorithm) != 0)
            {
                destSetting.resizeAlgorithm = sourceSetting.resizeAlgorithm;
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_Format) != 0)
            {
                destSetting.format = sourceSetting.format;
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_CompressionQuality) != 0)
            {
                destSetting.compressionQuality = sourceSetting.compressionQuality;
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_Overridden) != 0)
            {
                destSetting.overridden = sourceSetting.overridden;
            }

            if (platformName == "Android")
            {
                destSetting.androidETC2FallbackOverride = sourceSetting.androidETC2FallbackOverride;
            }

            dest.SetPlatformTextureSettings(destSetting);
        }

        /// <summary>
        /// 对比图片不同平台信息
        /// </summary>
        static bool EqualsPlatformSettings(TextureImporter x, TextureImporter y, string platformName, TextureImporterOverrideType overrideType)
        {
            TextureImporterPlatformSettings destSetting = x.GetPlatformTextureSettings(platformName);
            TextureImporterPlatformSettings sourceSetting = y.GetPlatformTextureSettings(platformName);

            bool value = true;
            
            if ((overrideType & TextureImporterOverrideType.platformSettings_MaxTextureSize) != 0)
            {
                bool isMobile = platformName == "iPhone" || platformName == "Android";
                int mobileMaxSize = CCTextureImporterUserData.GetPropInt(x, CCTextureImporterUserData.PROP_MOBILE_MAX_SIZE);

                if (isMobile && mobileMaxSize > 0)
                {
                    value = value && (destSetting.maxTextureSize == mobileMaxSize);
                }
                else
                {
                    value = value && (destSetting.maxTextureSize == sourceSetting.maxTextureSize);
                }
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_ResizeAlgorithm) != 0)
            {
                value = value && (destSetting.resizeAlgorithm == sourceSetting.resizeAlgorithm);
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_Format) != 0)
            {
                value = value && (destSetting.format == sourceSetting.format);
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_CompressionQuality) != 0)
            {
                value = value && (destSetting.compressionQuality == sourceSetting.compressionQuality);
            }

            if ((overrideType & TextureImporterOverrideType.platformSettings_Overridden) != 0)
            {
                value = value && (destSetting.overridden == sourceSetting.overridden);
            }

            if (platformName == "Android")
            {
                value = value && destSetting.androidETC2FallbackOverride == sourceSetting.androidETC2FallbackOverride;
            }

            return value;
        }
    }
}
