using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BorsalinoTools
{

    public class CommandsConsole : MonoBehaviour
    {

        private bool m_showWindow;
        private string m_inputText;
        private bool m_isFirstTimeTrigger;
        public delegate  void LogAction(string s, params object[] args);
        public static LogAction logAction;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void Update()
        {
          if(Event.current!= null)
                Debug.Log("Key code : " + Event.current.keyCode);

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Debug.Log("Test Input");
                m_showWindow = !m_showWindow;
                if (m_showWindow) m_isFirstTimeTrigger = false;
            }
        }


        public static int IsInteger(object arg)
        {
            int result = arg is int ? (int)arg : int.MinValue;
            return result;
        }
        public static float IsFloat(object arg)
        {
            float result = arg is float ? (float)arg : float.MinValue;
            return result;
        }


        public void OnGUI()
        {
            if (m_showWindow)
            {
                GUILayout.BeginArea(new Rect(20, Screen.height - 30, 500, 30));
                GUI.SetNextControlName("CommandArea");
                m_inputText = GUILayout.TextField(m_inputText);
                ComputeCommand();
                if (!m_isFirstTimeTrigger)
                {
                    m_isFirstTimeTrigger = true;
    
                    GUI.FocusControl("CommandArea");
                }

                GUILayout.EndArea();
            }
        }

        public void ComputeCommand()
        {
            if ((Event.current.keyCode == KeyCode.Return) && m_inputText != "")
            {
                m_inputText = m_inputText.Trim();
                string[] instruction = m_inputText.Split(" ");
                object[] parameters = new object[instruction.Length - 1];
                int countValidParameter = 0;
                for (int i = 1; i < instruction.Length; i++)
                {
                    float valFloat = 0;
                    int val = 0;
                    bool isFloat = float.TryParse(instruction[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out valFloat);
                    if (instruction[i] == "")
                    {
                        continue;
                    }
                    else if ((instruction[i].Contains('.') || instruction[i].Contains(',')) && isFloat)
                    {
                        parameters[countValidParameter] = valFloat;
                        countValidParameter++;
                    }
                    else if (int.TryParse(instruction[i], out val))
                    {
                        parameters[countValidParameter] = val;
                        countValidParameter++;
                    }
                    else
                    {
                        parameters[countValidParameter] = instruction[i];
                        countValidParameter++;
                    }
                }
                object[] finalParameters = new object[countValidParameter];
                System.Array.Copy(parameters, finalParameters, countValidParameter);


                logAction?.Invoke(instruction[0], finalParameters);
                m_inputText = "";
            }
        }

#endif
    }
}