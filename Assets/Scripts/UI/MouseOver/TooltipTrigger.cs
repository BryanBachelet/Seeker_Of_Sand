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

        public bool IsActive;

        public Sprite tooltip_Image;
        [HideInInspector] public bool associated_Image_Bool;
        public Vector2 offSet_Image;
        public void Start()
        {

            m_rectTransform = GetComponent<RectTransform>();
            if(tooltip_Image != null ) {associated_Image_Bool = true;}
            else { associated_Image_Bool=false;}
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActive) return;

            if (OnEnter != null) OnEnter.Invoke();
            if (OnEnterData != null) OnEnterData.Invoke(tooltipEventData);
            m_coroutine = DelayCall(delay);

            StartCoroutine(m_coroutine);
        }

        public void OnDisable()
        {
            if (m_coroutine != null) StopCoroutine(m_coroutine);
            TooltipManager.Hide();
        }

        public void HideTooltip()
        {
            if (m_coroutine != null)  StopCoroutine(m_coroutine);
            TooltipManager.Hide();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsActive || m_coroutine == null) return;

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
            }
            else
            {
                TooltipManager.Show(displayData);
            }

        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!IsActive) return;

            TooltipPositionData positionData = CreatePositionData();
            TooltipManager.SetTooltipPosition(positionData);
        }

        #region Managing Tooltip data function
        private TooltipDisplayData CreateDisplayData()
        {
            TooltipDisplayData displayData = new TooltipDisplayData();

            displayData.content = content;
            displayData.header = header;
            displayData.contentImage = tooltip_Image;
            displayData.asImageInData = associated_Image_Bool;
            return displayData;
        }

        private TooltipPositionData CreatePositionData()
        {
            TooltipPositionData positionData = new TooltipPositionData();
            positionData.offset = offset;
            positionData.additionnalOffset = offSet_Image;
            Vector2 vector = Camera.main.WorldToScreenPoint(m_rectTransform.position);
            positionData.basePosition = vector;
            return positionData;
        }

        #endregion
    }
}