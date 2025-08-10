using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

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

        public void SetTimerDuration(float duration, Image clockImage = null, TMPro.TMP_Text textStack = null)
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
                if (textStackAssociated != null) textStackAssociated.text = "" + stack;
                return false;
            }

        }

        public void RemoveAllStack()
        {
            //stack = 0;
        }
        public void RemoveStack()
        {
            //stack -= 1;
        }
        public void UpdateStackByCurrent(int stackCurrent)
        {
            stack = stackCurrent + 1;
        }
        public void ActiaveClock()
        {
            isActivate = true;
        }
        public void DeactivateClock()
        {
            if (clockAssociated != null) clockAssociated.fillAmount = 1;
            isActivate = false;
        }
        
        public float GetDuration() { return m_duration; }
        
        public float GetTimer() { return m_timer; }

        public float GetRatio() { return m_timer / m_duration; }



    }

    public static class Tools
    {
        public static bool IsBelongToLayer(int layerIndex, GameObject obj)
        {
            return layerIndex == obj.layer;
        }

        public static bool IsBelongToLayerMask(LayerMask mask, GameObject obj)
        {
            return (mask.value & (1 << obj.layer)) > 0;
        }

        /// <summary>
        /// Allow to change the layer of a Gameobject and it's children
        /// </summary>
        public static void ChangeLayerGameObject(int layer, GameObject obj)
        {
            obj.layer = layer;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                ChangeLayerGameObject(layer, obj.transform.GetChild(i).gameObject);
            }
        }


        public static float RandomThreshold(float baseNumber, float range)
        {
            return UnityEngine.Random.Range(baseNumber - range, baseNumber + range);
        }

        public static T[] RemoveObjectArray<T>(T[] array, T element)
        {
            int index = Array.IndexOf(array, element);
            if (index == -1)
                return array;

            T[] dest = new T[array.Length - 1];
            if (index > 0)
                Array.Copy(array, 0, dest, 0, index);

            if (index < array.Length - 1)
                Array.Copy(array, index + 1, dest, index, array.Length - index - 1);

            return dest;
        }



    }



}

