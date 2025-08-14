using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;



namespace BorsalinoTools
{
    public class DebugMessageData
    {
        public string text;
        public Color color;
        public float duration;
        public int key;
    }

    public class DebugMessageMetaData
    {
        public string memberName;
        public string filepath;
        public int lineNumber;
        public int key;
        public bool isEmpty = true;

        public override bool Equals(object obj)
        {
            DebugMessageMetaData b = (DebugMessageMetaData)obj;
            if (b.key == -1)
                return false;

            return key == b.key;
        }

    }




    public class ScreenDebuggerTool : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private const int m_debugMessageFontSize = 15;
        private static Color m_noColor = new Color(0, 0, 0, 0);

        private static List<DebugMessageData> debugMessageDataList = new List<DebugMessageData>();
        private static List<DebugMessageMetaData> debugMessageMetaDataList = new List<DebugMessageMetaData>();

        private bool m_enableScreenMessage = true;
#endif 

        public static void AddMessage(string text, int key = -1, float duration = 2, Color color = new Color(), [System.Runtime.CompilerServices.CallerMemberName] string membName = "",
                                                    [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
                                                    [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD


            DebugMessageData messageData;
            messageData = new DebugMessageData();
            messageData.duration = duration;
            messageData.text = text;
            messageData.color = color == m_noColor ? Color.white : color;
            messageData.key = key;

            DebugMessageMetaData messageMetaData = new DebugMessageMetaData();
            messageMetaData.memberName = membName;
            messageMetaData.filepath = filePath;
            messageMetaData.lineNumber = lineNumber;
            messageMetaData.key = key;
            messageMetaData.isEmpty = false;

            if (!debugMessageMetaDataList.Contains(messageMetaData))
            {
                debugMessageDataList.Add(messageData);
                debugMessageMetaDataList.Add(messageMetaData);

            }
            else
            {
               int indexArray = debugMessageMetaDataList.IndexOf(messageMetaData);
                debugMessageDataList[indexArray].duration = duration;
                debugMessageDataList[indexArray].text = text;
                debugMessageMetaDataList[indexArray].isEmpty = false;
            }
#endif 

        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        public void Awake()
        {
          
        }

        public void Start()
        {
            CommandsConsole.logAction += SetScreenDebugStateCommand;

        }
        public string inputText = "";
        public void Update()
        {

            for (int i = 0; i < debugMessageDataList.Count; i++)
            {
                if (debugMessageDataList[i].duration > 0)
                    debugMessageDataList[i].duration -= Time.deltaTime;
                else
                {
                    debugMessageMetaDataList.RemoveAt(i);
                    debugMessageDataList.RemoveAt(i);
                }
            }


        }


        public void SetScreenDebugStateCommand(string inputdata, params object[] args)
        {


            if (inputdata == "DisableScreenMessage")
            {
                m_enableScreenMessage = false;
            }

            if (inputdata == "EnableScreenMessage")
            {
                m_enableScreenMessage = true;
            }

        }


        private void OnGUI()
        {

            if (m_enableScreenMessage)
            {
                GUILayout.BeginArea(new Rect(Screen.width - 400, 50, 400, 400));

                DisplayDebugMessage();
                GUILayout.EndArea();
            }
        }

        private void DisplayDebugMessage()
        {
            for (int i = 0; i < debugMessageDataList.Count; i++)
            {
                DebugMessageData messageData = debugMessageDataList[i];
                DebugMessageMetaData metaData = debugMessageMetaDataList[i];
                if (!metaData.isEmpty)
                {
                    GUIStyle style1 = new GUIStyle();
                    style1.normal.textColor = messageData.color;
                    style1.fontSize = m_debugMessageFontSize;
                    GUILayout.Label(messageData.text, style1);
                }
            }
        }

#endif
    }

}