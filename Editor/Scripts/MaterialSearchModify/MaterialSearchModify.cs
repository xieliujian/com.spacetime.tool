using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using ST.Core;

namespace ST.Tool
{
    /// <summary>
    /// 材质属性修改类型：Float、Int、Vector、Color。
    /// </summary>
    public enum MaterialSearchModifyMatType
    {
        Float,
        Int,
        Vector,
        Color,
    }

    /// <summary>
    /// 材质搜索模式：不过滤、按 Keyword 过滤、按属性值过滤。
    /// </summary>
    public enum MaterialSearchModifySearchType
    {
        None,
        SearchByKeyword,
        SearchByProperty,
    }

    /// <summary>
    /// 材质分组：将材质列表按固定数量分页，便于编辑器窗口分批显示与操作。
    /// </summary>
    public class MaterialSearchModifyMatGroup
    {
        /// <summary>
        /// 当前分组的索引。
        /// </summary>
        public int groupIndx;

        /// <summary>
        /// 本组包含的材质列表。
        /// </summary>
        public List<Material> matList = new List<Material>();

        /// <summary>
        /// 当前在编辑器中选中的材质索引。
        /// </summary>
        public int curMatIndex;

        /// <summary>
        /// 本组中材质的总数量。
        /// </summary>
        public int matCount
        {
            get { return matList.Count; }
        }

        /// <summary>
        /// 将当前选中材质索引重置为 0。
        /// </summary>
        public void Reset()
        {
            curMatIndex = 0;
        }

        /// <summary>
        /// 向本组追加一个材质。
        /// </summary>
        /// <param name="mat">要追加的材质。</param>
        public void AddMat(Material mat)
        {
            matList.Add(mat);
        }
    }

    /// <summary>
    /// 某个 Shader 下所有材质的管理器：维护全量分组列表及按 Keyword/属性值筛选后的分组列表，
    /// 支持批量修改 Keyword 开关与材质属性值。
    /// </summary>
    public class MaterialSearchModifyMatList
    {
        /// <summary>
        /// 每个分组最多容纳的材质数量。
        /// </summary>
        public const int GROUP_MAT_NUM = 100;

        /// <summary>
        /// Keyword 列表输入时的分隔符（空格）。
        /// </summary>
        const char STR_KEYWORD_ARRAY_SPLIT = ' ';

        /// <summary>
        /// 材质属性值各分量输入时的分隔符（逗号）。
        /// </summary>
        const char STR_MAT_PROPERTY_SPLIT = ',';

        /// <summary>
        /// 本列表对应的 Shader。
        /// </summary>
        public Shader shader;

        /// <summary>
        /// 该 Shader 下的所有材质。
        /// </summary>
        public List<Material> matList = new List<Material>();

        /// <summary>
        /// 全量材质的分组列表。
        /// </summary>
        public List<MaterialSearchModifyMatGroup> groupList = new List<MaterialSearchModifyMatGroup>();

        /// <summary>
        /// 按 Keyword 筛选后的分组列表。
        /// </summary>
        public List<MaterialSearchModifyMatGroup> groupListByKeyword = new List<MaterialSearchModifyMatGroup>();

        /// <summary>
        /// 按属性值筛选后的分组列表。
        /// </summary>
        public List<MaterialSearchModifyMatGroup> groupListByProperty = new List<MaterialSearchModifyMatGroup>();

        /// <summary>
        /// 当前选中的分组索引。
        /// </summary>
        public int curGroupIndex;

        /// <summary>
        /// 当前搜索模式。
        /// </summary>
        public MaterialSearchModifySearchType searchType = MaterialSearchModifySearchType.None;

        /// <summary>
        /// 获取当前搜索模式下的材质总数。
        /// </summary>
        public int GetMatNum()
        {
            var groupList = GetGroupList();

            int matNum = 0;

            foreach(var group in groupList)
            {
                matNum += group.matCount;
            }

            return matNum;
        }

        /// <summary>
        /// 获取当前搜索模式下的分组总数。
        /// </summary>
        /// <returns>当前分组列表的数量。</returns>
        public int GetGroupNum()
        {
            var groupList = GetGroupList();

            return groupList.Count;
        }

        /// <summary>
        /// 重置当前分组选中索引并清除搜索状态，回到全量视图。
        /// </summary>
        public void Reset()
        {
            var curGroup = GetCurGroup();
            if (curGroup != null)
            {
                curGroup.Reset();
            }

            curGroupIndex = 0;
            searchType = MaterialSearchModifySearchType.None;
        }

