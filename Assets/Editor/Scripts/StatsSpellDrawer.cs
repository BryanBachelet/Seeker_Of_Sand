using UnityEditor;
using UnityEngine;
using SpellSystem;

[CustomPropertyDrawer(typeof(StatData))]
public class StatsSpellDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);
        position.width *= 0.5f;
        position.height = 18.0f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("stat"), GUIContent.none);
        
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

        position.x += position.width + 30;
        GUIContent label2 = new GUIContent();
        label2.text = "V";
        EditorGUI.PrefixLabel(position, label2);
        position.x += 10;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("isVisible"), GUIContent.none);
        position.width += 10;
        EditorGUI.EndProperty();
       
        // base.OnGUI(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20.0f;
    }
}
