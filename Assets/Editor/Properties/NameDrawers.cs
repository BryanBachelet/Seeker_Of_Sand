
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
        Debug.Log(property.propertyPath[test + 1]);
        int testIndex = int.Parse(property.propertyPath[test + 1].ToString());
        Debug.Log(property.propertyPath);

        //var serializedproperty = SerializationUtility.f .FindParentProperty(property);
        // System.Array.IndexOf(serializedproperty, -? -);
        GUIContent content = new GUIContent(((CustomArrayName)attribute).nametest +" "+ testIndex.ToString());
        
       // property.floatValue  = EditorGUI.FloatField(rect, content, property.floatValue);
        EditorGUI.PropertyField(rect,  property, content);

        EditorGUI.EndProperty();
        //foreach (SerializedProperty A in property)
        //{
        //    if (property.isArray)
        //        Debug.Log("True Array");

        //    GUIContent content = new GUIContent(((CustomArrayName)attribute).nametest);
        //    float height = GetPropertyHeight(A, content);
        //    Rect a = A.rectValue;
        //    a.height = 100;
        //    EditorGUI.FloatField(a, content, A.floatValue) ;
        //}

    }
}
