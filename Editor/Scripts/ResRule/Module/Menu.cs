using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.Tool
{
    public class Menu
    {
        [MenuItem("MHT/ResTools/ResCheckPanel")]
        internal static ResCheckPanel OpenResCheckPanel()
        {
            return ResCheckPanel.GetWindow();
        }

        //[MenuItem("MHT/Close ResCheckPanel")]
        //internal static void CloseResCheckPanel()
        //{
        //    EditorWindow.GetWindow<ResCheckPanel>().Close();
        //}

        [MenuItem("MHT/ResTools/ResRulePanel")]
        internal static ResRulePanel OpenResRulePanel()
        {
            return ResRulePanel.GetWindow();
        }

        [MenuItem("MHT/ResTools/ClearViewDataCache")]
        internal static void OnClearViewDataCache()
        {
            ViewItemModule.S.Clear();
        }

        //[MenuItem("MHT/Close ResRulePanel")]
        //internal static void CloseResRulePanel()
        //{
        //    EditorWindow.GetWindow<ResRulePanel>().Close();
        //}
    }
}
