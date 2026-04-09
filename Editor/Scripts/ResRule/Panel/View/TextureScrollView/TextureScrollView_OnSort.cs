using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class TextureScrollView
    {
        protected override void SortItems(List<TreeViewItem> dataList)
        {
            if (dataList == null || dataList.Count <= 0)
            {
                return;
            }

            int sortedColumnIndex = multiColumnHeader.state.sortedColumnIndex;

            if (sortedColumnIndex < 0)
            {
                multiColumnHeader.state.sortedColumnIndex = (int)TextureProps.CompressSize;
            }

            dataList.Sort(OnSort);
        }

        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SortItems(rootItem.children);
            Reload();
        }

        int OnSort(TreeViewItem data1, TreeViewItem data2)
        {
            var ascending = multiColumnHeader.IsSortedAscending(multiColumnHeader.state.sortedColumnIndex);
            var tvd1 = (ascending ? data1 : data2) as TextureViewItem;
            var tvd2 = (ascending ? data2 : data1) as TextureViewItem;

            if (tvd1 == null || tvd2 == null)
            {
                return 0;
            }

            var sortType = (TextureProps)multiColumnHeader.state.sortedColumnIndex;

            switch (sortType)
            {
                case TextureProps.RuleName:
                    return CompareToRuleName(tvd1, tvd2);
                case TextureProps.Path:
                    return tvd1.Path.CompareTo(tvd2.Path);
                case TextureProps.CompressSize:
                    return tvd1.CompressSize.CompareTo(tvd2.CompressSize);
                case TextureProps.Size:
                    return tvd1.Size.CompareTo(tvd2.Size);
                case TextureProps.WindeowFormat:
                    return tvd1.WindeowFormat.CompareTo(tvd2.WindeowFormat);
                case TextureProps.IOSFormat:
                    return tvd1.IOSFormat.CompareTo(tvd2.IOSFormat);
                case TextureProps.AndroidFormat:
                    return tvd1.AndroidFormat.CompareTo(tvd2.AndroidFormat);
                case TextureProps.TextureType:
                    return tvd1.TextureType.CompareTo(tvd2.TextureType);
                case TextureProps.AlphaSource:
                    return tvd1.AlphaSource.CompareTo(tvd2.AlphaSource);
                case TextureProps.sRGB:
                    return tvd1.sRGB.CompareTo(tvd2.sRGB);
                case TextureProps.Read_Write:
                    return tvd1.Read_Write.CompareTo(tvd2.Read_Write);
                case TextureProps.CenerateMipMaps:
                    return tvd1.CenerateMipMaps.CompareTo(tvd2.CenerateMipMaps);
                case TextureProps.WrapMode:
                    return tvd1.WrapMode.CompareTo(tvd2.WrapMode);
            }

            return 0;
        }

        /// <summary>
        /// 对比规则名字
        /// </summary>
        int CompareToRuleName(TextureViewItem data1, TextureViewItem data2)
        {
            var name1 = data1.GetRuleName(out Color color1);
            var name2 = data2.GetRuleName(out Color color2);

            var colorValue1 = color1 == Color.red ? 1 : 0;
            var colorValue2 = color2 == Color.red ? 1 : 0;

            if (colorValue1 != colorValue2)
            {
                return colorValue1.CompareTo(colorValue2);
            }

            return name1.CompareTo(name2);
        }
    }
}
