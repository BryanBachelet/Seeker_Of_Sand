using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerieController : MonoBehaviour
{
    [SerializeField] private float m_timeMaintienSeries = 1;
    [SerializeField] private bool m_SerieActive = false;
    [SerializeField] private int m_currentCount = 0;
    [SerializeField] private AnimationCurve m_multiplicatorCurve;
    [SerializeField] public TMPro.TMP_Text m_multiplicatorDisplay;
    [SerializeField] public UnityEngine.UI.Image m_serieTimeDisplay;
    [SerializeField] public TMPro.TMP_Text m_serieKillCount;

    public float m_biggestMultiplicator { get; private set; }

    private float m_lastTimeEnemyHit = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_SerieActive)
        {
            if (Time.time - m_lastTimeEnemyHit > m_timeMaintienSeries)
            {
                m_SerieActive = false;
                m_currentCount = 0;
            }
        }
        if (m_serieTimeDisplay) m_serieTimeDisplay.fillAmount = 1 - (Time.time - m_lastTimeEnemyHit) / m_timeMaintienSeries;
    }

    public void RefreshSeries(bool refresh)
    {

        m_lastTimeEnemyHit = Time.time;
        if (!refresh)
        {
            m_currentCount += 1;
            if (m_biggestMultiplicator < m_currentCount) m_biggestMultiplicator = m_currentCount;
            if (m_serieKillCount)
                m_serieKillCount.text = "" + m_currentCount;
            m_SerieActive = true;
        }
    }

    public float GetXpMultiplicator()
    {
        float multiplicatorValue = m_multiplicatorCurve.Evaluate(m_currentCount);

        m_multiplicatorDisplay.text = "x" + multiplicatorValue;
        return multiplicatorValue;
    }
}
