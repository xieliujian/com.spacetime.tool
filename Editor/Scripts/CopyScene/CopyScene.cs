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
        static string SVN_SUFFIX = ".svn";                  // svn�ļ�
        static string META_SUFFIX = ".meta";                // meta�ļ�
        static string ASSET_SUFFIX = ".asset";              // asset�ļ�
        static string MAT_SUFFIX = ".mat";                  // mat�ļ�
        static string PREFAB_SUFFIX = ".prefab";            // prefab�ļ�
        static string UNITY_SUFFIX = ".unity";              // unity�ļ�
        static string CONTROLLER_SUFFIX = ".controller";    // anim�ļ�

        /// <summary>
        /// ����Ҫ���Ƶ�Ŀ¼
        /// </summary>
        static string[] UNCOPY_DIR_ARRAY =
        {
        "-stream",      // ����Ҫ���Ƶ�StreamĿ¼
        "/river/"       // ����Ҫ���Ƶ�riverĿ¼
    };

        /// <summary>
        /// ��Ҫɾ��������
        /// </summary>
        static string[] DEL_DATA_ARRAY =
        {
        "LightingData.asset"        // ��Ҫɾ����LightingData
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
        /// ԴĿ¼
        /// </summary>
        static string m_SrcDir;

        /// <summary>
        /// Ŀ��Ŀ¼
        /// </summary>
        static string m_DstDir;

        /// <summary>
        /// Դ��������
        /// </summary>
        static string m_SrcSceneName;

        /// <summary>
        /// Ŀ�곡������
        /// </summary>
        static string m_DstSceneName;

        /// <summary>
        /// Դ�ļ��б�
        /// </summary>
        static List<string> m_SrcFileList = new List<string>();

        /// <summary>
        /// Ŀ���ļ�Ŀ¼
        /// </summary>
        static List<string> m_DstFileList = new List<string>();

        /// <summary>
        /// �ļ���Ϣ�б�
        /// </summary>
        static List<FileInfo> m_FileInfoList = new List<FileInfo>();

        /// <summary>
        /// ��Ҫɾ�����ļ��б�
        /// </summary>
        static List<string> m_DelFileList = new List<string>();

        /// <summary>
        /// ���ܷ������ļ��б�
        /// </summary>
        static List<string> m_CannotAnalyseList = new List<string>();

        /// <summary>
        /// ����
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
        /// ����Դ��������
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
        /// Ŀ��Ŀ¼����
        /// </summary>
        static void DstFolderAnalyse()
        {
            CollectDstFileList();
            DstFileListAnalyse();
            DstFileListReWrite();
            DelFileListProc();
        }

        /// <summary>
        /// TerrainData��Asset�ļ���д
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
        /// asset�ļ���д
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
        /// Meta�ļ���д
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
        /// �Ƿ���Ҫɾ�����ļ�
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
        /// ɾ���ļ��б���
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
        /// Ŀ���ļ��б���д
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

            // δд�ļ����
            foreach (var file in m_CannotAnalyseList)
            {
                Debug.Log("[CopyScene][CannotAnalyse] : " + file);
            }
        }

        /// <summary>
        /// �Ƿ�asset�ļ�
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsAssetFile(string filename)
        {
            return filename.Contains(ASSET_SUFFIX);
        }

        /// <summary>
        /// �Ƿ�����ļ�
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsMatFile(string filename)
        {
            return filename.Contains(MAT_SUFFIX);
        }

        /// <summary>
        /// �Ƿ�prefab�ļ�
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsPrefabFile(string filename)
        {
            return filename.Contains(PREFAB_SUFFIX);
        }

        /// <summary>
        /// ������������׺
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsControllerFile(string filename)
        {
            return filename.Contains(CONTROLLER_SUFFIX);
        }

        /// <summary>
        /// �Ƿ�unity�ļ�
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool IsUnityFile(string filename)
        {
            return filename.Contains(UNITY_SUFFIX);
        }

        /// <summary>
        /// ����Դ·����Դ·��ת��ΪĿ��·��
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
        /// Ŀ���ļ�����
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
        /// Ŀ���ļ��б����
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
        /// �ռ�Ŀ���ļ��б�
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
        /// �Ƿ񲻿ɸ����ļ�
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
        /// ����Ŀ¼2
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

                // �Ƿ���Ҫ���Ƶ��ļ�
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
        /// �޸�Ŀ��·��
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
    /// ����Ŀ¼
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







