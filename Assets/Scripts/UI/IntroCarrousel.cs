using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCarrousel : MonoBehaviour
{
    [Header("Carrousel Parameters")]
    [SerializeField] private GameObject[] m_panelInfo = new GameObject[9];
    [SerializeField] private int m_gameplaySceneIndex = 2;
    private int m_currentIndex;

    public void ChangeScene()
    {
        SceneManager.LoadScene(m_gameplaySceneIndex);
    }

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

        if (m_currentIndex - 1 == 0)
            m_currentIndex = m_panelInfo.Length-1;
        else
            m_currentIndex--;

        m_panelInfo[m_currentIndex].SetActive(true);
    }
}
