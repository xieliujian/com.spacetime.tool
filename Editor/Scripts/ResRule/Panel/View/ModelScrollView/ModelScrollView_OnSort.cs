using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ModelScrollView
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

        int OnSort(TreeViewItem data11, TreeViewItem data21)
        {
            var ascending = multiColumnHeader.IsSortedAscending(multiColumnHeader.state.sortedColumnIndex);
            var data1 = (ascending ? data11 : data21) as ModelViewItem;
            var data2 = (ascending ? data21 : data11) as ModelViewItem;

            if (data1 == null || data2 == null)
            {
                return 0;
            }

            var sortType = (ModelProps)multiColumnHeader.state.sortedColumnIndex;

            switch (sortType)
            {
                case ModelProps.RuleName:
                    return CompareToRuleName(data1, data2);
                case ModelProps.Path:
                    return data1.Path.CompareTo(data2.Path);
                case ModelProps.MeshCompression:
                    return data1.MeshCompression.CompareTo(data2.MeshCompression);
                case ModelProps.Read_Write:
                    return data1.Read_Write.CompareTo(data2.Read_Write);
                case ModelProps.OptimizeMesh:
                    return data1.OptimizeMesh.CompareTo(data2.OptimizeMesh);
                case ModelProps.Normal:
                    return data1.Normal.CompareTo(data2.Normal);
                case ModelProps.UVS:
                    return data1.UVS.CompareTo(data2.UVS);
                case ModelProps.MeshCount:
                    return data1.MeshCount.CompareTo(data2.MeshCount);
                case ModelProps.VertexCount:
                    return data1.VertexCount.CompareTo(data2.VertexCount);
                case ModelProps.TriCount:
                    return data1.TriCount.CompareTo(data2.TriCount);
                case ModelProps.SkinCount:
                    return data1.SkinCount.CompareTo(data2.SkinCount);
                case ModelProps.BoneCount:
                    return data1.BoneCount.CompareTo(data2.BoneCount);
                case ModelProps.AnimationType:
                    return data1.AnimationType.CompareTo(data2.AnimationType);
                case ModelProps.OptimizeGameObjects:
                    return data1.OptimizeGameObjects.CompareTo(data2.OptimizeGameObjects);
                case ModelProps.AnimCompression:
                    return data1.AnimCompression.CompareTo(data2.AnimCompression);
                case ModelProps.AnimationClipLength:
                    return data1.AnimationClipLength.CompareTo(data2.AnimationClipLength);
                case ModelProps.IsLoop:
                    return data1.IsLoop.CompareTo(data2.IsLoop);
                case ModelProps.AnimationClipSize:
                    return data1.AnimationClipSize.CompareTo(data2.AnimationClipSize);
            }

            return 0;
        }

        /// <summary>
        /// 对比规则名字
        /// </summary>
        int CompareToRuleName(ModelViewItem data1, ModelViewItem data2)
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
