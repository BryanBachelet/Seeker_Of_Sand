using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpellSystem;

[CustomEditor(typeof(UpgradeObject))]
public class UpgradeObjectCustomInterface : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle style = new GUIStyle();
        style.fixedWidth = 100;

        if (GUILayout.Button("Update Tag Stats", GUILayout.Width(120), GUILayout.Height(20)))
        {
            UpgradeObject upgradeObject = (UpgradeObject)target;
            upgradeObject.gameEffectStats.UpdateOnlyTag();
        }
    }
}
