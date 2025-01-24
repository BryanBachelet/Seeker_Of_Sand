using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GuerhoubaTools
{
    public class SceneLoaderComponent : MonoBehaviour
    {
        public int hubSceneIndex = 5;
        public int gameSceneIndex = 6;
        public int sceneRenderIndex = 1;
        public int tutorielSceneIndex = 1;

        private List<AsyncOperation> m_asyncOperationsList = new List<AsyncOperation>();

        public bool IsSceneReady()
        {
            if (m_asyncOperationsList.Count != 0)
                return m_asyncOperationsList[0].progress >= 0.9f;
            else
                return false;
        }

        public void Start()
        {
            StartCoroutine(LoadScene());
            DontDestroyOnLoad(gameObject);
        }


        public void LoadHubScene()
        {
            //m_asyncOperationsList.Add(new AsyncOperation());

            //m_asyncOperationsList[0] = SceneManager.LoadSceneAsync(gameSceneIndex, LoadSceneMode.Additive);
            //m_asyncOperationsList[0].allowSceneActivation = false;



            //  StartCoroutine(LoadScene());

        }


        IEnumerator LoadScene()
        {
            //yield return null;

            //Begin to load the Scene you specify
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(gameSceneIndex, LoadSceneMode.Additive);
            m_asyncOperationsList.Add(asyncOperation);
            m_asyncOperationsList[0].completed += LoadSceneRender;

            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            Debug.Log("Pro :" + asyncOperation.progress);
            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                ////Output the current progress
                //m_Text.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";

                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    Debug.Log("Pro :" + asyncOperation.progress);

                    ////Wait to you press the space key to activate the Scene
                    //if (Input.GetKeyDown(KeyCode.Space))
                    //ActiveScene();
                }

                yield return null;
            }
        }

        public void ActiveScene()
        {
            m_asyncOperationsList[0].allowSceneActivation = true;
            m_asyncOperationsList.Clear();
            //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(gameSceneIndex));
            Debug.Log("UnloadLaunch");

            //StartCoroutine(Unload());
        }


        public void LoadSceneRender(AsyncOperation asyncOperation)
        {

            SceneManager.UnloadSceneAsync(tutorielSceneIndex);
        }
        IEnumerator Unload()
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            while (!asyncOperation.isDone)
            {
                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    Debug.Log("Pro :" + asyncOperation.progress);
                }
                yield return null;
            }
        }

        public float GetLoadProgress()
        {
            if (m_asyncOperationsList.Count > 0 && m_asyncOperationsList[0] != null && !m_asyncOperationsList[0].isDone)
                return m_asyncOperationsList[0].progress;

            return 0.0f;
        }

        private void LoadSceneRender()
        {


            //m_asyncOperationsList.Add(SceneManager.LoadSceneAsync(sceneRenderIndex, LoadSceneMode.Additive));
            //m_asyncOperationsList[1].allowSceneActivation = false;
            //m_asyncOperationsList[1].completed += LoadGameScene;

        }

        public void ActiveSceneRender()
        {
            m_asyncOperationsList.RemoveAt(0);

            m_asyncOperationsList[0].allowSceneActivation = true;

        }


        private void LoadGameScene(AsyncOperation op)
        {
            m_asyncOperationsList.RemoveAt(0);
            m_asyncOperationsList.Add(SceneManager.UnloadSceneAsync(hubSceneIndex));
            m_asyncOperationsList[0].completed += AllowLoadGameScene;
            m_asyncOperationsList.Add(SceneManager.LoadSceneAsync(gameSceneIndex));
            m_asyncOperationsList[1].allowSceneActivation = false;


        }

        private void AllowLoadGameScene(AsyncOperation op)
        {
            m_asyncOperationsList.RemoveAt(0);
            m_asyncOperationsList[0].allowSceneActivation = true;
            m_asyncOperationsList[0].completed += SetGameSceneActive;

        }

        private void SetGameSceneActive(AsyncOperation op)
        {
            //            SceneManager.SetActiveScene(SceneManager.GetSceneAt(gameSceneIndex));
        }

    }
}