using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetuper : MonoBehaviour
{
    public int sceneToLoadByIndex = 2;
    public bool blockStart = false;
    // Start is called before the first frame update
    void Start()
    {
        if (blockStart) return;
        SceneManager.LoadScene(sceneToLoadByIndex, LoadSceneMode.Additive);
        Debug.Log("Scene has been load");
    }

    public void LoadSpecificScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene, LoadSceneMode.Single);
    }

}
