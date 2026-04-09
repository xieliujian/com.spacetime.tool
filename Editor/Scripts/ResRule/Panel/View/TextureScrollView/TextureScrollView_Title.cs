using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ST.Tool
{
    internal partial class TextureScrollView
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
                    headerContent = EditorGUIUtility.TrTextContent("CompressSize(KB)"),
                    width = 120, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("Size"),
                    width = 60, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("W Format", "Windows Format"),
                    width = 80, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("I Format", "IOS Format"),
                    width = 80, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("A Format", "Android Format"),
                    width = 80, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("TextureType"),
                    width = 100, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("AlphaSource"),
                    width = 100, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("sRGB"),
                    width = 70, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("Read|Write"),
                    width = 50, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("CenerateMipMaps"),
                    width = 130, autoResize = false,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = EditorGUIUtility.TrTextContent("WrapMode"),
                    width = 100, autoResize = false,
                },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(TextureProps)).Length, "列数和枚举数量必须一致");
            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}
