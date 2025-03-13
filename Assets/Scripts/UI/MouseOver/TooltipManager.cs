using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.UI
{
    [System.Serializable]
    public struct TooltipPositionData
    {
        [HideInInspector] public Vector2 basePosition;
        [HideInInspector] public Vector2 offset;
        [HideInInspector] public Vector2 additionnalOffset;
        [HideInInspector] public TooltipPositionData[] tooltipPositionDatasAdditional;
    }

    [System.Serializable]
    public struct TooltipDisplayData
    {
        public string header;
        [TextArea]
        public string content;
        public Sprite contentImage;
        public bool asImageInData;
        [HideInInspector] public TooltipDisplayData[] tooltipEventDatasAdditional;
    }

    public class TooltipManager : MonoBehaviour
    {
        private static TooltipManager current;
        public Tooltip tooltip;
        private bool isShown;
        public void Awake()
        {
            current = this;
        }

        public static void SetTooltipPosition(TooltipPositionData positionData)
        {
            current.tooltip.ApplyPosition(positionData);
        }

        public static void GiveData(TooltipDisplayData displayData)
        {
            current.tooltip.ApplyTextData(displayData, true);

        }

        public static void GivePositionData(TooltipPositionData positionData)
        {
            current.tooltip.ApplyPosition(positionData, true);

        }
        public static void Show(int elementToDisplay)
        {
            current.tooltip.ShowElement(elementToDisplay);
            current.isShown = true;
        }

        public static void AddSystemTooltip(string idTooltip)
        {
            string content = InfoSystemic.instance.GetIdDescription(idTooltip);
            string header = InfoSystemic.instance.GetIdTitle(idTooltip);

            if (string.IsNullOrEmpty(content)) return;

            current.tooltip.SetNewSystemicTooltip(header, content);


        }

        public static void Hide()
        {
            if (current.isShown)
            {
                current.tooltip.HideTooltip();
                current.isShown = false;
            }


        }

    }

}

