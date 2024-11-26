using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CopyScene
    {
        /// <summary>
        /// 
        /// </summary>
        static string SVN_SUFFIX = ".svn";                  // svn文件
        static string META_SUFFIX = ".meta";                // meta文件
        static string ASSET_SUFFIX = ".asset";              // asset文件
        static string MAT_SUFFIX = ".mat";                  // mat文件
        static string PREFAB_SUFFIX = ".prefab";            // prefab文件
        static string UNITY_SUFFIX = ".unity";              // unity文件
        static string CONTROLLER_SUFFIX = ".controller";    // anim文件

        /// <summary>
        /// 不需要复制的目录
        /// </summary>
        static string[] UNCOPY_DIR_ARRAY =
        {
        "-stream",      // 不需要复制的Stream目录
        "/river/"       // 不需要复制的river目录
    };

        /// <summary>
        /// 需要删除的数据
        /// </summary>
        static string[] DEL_DATA_ARRAY =
        {
        "LightingData.asset"        // 需要删除的LightingData
    };

        /// <summary>
        /// .
        /// </summary>
        public class FileInfo
        {
            public string path;
            public List<string> dependList = new List<string>();
        }

        /// <summary>
        /// 源目录
        /// </summary>
        static string m_SrcDir;

        /// <summary>
        /// 目标目录
        /// </summary>
        static string m_DstDir;

        /// <summary>
        /// 源场景名字
        /// </summary>
        static string m_SrcSceneName;

        /// <summary>
        /// 目标场景名字
        /// </summary>
        static string m_DstSceneName;

        /// <summary>
        /// 源文件列表
        /// </summary>
        static List<string> m_SrcFileList = new List<string>();

        /// <summary>
        /// 目标文件目录
        /// </summary>
        static List<string> m_DstFileList = new List<string>();

        /// <summary>
        /// 文件信息列表
        /// </summary>
        static List<FileInfo> m_FileInfoList = new List<FileInfo>();

        /// <summary>
        /// 需要删除的文件列表
        /// </summary>
        static List<string> m_DelFileList = new List<string>();

        /// <summary>
        /// 不能分析的文件列表
        /// </summary>
        static List<string> m_CannotAnalyseList = new List<string>();

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="dstDir"></param>
        public static void Copy(string srcDir, string dstDir, string sceneName)
        {
            m_SrcDir = srcDir;
            m_DstDir = dstDir + "/" + sceneName;
            CalcSrcSceneName();
            m_DstSceneName = sceneName;

            CopyFolder2();
            DstFolderAnalyse();

        }

        /// <summary>
        /// 计算源场景名字
        /// </summary>
        static void CalcSrcSceneName()
        {
            var scenedir = m_SrcDir.Replace("\\", "/");
            var splitArray = scenedir.Split('/');

            if (splitArray == null || splitArray.Length <= 0)
            {
                m_SrcSceneName = "";
            }

            m_SrcSceneName = splitArray[splitArray.Length - 1];
        }

        /// <summary>
        /// 目标目录分析
        /// </summary>
        static void DstFolderAnalyse()
        {
            CollectDstFileList();
            DstFileListAnalyse();
            DstFileListReWrite();
            DelFileListProc();
        }

        /// <summary>
        /// TerrainData的Asset文件重写
        /// </summary>
        /// <param name="fileinfo"></param>
        static void TerrainDataAssetFileReWrite(TerrainData terrain, FileInfo fileinfo)
        {
            List<TerrainLayer> newLayerList = new List<TerrainLayer>();
            var layerArray = terrain.terrainLayers;
            if (layerArray != null)
            {
                for (int i = 0; i < layerArray.Length; i++)
                {
                    var layer = layerArray[i];
                    if (layer == null)
                    {
                        newLayerList.Add(layer);
                        continue;
                    }

                    var assetSrcPath = AssetDatabase.GetAssetPath(layer);
                    var absSrcPath = Core.EditorUtils.AssetsPath2ABSPath(assetSrcPath);
                    var absDstFile = ConvertResSrcPath2DstPath(absSrcPath);
                    if (!File.Exists(absDstFile))
                    {
                        Debug.Log("[CopyScene][UnReWrite] : " + fileinfo.path);
                        newLayerList.Add(layer);
                        continue;
                    }

                    var assetDstFile = Core.EditorUtils.ABSPath2AssetsPath(absDstFile);
                    var dstLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(assetDstFile);
                    if (dstLayer == null)
                    {
                        newLayerList.Add(layer);
                        continue;
                    }

                    newLayerList.Add(dstLayer);
                }
            }

            terrain.terrainLayers = newLayerList.ToArray();

            EditorUtility.SetDirty(terrain);
            AssetDatabase.SaveAssetIfDirty(terrain);
        }

        /// <summary>
        /// asset文件重写
        /// </summary>
        static bool AssetFileReWrite(FileInfo fileinfo)
        {
            bool isExist = false;

            var absPath = fileinfo.path;
            var assetPath = Core.EditorUtils.ABSPath2AssetsPath(absPath);

            var terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(assetPath);
            if (terrainData != null)
            {
                TerrainDataAssetFileReWrite(terrainData, fileinfo);
                isExist = true;
            }

            return isExist;
        }

        /// <summary>
        /// Meta文件重写
        /// </summary>
        /// <param name="fileinfo"></param>
        static void MetaFileReWrite(FileInfo fileinfo)
        {
            var text = File.ReadAllText(fileinfo.path);

            foreach (var dependfile in fileinfo.dependList)
            {
                var assetSrcFile = Core.EditorUtils.ABSPath2AssetsPath(dependfile);
                var srcguid = AssetDatabase.AssetPathToGUID(assetSrcFile);

                var dstfile = ConvertResSrcPath2DstPath(dependfile);
                if (!File.Exists(dstfile))
                {
                    Debug.Log("[CopyScene][UnReWrite] : " + fileinfo.path);
                    continue;
                }

                var assetDstFile = Core.EditorUtils.ABSPath2AssetsPath(dstfile);
                var dstguid = AssetDatabase.AssetPathToGUID(assetDstFile);
                text = text.Replace(srcguid, dstguid);
            }

            File.WriteAllText(fileinfo.path, text);
        }

        /// <summary>
        /// 是否需要删除的文件
        /// </summary>
        /// <param name="fileinfo"></param>
        /// <returns></returns>
        static bool IsDelFile(FileInfo fileinfo)
        {
            bool isdel = false;

            foreach (var delData in DEL_DATA_ARRAY)
            {
                if (fileinfo.path.Contains(delData))
                {
                    isdel = true;
                    break;
                }
            }

            return isdel;
        }

        /// <summary>
        /// 删除文件列表处理
        /// </summary>
        static void DelFileListProc()
        {
            foreach (var file in m_DelFileList)
            {
                var assetPath = Core.EditorUtils.ABSPath2AssetsPath(file);
                AssetDatabase.DeleteAsset(assetPath);
            }

            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 目标文件列表重写
        /// </summary>
        static void DstFileListReWrite()
        {
            m_DelFileList.Clear();
            m_CannotAnalyseList.Clear();

            foreach (var fileinfo in m_FileInfoList)
            {
                if (fileinfo == null)
                    continue;

                var isDel = IsDelFile(fileinfo);
                if (isDel)
                {
                    m_DelFileList.Add(fileinfo.path);
                    continue;
                }

                var isUnity = IsUnityFile(fileinfo.path);
                var isPrefab = IsPrefabFile(fileinfo.path);
                var isMat = IsMatFile(fileinfo.path);
                var isAsset = IsAssetFile(fileinfo.path);
                var isController = IsControllerFile(fileinfo.path);

                if (isUnity || isPrefab || isMat || isController)
                {
                    MetaFileReWrite(fileinfo);
                }
                else if (isAsset)
                {
                    var isAssetExist = AssetFileReWrite(fileinfo);
                    if (!isAssetExist)
                    {
                        m_CannotAnalyseList.Add(fileinfo.path);
                    }
                }
                else
                {
                    m_CannotAnalyseList.Add(fileinfo.path);
                }
            }

            // 未写文件输出
            foreach (var file in m_CannotAnalyseList)
            {
                Debug.Log("[CopyScene][CannotAnalyse] : " + file);
            }
        }

        /// <summary>
        /// 是否asset文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsAssetFile(string filename)
        {
            return filename.Contains(ASSET_SUFFIX);
        }

        /// <summary>
        /// 是否材质文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsMatFile(string filename)
        {
            return filename.Contains(MAT_SUFFIX);
        }

        /// <summary>
        /// 是否prefab文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsPrefabFile(string filename)
        {
            return filename.Contains(PREFAB_SUFFIX);
        }

        /// <summary>
        /// 动画控制器后缀
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsControllerFile(string filename)
        {
            return filename.Contains(CONTROLLER_SUFFIX);
        }

        /// <summary>
        /// 是否unity文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsUnityFile(string filename)
        {
            return filename.Contains(UNITY_SUFFIX);
        }

        /// <summary>
        /// 把资源路径从源路径转换为目标路径
        /// </summary>
        static string ConvertResSrcPath2DstPath(string srcfile)
        {
            srcfile = srcfile.Replace("\\", "/");

            var dstfile = srcfile.Replace(m_SrcDir, "");
            dstfile = m_DstDir + dstfile;

            dstfile = ModifyDstPath(dstfile);

            return dstfile;
        }

        /// <summary>
        /// 目标文件分析
        /// </summary>
        /// <param name="dstfile"></param>
        static void DstFileAnalyse(string dstfile)
        {
            var assetDstFile = Core.EditorUtils.ABSPath2AssetsPath(dstfile);

            var depends = AssetDatabase.GetDependencies(assetDstFile);
            if (depends == null || depends.Length <= 0)
                return;

            List<string> dependlist = new List<string>();
            foreach (var depend in depends)
            {
                if (string.IsNullOrEmpty(depend))
                    continue;

                var assetSrcDir = Core.EditorUtils.ABSPath2AssetsPath(m_SrcDir);
                var isSrcDir = depend.StartsWith(assetSrcDir);
                if (!isSrcDir)
                    continue;

                var abspath = Core.EditorUtils.AssetsPath2ABSPath(depend);
                dependlist.Add(abspath);
            }

            if (dependlist.Count <= 0)
                return;

            FileInfo fileinfo = new FileInfo();
            m_FileInfoList.Add(fileinfo);

            fileinfo.path = dstfile;
            fileinfo.dependList.AddRange(dependlist);
        }

        /// <summary>
        /// 目标文件列表分析
        /// </summary>
        static void DstFileListAnalyse()
        {
            m_FileInfoList.Clear();

            foreach (var dstfile in m_DstFileList)
            {
                DstFileAnalyse(dstfile);
            }
        }

        /// <summary>
        /// 收集目标文件列表
        /// </summary>
        /// <param name="dstDir"></param>
        static void CollectDstFileList()
        {
            m_DstFileList.Clear();

            string[] files = Directory.GetFiles(m_DstDir, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (file.Contains(SVN_SUFFIX) || file.Contains(META_SUFFIX))
                    continue;

                var newfile = file.Replace("\\", "/");

                m_DstFileList.Add(newfile);
            }
        }

        /// <summary>
        /// 是否不可复制文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static bool IsUnCopyFile(string file)
        {
            foreach (var dir in UNCOPY_DIR_ARRAY)
            {
                var unCopyDir = m_SrcSceneName + dir;
                if (file.Contains(unCopyDir))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 复制目录2
        /// </summary>
        static void CopyFolder2()
        {
            // 1.
            m_SrcFileList.Clear();

            string[] files = Directory.GetFiles(m_SrcDir, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (file.Contains(SVN_SUFFIX) || file.Contains(META_SUFFIX))
                    continue;

                // 是否不需要复制的文件
                var isUnCopy = IsUnCopyFile(file);
                if (isUnCopy)
                    continue;

                m_SrcFileList.Add(file);
            }

            // 2.
            foreach (var srcfile in m_SrcFileList)
            {
                var dstfile = ConvertResSrcPath2DstPath(srcfile);

                var dstdir = Path.GetDirectoryName(dstfile);
                if (!Directory.Exists(dstdir))
                {
                    Directory.CreateDirectory(dstdir);
                }

                var assetSrcFile = Core.EditorUtils.ABSPath2AssetsPath(srcfile);
                var assetDstFile = Core.EditorUtils.ABSPath2AssetsPath(dstfile);
                AssetDatabase.CopyAsset(assetSrcFile, assetDstFile);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 修改目标路径
        /// </summary>
        /// <param name="dstfile"></param>
        /// <returns></returns>
        static string ModifyDstPath(string dstfile)
        {
            var newpath = dstfile.Replace(m_SrcSceneName, m_DstSceneName);
            return newpath;
        }

#if false

    /// <summary>
    /// 复制目录
    /// </summary>
    static void CopyFolder()
    {      
        // 1.
        m_SrcFileList.Clear();

        string[] files = Directory.GetFiles(m_SrcDir, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            if (file.Contains(SVN_SUFFIX) || file.Contains(META_SUFFIX))
                continue;

            m_SrcFileList.Add(file);
        }

        // 2.
        foreach (var srcfile in m_SrcFileList)
        {
            var dstfile = ConvertResSrcPath2DstPath(srcfile);

            var dstdir = Path.GetDirectoryName(dstfile);
            if (!Directory.Exists(dstdir))
            {
                Directory.CreateDirectory(dstdir);
            }

            File.Copy(srcfile, dstfile, true);
        }

        AssetDatabase.Refresh();
    }

#endif
    }
}







