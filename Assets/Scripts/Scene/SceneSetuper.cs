using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetuper : MonoBehaviour
{
    public int sceneToLoadByIndex = 2;
    public bool blockStart = false;
    public bool haveToUInitializeComponent = false;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        if (blockStart) return;
        if (!SceneManager.GetSceneByBuildIndex(sceneToLoadByIndex).isLoaded)
        {
            SceneManager.LoadScene(sceneToLoadByIndex, LoadSceneMode.Additive);
        }

        if(haveToUInitializeComponent)
        {
            //GameObject.Find("ComponentLinker").GetComponent<ComponentLinkerCrossScene>().ComponentInitialization();
        }

    }

    public void LoadSpecificScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene, LoadSceneMode.Single);
    }

}
