using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    public class Menu
    {
        [MenuItem("SpaceTime/ResTools/ResCheckPanel")]
        internal static ResCheckPanel OpenResCheckPanel()
        {
            return ResCheckPanel.GetWindow();
        }

        [MenuItem("SpaceTime/ResTools/ResRulePanel")]
        internal static ResRulePanel OpenResRulePanel()
        {
            return ResRulePanel.GetWindow();
        }

        [MenuItem("SpaceTime/ResTools/ClearViewDataCache")]
        internal static void OnClearViewDataCache()
        {
            ViewItemModule.S.Clear();
        }
    }
}
