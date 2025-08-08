using UnityEditor;
using UnityEngine;
using SpellSystem;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(StatData))]
public class StatsSpellDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        label = EditorGUI.BeginProperty(position, label, property);
        BaseDisplay(position, property, label);
        EditorGUI.EndProperty();


        // base.OnGUI(position, property, label);
    }


    public void BaseDisplay(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.PrefixLabel(position, label);
        position.width *= 0.5f;
        position.height = 18.0f;

        GUIContent guiContent = new GUIContent();
        guiContent.text = property.FindPropertyRelative("nameStat").stringValue;
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(guiContent).x;
        GUIContent guiContent2 = new GUIContent();
        SerializedProperty statsProperty = property.FindPropertyRelative("stat");
        EditorGUI.PropertyField(position, statsProperty, guiContent);

        position.width -= 10;
        position.x += position.width + 20;
        position.width *= 2f;
        position.width *= 0.4f;

        string[] arrayName = { "val_bool", "val_int", "val_float", "val_string" };

        for (int i = 0; i < 4; i++)
        {
            if (property.FindPropertyRelative("stat").intValue - ((i * 1000)) < 1000)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative(arrayName[i]), GUIContent.none);
                break;
            }
        }

        position.x += position.width + 10;
        GUIContent label2 = new GUIContent();
        label2.text = "V";
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(label2).x;
        EditorGUI.PrefixLabel(position, label2);
        position.x += 10;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("isVisible"), GUIContent.none);
        position.width += 10;
      



    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

       return 20.0f;

    }
}


[CustomPropertyDrawer(typeof(StatDataLevel))]
public class StatsSpellLevelDrawer : StatsSpellDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect basePosition = position;

        GUIContent label2 = new GUIContent();

        BaseDisplay(position, property, label);
        label2.text = "Multiplier";
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(label2).x;
        basePosition.y += 20;
        basePosition.width *= 0.5f;
        basePosition.height = 18.0f;

        label = EditorGUI.BeginProperty(basePosition, label, property);
        EditorGUI.PrefixLabel(basePosition, label2);
        basePosition.width -= 10;
        basePosition.x += basePosition.width + 20;
        basePosition.width *= 2f;
        basePosition.width *= 0.4f;
        EditorGUI.PropertyField(basePosition, property.FindPropertyRelative("multiply"), GUIContent.none);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 50.0f;
    }
}


[CustomPropertyDrawer(typeof(StatDataUpgrade))]
public class StatsSpellUpgradeDrawer : StatsSpellDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect basePosition = position;

        GUIContent label2 = new GUIContent();

        BaseDisplay(position, property, label);
        label2.text = "isOnlyAddWithTag";
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(label2).x;
        basePosition.y += 20;
        basePosition.width = EditorStyles.label.CalcSize(label2).x + 25f;
        basePosition.height = 18.0f;

        label = EditorGUI.BeginProperty(basePosition, label, property);
        EditorGUI.PrefixLabel(basePosition, label2);
        basePosition.width -= 10;
        basePosition.x += basePosition.width + 20;
        basePosition.width *= 2f;
        basePosition.width *= 0.4f;
        EditorGUI.PropertyField(basePosition, property.FindPropertyRelative("isOnlyAddWithTag"), GUIContent.none);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 50.0f;
    }
}