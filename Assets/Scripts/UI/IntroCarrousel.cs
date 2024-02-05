using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCarrousel : MonoBehaviour
{
    public static IntroCarrousel instance;
    private Scene thisScene;

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private UnityEngine.UI.Image _progressBar;
    private float _target;

    [Header("Carrousel Parameters")]
    [SerializeField] private GameObject[] m_panelInfo = new GameObject[9];
    [SerializeField] private int[] m_gameplaySceneIndex = new int[1];
    private int m_currentIndex;

    public TMPro.TMP_Text textProgression;


    private GuerhoubaTools.SceneLoaderComponent m_sceneLoaderComponent;
    private void Awake()
    {
        thisScene = SceneManager.GetActiveScene();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        m_sceneLoaderComponent = FindAnyObjectByType<GuerhoubaTools.SceneLoaderComponent>();
    }

    #region Panel Functions
    public void ChangeUp()
    {
        m_panelInfo[m_currentIndex].SetActive(false);

        if (m_currentIndex + 1 == m_panelInfo.Length)
            m_currentIndex = 0;
        else
            m_currentIndex++;

        m_panelInfo[m_currentIndex].SetActive(true);
    }

    public void ChangeDown()
    {
        m_panelInfo[m_currentIndex].SetActive(false);

        if (m_currentIndex - 1 == -1)
            m_currentIndex = m_panelInfo.Length - 1;
        else
            m_currentIndex--;

        m_panelInfo[m_currentIndex].SetActive(true);
    }
    #endregion 


    public void ActiveLoadScene()
    {
        _loaderCanvas.SetActive(true);
        m_sceneLoaderComponent.LoadHubScene();
    }

    private void Update()
    {

        _progressBar.fillAmount = m_sceneLoaderComponent.GetLoadProgress();
        textProgression.text = "Game is loading \n" + (_target * 100) + "%";
    }


}
