using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
            Debug.Log(AssetDatabase.GetAssetPath(csvfile));
            if (csvfile.name == "*.csv")
            {

            }
            else
            {
                return;
            }
        }
#endif

    }
}