        /// <summary>
        /// 根据当前 <see cref="searchType"/> 返回对应的分组列表。
        /// </summary>
        /// <returns>当前激活的分组列表。</returns>
        public List<MaterialSearchModifyMatGroup> GetGroupList()
        {
            List <MaterialSearchModifyMatGroup> _groupList = null;

            if (searchType == MaterialSearchModifySearchType.None)
            {
                _groupList = groupList;
            }
            else if (searchType == MaterialSearchModifySearchType.SearchByKeyword)
            {
                _groupList = groupListByKeyword;
            }
            else if (searchType == MaterialSearchModifySearchType.SearchByProperty)
            {
                _groupList = groupListByProperty;
            }

            return _groupList;
        }

        /// <summary>
        /// 返回当前选中的分组，索引越界时返回 <c>null</c>。
        /// </summary>
        /// <returns>当前分组或 <c>null</c>。</returns>
        public MaterialSearchModifyMatGroup GetCurGroup()
        {
            var _groupList = GetGroupList();

            if (curGroupIndex < 0 || curGroupIndex >= _groupList.Count)
                return null;

            return _groupList[curGroupIndex];
        }

        /// <summary>
        /// 将材质追加到指定分组列表，超出 <see cref="GROUP_MAT_NUM"/> 上限时自动创建新分组。
        /// </summary>
        /// <param name="mat">要追加的材质。</param>
        /// <param name="_groupList">目标分组列表。</param>
        public void AddMat(Material mat, List<MaterialSearchModifyMatGroup> _groupList)
        {
            MaterialSearchModifyMatGroup group = null;
            if (_groupList.Count <= 0)
            {
                group = new MaterialSearchModifyMatGroup();
                group.groupIndx = _groupList.Count;
                _groupList.Add(group);
            }
            else
            {
                group = _groupList[_groupList.Count - 1];
            }

            if (group.matCount >= GROUP_MAT_NUM)
            {
                group = new MaterialSearchModifyMatGroup();
                group.groupIndx = _groupList.Count;
                _groupList.Add(group);
            }

            group.AddMat(mat);
        }


        /// <summary>
        /// 将材质追加到全量列表及全量分组中。
        /// </summary>
        /// <param name="mat">要追加的材质。</param>
        public void AddMat(Material mat)
        {
            matList.Add(mat);

            AddMat(mat, groupList);
        }

        /// <summary>
        /// 按空格分隔的 Keyword 集合筛选同时包含所有关键字的材质，结果存入 <see cref="groupListByKeyword"/>。
        /// </summary>
        /// <param name="matKeywordSel">空格分隔的 Keyword 列表字符串。</param>
        public void SearchAllMaterialByKeywordList(string matKeywordSel)
        {
            if (string.IsNullOrEmpty(matKeywordSel))
                return;

            var splitArray = matKeywordSel.Split(STR_KEYWORD_ARRAY_SPLIT);
            if (splitArray == null || splitArray.Length <= 0)
                return;

            Reset();
            searchType = MaterialSearchModifySearchType.SearchByKeyword;
            groupListByKeyword.Clear();

            foreach(var mat in matList)
            {
                if (mat == null)
                    continue;

                int index = 0;
                foreach(var split in splitArray)
                {
                    if (split == null)
                        continue;

                    foreach (var keyword in mat.shaderKeywords)
                    {
                        if (keyword == split)
                        {
                            index++;
                            break;
                        }
                    }
                }

                bool find = false;
                if (index == splitArray.Length)
                {
                    find = true;
                }

                if (find)
                {
                    AddMat(mat, groupListByKeyword);
                }
            }
        }

