using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    internal partial class ModelScrollView
    {
        protected override void OnRowGUI(RowGUIArgs args)
        {
            var viewItem = (ModelViewItem)args.item;
            var assetImporter = viewItem.importer;

            if (assetImporter == null)
            {
                base.RowGUI(args);
                return;
            }

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                var cellRect = args.GetCellRect(i);
                // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
                CenterRectUsingSingleLineHeight(ref cellRect);
                OnCellGUI(cellRect, viewItem, args.GetColumn(i), ref args);
                GUI.color = Color.white;
            }
        }

        void OnCellGUI(Rect cellRect, ModelViewItem viewData, int column, ref RowGUIArgs args)
        {
            string value = "Missing";
            var columnType = (ModelProps)column;

            switch (columnType)
            {
                case ModelProps.RuleName:
                    value = viewData.GetRuleName(out Color ruleNameColor);
                    GUI.color = ruleNameColor;
                    break;

                case ModelProps.Path:
                    value = viewData.displayName;
                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    return;

                case ModelProps.MeshCompression:
                    value = viewData.MeshCompression.ToString();
                    break;

                case ModelProps.Read_Write:
                    value = viewData.Read_Write.ToString();
                    break;

                case ModelProps.OptimizeMesh:
                    value = viewData.OptimizeMesh.ToString();
                    break;

                case ModelProps.Normal:
                    value = viewData.Normal.ToString();
                    break;

                case ModelProps.UVS:
                    value = viewData.UVS.ToString();
                    break;

                case ModelProps.MeshCount:
                    value = viewData.MeshCount.ToString();
                    break;

                case ModelProps.VertexCount:
                    value = viewData.VertexCount.ToString();
                    break;

                case ModelProps.TriCount:
                    value = viewData.TriCount.ToString();
                    break;

                case ModelProps.SkinCount:
                    value = viewData.SkinCount.ToString();
                    break;

                case ModelProps.BoneCount:
                    value = viewData.BoneCount.ToString();
                    break;

                case ModelProps.AnimationType:
                    value = viewData.AnimationType.ToString();
                    break;

                case ModelProps.OptimizeGameObjects:
                    value = viewData.OptimizeGameObjects.ToString();
                    break;

                case ModelProps.AnimCompression:
                    value = viewData.AnimCompression.ToString();
                    break;

                case ModelProps.AnimationClipLength:
                    value = viewData.AnimationClipLength.ToString();
                    break;

                case ModelProps.IsLoop:
                    value = viewData.IsLoop.ToString();
                    break;

                case ModelProps.AnimationClipSize:
                    value = EditorUtility.FormatBytes(viewData.AnimationClipSize);
                    break;

                default:
                    break;
            }

            DefaultGUI.Label(cellRect, value, args.selected, args.focused);
        }
    }
}
