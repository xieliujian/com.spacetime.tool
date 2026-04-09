using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ST.Tool
{
    /// <summary>
    /// 编辑器文件/资产工具（从 LREditorFileUtils + LREditorUtils 精简迁移）。
    /// </summary>
    public static class EditorFileUtils
    {
        // ── 项目工具 ───────────────────────────────────────────────

        /// <summary>获取当前 Unity 工程名（文件夹名）。</summary>
        public static string GetProjectName()
        {
#if ART_SCENE_PROJECT
            return "scene";
#endif
            var path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf("/"));
            return path.Substring(path.LastIndexOf("/") + 1);
        }

        // ── 文件操作 ───────────────────────────────────────────────

        /// <summary>递归获取目录下所有文件的绝对路径。</summary>
        public static List<string> GetDirSubFilePathList(string dirABSPath, bool isRecursive = true, string suffix = "")
        {
            List<string> pathList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);
            if (!di.Exists) return pathList;

            foreach (FileInfo fi in di.GetFiles())
            {
                if (!string.IsNullOrEmpty(suffix) &&
                    !fi.FullName.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    if (d.Name.Contains(".svn")) continue;
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        /// <summary>获取目录下所有子目录名（非递归，跳过 .svn）。</summary>
        public static List<string> GetDirSubDirNameList(string dirABSPath)
        {
            List<string> nameList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);
            if (!di.Exists) return nameList;
            foreach (DirectoryInfo d in di.GetDirectories())
            {
                if (!d.Name.Contains(".svn")) nameList.Add(d.Name);
            }
            return nameList;
        }

        /// <summary>文件存在则删除。</summary>
        public static void SafeDeleteFile(string fileAbsPath)
        {
            if (File.Exists(fileAbsPath)) File.Delete(fileAbsPath);
        }

        /// <summary>目录不存在则创建（支持递归创建父目录）。</summary>
        public static void SafeCreateDir(string dirAbsPath, bool recursive = false)
        {
            if (Directory.Exists(dirAbsPath)) return;

            if (!recursive)
            {
                Directory.CreateDirectory(dirAbsPath);
                return;
            }

            dirAbsPath = dirAbsPath.Replace("/", "\\");
            string[] pathes = dirAbsPath.Split('\\');
            string path = pathes[0];
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            for (int i = 1; i < pathes.Length; i++)
            {
                path += "\\" + pathes[i];
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
        }

        /// <summary>移动文件（可选覆盖目标）。</summary>
        public static void MoveFile(string srcAbsPath, string dstAbsPath, bool isDeleteDst = true)
        {
            Debug.Log($"MoveFile {srcAbsPath} -> {dstAbsPath}");
            if (isDeleteDst && File.Exists(dstAbsPath)) File.Delete(dstAbsPath);
            File.Move(srcAbsPath, dstAbsPath);
        }

        /// <summary>递归复制目录。</summary>
        public static void CopyDir(string srcDir, string dstDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo dst = new DirectoryInfo(dstDir);
            if (!source.Exists) return;
            if (!dst.Exists) dst.Create();
            foreach (FileInfo fi in source.GetFiles())
                File.Copy(fi.FullName, dst.FullName + "/" + fi.Name, true);
            foreach (DirectoryInfo d in source.GetDirectories())
                CopyDir(d.FullName, dst.FullName + "/" + d.Name);
        }

        /// <summary>读取文件，替换内容后以 UTF-8 NoBOM 写回。</summary>
        public static void FileContentReplace(string filePath, string oldStr, string newStr)
        {
            string oldContent = File.ReadAllText(filePath, Encoding.UTF8);
            string newContent = oldContent.Replace(oldStr, newStr);
            if (oldContent == newContent)
                Debug.LogError($"FileContentReplace Failed path:{filePath} oldStr:{oldStr}");
            File.WriteAllText(filePath, newContent, new UTF8Encoding(false));
        }

        // ── 路径工具 ───────────────────────────────────────────────

        /// <summary>从路径中取文件名（含扩展名）。</summary>
        public static string GetFileName(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");
            return lastIndex >= 0 ? name.Substring(lastIndex + 1) : name;
        }

        /// <summary>从路径中取文件名（不含扩展名）。</summary>
        public static string GetFileNameWithoutExtend(string absOrAssetsPath)
        {
            string fileName = GetFileName(absOrAssetsPath);
            int lastIndex = fileName.LastIndexOf(".");
            return lastIndex >= 0 ? fileName.Substring(0, lastIndex) : fileName;
        }

        /// <summary>取文件扩展名（含点）。</summary>
        public static string GetFileExtendName(string absOrAssetsPath) =>
            Path.GetExtension(absOrAssetsPath);

        /// <summary>取路径所在目录部分（末尾保留 /）。</summary>
        public static string GetDirPath(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        /// <summary>取路径的直接父目录名称。</summary>
        public static string GetParentDirName(string absOrAssetsPath)
        {
            string parentDirPath = GetDirPath(absOrAssetsPath).TrimEnd('/');
            int lastPos = parentDirPath.LastIndexOf("/");
            return lastPos >= 0 ? parentDirPath.Substring(lastPos + 1) : string.Empty;
        }

        // ── 资产工具 ───────────────────────────────────────────────

        /// <summary>获取 Assets 目录下指定类型的所有资源路径。</summary>
        public static List<string> GetAssetsPaths<T>() => GetAssetsPaths(typeof(T));

        /// <summary>获取 Assets 目录下指定类型的所有资源路径。</summary>
        public static List<string> GetAssetsPaths(Type needResType)
        {
            List<string> result = new List<string>();
            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                if (!path.StartsWith("Assets")) continue;
                if (AssetDatabase.GetMainAssetTypeAtPath(path) == needResType) result.Add(path);
            }
            return result;
        }

        static FieldInfo m_AnimationClipSizeFieldInfo;
        static MethodInfo m_AnimationClipStatsMethodInfo;

        /// <summary>获取资源运行时内存大小（字节）。AnimationClip 使用内部统计接口。</summary>
        public static long GetResSize<T>(string assetsPath) where T : Object
        {
            var data = AssetDatabase.LoadAssetAtPath<T>(assetsPath);
            if (data == null) return 0;

            if (data is AnimationClip)
            {
                if (m_AnimationClipStatsMethodInfo == null || m_AnimationClipSizeFieldInfo == null)
                {
                    Assembly asm = Assembly.GetAssembly(typeof(Editor));
                    m_AnimationClipStatsMethodInfo = typeof(AnimationUtility).GetMethod(
                        "GetAnimationClipStats", BindingFlags.Static | BindingFlags.NonPublic);
                    Type statsType = asm.GetType("UnityEditor.AnimationClipStats");
                    m_AnimationClipSizeFieldInfo = statsType.GetField(
                        "size", BindingFlags.Public | BindingFlags.Instance);
                }
                var stats = m_AnimationClipStatsMethodInfo.Invoke(null, new object[] { data as AnimationClip });
                return (int)m_AnimationClipSizeFieldInfo.GetValue(stats);
            }

            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(data);
        }

        /// <summary>获取资源大小的格式化字符串（如 "1.2 MB"）。</summary>
        public static string GetResSizeStr<T>(string assetsPath) where T : Object =>
            EditorUtility.FormatBytes(GetResSize<T>(assetsPath));

        /// <summary>通过反射读取 TextureImporter 的原始贴图尺寸。</summary>
        public static void GetTextureOriginalSize(TextureImporter ti, out int width, out int height)
        {
            if (ti == null) { width = 0; height = 0; return; }
            object[] args = new object[2] { 0, 0 };
            MethodInfo mi = typeof(TextureImporter).GetMethod(
                "GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(ti, args);
            width = (int)args[0];
            height = (int)args[1];
        }

        /// <summary>获取贴图原始最大边长。</summary>
        public static int GetTextureOriginalMaxSize(TextureImporter ti)
        {
            GetTextureOriginalSize(ti, out int width, out int height);
            return Mathf.Max(width, height);
        }

        // ── 字符串工具 ─────────────────────────────────────────────

        /// <summary>将字符串列表拼接为单字符串（分隔符默认换行）。</summary>
        public static string StringList2String(List<string> strList, string spliter = "\n")
        {
            if (strList == null) return string.Empty;
            var sb = new StringBuilder();
            foreach (string s in strList) { sb.Append(s); sb.Append(spliter); }
            return sb.ToString();
        }

        /// <summary>将字符串按分隔符拆为列表。</summary>
        public static List<string> String2StringList(string str, char spliter = '\n')
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(str)) return result;
            result.AddRange(str.Split(spliter));
            return result;
        }

        // ── 扩展方法 ───────────────────────────────────────────────

        /// <summary>安全地按索引获取列表元素；越界时返回 defaultData。</summary>
        public static T Get<T>(this IList<T> dataList, int index, T defaultData = default(T))
        {
            if (index < 0 || dataList == null || index >= dataList.Count) return defaultData;
            return dataList[index];
        }

        /// <summary>获取泛型类/接口的所有实现类型（不含自身）。</summary>
        internal static void GetGenericImpAll([NotNull] this Type generic, List<Type> result)
        {
            if (result == null) return;
            result.Clear();
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            foreach (Module module in generic.Assembly.GetModules())
            {
                foreach (Type type in module.GetTypes())
                {
                    if (type != generic && type.HasImplementedRawGeneric(generic))
                        result.Add(type);
                }
            }
        }

        /// <summary>判断 type 是否实现了指定原始泛型类型/接口。</summary>
        internal static bool HasImplementedRawGeneric([NotNull] this Type type, [NotNull] Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            bool IsTheRawGenericType(Type t) =>
                generic == (t.IsGenericType ? t.GetGenericTypeDefinition() : t);

            if (type.GetInterfaces().Any(IsTheRawGenericType)) return true;
            while (type != null && type != typeof(object))
            {
                if (IsTheRawGenericType(type)) return true;
                type = type.BaseType;
            }
            return false;
        }
    }
}
