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
        private bool isActivate = false;
        private Image clockAssociated;
        private TMPro.TMP_Text textStackAssociated;
        private int stack;

        public void SetTimerDuration(float duration, Image clockImage, TMPro.TMP_Text textStack)
        {
            m_duration = duration;
            clockAssociated = clockImage;
            textStackAssociated = textStack;
            stack = 0;
        }

        public bool UpdateTimer()
        {
            if (!isActivate) return false;

            if (clockAssociated != null) { clockAssociated.fillAmount = m_timer / m_duration; }
            if (m_timer>m_duration)
            {
                m_timer = 0.0f;
                stack += 1;
                return true;
            }else
            {
                m_timer += Time.deltaTime;
                textStackAssociated.text = "" + stack;
                return false;
            }

        }

        public void ActiaveClock()
        {
            isActivate = true;
        }
        public void DeactivateClock()
        {
            clockAssociated.fillAmount = 1;
            isActivate = false;
        }
        
        public float GetDuration() { return m_duration; }
        
        public float GetTimer() { return m_timer; }




    }

    public static class Tools
    {
        public static bool IsBelongToLayer(int layerIndex,GameObject obj)
        { 
            return layerIndex == obj.layer;
        }

        public static bool IsBelongToLayerMask(LayerMask mask, GameObject obj)
        {
            return (mask.value & (1 << obj.layer)) > 0;
        }
    }



}
