using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuitAuto : MonoBehaviour
{
    private float timer = 2;
    private float countdown = 0;

    // Update is called once per frame
    void Update()
    {
        if(countdown < timer)
        {
            countdown += Time.deltaTime;
        }else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            Application.ExternalEval("window.close();");
#elif UNITY_STANDALONE
            Application.Quit();
#else
            Application.Quit();
#endif
        }
    }
}
