using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class MaterialSearchModifyEditor : EditorWindow
    {
        /// <summary>
        /// 
        /// </summary>
        [MenuItem("gtm/Scene/���ʱ༭��", false, 300)]
        static void OnOpen()
        {
            MaterialSearchModifyEditor window = new MaterialSearchModifyEditor();
            window.titleContent = new GUIContent("���ʱ༭��");
            window.Show();

            MaterialSearchModify.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        const float SHADER_SCROLL_WIDTH = 1000f;

        /// <summary>
        /// 
        /// </summary>
        const float SHADER_SCROLL_HEIGHT = 150f;

        /// <summary>
        /// 
        /// </summary>
        const float MAT_GROUP_INDEX_HEIGHT = 150f;

        /// <summary>
        /// 
        /// </summary>
        static Color SEL_BUTTON_COLOR = Color.blue;

        /// <summary>
        /// 
        /// </summary>
        static Color UN_SEL_BUTTON_COLOR = Color.gray;

        /// <summary>
        /// 
        /// </summary>
        const float BUTTON_COMMON_WIDTH = 200f;

        /// <summary>
        /// 
        /// </summary>
        string m_MatPropertyName;

        /// <summary>
        /// 
        /// </summary>
        MaterialSearchModifyMatType m_MatPropertyType;

        /// <summary>
        /// 
        /// </summary>
        string m_MatPropertyVal;

        /// <summary>
        /// 
        /// </summary>
        string m_MatKeywordName;

        /// <summary>
        /// 
        /// </summary>
        string m_MatKeywordSel;

        /// <summary>
        /// 
        /// </summary>
        string m_MatKeywordVal;

        /// <summary>
        /// 
        /// </summary>
        Vector2 m_ShaderScrollPos;

        /// <summary>
        /// 
        /// </summary>
        Vector2 m_MatGroupIndexScrollPos;

        /// <summary>
        /// 
        /// </summary>
        Vector2 m_MatGroupScrollPos;

        /// <summary>
        /// 
        /// </summary>
        MaterialSearchModifyMatList m_MatListClass;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        void DrawMatListGroupIndexToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            if (m_MatListClass != null)
            {
                GUI.backgroundColor = UN_SEL_BUTTON_COLOR;

                var matNum = m_MatListClass.GetMatNum();
                var groupNum = m_MatListClass.GetGroupNum();
                GUILayout.Label($"������Ŀ�� ��{matNum}");
                GUILayout.Label($"һ���ֳ� ��{groupNum}����");
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        void DrawMatListGroupPropertySet()
        {
            EditorGUILayout.BeginHorizontal();

            m_MatPropertyName = EditorGUILayout.TextField("������������ ��", m_MatPropertyName);
            m_MatPropertyType = (MaterialSearchModifyMatType)EditorGUILayout.EnumPopup("������������ ��", m_MatPropertyType);
            m_MatPropertyVal = EditorGUILayout.TextField("��������ֵ�����Ÿ����� ��", m_MatPropertyVal, GUILayout.Width(500));

            if (GUILayout.Button("���ݲ�������ɸѡ����", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SearchAllMaterialByProperty(m_MatPropertyName, m_MatPropertyType, m_MatPropertyVal);
            }

            if (GUILayout.Button("�������в�������", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SetAllMaterialProperty(m_MatPropertyName, m_MatPropertyType, m_MatPropertyVal);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        void DrawMatListGroupKeywordSelect()
        {
            EditorGUILayout.BeginHorizontal();

            m_MatKeywordSel = EditorGUILayout.TextField("ɸѡ��Keyword�б�, �ո���� ��", m_MatKeywordSel);

            if (GUILayout.Button("����Keyword�б�ɸѡ����", GUILayout.Width(300f)))
            {
                m_MatListClass.SearchAllMaterialByKeywordList(m_MatKeywordSel);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        void SkipAllMatUnvalidKeyword()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("�޳����е���ЧKeyword", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SkipAllMatUnvalidKeyword();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        void DrawMatListGroupKeywordSet()
        {
            EditorGUILayout.BeginHorizontal();

            m_MatKeywordName = EditorGUILayout.TextField("����Keyword���� ��", m_MatKeywordName);
            m_MatKeywordVal = EditorGUILayout.TextField("����Keyword���� ��", m_MatKeywordVal);

            if (GUILayout.Button("�������в���Keyword", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_MatListClass.SetAllMaterialKeyword(m_MatKeywordName, m_MatKeywordVal);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        void DrawShaderToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = UN_SEL_BUTTON_COLOR;

            if (GUILayout.Button("ˢ��Shader�б�", GUILayout.Width(BUTTON_COMMON_WIDTH)))
            {
                m_ShaderScrollPos = Vector2.zero;
                m_MatListClass = null;
                MaterialSearchModify.RefreshShaderList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
