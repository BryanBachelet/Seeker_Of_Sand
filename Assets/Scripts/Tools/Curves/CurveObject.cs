using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
namespace GuerhoubaTools.Curves
{
    [CreateAssetMenu(fileName = "Curve Object", menuName = "Tools Guerhouba", order = 0)]
    public class CurveObject : ScriptableObject
    {
        [SerializeField] private AnimationCurve m_curve;
        public float Evaluate(float time)
        {
            return m_curve.Evaluate(time);
        }

#if UNITY_EDITOR

       [HideInInspector] [SerializeField] private Object csvfile;

        [HideInInspector][SerializeField] private bool m_maxLength;
        [HideInInspector][SerializeField] private int m_lineTimeIndex=0;
        [HideInInspector][SerializeField] private int m_columnTimeStart=0;

        [HideInInspector][SerializeField] private int m_lineValueIndex = 0;
        [HideInInspector][SerializeField] private int m_columnValueStart = 0;

        [HideInInspector][SerializeField] private int m_dataRangeSize = 0;



        // Stop column length when null;
        // Check if line index is valid 
        // Check column length is valid 
        // Check column start is valid

        public void SetCurve()
        {
            string path =AssetDatabase.GetAssetPath(csvfile);
            Debug.Log(path);

            if (!path.EndsWith(".csv"))
            {
                Debug.Log("The csvfile is not as csv file");
                return;
            }

            string allText = System.IO.File.ReadAllText( path);
            string[] lines = allText.Split('\n');
            int linesCount =  lines.Length;
            int cellsMaxCount = lines[1].Split(',').Length;

            if (!IsCSVIndexValid(linesCount,cellsMaxCount)) return;

            AnimationCurve tmpAnimationCurve = m_curve;

            string[] cellsTime = lines[m_lineTimeIndex].Split(',');
            string[] cellsValue = lines[m_lineValueIndex].Split(',');


            tmpAnimationCurve.ClearKeys();
            for (int i = 0; i < m_dataRangeSize; i++)
            {
                string valueStr = cellsValue[m_columnValueStart + i].Trim('\"');
                string timeValueStr = cellsTime[m_columnTimeStart + i].Trim('\"');

                float timeValue =0;
                float ordoValue =0;
                bool isTimeValid = float.TryParse(timeValueStr,out timeValue);
                bool isOrdoValid = float.TryParse(valueStr,out ordoValue);
                if (!isTimeValid || !isOrdoValid)
                {
                    if (!isTimeValid) Debug.LogError("Cant parse time data in the index " + (m_columnTimeStart + i));
                    if (!isOrdoValid) Debug.LogError("Cant parse value data int the index " + (m_columnValueStart + i));
                    return;
                }
                   
                tmpAnimationCurve.AddKey(timeValue, ordoValue);
            }

            m_curve = tmpAnimationCurve; 

        }

        private bool IsCSVIndexValid(int lineMaxValue, int columnMaxValue)
        {


            if (!IsValidIndex("Line Value",m_lineValueIndex, lineMaxValue)) return false;
            if (!IsValidIndex("Line Time", m_lineTimeIndex, lineMaxValue)) return false;
            if (!IsValidIndex("Column Start Value",m_columnValueStart,columnMaxValue)) return false;
            if (!IsValidIndex("Column Start Time", m_columnTimeStart, columnMaxValue)) return false;
            if (!IsValidIndex("Column Length ", m_dataRangeSize, columnMaxValue)) return false;

            return true;
        }

        private bool IsValidIndex(string nameVariable, int value, int maxValue)
        {
            if (value < 0 || value >= maxValue )
            {
                Debug.LogError("The " + nameVariable + " value is not valid. The max line value is " + maxValue);
            
                return false;
            }
            return true;
        }
#endif

    }
}