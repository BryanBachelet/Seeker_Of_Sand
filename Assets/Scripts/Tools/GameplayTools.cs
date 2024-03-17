using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GuerhoubaTools.Gameplay
{
   public class ClockTimer
    {
        private float m_timer = 0.0f;
        private float m_duration = 0.0f;
        public bool isActivate = false;
        private Image clockAssociated;
        public void SetTimerDuration(float duration, Image clockImage)
        {
            m_duration = duration;
            clockAssociated = clockImage;
        }

        public bool UpdateTimer()
        {
            if (clockAssociated != null) { clockAssociated.fillAmount = m_timer / m_duration; }
            if (m_timer>m_duration)
            {
                m_timer = 0.0f;
                return true;
            }else
            {
                m_timer += Time.deltaTime;
                return false;
            }

        }
        
        public float GetDuration() { return m_duration; }
        
        public float GetTimer() { return m_timer; }


    }

}
