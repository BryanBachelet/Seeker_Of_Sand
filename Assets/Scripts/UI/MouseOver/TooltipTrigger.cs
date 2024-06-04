using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace GuerhoubaGames.UI
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public string header;
        [Multiline] public string content;

        private float m_timer;

        private IEnumerator m_coroutine;

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_coroutine = DelayCall(0.5f);

            StartCoroutine(m_coroutine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopCoroutine(m_coroutine);
            TooltipManager.Hide();
        }

        public IEnumerator DelayCall(float time)
        {
            float timer = 0;
            while (timer < time)

            {
                timer += Time.deltaTime;
                yield return Time.deltaTime;

            }

            TooltipManager.Show(content, header);
        }
    }
}