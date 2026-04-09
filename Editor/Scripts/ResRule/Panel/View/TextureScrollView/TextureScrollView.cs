using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace ST.Tool
{
    internal partial class TextureScrollView : IScrollView
    {
        public static TextureScrollView Get()
        {
            var state = new TreeViewState();
            var h = GetMultiColumnHeader();
            return new TextureScrollView("t:texture", typeof(TextureImporter), state, h);
        }

        public TextureScrollView(string filter, Type type, TreeViewState state, MultiColumnHeader multiColumnHeader)
            : base(type, filter, state, multiColumnHeader)
        {
            rowHeight = 20;
            showAlternatingRowBackgrounds = true;
            multiColumnHeader.sortingChanged += OnSortingChanged;
            Reload();
        }

        /// <summary>
        /// 尝试添加资源信息
        /// </summary>
        protected override bool TryAddRes(TreeViewItem root, int id, string resGUID)
        {
            if (root == null || id < 0 || string.IsNullOrEmpty(resGUID))
            {
                return false;
            }

            var viewData = ViewItemModule.S.GetData<TextureViewItem>(resGUID);

            if (viewData != null)
            {
                viewData.id = id;
                viewData.RefreshRuleData<TextureImporter>(m_RuleManager);
                root.AddChild(viewData);
                return true;
            }

            return false;
        }
    }
}
