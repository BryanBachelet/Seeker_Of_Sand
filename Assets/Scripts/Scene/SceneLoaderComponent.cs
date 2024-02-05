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
        public void Start()
        {
            DontDestroyOnLoad(gameObject);
        }


        public void LoadHubScene()
        {
            m_asyncOperationsList.Add(new AsyncOperation());
            m_asyncOperationsList[0] = SceneManager.LoadSceneAsync(hubSceneIndex, LoadSceneMode.Additive);
            m_asyncOperationsList[0].completed += LoadSceneRender;

        }

        public float GetLoadProgress()
        {
            if (m_asyncOperationsList.Count > 0 && m_asyncOperationsList[0] != null && !m_asyncOperationsList[0].isDone)
                return m_asyncOperationsList[0].progress;

            return 0.0f;
        }

        private void LoadSceneRender(AsyncOperation action)
        {
            m_asyncOperationsList.Clear();
            m_asyncOperationsList.Add(SceneManager.UnloadSceneAsync(tutorielSceneIndex));
            m_asyncOperationsList.Add(SceneManager.LoadSceneAsync(sceneRenderIndex, LoadSceneMode.Additive));
            m_asyncOperationsList[1].allowSceneActivation = false;

        }

        public void ActiveSceneRender()
        {
            m_asyncOperationsList.RemoveAt(0);
            m_asyncOperationsList[0].completed += LoadGameScene;
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
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(gameSceneIndex));
        }

    }
}