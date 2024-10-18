using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{

    public class UI_HealthPlayer : MonoBehaviour
    {
        [SerializeField] public Image m_currentHealthSlider;
        [SerializeField] private Image m_healthBufferSlider ;
        [SerializeField] public Image m_currentQuarterSlider;

        [Header("Buffer Parameter")]
        public float bufferTime = 0.5f;
        private float m_lastLifeUpdateTime;
        private float m_currentLifeRatio;
        private float m_lastLifeRatio;

        public void Update()
        {
            UpdateLifeBuffer();
        }

        private void UpdateLifeBuffer()
        {
            float currentLifeRatio = Mathf.Lerp(m_lastLifeRatio, m_currentLifeRatio, (Time.time - m_lastLifeUpdateTime) / bufferTime);
            m_currentHealthSlider.fillAmount = currentLifeRatio; 
        }


        public void UpdateLifeBar(float lifeRatio,float quarterRatio)
        {
            m_lastLifeRatio = m_healthBufferSlider.fillAmount;
            m_lastLifeUpdateTime = Time.time;
            m_currentLifeRatio = lifeRatio;

            m_healthBufferSlider.fillAmount = lifeRatio;
            m_currentQuarterSlider.fillAmount = quarterRatio;

        }
    }
}