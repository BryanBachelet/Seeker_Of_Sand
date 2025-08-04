
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaTools.Curves;

[CustomEditor(typeof(CurveObject))]
public class CurveObjectCustom : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Stats", GUILayout.Width(100), GUILayout.Height(20)))
        {
            CurveObject curve = (CurveObject)target;
            curve.SetCurve();
        }
    }
}
