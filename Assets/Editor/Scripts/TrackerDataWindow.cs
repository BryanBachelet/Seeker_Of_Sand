using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TrackerDataWindow : EditorWindow
{

    private string m_dataFolderPath ="";
    private string m_feedbackInfo = "";

    [MenuItem("Tools/DataTrackerTool ")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TrackerDataWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Tool Settings", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Folder path", m_dataFolderPath, EditorStyles.textField);
        if (GUILayout.Button("SelectFolder"))
        {
            m_dataFolderPath = EditorUtility.OpenFolderPanel("Test", Application.dataPath, "");
        }

        if (GUILayout.Button("Create Data Sheet"))
        {
            Tracker.StatsTracker.CreateStatSheet(m_dataFolderPath, m_dataFolderPath,ref m_feedbackInfo);
        }
        GUILayout.Label(m_feedbackInfo);
    }
}
