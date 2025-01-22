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

    [System.Serializable]
    public struct TooltipAdditionnalData
    {
        public TooltipDisplayData additionnalTooltipDisplay;
        public TooltipPositionData additionnalTooltipPosition;
    }


    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        public float delay = 0.5f;
        public Vector2 offset;
        public string header;
        [Multiline] public string content;

        [HideInInspector] public Action OnEnter;
        [HideInInspector] public Action<TooltipEventData> OnEnterData;
        public TooltipEventData TooltipEventData;
        private float m_timer;

        private IEnumerator m_coroutine;
        private RectTransform m_rectTransform;

        public TooltipAdditionnalData[] additionnalDatas;
        public bool IsActive;


        private bool isActiveShow;

        [Header("Image Tooltip")]
        public Sprite tooltip_Image;
        [HideInInspector] public bool associated_Image_Bool;
        public Vector2 offSet_Image;
        public void Start()
        {

            m_rectTransform = GetComponent<RectTransform>();
            if (tooltip_Image != null) { associated_Image_Bool = true; }
            else { associated_Image_Bool = false; }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActive) return;


            if (OnEnter != null) OnEnter.Invoke();
            if (OnEnterData != null) OnEnterData.Invoke(TooltipEventData);
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
            if (m_coroutine != null) StopCoroutine(m_coroutine);

  
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
            TooltipDisplayData displayData = CreateDisplayData();
            TooltipPositionData positionData = CreatePositionData();
            TooltipManager.GiveData(displayData);
            TooltipManager.Show(displayData.tooltipEventDatasAdditional.Length);
            positionData = CreatePositionData();
            TooltipManager.GivePositionData(positionData);
            float timer = 0;
            while (timer < time)

            {
    
                timer += Time.deltaTime;
                yield return Time.deltaTime;

            }
           
            yield return Time.deltaTime;

            if (String.IsNullOrEmpty(header) && String.IsNullOrEmpty(content))
            {
                Debug.LogWarning(this.gameObject.name + " tooltip is empty in content or header ");
                yield return null;
            }
            else
            {
               
              
                positionData = CreatePositionData();
                TooltipManager.SetTooltipPosition(positionData);
            }

        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!IsActive) return;

            //TooltipPositionData positionData = CreatePositionData();
            //TooltipManager.SetTooltipPosition(positionData);
        }


        public TooltipDisplayData[] GetArrayDisplayData()
        {
            TooltipDisplayData[] array = new TooltipDisplayData[additionnalDatas.Length];
            for (int i = 0; i < additionnalDatas.Length; i++)
            {
                array[i] = additionnalDatas[i].additionnalTooltipDisplay;
            }

            return array;
        }
        public TooltipPositionData[] GetArrayPositionData()
        {
            TooltipPositionData[] array = new TooltipPositionData[additionnalDatas.Length];
            for (int i = 0; i < additionnalDatas.Length; i++)
            {
                array[i] = additionnalDatas[i].additionnalTooltipPosition;
            }

            return array;
        }
        #region Managing Tooltip data function
        private TooltipDisplayData CreateDisplayData()
        {
            TooltipDisplayData displayData = new TooltipDisplayData();

            displayData.content = content;
            displayData.header = header;
            displayData.contentImage = tooltip_Image;
            displayData.asImageInData = associated_Image_Bool;
            displayData.tooltipEventDatasAdditional = GetArrayDisplayData();
            return displayData;
        }

        private TooltipPositionData CreatePositionData()
        {
            TooltipPositionData positionData = new TooltipPositionData();
            positionData.offset = offset;
            positionData.additionnalOffset = offSet_Image;
            Vector2 vector = Camera.main.WorldToScreenPoint(m_rectTransform.position);
            positionData.basePosition = vector;
            positionData.tooltipPositionDatasAdditional = GetArrayPositionData();
            for (int i = 0; i < positionData.tooltipPositionDatasAdditional.Length; i++)
            {
                positionData.tooltipPositionDatasAdditional[i].basePosition = vector;
            }
            return positionData;
        }

        #endregion
    }
}