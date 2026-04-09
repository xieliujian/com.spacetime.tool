using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    public static class CCAudioImporter
    {
        /// <summary>
        /// 对拷图片资源信息
        /// </summary>
        public static bool Copy(ResRuleData ruleData, AudioImporter source, AudioImporter dest)
        {
            if (ruleData == null || source == null || dest == null)
            {
                return false;
            }

            dest.defaultSampleSettings = source.defaultSampleSettings;

            dest.forceToMono = source.forceToMono;
            dest.ambisonic = source.ambisonic;
            dest.loadInBackground = source.loadInBackground;
#if !UNITY_2022_2_OR_NEWER
            dest.preloadAudioData = source.preloadAudioData;
#endif       

            SetPlatformSettings(source, dest, "Standalone");
            SetPlatformSettings(source, dest, "iPhone");
            SetPlatformSettings(source, dest, "Android");
            return true;
        }

        /// <summary>
        /// 图片资源对比
        /// </summary>
        public static bool IsEqual(ResRuleData ruleData, AudioImporter x, AudioImporter y)
        {
            if (ruleData == null || x == null || y == null)
            {
                return false;
            }

            return EqualsSampleSettings(x.defaultSampleSettings, y.defaultSampleSettings) &&
                x.forceToMono == y.forceToMono &&
                x.ambisonic == y.ambisonic &&
                x.loadInBackground == y.loadInBackground &&
#if !UNITY_2022_2_OR_NEWER
                x.preloadAudioData == y.preloadAudioData &&
#endif
                EqualsPlatformSettings(y, x, "Standalone") &&
                EqualsPlatformSettings(y, x, "iPhone") &&
                EqualsPlatformSettings(y, x, "Android");
        }

        /// <summary>
        /// 设置图片不同平台信息
        /// </summary>
        static void SetPlatformSettings(AudioImporter source, AudioImporter dest, string platformName)
        {
            AudioImporterSampleSettings sourceSetting = source.GetOverrideSampleSettings(platformName);
            AudioImporterSampleSettings destSetting = dest.GetOverrideSampleSettings(platformName);

            destSetting.loadType = sourceSetting.loadType;
            destSetting.sampleRateSetting = sourceSetting.sampleRateSetting;
            destSetting.sampleRateOverride = sourceSetting.sampleRateOverride;
            destSetting.compressionFormat = sourceSetting.compressionFormat;
            destSetting.quality = sourceSetting.quality;
            destSetting.conversionMode = sourceSetting.conversionMode;

            dest.SetOverrideSampleSettings(platformName, destSetting);
        }

        /// <summary>
        /// 对比图片不同平台信息
        /// </summary>
        static bool EqualsPlatformSettings(AudioImporter x, AudioImporter y, string platformName)
        {
            AudioImporterSampleSettings xSetting = x.GetOverrideSampleSettings(platformName);
            AudioImporterSampleSettings ySetting = y.GetOverrideSampleSettings(platformName);

            return EqualsSampleSettings(xSetting, ySetting);
        }

        static bool EqualsSampleSettings(AudioImporterSampleSettings xSetting, AudioImporterSampleSettings ySetting)
        {
            return ySetting.loadType == xSetting.loadType &&
                ySetting.sampleRateSetting == xSetting.sampleRateSetting &&
                ySetting.sampleRateOverride == xSetting.sampleRateOverride &&
                ySetting.compressionFormat == xSetting.compressionFormat &&
                ySetting.quality == xSetting.quality &&
                ySetting.conversionMode == xSetting.conversionMode;
        }
    }
}
