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
    // Start is called before the first frame update
    void Start()
    {
        thisScene = SceneManager.GetActiveScene();
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
                    //LoadSceneWithLoading();
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

    public async void LoadSceneWithLoading()
    {
        for (int i = 0; i < sceneToLoadAtStart.Count; i++)
        {
            sceneToLoadAtStart[i].allowSceneActivation = true;
            Debug.Log(sceneToLoadAtStart.Count);
            do
            {
                await System.Threading.Tasks.Task.Delay(100);

            } while (sceneToLoadAtStart[i].progress < 0.9f);
            await System.Threading.Tasks.Task.Delay(1000);
            //Debug.Log(lastScene.name);
        }

        //Debug.Log(lastScene.Length);
        //for (int i = 1; i < lastScene.Length - 1; i++)
        //{
        //
        //    lastScene[i] = SceneManager.GetSceneAt(SceneManager.sceneCount + 2 + i);
        //    SceneManager.SetActiveScene(lastScene[i]);
        //
        //}

        for (int j = 0; j < GOToUnload.Length; j++)
        {
            if (GOToUnload[j] != this.gameObject)
            {
                Destroy(GOToUnload[j]);
            }
        }
        Scene[] activeScene = SceneManager.GetAllScenes();
        string DebugSceneActive = "";
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            DebugSceneActive = DebugSceneActive + SceneManager.GetSceneAt(i).name + " [Active] || ";
        }
        SceneManager.UnloadSceneAsync("Hub - 1");
        Destroy(this.gameObject);
    }


}