        /// <summary>
        /// 批量设置指定分组列表中所有材质的 Shader Keyword 开关，并显示进度条。
        /// </summary>
        /// <param name="strMatKeywordName">Keyword 名称。</param>
        /// <param name="strKeywordOn">大于 0 表示开启，否则关闭。</param>
        /// <param name="_groupList">要操作的分组列表。</param>
        void SetAllMaterialKeyword(string strMatKeywordName, string strKeywordOn, List<MaterialSearchModifyMatGroup> _groupList)
        {
            for (int m = 0; m < _groupList.Count; m++)
            {
                var group = _groupList[m];
                if (group == null)
                    continue;

                for (int i = 0; i < group.matList.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("SetAllMaterialKeyword", $"SetAllMaterialKeyword 组{m} 数目{i}/{group.matList.Count}",
                        (float)(i) / group.matList.Count);

                    var mat = group.matList[i];
                    if (mat == null)
                        continue;

                    bool isOpen = int.Parse(strKeywordOn) > 0;

                    if (isOpen)
                    {
                        mat.EnableKeyword(strMatKeywordName);
                    }
                    else
                    {
                        mat.DisableKeyword(strMatKeywordName);
                    }

                    //Debug.Log("[xlj] :" + AssetDatabase.GetAssetPath(mat));

                    EditorUtility.SetDirty(mat);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 遍历全量材质，移除每个材质中在 Shader 各 PassType 下均无效的 Keyword。
        /// </summary>
        public void SkipAllMatUnvalidKeyword()
        {
            for (int m = 0; m < groupList.Count; m++)
            {
                var group = groupList[m];
                if (group == null)
                    continue;

                for (int i = 0; i < group.matList.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("SkipAllMatUnvalidKeyword", $"SkipAllMatUnvalidKeyword 组{m} 数目{i}/{group.matList.Count}",
                        (float)(i) / group.matList.Count);

                    var mat = group.matList[i];
                    if (mat == null)
                        continue;

                    List<string> validKeywordList = new List<string>();

                    var shaderKeywords = mat.shaderKeywords;
                    if (shaderKeywords != null)
                    {
                        foreach (var keyword in shaderKeywords)
                        {
                            var isKeywordValid = IsKeywordValid(mat, keyword);
                            if (isKeywordValid)
                            {
                                validKeywordList.Add(keyword);
                            }
                        }
                    }

                    mat.shaderKeywords = validKeywordList.ToArray();

                    //Debug.Log("[xlj] : " + AssetDatabase.GetAssetPath(mat) + " Pre : " + shaderKeywords);
                    //Debug.Log("[xlj] : " + AssetDatabase.GetAssetPath(mat) + " Post : " + validKeywordList.ToArray());

                    //var shader = mat.shader;
                    //if (shader == null)
                    //    continue;

                    //SerializedObject shaderProperty = new SerializedObject(shader);
                    //if (shaderProperty == null)
                    //    continue;

                    //var keywordSpaceProp = shaderProperty.FindProperty("keywordSpace");
                    //if (keywordSpaceProp == null)
                    //    continue;

                    //var keywordSpaceObj = keywordSpaceProp.serializedObject;
                    //if (keywordSpaceObj == null)
                    //    continue;

                    //var keywordsProp = keywordSpaceObj.FindProperty("keywords");
                    //if (keywordsProp == null)
                    //    continue;

                    EditorUtility.SetDirty(mat);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 检查指定 Keyword 在给定材质的 Shader 各 PassType 中是否有效。
        /// </summary>
        /// <param name="mat">目标材质。</param>
        /// <param name="keyword">要验证的 Keyword 字符串。</param>
        /// <returns>至少有一个 PassType 支持该 Keyword 则返回 <c>true</c>。</returns>
        public static bool IsKeywordValid(Material mat, string keyword)
        {
            for (int i = (int)PassType.Normal; i <= (int)PassType.ScriptableRenderPipelineDefaultUnlit; ++i)
            {
                PassType passType = (PassType)i;

                bool isValid = ShaderVariantTools.IsValidPassTypeKeywords(mat, passType, new string[] { keyword });
                if (isValid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 对当前搜索模式下的所有材质批量设置 Keyword 开关。
        /// </summary>
        /// <param name="strMatKeywordName">Keyword 名称。</param>
        /// <param name="strKeywordOn">大于 0 表示开启，否则关闭。</param>
        public void SetAllMaterialKeyword(string strMatKeywordName, string strKeywordOn)
        {
            if (string.IsNullOrEmpty(strMatKeywordName) || string.IsNullOrEmpty(strKeywordOn))
                return;

            var _groupList = GetGroupList();
            SetAllMaterialKeyword(strMatKeywordName, strKeywordOn, _groupList);
        }

        /// <summary>
        /// 批量设置指定分组列表中所有材质的属性值（支持 Float、Int、Vector、Color），并显示进度条。
        /// </summary>
        /// <param name="strMatPropertyName">属性名称。</param>
        /// <param name="matPropertyType">属性类型。</param>
        /// <param name="strMatPropertyVal">属性值字符串（Vector/Color 用逗号分隔四个分量）。</param>
        /// <param name="_groupList">要操作的分组列表。</param>
        void SetAllMaterialProperty(string strMatPropertyName, MaterialSearchModifyMatType matPropertyType, string strMatPropertyVal,
            List<MaterialSearchModifyMatGroup> _groupList)
        {
            for (int m = 0; m < _groupList.Count; m++)
            {
                var group = _groupList[m];
                if (group == null)
                    continue;

                for (int i = 0; i < group.matList.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("SetAllMaterialProperty", $"SetAllMaterialProperty 组{m} 数目{i}/{group.matList.Count}", 
                        (float)(i) / group.matList.Count);

                    var mat = group.matList[i];
                    if (mat == null)
                        continue;

                    var hasProperty = mat.HasProperty(strMatPropertyName);
                    if (!hasProperty)
                        continue;

                    if (matPropertyType == MaterialSearchModifyMatType.Float)
                    {
                        var matPropertyVal = float.Parse(strMatPropertyVal);
                        mat.SetFloat(strMatPropertyName, matPropertyVal);
                    }
                    else if (matPropertyType == MaterialSearchModifyMatType.Int)
                    {
                        var matPropertyVal = int.Parse(strMatPropertyVal);
                        mat.SetInt(strMatPropertyName, matPropertyVal);
                    }
                    else if (matPropertyType == MaterialSearchModifyMatType.Vector)
                    {
                        var splitArray = strMatPropertyVal.Split(STR_MAT_PROPERTY_SPLIT);
                        if (splitArray.Length == 4)
                        {
                            var x = float.Parse(splitArray[0]);
                            var y = float.Parse(splitArray[1]);
                            var z = float.Parse(splitArray[2]);
                            var w = float.Parse(splitArray[3]);
                            mat.SetVector(strMatPropertyName, new Vector4(x, y, z, w));
                        }
                    }
                    else if (matPropertyType == MaterialSearchModifyMatType.Color)
                    {
                        var splitArray = strMatPropertyVal.Split(STR_MAT_PROPERTY_SPLIT);
                        if (splitArray.Length == 4)
                        {
                            var x = float.Parse(splitArray[0]);
                            var y = float.Parse(splitArray[1]);
                            var z = float.Parse(splitArray[2]);
                            var w = float.Parse(splitArray[3]);

                            x = Mathf.Clamp01(x / 255f);
                            y = Mathf.Clamp01(y / 255f);
                            z = Mathf.Clamp01(z / 255f);
                            w = Mathf.Clamp01(w / 255f);

                            mat.SetColor(strMatPropertyName, new Color(x, y, z, w));
                        }
                    }

                    //Debug.Log("[xlj] :" + AssetDatabase.GetAssetPath(mat));

                    EditorUtility.SetDirty(mat);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 按属性名称、类型与值筛选匹配的材质，结果存入 <see cref="groupListByProperty"/>。
        /// </summary>
        /// <param name="strMatPropertyName">属性名称。</param>
        /// <param name="matPropertyType">属性类型。</param>
        /// <param name="strMatPropertyVal">目标属性值字符串。</param>
        public void SearchAllMaterialByProperty(string strMatPropertyName, MaterialSearchModifyMatType matPropertyType, string strMatPropertyVal)
        {
            Reset();
            searchType = MaterialSearchModifySearchType.SearchByProperty;
            groupListByProperty.Clear();

            if (string.IsNullOrEmpty(strMatPropertyName) || string.IsNullOrEmpty(strMatPropertyVal))
                return;

            foreach (var mat in matList)
            {
                if (mat == null)
                    continue;

                var hasProperty = mat.HasProperty(strMatPropertyName);
                if (!hasProperty)
                    continue;

                bool find = false;

                if (matPropertyType == MaterialSearchModifyMatType.Float)
                {
                    var matPropertyVal = float.Parse(strMatPropertyVal);
                    var srcMatPropertyVal = mat.GetFloat(strMatPropertyName);
                    if (srcMatPropertyVal == matPropertyVal)
                    {
                        find = true;
                    }
                }
                else if (matPropertyType == MaterialSearchModifyMatType.Int)
                {
                    var matPropertyVal = int.Parse(strMatPropertyVal);
                    var srcMatPropertyVal = mat.GetInt(strMatPropertyName);
                    if (srcMatPropertyVal == matPropertyVal)
                    {
                        find = true;
                    }
                }
                else if (matPropertyType == MaterialSearchModifyMatType.Vector)
                {
                    var splitArray = strMatPropertyVal.Split(STR_MAT_PROPERTY_SPLIT);
                    if (splitArray.Length == 4)
                    {
                        var x = float.Parse(splitArray[0]);
                        var y = float.Parse(splitArray[1]);
                        var z = float.Parse(splitArray[2]);
                        var w = float.Parse(splitArray[3]);

                        var srcMatPropertyVal = mat.GetVector(strMatPropertyName);
                        if (srcMatPropertyVal.x == x &&
                            srcMatPropertyVal.y == y &&
                            srcMatPropertyVal.z == z &&
                            srcMatPropertyVal.w == w)
                        {
                            find = true;
                        }
                    }
                }
                else if (matPropertyType == MaterialSearchModifyMatType.Color)
                {
                    var splitArrayColor = strMatPropertyVal.Split(STR_MAT_PROPERTY_SPLIT);
                    if (splitArrayColor.Length == 4)
                    {
                        var x = float.Parse(splitArrayColor[0]);
                        var y = float.Parse(splitArrayColor[1]);
                        var z = float.Parse(splitArrayColor[2]);
                        var w = float.Parse(splitArrayColor[3]);

                        var srcMatPropertyVal = mat.GetColor(strMatPropertyName);
                        if (srcMatPropertyVal.r == x &&
                            srcMatPropertyVal.g == y &&
                            srcMatPropertyVal.b == z &&
                            srcMatPropertyVal.a == w)
                        {
                            find = true;
                        }
                    }
                }

                //Debug.Log("[xlj] :" + AssetDatabase.GetAssetPath(mat));

                if (find)
                {
                    AddMat(mat, groupListByProperty);
                }
            }
        }

        /// <summary>
        /// 对当前搜索模式下的所有材质批量设置属性值。
        /// </summary>
        /// <param name="strMatPropertyName">属性名称。</param>
        /// <param name="matPropertyType">属性类型。</param>
        /// <param name="strMatPropertyVal">目标属性值字符串。</param>
        public void SetAllMaterialProperty(string strMatPropertyName, MaterialSearchModifyMatType matPropertyType, string strMatPropertyVal)
        {
            if (string.IsNullOrEmpty(strMatPropertyName) || string.IsNullOrEmpty(strMatPropertyVal))
                return;

            var _groupList = GetGroupList();
            SetAllMaterialProperty(strMatPropertyName, matPropertyType, strMatPropertyVal, _groupList);
        }

    }

    /// <summary>
    /// 材质搜索与批量修改的入口：维护 Shader → <see cref="MaterialSearchModifyMatList"/> 的字典，
    /// 供编辑器窗口查询与操作。
    /// </summary>
    public class MaterialSearchModify
    {
        /// <summary>
        /// 以 Shader 为键、对应材质管理器为值的字典。
        /// </summary>
        static Dictionary<Shader, MaterialSearchModifyMatList> s_ShaderDict = new Dictionary<Shader, MaterialSearchModifyMatList>();

        /// <summary>
        /// 对外暴露的 Shader 字典只读访问器。
        /// </summary>
        public static Dictionary<Shader, MaterialSearchModifyMatList> shaderDict
        {
            get { return s_ShaderDict; }
        }

        /// <summary>
        /// 清空 Shader 字典。
        /// </summary>
        public static void Clear()
        {
            s_ShaderDict.Clear();
        }

        /// <summary>
        /// 扫描工程内所有 <c>.mat</c> 文件，重新构建 Shader → 材质列表 的字典。
        /// </summary>
        public static void RefreshShaderList()
        {
            Clear();

            var allmat = ShaderVariantTools.GetAllMaterials();
            if (allmat == null)
                return;

            foreach(var mat in allmat)
            {
                if (mat == null)
                    continue;

                var shader = mat.shader;
                if (shader == null)
                    continue;

                MaterialSearchModifyMatList matListClass = null;
                s_ShaderDict.TryGetValue(shader, out matListClass);
                if (matListClass == null)
                {
                    matListClass = new MaterialSearchModifyMatList();
                    matListClass.shader = shader;
                    s_ShaderDict.Add(shader, matListClass);
                }

                matListClass.AddMat(mat);
            }
        }
    }
}
