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

    public void StartEvent()
    {
        if(m_pilarActive < 3)
            m_pilarActive++;
        
        for (int i = 0; i < m_pilarActive; i++)
        {
            m_pilarHealthSystem[i].ChangeState(EventObjectState.Active);
        }
    }

    

    private void GameOver()
    {
        SceneManager.LoadScene(0);
    }

}
