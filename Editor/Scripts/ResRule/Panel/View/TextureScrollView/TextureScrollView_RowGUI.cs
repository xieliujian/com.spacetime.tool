using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    internal partial class TextureScrollView
    {
        protected override void OnRowGUI(RowGUIArgs args)
        {
            var viewItem = (TextureViewItem)args.item;
            var assetImporter = viewItem.textureImporter;

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

        void OnCellGUI(Rect cellRect, TextureViewItem viewData, int column, ref RowGUIArgs args)
        {
            string value = "Missing";
            var columnType = (TextureProps)column;

            switch (columnType)
            {
                case TextureProps.RuleName:
                    value = viewData.GetRuleName(out Color ruleNameColor);
                    GUI.color = ruleNameColor;
                    break;

                case TextureProps.Path:
                    value = viewData.Path;
                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    return;

                case TextureProps.CompressSize:
                    value = EditorUtility.FormatBytes(viewData.CompressSize);
                    break;

                case TextureProps.Size:
                    value = viewData.Size.ToString();
                    break;

                case TextureProps.WindeowFormat:
                    value = viewData.WindeowFormat.ToString();
                    break;

                case TextureProps.IOSFormat:
                    value = viewData.IOSFormat.ToString();
                    break;

                case TextureProps.AndroidFormat:
                    value = viewData.AndroidFormat.ToString();
                    break;

                case TextureProps.TextureType:
                    value = viewData.TextureType.ToString();
                    break;

                case TextureProps.AlphaSource:
                    value = viewData.AlphaSource.ToString();
                    break;

                case TextureProps.sRGB:
                    value = viewData.sRGB.ToString();
                    break;

                case TextureProps.Read_Write:
                    value = viewData.Read_Write.ToString();
                    break;

                case TextureProps.CenerateMipMaps:
                    value = viewData.CenerateMipMaps.ToString();
                    break;

                case TextureProps.WrapMode:
                    value = viewData.WrapMode.ToString();
                    break;

                default:
                    break;
            }

            DefaultGUI.Label(cellRect, value, args.selected, args.focused);
        }
    }
}
