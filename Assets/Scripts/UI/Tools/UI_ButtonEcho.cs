using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GuerhoubaGames.UI
{
    [System.Serializable]
    public class UI_ButtonEcho
    {
        public Button button;
        public Action onSelectButton;
        public Action onDeselectButton;

        [HideInInspector] public bool isFirstFrameSelect = true;
        [HideInInspector] public bool isFirstFrameDeselect = true;

        public void InitComponent()
        {
            isFirstFrameSelect = true;
            isFirstFrameDeselect = false ;
        }

        public void UpdateSystem()
        {
            bool value = EventSystem.current.currentSelectedGameObject == button.gameObject;

            if (value && isFirstFrameSelect)
            {
                isFirstFrameDeselect = true;
                isFirstFrameSelect = false;
                if (onSelectButton != null)
                {
                    onSelectButton.Invoke();
                }
                return;
            }
            if (!value && isFirstFrameDeselect)
            {
                isFirstFrameDeselect = false;
                isFirstFrameSelect = true;

                if (onDeselectButton != null)
                {
                    onDeselectButton.Invoke();
                }

                return;
            }
        }

    }
}