using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class AudioScrollView
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
                return;
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
            var tvd1 = (ascending ? data1 : data2) as AudioViewItem;
            var tvd2 = (ascending ? data2 : data1) as AudioViewItem;

            if (tvd1 == null || tvd2 == null)
            {
                return 0;
            }

            var sortType = (AudioProps)multiColumnHeader.state.sortedColumnIndex;

            switch (sortType)
            {
                case AudioProps.RuleName:
                    return CompareToRuleName(tvd1, tvd2);
                case AudioProps.Path:
                    return tvd1.Path.CompareTo(tvd2.Path);
                case AudioProps.ForceToMono:
                    return tvd1.ForceToMono.CompareTo(tvd2.ForceToMono);
                case AudioProps.LoadInBackground:
                    return tvd1.LoadInBackground.CompareTo(tvd2.LoadInBackground);
                case AudioProps.LoadType:
                    return tvd1.LoadType.CompareTo(tvd2.LoadType);
                case AudioProps.WindeowFormat:
                    return tvd1.WindeowFormat.CompareTo(tvd2.WindeowFormat);
                case AudioProps.IOSFormat:
                    return tvd1.IOSFormat.CompareTo(tvd2.IOSFormat);
                case AudioProps.AndroidFormat:
                    return tvd1.AndroidFormat.CompareTo(tvd2.AndroidFormat);
                case AudioProps.Quality:
                    return tvd1.Quality.CompareTo(tvd2.Quality);
                case AudioProps.SamplreRateSetting:
                    return tvd1.SamplreRateSetting.CompareTo(tvd2.SamplreRateSetting);
                case AudioProps.MemorySize:
                    return tvd1.MemorySize.CompareTo(tvd2.MemorySize);
                case AudioProps.TimeLanght:
                    return tvd1.TimeLanght.CompareTo(tvd2.TimeLanght);
            }

            return 0;
        }

        /// <summary>
        /// 对比规则名字
        /// </summary>
        int CompareToRuleName(AudioViewItem data1, AudioViewItem data2)
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
