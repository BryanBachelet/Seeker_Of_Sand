using GuerhoubaGames.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainEventManager : MonoBehaviour
{
    [Header("Events Parameters")]
    [SerializeField] private ObjectHealthSystem[] m_pilarHealthSystem;
    [SerializeField] private bool m_isEventActive;
    [SerializeField] private DayCyclecontroller m_dayCycleScript;

    private EnemyManager m_enemiesManager;
    private int m_pilarActive = 0;

    public void Start()
    {
        SetDayCycleEvent();
        m_enemiesManager = GetComponent<EnemyManager>();
    }

    private void SetDayCycleEvent()
    {
        if (m_dayCycleScript == null) return;

        m_dayCycleScript.nightStartEvent += StartEvent;
        m_dayCycleScript.dayStartEvent += EndEvent;
    }

    public void Update()
    {
        CheckPilarState();
    }

    public void StartEvent()
    {
        return;

        if (m_pilarActive < 3)
            m_pilarActive++;
        m_isEventActive = true;

        for (int i = 0; i < m_pilarActive; i++)
        {
            // m_pilarHealthSystem[i].GetComponent<MeshRenderer>().enabled = true;
            m_pilarHealthSystem[i].transform.GetChild(0).gameObject.SetActive(true);
            m_pilarHealthSystem[i].ChangeState(EventObjectState.Active);
            m_pilarHealthSystem[i].ResetCurrentHealth();
          

        }
    }
    public void EndEvent()
    {
        return;
        m_isEventActive = false;
        for (int i = 0; i < m_pilarActive; i++)
        {
           // m_pilarHealthSystem[i].GetComponent<MeshRenderer>().enabled = false;
            m_pilarHealthSystem[i].transform.GetChild(0).gameObject.SetActive(false);
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
        GameState.DeathActivation();
    }

}
