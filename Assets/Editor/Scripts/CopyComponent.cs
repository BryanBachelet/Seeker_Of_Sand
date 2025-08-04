using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class CopyComponent : EditorWindow
{
    [SerializeField] private GameObject srcObj;
    [SerializeField] private GameObject dstObj;

    [MenuItem("Tools/CopyComponent")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CopyComponent));
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Component Window", EditorStyles.boldLabel);

        srcObj = (GameObject)EditorGUILayout.ObjectField(srcObj, typeof(GameObject), true);
        dstObj = (GameObject)EditorGUILayout.ObjectField(dstObj, typeof(GameObject), true);

        if (GUILayout.Button("Copy"))
        {
            if (srcObj == null || dstObj == null)
            {
                ShowNotification(new GUIContent("No object selected for copying"));
            }else
            {
                CopyComponnents();
            }
        }
    }


    private void CopyComponnents()
    {
        Component[] comps = srcObj.GetComponents(typeof(Component));
        for (int i = 0; i < comps.Length; i++)
        {
            Debug.Log("Comps " + i + " " + comps[i].GetType().Name);
            if (dstObj.GetComponent(comps[i].GetType()) == null)
            {
                ComponentUtility.CopyComponent(comps[i]);
                ComponentUtility.PasteComponentAsNew(dstObj);
            }

            

        }


    }


}
