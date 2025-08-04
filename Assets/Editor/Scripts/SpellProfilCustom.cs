using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpellSystem;

[CustomEditor(typeof(SpellProfil))]
public class SpellProfilCustom : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle style = new GUIStyle();
        style.fixedWidth = 50;

        if (GUILayout.Button("Update Stats", GUILayout.Width(100), GUILayout.Height(20)))
        {
            SpellProfil spell = (SpellProfil)target;
            spell.UpdateStatistics();
        }
    }
}
