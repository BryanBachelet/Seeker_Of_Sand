using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelLibrary.SpreadSheet;
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

        [SerializeField] public Object csvfile;
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
            Debug.Log("The lines count  " + lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] cells1 = lines[1].Split(',');
                Debug.Log("The cells " +i+" count  " + cells1.Length);
            }
            


        }
#endif

    }
}