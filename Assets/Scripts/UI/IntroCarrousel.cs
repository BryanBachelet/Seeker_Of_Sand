using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCarrousel : MonoBehaviour
{
    public static IntroCarrousel instance;

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private UnityEngine.UI.Image _progressBar;
    private float _target;

    [Header("Carrousel Parameters")]
    [SerializeField] private GameObject[] m_panelInfo = new GameObject[9];
    [SerializeField] private int[] m_gameplaySceneIndex = new int[1];
    private int m_currentIndex;


    public TMPro.TMP_Text textProgression;

    Scene lastScene;
    private int m_currentsSceneIndex = 0;
    AsyncOperation[] scene = new AsyncOperation[3];

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene(m_gameplaySceneIndex[0]);
    }

    public void ChangeUp()
    {
        m_panelInfo[m_currentIndex].SetActive(false);

        if (m_currentIndex + 1 == m_panelInfo.Length)
            m_currentIndex = 0;
        else
            m_currentIndex++;

        m_panelInfo[m_currentIndex].SetActive(true);
        Debug.Log("Change up !");
    }

    public void ChangeDown()
    {
        m_panelInfo[m_currentIndex].SetActive(false);

        if (m_currentIndex - 1 == -1)
            m_currentIndex = m_panelInfo.Length-1;
        else
            m_currentIndex--;

        m_panelInfo[m_currentIndex].SetActive(true);
    }


    public async void LoadSceneWithLoading()
    {
        scene[m_currentsSceneIndex] = SceneManager.LoadSceneAsync(m_gameplaySceneIndex[m_currentsSceneIndex], LoadSceneMode.Additive);
      
        if (m_currentsSceneIndex == 0)
        {
            scene[m_currentsSceneIndex].allowSceneActivation = true;
        
        }
        else
        {
            scene[m_currentsSceneIndex].allowSceneActivation = false;
            _loaderCanvas.SetActive(true);
        }
        _target = 0;
        _progressBar.fillAmount = 0;


        do
        {
            await System.Threading.Tasks.Task.Delay(100);

        } while (scene[m_currentsSceneIndex].progress < 0.9f && scene[0].isDone);
        _target = scene[m_currentsSceneIndex].progress;
        await System.Threading.Tasks.Task.Delay(1000);


        if (m_currentsSceneIndex == 0)
        {
            lastScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            _loaderCanvas.SetActive(false);
            SceneManager.SetActiveScene(lastScene); 
        }
    
        m_currentsSceneIndex++;
        
        if(m_currentsSceneIndex < m_gameplaySceneIndex.Length) LoadSceneWithLoading();

    }

    private void Update()
    {
      
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
        textProgression.text = "Game is loading \n" + (_target * 100) + "%";
    }

    public void ActiveLoadingSceneBuffer()
    {
        
        LoadSceneWithLoading();
    }
}
