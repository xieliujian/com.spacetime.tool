using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using ST.Core;

namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public enum MaterialSearchModifyMatType
    {
        Float,
        Int,
        Vector,
        Color,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MaterialSearchModifySearchType
    {
        None,
        SearchByKeyword,
        SearchByProperty,
    }

    /// <summary>
    /// 
    /// </summary>
    public class MaterialSearchModifyMatGroup
    {
        /// <summary>
        /// 
        /// </summary>
        public int groupIndx;

        /// <summary>
        /// 
        /// </summary>
        public List<Material> matList = new List<Material>();

        /// <summary>
        /// 
        /// </summary>
        public int curMatIndex;

        /// <summary>
        /// 
        /// </summary>
        public int matCount
        {
            get { return matList.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            curMatIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public void AddMat(Material mat)
        {
            matList.Add(mat);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MaterialSearchModifyMatList
    {
        /// <summary>
        /// 
        /// </summary>
        public const int GROUP_MAT_NUM = 100;

        /// <summary>
        /// 
        /// </summary>
        const char STR_KEYWORD_ARRAY_SPLIT = ' ';

        /// <summary>
        /// 
        /// </summary>
        const char STR_MAT_PROPERTY_SPLIT = ',';

        /// <summary>
        /// 
        /// </summary>
        public Shader shader;

        /// <summary>
        /// 
        /// </summary>
        public List<Material> matList = new List<Material>();

        /// <summary>
        /// 
        /// </summary>
        public List<MaterialSearchModifyMatGroup> groupList = new List<MaterialSearchModifyMatGroup>();

        /// <summary>
        /// 
        /// </summary>
        public List<MaterialSearchModifyMatGroup> groupListByKeyword = new List<MaterialSearchModifyMatGroup>();

        /// <summary>
        /// 
        /// </summary>
        public List<MaterialSearchModifyMatGroup> groupListByProperty = new List<MaterialSearchModifyMatGroup>();

        /// <summary>
        /// 
        /// </summary>
        public int curGroupIndex;

        /// <summary>
        /// 
        /// </summary>
        public MaterialSearchModifySearchType searchType = MaterialSearchModifySearchType.None;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetGroupNum()
        {
            var groupList = GetGroupList();

            return groupList.Count;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public MaterialSearchModifyMatGroup GetCurGroup()
        {
            var _groupList = GetGroupList();

            if (curGroupIndex < 0 || curGroupIndex >= _groupList.Count)
                return null;

            return _groupList[curGroupIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="groupList"></param>
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
        /// 
        /// </summary>
        public void AddMat(Material mat)
        {
            matList.Add(mat);

            AddMat(mat, groupList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matKeywordSel"></param>
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
        /// 
        /// </summary>
        /// <param name="strMatKeywordName"></param>
        /// <param name="strKeywordOn"></param>
        /// <param name="_groupList"></param>
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        public void SetAllMaterialKeyword(string strMatKeywordName, string strKeywordOn)
        {
            if (string.IsNullOrEmpty(strMatKeywordName) || string.IsNullOrEmpty(strKeywordOn))
                return;

            var _groupList = GetGroupList();
            SetAllMaterialKeyword(strMatKeywordName, strKeywordOn, _groupList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMatPropertyName"></param>
        /// <param name="matPropertyType"></param>
        /// <param name="strMatPropertyVal"></param>
        /// <param name="_groupList"></param>
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
        /// 
        /// </summary>
        /// <param name="strMatPropertyName"></param>
        /// <param name="matPropertyType"></param>
        /// <param name="strMatPropertyVal"></param>
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
        /// 
        /// </summary>
        public void SetAllMaterialProperty(string strMatPropertyName, MaterialSearchModifyMatType matPropertyType, string strMatPropertyVal)
        {
            if (string.IsNullOrEmpty(strMatPropertyName) || string.IsNullOrEmpty(strMatPropertyVal))
                return;

            var _groupList = GetGroupList();
            SetAllMaterialProperty(strMatPropertyName, matPropertyType, strMatPropertyVal, _groupList);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class MaterialSearchModify
    {
        /// <summary>
        /// 
        /// </summary>
        static Dictionary<Shader, MaterialSearchModifyMatList> s_ShaderDict = new Dictionary<Shader, MaterialSearchModifyMatList>();

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<Shader, MaterialSearchModifyMatList> shaderDict
        {
            get { return s_ShaderDict; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            s_ShaderDict.Clear();
        }

        /// <summary>
        /// 
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
