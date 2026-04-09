using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ST.Tool
{
    internal partial class ModelScrollView
    {
        static MultiColumnHeader GetMultiColumnHeader()
        {
            var headerState = GetMultiColumnHeaderState();          // 该字段需序列号，否则调整标题宽高后再次选中会还原
            var multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.ResizeToFit();
            return multiColumnHeader;
        }

        static MultiColumnHeaderState GetMultiColumnHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("RuleName"),
                    allowToggleVisibility = false,
                    minWidth = 80, autoResize = true,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("AssetPath", "AssetPath Desc"),      // 内容
                    minWidth = 150, autoResize = true,                                                  // 自动调整大小
                    headerTextAlignment = TextAlignment.Right,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("MeshCompression"),
                    width = 120, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("Read|Write"),
                    width = 80, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("OptimizeMesh"),
                    width = 90, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("Normal"),
                    width = 60, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("UVS"),
                    width = 50, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("MeshCount"),
                    width = 80, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("顶点数"),
                    width = 50, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("面数量"),
                    width = 50, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("蒙皮数"),
                    width = 50, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("骨骼数"),
                    width = 50, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("AnimationType"),
                    width = 100, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("OptimizeGameObjects"),
                    width = 130, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("Anim.Compression"),
                    width = 120, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("动作时长"),
                    width = 80, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("循环"),
                    width = 40, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("动作大小"),
                    width = 60, autoResize = false,
                },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(ModelProps)).Length, "列数和枚举数量必须一致");
            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}
