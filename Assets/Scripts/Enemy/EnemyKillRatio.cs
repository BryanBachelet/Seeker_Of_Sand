using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyKillRatio : MonoBehaviour
{
    [Header("Enemy Kill Ratio Parameter")]
    [HideInInspector] private float m_killRatioTime = 60;
    [HideInInspector] private int m_maxKillPerMinute = 70;

    [HideInInspector] private List<float> m_enemyCount = new List<float>();

    [HideInInspector] private Experience_System m_PlayerExperienceRef;


    private void Start()
    {
        if(m_PlayerExperienceRef==null)
        {
            m_PlayerExperienceRef = GameObject.Find("Player").GetComponent<Experience_System>();
        }
    }

    void Update()
    {
        UpdateEnemyCount();
    }

    private void UpdateEnemyCount()
    {
        for (int i = 0; i < m_enemyCount.Count; i++)
        {
            if (m_enemyCount[i] <= 0)
            {
                m_enemyCount.RemoveAt(i);
                continue;
            }
            m_enemyCount[i] -= Time.deltaTime;
        }
    }

    public float GetRatioValue()
    {
        return ((float)m_enemyCount.Count / (float)m_maxKillPerMinute);
    }

    public void AddEnemiKill()
    {
        m_enemyCount.Add(m_killRatioTime);
        //m_PlayerExperienceRef.OnEnemyKilled();
    }
}
