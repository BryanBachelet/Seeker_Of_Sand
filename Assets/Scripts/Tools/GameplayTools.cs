using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaTools.Gameplay
{
   public class ClockTimer
    {
        private float m_timer = 0.0f;
        private float m_duration = 0.0f;
        public bool isActivate = false;

        public void SetTimerDuration(float duration)
        {
            m_duration = duration;
        }

        public bool UpdateTimer()
        {
            if(m_timer>m_duration)
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
