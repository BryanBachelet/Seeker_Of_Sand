using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using ExcelLibrary.BinaryFileFormat;

namespace GuerhoubaGames.UI
{
    public class Tooltip : MonoBehaviour
    {

        public PlayerInput instance;
        private InputSystemUIInputModule m_inputModule;

        [HideInInspector] public TooltipAdditionnalData[] tooltipAdditionnalData = new TooltipAdditionnalData[5];
        [HideInInspector] public int currentSystemicTooltip =0;

        public TooltipAdditionnal mainTooltip;
        public TooltipAdditionnal[] tooltipAdditionnals;
        public TooltipAdditionnal[] tooltipSystem;

        private TooltipPositionData m_currentTooltipPositionData;

        public void Start()
        {
            m_inputModule = instance.uiInputModule;
        }

        public void ApplyPosition(TooltipPositionData positionData, bool isJustData = false)
        {
            if (mainTooltip != null)
            {
                if (isJustData)
                    positionData.offset.y += 2000;
                mainTooltip.SetPosition(positionData, TooltipAdditionnal.PositionTooltipOffset.None);
            }
            m_currentTooltipPositionData = positionData;
            float height = 0;
            for (int i = 0; i < positionData.tooltipPositionDatasAdditional.Length; i++)
            {
                positionData.tooltipPositionDatasAdditional[i].basePosition = m_currentTooltipPositionData.basePosition;
                positionData.tooltipPositionDatasAdditional[i].offset = m_currentTooltipPositionData.offset;
                Vector2 posToAdd = new Vector2(mainTooltip.GetExtenalWidth(), 0);
           
                if (i != 0)
                {
                 
                    height +=  tooltipAdditionnals[i - 1].GetExternalHeight();
                    posToAdd += new Vector2(0, height);
                    tooltipAdditionnals[i].SetPosition(positionData.tooltipPositionDatasAdditional[i], TooltipAdditionnal.PositionTooltipOffset.Both, posToAdd);
                    height += tooltipAdditionnals[i].GetInternalHeight();
                }
                else
                {
                    tooltipAdditionnals[i].SetPosition(positionData.tooltipPositionDatasAdditional[i], TooltipAdditionnal.PositionTooltipOffset.Horizontal, posToAdd);

                }


            }

            PositionSystemicTooltip();
        }

        public void ShowElement(int count)
        {
            if (mainTooltip != null)
            {
                mainTooltip.gameObject.SetActive(true);
            }
            for (int i = 0; i < count; i++)
            {
                tooltipAdditionnals[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < currentSystemicTooltip; i++)
            {
                tooltipSystem[i].gameObject.SetActive(true);
            }
        }

        public void ApplyTextData(TooltipDisplayData displayData, bool isJustData = false)
        {
            if (mainTooltip != null)
            {
                mainTooltip.gameObject.SetActive(!isJustData);
                mainTooltip.SetText(displayData);
                if (!isJustData) mainTooltip.ResetLayout();
            }
            for (int i = 0; i < displayData.tooltipEventDatasAdditional.Length; i++)
            {
                tooltipAdditionnals[i].gameObject.SetActive(!isJustData);
                tooltipAdditionnals[i].SetText(displayData.tooltipEventDatasAdditional[i]);
                if (!isJustData) tooltipAdditionnals[i].ResetLayout();
            }
            ApplySystemicTooltip("tEST", isJustData);
        }

        public void SetNewSystemicTooltip(string header, string content)
        {
            tooltipAdditionnalData[currentSystemicTooltip].additionnalTooltipDisplay.content = content;
            tooltipAdditionnalData[currentSystemicTooltip].additionnalTooltipDisplay.header = header;
            currentSystemicTooltip++;
        }

        public void PositionSystemicTooltip()
        {
            float height = 0;

            for (int j = 0; j < currentSystemicTooltip; j++)
            {
                TooltipPositionData tooltipPositionData = tooltipAdditionnalData[j].additionnalTooltipPosition;
                tooltipPositionData.basePosition = m_currentTooltipPositionData.basePosition;
                tooltipPositionData.offset = m_currentTooltipPositionData.offset;
                Vector2 posToAdd = new Vector2(0, 0);
             //   posToAdd += m_currentTooltipPositionData.offset;

                tooltipSystem[j].SetPreferredWidth(mainTooltip.rectTransform.rect.width);
                if (j >= 1)
                    height +=  tooltipSystem[j - 1].GetExternalHeight();
                if (j == 0)
                    height +=  mainTooltip.GetExternalHeight();

                posToAdd += new Vector2(0, height);
                tooltipSystem[j].SetPosition(tooltipPositionData, TooltipAdditionnal.PositionTooltipOffset.Vertical, posToAdd,true);
                height += tooltipSystem[j].GetInternalHeight();
            }
        }



        public void ApplySystemicTooltip(string id, bool isJustData = false)
        {
            for (int j = 0; j < currentSystemicTooltip; j++)
            {

                TooltipDisplayData displayData = tooltipAdditionnalData[j].additionnalTooltipDisplay;
                tooltipSystem[j].gameObject.SetActive(!isJustData);
                tooltipSystem[j].SetText(displayData);
                if (!isJustData) tooltipSystem[j].ResetLayout();
            }
        }

        public void HideTooltip()
        {
            Debug.Log("Hide tooltip");
            if (mainTooltip != null)
            {
                //   mainTooltip.rectTransform.localPosition = new Vector2(3000, 3000);
                mainTooltip.gameObject.SetActive(false);
            }
            for (int i = 0; i < tooltipAdditionnals.Length; i++)
            {
                //  tooltipAdditionnals[i].rectTransform.localPosition= new Vector2(3000, 3000);
                tooltipAdditionnals[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < tooltipSystem.Length; i++)
            {
                //tooltipSystem[i].rectTransform.anchoredPosition = new Vector2(3000, 3000);
                tooltipSystem[i].gameObject.SetActive(false);
            }
            currentSystemicTooltip = 0;
        }






    }
}