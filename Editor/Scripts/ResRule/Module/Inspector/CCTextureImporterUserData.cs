using System.Text;
using UnityEditor;

namespace ST.Tool
{
public class CCTextureImporterUserData
{
    public const string PROP_MOBILE_MAX_SIZE = "MobileMaxSize";
    public const string PROP_MOBILE_FORMAT = "MobileFormat";
    static StringBuilder s_SB = new StringBuilder();

    public static void SetProp(TextureImporter ti, string propName, string value)
    {
        if (ti == null)
        {
            return;
        }
        
        string userDataStr = ti.userData;
        string[] items = string.IsNullOrEmpty(userDataStr) ? new string[0] : userDataStr.Split("@@");
        bool exists = false;
        
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].StartsWith(propName))
            {
                exists = true;
                items[i] = $"{propName}={value}";
            }
        }

        s_SB.Clear();
        
        if (!exists)
        {
            s_SB.Append($"{propName}={value}@@");
        }

        foreach (var item in items)
        {
            s_SB.Append(item);
            s_SB.Append("@@");
        }
        
        ti.userData = s_SB.Length == 0 ? string.Empty : s_SB.Remove(s_SB.Length - 2, 2).ToString();
    }
    
    public static void SetProp(TextureImporter ti, string propName, int value)
    {
        if (ti == null)
        {
            return;
        }

        SetProp(ti, propName, value.ToString());
    }
    
    public static void RemoveProp(TextureImporter ti, string propName)
    {
        if (ti == null)
        {
            return;
        }
        
        string userDataStr = ti.userData;
        string[] items = string.IsNullOrEmpty(userDataStr) ? new string[0] : userDataStr.Split("@@");
        int index = -1;
        
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].StartsWith(propName))
            {
                index = i;
            }
        }

        s_SB.Clear();

        for (int i = 0; i < items.Length; i++)
        {
            if (i == index)
            {
                continue;
            }
            
            s_SB.Append(items[i]);
            s_SB.Append("@@");
        }
        
        ti.userData = s_SB.Length == 0 ? string.Empty : s_SB.Remove(s_SB.Length - 2, 2).ToString();
    }

    public static string GetPropStr(TextureImporter ti, string propName)
    {
        if (ti == null)
        {
            return string.Empty;
        }
        
        string userDataStr = ti.userData;
        if (string.IsNullOrEmpty(userDataStr))
        {
            return string.Empty;
        }
        
        string[] items = string.IsNullOrEmpty(userDataStr) ? new string[0] : userDataStr.Split("@@");
        
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].StartsWith(propName))
            {
                string value = items[i].Substring(propName.Length + 1);
                return value;
            }
        }

        return string.Empty;
    }
    
    public static int GetPropInt(TextureImporter ti, string propName)
    {
        string val = GetPropStr(ti, propName);
        if (int.TryParse(val, out int value))
        {
            return value;
        }

        return 0;
    }
}
}
