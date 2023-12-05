using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ManualDepthTraitement))]
public class CustomCameraDepth : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ManualDepthTraitement m_manualDepthTraitement = (ManualDepthTraitement)target;
        if(GUILayout.Button("Test"))
        {
            m_manualDepthTraitement.GenerateRenderTexture();
        }
    }

}
