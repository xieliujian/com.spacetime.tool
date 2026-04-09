using UnityEditor;
using UnityEngine;

namespace ST.Tool
{
    internal partial class AudioScrollView
    {
        protected override void OnRowGUI(RowGUIArgs args)
        {
            var viewItem = (AudioViewItem)args.item;
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

        void OnCellGUI(Rect cellRect, AudioViewItem viewData, int column, ref RowGUIArgs args)
        {
            string value = "Missing";
            var columnType = (AudioProps)column;

            switch (columnType)
            {
                case AudioProps.RuleName:
                    value = viewData.GetRuleName(out Color ruleNameColor);
                    GUI.color = ruleNameColor;
                    break;

                case AudioProps.Path:
                    value = viewData.displayName;
                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    return;

                case AudioProps.ForceToMono:
                    value = viewData.ForceToMono.ToString();
                    break;

                case AudioProps.LoadInBackground:
                    value = viewData.LoadInBackground.ToString();
                    break;

                case AudioProps.LoadType:
                    value = viewData.LoadType.ToString();
                    break;

                case AudioProps.WindeowFormat:
                    value = viewData.WindeowFormat.ToString();
                    break;

                case AudioProps.IOSFormat:
                    value = viewData.IOSFormat.ToString();
                    break;

                case AudioProps.AndroidFormat:
                    value = viewData.AndroidFormat.ToString();
                    break;

                case AudioProps.Quality:
                    value = viewData.Quality.ToString();
                    break;

                case AudioProps.SamplreRateSetting:
                    value = viewData.SamplreRateSetting.ToString();
                    break;

                case AudioProps.MemorySize:
                    value = EditorUtility.FormatBytes(viewData.MemorySize);
                    break;

                case AudioProps.TimeLanght:
                    value = viewData.TimeLanght.ToString();
                    break;

                default:
                    break;
            }

            DefaultGUI.Label(cellRect, value, args.selected, args.focused);
        }
    }
}
