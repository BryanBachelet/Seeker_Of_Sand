
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(CustomArrayName))]
public class NameDrawers : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        //  SerializedProperty PROP = property.


        string propertyPath = property.propertyPath;
        int idx = propertyPath.LastIndexOf('.');
        propertyPath = propertyPath.Substring(0, idx);
        SerializedProperty serializedProperty = property.serializedObject.FindProperty(propertyPath);
        EditorGUI.BeginProperty(rect, label, property);

        if (property.propertyType != SerializedPropertyType.Float)
        {
            EditorGUI.HelpBox(rect, $"{nameof(CustomArrayName)} can only be used for strings!", MessageType.Error);
            return;
        }
        int test = property.propertyPath.LastIndexOf('[');
        int testIndex = int.Parse(property.propertyPath[test + 1].ToString());



        GUIContent content = new GUIContent(((CustomArrayName)attribute).nametest +" "+ testIndex.ToString());
        
       
        EditorGUI.PropertyField(rect,  property, content);

        EditorGUI.EndProperty();
     

    }
}
