using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace GuerhoubaGames.UI
{
    [System.Serializable]
    public struct TooltipEventData
    {
       public int index;
    }

    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        public float delay = 0.5f;
        public Vector2 offset;
        public string header;
        [Multiline] public string content;

        [HideInInspector] public Action OnEnter;
        [HideInInspector] public Action<TooltipEventData> OnEnterData;
        public TooltipEventData tooltipEventData;
        private float m_timer;

        private IEnumerator m_coroutine;
        private RectTransform m_rectTransform;

        public void Start()
        {
            
            m_rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           if(OnEnter != null) OnEnter.Invoke();
           if(OnEnterData != null) OnEnterData.Invoke(tooltipEventData);
            m_coroutine = DelayCall(delay);

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
            TooltipDisplayData displayData = CreateDisplayData();

            if (String.IsNullOrEmpty(header) && String.IsNullOrEmpty(content))
            {
                yield return null;
            }else
            {
                TooltipManager.Show(displayData);
            }

        }
            


    

        public void OnPointerMove(PointerEventData eventData)
        {
            TooltipPositionData positionData = CreatePositionData();
            TooltipManager.SetTooltipPosition(positionData);
        }

        #region Managing Tooltip data function
        private TooltipDisplayData CreateDisplayData()
        {
            TooltipDisplayData displayData = new TooltipDisplayData();
            displayData.content = content;
            displayData.header = header;
            return displayData;
        }

        private TooltipPositionData CreatePositionData()
        {
            TooltipPositionData positionData = new TooltipPositionData();
            positionData.offset = offset;
            Vector2 vector = Camera.main.WorldToScreenPoint(m_rectTransform.position);
            positionData.basePosition = vector;
            return positionData;
        }

        #endregion
    }
}