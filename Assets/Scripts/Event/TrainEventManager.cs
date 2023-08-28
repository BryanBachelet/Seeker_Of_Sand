using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainEventManager : MonoBehaviour
{
    [Header("Events Parameters")]
    [SerializeField] private ObjectHealthSystem[] m_pilarHealthSystem;
    [SerializeField] private bool m_isEventActive;

    private int m_pilarActive = 0;



    public void Update()
    {
        CheckPilarState();
    }

    public void StartEvent()
    {
        if (m_pilarActive < 3)
            m_pilarActive++;
        m_isEventActive = true;
        for (int i = 0; i < m_pilarActive; i++)
        {
            m_pilarHealthSystem[i].ChangeState(EventObjectState.Active);
        }
    }
    public void EndEvent()
    {
        m_isEventActive = false;
        for (int i = 0; i < m_pilarActive; i++)
        {
            m_pilarHealthSystem[i].ChangeState(EventObjectState.Deactive);
        }
    }
    private void CheckPilarState()
    {
        if (!m_isEventActive) return;
        for (int i = 0; i < m_pilarActive; i++)
        {
            if (!m_pilarHealthSystem[i].IsEventActive())
                GameOver();
        }
    }

    private void GameOver()
    {
        SceneManager.LoadScene(0);
    }

}
