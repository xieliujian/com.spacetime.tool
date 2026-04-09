using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// 材质编辑器窗口：以 Shader 为维度展示工程内所有材质，
    /// 支持按 Keyword 或属性值筛选，并提供批量修改 Keyword/属性及剔除无效 Keyword 的功能。
    /// </summary>
    public class MaterialSearchModifyEditor : EditorWindow
    {
        /// <summary>
        /// 打开材质编辑器窗口，菜单位于 <c>SpaceTime/Tool/材质编辑器</c>。
        /// </summary>
        [MenuItem("SpaceTime/Tool/材质编辑器", false, 300)]
        static void OnOpen()
        {
            MaterialSearchModifyEditor window = ScriptableObject.CreateInstance<MaterialSearchModifyEditor>();
            window.titleContent = new GUIContent("材质编辑器");
            window.Show();

            MaterialSearchModify.Clear();
        }

        /// <summary>
        /// Shader 列表滚动区域的宽度。
        /// </summary>
        const float SHADER_SCROLL_WIDTH = 1000f;

        /// <summary>
        /// Shader 列表滚动区域的高度。
        /// </summary>
        const float SHADER_SCROLL_HEIGHT = 150f;

        /// <summary>
        /// 分组索引滚动区域的高度。
        /// </summary>
        const float MAT_GROUP_INDEX_HEIGHT = 150f;

        /// <summary>
        /// 当前选中按钮的背景色。
        /// </summary>
        static Color SEL_BUTTON_COLOR = Color.blue;

        /// <summary>
        /// 未选中按钮的背景色。
        /// </summary>
        static Color UN_SEL_BUTTON_COLOR = Color.gray;

        /// <summary>
        /// 通用按钮宽度。
        /// </summary>
        const float BUTTON_COMMON_WIDTH = 200f;

        /// <summary>
        /// 材质属性名称输入。
        /// </summary>
        string m_MatPropertyName;

        /// <summary>
        /// 材质属性修改类型选择。
        /// </summary>
        MaterialSearchModifyMatType m_MatPropertyType;

        /// <summary>
        /// 材质属性值输入（Vector/Color 用逗号分隔四分量）。
        /// </summary>
        string m_MatPropertyVal;

        /// <summary>
        /// 要批量设置的 Keyword 名称输入。
        /// </summary>
        string m_MatKeywordName;

        /// <summary>
        /// 用于筛选材质的 Keyword 列表输入（空格分隔）。
        /// </summary>
        string m_MatKeywordSel;

        /// <summary>
        /// Keyword 开关值输入（大于 0 表示开启）。
        /// </summary>
        string m_MatKeywordVal;

        /// <summary>
        /// Shader 列表的滚动位置。
        /// </summary>
        Vector2 m_ShaderScrollPos;

        /// <summary>
        /// 分组索引面板的滚动位置。
        /// </summary>
        Vector2 m_MatGroupIndexScrollPos;

        /// <summary>
        /// 当前分组材质列表的滚动位置。
        /// </summary>
        Vector2 m_MatGroupScrollPos;

        /// <summary>
        /// 当前选中 Shader 对应的材质管理器。
        /// </summary>
        MaterialSearchModifyMatList m_MatListClass;

        /// <summary>
        /// 绘制完整编辑器界面：Shader 工具栏、Shader 列表、分组索引及材质列表。
        /// </summary>
        void OnGUI()
        {
            DrawShaderToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawShaderList();

            EditorGUILayout.BeginVertical();
            DrawMatListGroupIndexToolbar();
            DrawMatListGroupIndex();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            DrawMatListGroupToolbar();
            DrawMatListGroup();
        }

        /// <summary>
        /// 显示当前材质数量与分组数量统计信息。
        /// </summary>
        void DrawMatListGroupIndexToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            if (m_MatListClass != null)
            {
                GUI.backgroundColor = UN_SEL_BUTTON_COLOR;

                var matNum = m_MatListClass.GetMatNum();
                var groupNum = m_MatListClass.GetGroupNum();
                GUILayout.Label($"材质数目是 ：{matNum}");
                GUILayout.Label($"一共分成 ：{groupNum}个组");
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制材质属性筛选与批量设置行：属性名、类型、值输入及对应操作按钮。
        /// </summary>
        void DrawMatListGroupPropertySet()
        {
            EditorGUILayout.BeginHorizontal();

            m_MatPropertyName = EditorGUILayout.TextField("材质属性名字 ：", m_MatPropertyName);
            m_MatPropertyType = (MaterialSearchModifyMatType)EditorGUILayout.EnumPopup("材质属性类型 ：", m_MatPropertyType);
            m_MatPropertyVal = EditorGUILayout.TextField("材质属性值（逗号隔开） ：", m_MatPropertyVal, GUILayout.Width(500));

            if (GUILayout.Button("根据材质属性筛选材质", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SearchAllMaterialByProperty(m_MatPropertyName, m_MatPropertyType, m_MatPropertyVal);
            }

            if (GUILayout.Button("设置所有材质属性", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SetAllMaterialProperty(m_MatPropertyName, m_MatPropertyType, m_MatPropertyVal);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制按 Keyword 列表筛选材质的输入行与操作按钮。
        /// </summary>
        void DrawMatListGroupKeywordSelect()
        {
            EditorGUILayout.BeginHorizontal();

            m_MatKeywordSel = EditorGUILayout.TextField("筛选的Keyword列表, 空格隔开 ：", m_MatKeywordSel);

            if (GUILayout.Button("根据Keyword列表筛选材质", GUILayout.Width(300f)))
            {
                m_MatListClass.SearchAllMaterialByKeywordList(m_MatKeywordSel);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制剔除所有无效 Keyword 的操作按钮行。
        /// </summary>
        void SkipAllMatUnvalidKeyword()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("剔除所有的无效Keyword", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SkipAllMatUnvalidKeyword();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制批量设置 Keyword 开关的输入行与操作按钮。
        /// </summary>
        void DrawMatListGroupKeywordSet()
        {
            EditorGUILayout.BeginHorizontal();

            m_MatKeywordName = EditorGUILayout.TextField("材质Keyword名字 ：", m_MatKeywordName);
            m_MatKeywordVal = EditorGUILayout.TextField("材质Keyword开关 ：", m_MatKeywordVal);

            if (GUILayout.Button("设置所有材质Keyword", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SetAllMaterialKeyword(m_MatKeywordName, m_MatKeywordVal);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制材质操作工具栏：依次渲染剔除无效 Keyword、Keyword 筛选、属性设置、Keyword 设置。
        /// </summary>
        void DrawMatListGroupToolbar()
        {
            if (m_MatListClass == null)
                return;

            GUI.backgroundColor = UN_SEL_BUTTON_COLOR;

            SkipAllMatUnvalidKeyword();
            DrawMatListGroupKeywordSelect();
            DrawMatListGroupPropertySet();
            DrawMatListGroupKeywordSet();
        }

        /// <summary>
        /// 绘制分组索引按钮列表，点击后切换当前显示分组。
        /// </summary>
        void DrawMatListGroupIndex()
        {
            m_MatGroupIndexScrollPos = EditorGUILayout.BeginScrollView(m_MatGroupIndexScrollPos, GUILayout.Height(MAT_GROUP_INDEX_HEIGHT));

            if (m_MatListClass != null)
            {
                var groupList = m_MatListClass.GetGroupList();
                foreach (var group in groupList)
                {
                    if (m_MatListClass != null && m_MatListClass.curGroupIndex == group.groupIndx)
                    {
                        GUI.backgroundColor = SEL_BUTTON_COLOR;
                    }
                    else
                    {
                        GUI.backgroundColor = UN_SEL_BUTTON_COLOR;
                    }

                    var strGroupIndex = group.groupIndx + 1;
                    if (GUILayout.Button(strGroupIndex.ToString()))
                    {
                        m_MatGroupScrollPos = Vector2.zero;
                        m_MatListClass.curGroupIndex = group.groupIndx;

                        var curGroup = m_MatListClass.GetCurGroup();
                        if (curGroup != null)
                        {
                            curGroup.Reset();
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制当前分组的材质列表，点击后高亮并 Ping 对应材质资产。
        /// </summary>
        void DrawMatListGroup()
        {
            if (m_MatListClass == null)
                return;

            var curGroup = m_MatListClass.GetCurGroup();
            if (curGroup == null)
                return;

            var matList = curGroup.matList;
            if (matList == null || matList.Count <= 0)
                return;

            m_MatGroupScrollPos = EditorGUILayout.BeginScrollView(m_MatGroupScrollPos);

            for (int i = 0; i < matList.Count; i++)
            {
                var mat = matList[i];
                if (mat == null)
                    continue;

                if (curGroup.curMatIndex == i)
                {
                    GUI.backgroundColor = SEL_BUTTON_COLOR;
                }
                else
                {
                    GUI.backgroundColor = UN_SEL_BUTTON_COLOR;
                }

                var matPath = AssetDatabase.GetAssetPath(mat);
                if (GUILayout.Button(matPath))
                {
                    curGroup.curMatIndex = i;
                    EditorGUIUtility.PingObject(mat);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制 Shader 列表的滚动视图，点击 Shader 按钮后切换当前材质管理器。
        /// </summary>
        void DrawShaderList()
        {
            var shaderDict = MaterialSearchModify.shaderDict;
            if (shaderDict.Count <= 0)
                return;

            m_ShaderScrollPos = EditorGUILayout.BeginScrollView(m_ShaderScrollPos, GUILayout.Width(SHADER_SCROLL_WIDTH),
                GUILayout.Height(SHADER_SCROLL_HEIGHT));

            foreach (var iter in shaderDict)
            {
                var shader = iter.Key;
                if (shader == null)
                    continue;

                if (m_MatListClass != null && m_MatListClass.shader == shader)
                {
                    GUI.backgroundColor = SEL_BUTTON_COLOR;
                }
                else
                {
                    GUI.backgroundColor = UN_SEL_BUTTON_COLOR;
                }

                if (GUILayout.Button(shader.name))
                {
                    m_MatGroupIndexScrollPos = Vector2.zero;
                    m_MatGroupScrollPos = Vector2.zero;
                    m_MatListClass = iter.Value;
                    m_MatListClass.Reset();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制顶部工具栏，提供"刷新 Shader 列表"按钮以重新扫描工程内所有材质。
        /// </summary>
        void DrawShaderToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = UN_SEL_BUTTON_COLOR;

            if (GUILayout.Button("刷新Shader列表", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_ShaderScrollPos = Vector2.zero;
                m_MatListClass = null;
                MaterialSearchModify.RefreshShaderList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
