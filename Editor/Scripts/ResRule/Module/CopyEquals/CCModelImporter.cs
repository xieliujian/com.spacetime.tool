using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    public static class CCModelImporter
    {
        //[MenuItem("Assets/打印选中对象类型")]
        //public static void Test()
        //{
        //    string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        //    var ai = AssetImporter.GetAtPath(assetPath) as ModelImporter;

        //    ai.SaveAndReimport();
        //    Debug.LogError(ai.GetType());
        //    AssetDatabase.SaveAssets();
        //}

        /// <summary>
        /// 对拷模型资源信息
        /// </summary>
        public static bool Copy(ResRuleData ruleData, ModelImporter source, ModelImporter dest)
        {
            if (ruleData == null || source == null || dest == null)
            {
                return false;
            }

            // Scene
            dest.globalScale = source.globalScale;
            dest.useFileScale = source.useFileScale;

            if (ruleData.modelData.isOverrideImportBlendShapes)
            {
                dest.importBlendShapes = source.importBlendShapes;
            }
            
            dest.importVisibility = source.importVisibility;
            dest.importCameras = source.importCameras;
            dest.importLights = source.importLights;
            dest.preserveHierarchy = source.preserveHierarchy;

            // Meshes
            dest.meshCompression = source.meshCompression;
            dest.isReadable = source.isReadable;
            dest.optimizeMeshPolygons = source.optimizeMeshPolygons;
            dest.optimizeMeshVertices = source.optimizeMeshVertices;
            dest.addCollider = source.addCollider;

            // Geometry
            dest.keepQuads = source.keepQuads;
            dest.weldVertices = source.weldVertices;
            dest.indexFormat = source.indexFormat;
            dest.importBlendShapeNormals = source.importBlendShapeNormals;
            dest.importNormals = source.importNormals;
            dest.normalCalculationMode = source.normalCalculationMode;
            dest.normalSmoothingSource = source.normalSmoothingSource;
            dest.normalSmoothingAngle = source.normalSmoothingAngle;
            dest.importTangents = source.importTangents;
            dest.swapUVChannels = source.swapUVChannels;
            dest.generateSecondaryUV = source.generateSecondaryUV;
            dest.secondaryUVHardAngle = source.secondaryUVHardAngle;
            dest.secondaryUVPackMargin = source.secondaryUVPackMargin;
            dest.secondaryUVAngleDistortion = source.secondaryUVAngleDistortion;
            dest.secondaryUVAreaDistortion = source.secondaryUVAreaDistortion;

            // Rig
            dest.animationType = source.animationType;
            dest.generateAnimations = source.generateAnimations;
            if (ruleData.modelData.isOverrideOptimizeGameObjects)
            {
                dest.optimizeGameObjects = source.optimizeGameObjects;
            }

            // Animation
            dest.importConstraints = source.importConstraints;
            dest.importAnimation = source.importAnimation;
            dest.bakeIK = source.bakeIK;

            if (ruleData.modelData.isOverrideResampleCurves)
            {
                dest.resampleCurves = source.resampleCurves;
            }
            
            if (ruleData.modelData.isOverrideAnimationCompress)
            {
                dest.animationCompression = source.animationCompression;
                dest.animationRotationError = source.animationRotationError;
                dest.animationPositionError = source.animationPositionError;
                dest.animationScaleError = source.animationScaleError;
            }
            
            // Materials
            dest.materialImportMode = source.materialImportMode;
            return true;
        }

        /// <summary>
        /// 模型资源对比
        /// </summary>
        public static bool IsEqual(ResRuleData ruleData, ModelImporter x, ModelImporter y, IExtraProcess extraProcess)
        {
            if (ruleData == null || x == null || y == null)
            {
                return false;
            }

            if (ruleData.modelData.isOverrideOptimizeGameObjects && (x.optimizeGameObjects != y.optimizeGameObjects))
            {
                return false;
            }

            bool isExtraOverrideAnimationCompress = extraProcess == null ? false : extraProcess.GetIsOverride("isOverrideAnimationCompress");
            if (!isExtraOverrideAnimationCompress &&
                ruleData.modelData.isOverrideAnimationCompress && (x.animationCompression != y.animationCompression  ||
                                                                   x.animationRotationError != y.animationRotationError  ||
                                                                   x.animationPositionError != y.animationPositionError  ||
                                                                   x.animationScaleError != y.animationScaleError))
            {
                return false;
            }

            if (ruleData.modelData.isOverrideImportBlendShapes && (x.importBlendShapes != y.importBlendShapes))
            {
                return false;
            }
            
            bool isExtraOverrideResampleCurves = extraProcess == null ? false : extraProcess.GetIsOverride("isOverrideResampleCurves");
            if (!isExtraOverrideResampleCurves &&
                ruleData.modelData.isOverrideResampleCurves && (x.resampleCurves != y.resampleCurves))
            {
                return false;
            }

            return // Scene
                x.globalScale == y.globalScale &&
                x.useFileScale == y.useFileScale  &&
                x.importVisibility == y.importVisibility  &&
                x.importCameras == y.importCameras  &&
                x.importLights == y.importLights  &&
                x.preserveHierarchy == y.preserveHierarchy  &&

                // Meshes
                x.meshCompression == y.meshCompression  &&
                x.isReadable == y.isReadable  &&
                x.optimizeMeshPolygons == y.optimizeMeshPolygons  &&
                x.optimizeMeshVertices == y.optimizeMeshVertices  &&
                x.addCollider == y.addCollider &&

                // Geometry
                x.keepQuads == y.keepQuads  &&
                x.weldVertices == y.weldVertices  &&
                x.indexFormat == y.indexFormat  &&
                x.importBlendShapeNormals == y.importBlendShapeNormals  &&
                x.importNormals == y.importNormals  &&
                x.normalCalculationMode == y.normalCalculationMode  &&
                x.normalSmoothingSource == y.normalSmoothingSource  &&
                x.normalSmoothingAngle == y.normalSmoothingAngle  &&
                x.importTangents == y.importTangents  &&
                x.swapUVChannels == y.swapUVChannels  &&
                x.generateSecondaryUV == y.generateSecondaryUV  &&
                x.secondaryUVHardAngle == y.secondaryUVHardAngle  &&
                x.secondaryUVPackMargin == y.secondaryUVPackMargin  &&
                x.secondaryUVAngleDistortion == y.secondaryUVAngleDistortion  &&
                x.secondaryUVAreaDistortion == y.secondaryUVAreaDistortion  &&

                // Rig
                x.animationType == y.animationType  &&
                x.generateAnimations == y.generateAnimations  &&

                // Animation
                x.importConstraints == y.importConstraints  &&
                x.importAnimation == y.importAnimation  &&
                x.bakeIK == y.bakeIK  &&

                // Materials
                x.materialImportMode == y.materialImportMode;
        }
    }
}
