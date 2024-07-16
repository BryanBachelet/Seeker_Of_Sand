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

        string[] arrayName = { "val_bool", "val_int", "val_float", "val_string" };

        for (int i = 0; i < 4; i++)
        {
            if (property.FindPropertyRelative("stat").intValue - ((i * 1000)) < 1000)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative(arrayName[i]), GUIContent.none);
                break;
            }
        }

        //if (property.FindPropertyRelative("stat").intValue - (int)ValueType.BOOL < 1000)
        //    EditorGUI.PropertyField(position, property.FindPropertyRelative("val_bool"), GUIContent.none);
        //else if ( property.FindPropertyRelative("stat").intValue - (int)ValueType.INT < 1000)
        //    EditorGUI.PropertyField(position, property.FindPropertyRelative("val_int"), GUIContent.none);
        //else if (property.FindPropertyRelative("stat").intValue - (int)ValueType.FLOAT < 1000 )
        //    EditorGUI.PropertyField(position, property.FindPropertyRelative("val_float"), GUIContent.none);
        //else if (property.FindPropertyRelative("stat").intValue - (int)ValueType.STRING < 1000)
        //    EditorGUI.PropertyField(position, property.FindPropertyRelative("val_string"), GUIContent.none);


        position.width += 10;
        EditorGUI.EndProperty();
       
        // base.OnGUI(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20.0f;
    }
}
