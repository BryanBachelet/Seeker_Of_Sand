using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{

    public class UI_HealthPlayer : MonoBehaviour
    {
        [SerializeField] public Image m_currentHealthSlider;
        [SerializeField] private Image m_healthBufferSlider ;
        [HideInInspector] private Material m_healthBarMaterial;
        [SerializeField] public Image m_currentQuarterSlider;
        [SerializeField] private TMP_Text m_healthText;

        [Header("Buffer Parameter")]
        public float bufferTime = 0.5f;
        private float m_lastLifeUpdateTime;
        private float m_currentLifeRatio;
        private float m_lastLifeRatio;

        private void Start()
        {
            m_healthBarMaterial = m_currentHealthSlider.material;
        }

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
            float ratioTest = 0.5f - lifeRatio * 0.5f;
            if(m_healthBarMaterial == null) { m_healthBarMaterial = m_currentHealthSlider.material; }
            m_healthBarMaterial.SetFloat("_ratio", ratioTest);
            m_healthBufferSlider.fillAmount = lifeRatio;
            //m_currentQuarterSlider.fillAmount = quarterRatio;


        }

        public void UpdateLifeData(int currentHealth, int maxHealth)
        {
            m_healthText.text = currentHealth + "<size=60%>/<size=100%>" + maxHealth;
        }
    }
}