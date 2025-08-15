using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CombatSceneManager))]
public class CombatSceneCustomInterface : Editor
{

    
    SerializedProperty mobCountProperty;
    public void OnEnable()
    {
        serializedObject.Update();
        mobCountProperty = serializedObject.FindProperty("mobCount");
        CombatSceneManager m_capsuleProfile = (CombatSceneManager)target;
        GuerhoubaGames.Enemies.EnemyType enemyType;
        int count = GuerhoubaGames.Enemies.EnemyType.GetNames(typeof(GuerhoubaGames.Enemies.EnemyType)).Length;

       if(mobCountProperty.arraySize != count) mobCountProperty.arraySize = count;
        serializedObject.ApplyModifiedProperties();
    }
    public override void OnInspectorGUI()
    {
        CombatSceneManager combatSceneManager = (CombatSceneManager)target;
        base.OnInspectorGUI();

        if (!combatSceneManager.specialEnemisSquad) return;
        serializedObject.Update();

        EditorGUILayout.Space(20);

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = new Color(200f / 255, 200f / 255, 200f / 255, 1);

        EditorGUILayout.LabelField("Enemis Squad Preset", titleStyle);


        GuerhoubaGames.Enemies.EnemyType enemyType;
        for (int i = 0; i < mobCountProperty.arraySize; i++)
        {
            enemyType = (GuerhoubaGames.Enemies.EnemyType)i;
            string value = enemyType.ToString();
            value = value[0] + value.Substring(1).ToLower();
            mobCountProperty.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(value, mobCountProperty.GetArrayElementAtIndex(i).intValue);
        }

        EditorGUILayout.HelpBox("Spawn Input : T", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
