using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

//[CustomEditor(typeof(TextureImporter))]
//[CanEditMultipleObjects]
namespace ST.Tool
{
public class CCTextureImporterInspector : Editor {
    private static readonly Lazy<Type> _textureImporterInspectorType = new(() => Type.GetType("UnityEditor.TextureImporterInspector, UnityEditor"));
    private static readonly Lazy<MethodInfo> _setAssetImporterTargetEditorMethod = new(() => _textureImporterInspectorType.Value.GetMethod("InternalSetAssetImporterTargetEditor", BindingFlags.Instance | BindingFlags.NonPublic));
    private static readonly Lazy<FieldInfo> _onEnableCalledField = new(() => typeof(AssetImporterEditor).GetField("m_OnEnableCalled", BindingFlags.Instance | BindingFlags.NonPublic));
    
    private Editor _defaultEditor;
    private List<TextureImporter> _targets;

    private void OnEnable() {
        if (_defaultEditor != null) {
            DestroyImmediate(_defaultEditor);
           _defaultEditor = null;
        }

        _defaultEditor = (AssetImporterEditor)CreateEditor(targets, _textureImporterInspectorType.Value);
        _setAssetImporterTargetEditorMethod.Value.Invoke(_defaultEditor, new object[] { this });
        _targets = targets.Cast<TextureImporter>().ToList();
    }

    public override void OnInspectorGUI()
    {
        _defaultEditor.OnInspectorGUI();

        DrawBtnGUI();
        
        hasUnsavedChanges = _defaultEditor.hasUnsavedChanges;
        saveChangesMessage = _defaultEditor.saveChangesMessage;
    }

    public override void SaveChanges() {
        base.SaveChanges();
        _defaultEditor.SaveChanges();
    }

    public override void DiscardChanges() {
        base.DiscardChanges();
        _defaultEditor.DiscardChanges();
    }
    
    void DrawBtnGUI()
    {
        TextureImporter importer = (TextureImporter)target;
        EditorFileUtils.GetTextureOriginalSize(importer, out int width, out int height);

        if (importer.assetPath.StartsWith("Assets/LingRen/Script/ResTools/EditorConfig/"))
        {
            return;
        }
        
        EditorGUILayout.Space();

        EditorGUILayout.LabelField($"OriginalSize: {width}x{height}");
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("2048"))
        {
            SetMobileMaxSize(_targets, 2048);
        }
        
        if (GUILayout.Button("1024"))
        {
            SetMobileMaxSize(_targets, 1024);
        }
        
        if (GUILayout.Button("512"))
        {
            SetMobileMaxSize(_targets, 512);
        }
        
        if (GUILayout.Button("256"))
        {
            SetMobileMaxSize(_targets, 256);
        }
        
        if (GUILayout.Button("Clear"))
        {
            RemoveProp(_targets, CCTextureImporterUserData.PROP_MOBILE_MAX_SIZE);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    void SetMobileMaxSize(List<TextureImporter> importerList, int size)
    {
        foreach (var importer in importerList)
        {
            CCTextureImporterUserData.SetProp(importer, CCTextureImporterUserData.PROP_MOBILE_MAX_SIZE, size);
            importer.SaveAndReimport();
        }
        
        SaveChanges();
    }
    
    void RemoveProp(List<TextureImporter> importerList, string propName)
    {
        foreach (var importer in importerList)
        {
            CCTextureImporterUserData.RemoveProp(importer, propName);
            importer.SaveAndReimport();
        }
        
        SaveChanges();
    }
}
}
