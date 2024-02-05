using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwaper : MonoBehaviour
{
    public TMPro.TMP_Text textGameStart;

    public bool playerEnter = false;
    private bool awaitForStart = true;
    private float tempsStartGame = 4;

    public List<AsyncOperation> sceneToLoadAtStart = new List<AsyncOperation>();

    private Scene thisScene;
    Scene[] lastScene;
    private GameObject[] GOToUnload;
    public Camera_Hub_Move CameraHubMove;
    private GuerhoubaTools.SceneLoaderComponent m_sceneLoaderComponent;
    // Start is called before the first frame update
    void Start()
    {
        thisScene = SceneManager.GetActiveScene();
        m_sceneLoaderComponent = FindAnyObjectByType<GuerhoubaTools.SceneLoaderComponent>();
        GOToUnload = thisScene.GetRootGameObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (awaitForStart)
        {
            if (playerEnter)
            {
                if (tempsStartGame > 0)
                {
                    tempsStartGame -= Time.deltaTime;
                    textGameStart.text = "Game Start in \n" + (int)tempsStartGame + "...";
                }
                else
                {
                    awaitForStart = false;
                    CameraHubMove.inputDebut = true;

                }
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            textGameStart.color = new Color(0, 0, 0, 1);
            tempsStartGame = 4;
            playerEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            textGameStart.color = new Color(0, 0, 0, 0);
            playerEnter = false;
        }
    }

    public void LoadSceneWithLoading()
    {
        m_sceneLoaderComponent.ActiveSceneRender();
       
    }


}
