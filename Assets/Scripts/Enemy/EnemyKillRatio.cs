using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyKillRatio : MonoBehaviour
{
    [Header("Enemy Kill Ratio Parameter")]
    [SerializeField] private float m_killRatioTime = 60;
    [SerializeField] private Text m_textFeedbackUI;
    [SerializeField] private int m_maxKillPerMinute = 70;
    private List<float> m_enemyCount = new List<float>();


    private void Start()
    {
        DrawUI();
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
                DrawUI();
                continue;
            }
            m_enemyCount[i] -= Time.deltaTime;
        }
    }

    private void DrawUI()
    {
        m_textFeedbackUI.text = m_enemyCount.Count.ToString();
    }

    public float GetRatioValue()
    {
        return ((float)m_enemyCount.Count / (float)m_maxKillPerMinute);
    }

    public void AddEnemiKill()
    {
        m_enemyCount.Add(m_killRatioTime);
        DrawUI();
    }
}
