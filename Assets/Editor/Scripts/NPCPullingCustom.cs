using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.Resources;

[CanEditMultipleObjects()]
[CustomEditor(typeof(NPCPullingData))]
public class NPCPullingCustom : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        GUILayout.BeginHorizontal();
    
        if (GUILayout.Button("Search", GUILayout.Width(200), GUILayout.Height(20)))
        {
            NPCPullingData pullingData = (NPCPullingData)target;
            EditorUtility.SetDirty(pullingData);
            pullingData.Search();
            serializedObject.SetIsDifferentCacheDirty();
        }
    
        if (GUILayout.Button("Reset", GUILayout.Width(200), GUILayout.Height(20)))
        {
            NPCPullingData pullingData = (NPCPullingData)target;
            EditorUtility.SetDirty(pullingData);
            pullingData.Reset();
        }
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
        
    }
}
