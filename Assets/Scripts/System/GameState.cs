using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ObjectState
{
    public bool isPlaying = true;
}

public class GameState : MonoBehaviour
{
    private static Enemies.EnemyManager m_enemyManager;
    private static List<ObjectState> listObject = new List<ObjectState>(0);

    [SerializeField] private static bool m_isPlaying = true;

    private static bool m_isDeath;
    private static float m_deathRatio;
    [SerializeField] private float m_timeBetweenDeath = 0.8f;
    [SerializeField] private int m_indexScene = 0;
    private bool m_isDeathProcessusActive;
    private float m_timerBetweenDeath = 0.0f;
    private GameObject m_pauseMenuObj;

    public void Start()
    {
        m_enemyManager = GetComponent<Enemies.EnemyManager>();
    }

    public static void DeathActivation()
    {
        m_isDeath = true;
    }

    public void Update()
    {
        if (m_isDeath)
        {
            if (!m_isDeathProcessusActive)
                DeathEffect();

            if(m_timerBetweenDeath>m_timeBetweenDeath)
            {
                SceneManager.LoadScene(m_indexScene);
                m_isDeath = false;
                m_timerBetweenDeath = 0;
            }
            else
            {
                m_timerBetweenDeath += Time.deltaTime;
            }
        }
    }

    public void DeathEffect()
    {
        ChangeState();
        m_isDeathProcessusActive = true;
    }

    public static void ChangeState()
    {
        m_isPlaying = !m_isPlaying;
        for (int i = 0; i < listObject.Count; i++)
        {
            listObject[i].isPlaying = m_isPlaying;
        }
        m_enemyManager.ChangePauseState(m_isPlaying);
    }

    public static void AddObject(ObjectState state)
    {
        listObject.Add(state);
        state.isPlaying = m_isPlaying;
    }

    public static void RemoveObject(ObjectState state)
    {
        listObject.Remove(state);
    }

    public void OpenGameMenu(InputAction.CallbackContext ctx)
    {
        m_pauseMenuObj.SetActive(!m_pauseMenuObj.activeSelf);
    }
}
