using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.UI
{
    public struct TooltipPositionData
    {
        public Vector2 basePosition;
        public Vector2 offset;
    }

    public struct TooltipDisplayData
    {
       public string content;
       public string header;
    }

    public class TooltipManager : MonoBehaviour
    {
        private static TooltipManager current;
        public Tooltip tooltip;

        public void Awake()
        {
            current = this;
        }
     
        public static void SetTooltipPosition(TooltipPositionData positionData)
        {
            current.tooltip.SetPosition(positionData);
        }
        public static void Show(TooltipDisplayData displayData)
        {
            current.tooltip.SetText(displayData);
            current.tooltip.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            current.tooltip.gameObject.SetActive(false);
        }

        

    }

}

