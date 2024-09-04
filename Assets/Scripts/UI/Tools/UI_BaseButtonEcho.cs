using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{
    public class UI_BaseButtonEcho : MonoBehaviour
    {
        public UI_ButtonEcho buttonEcho = new UI_ButtonEcho();
        public Image sprite;
        public Color selectColor;
        public Color deselectColor;

        public void Start()
        {
            buttonEcho.onSelectButton += SelectButton;
            buttonEcho.onDeselectButton += DeselectButton;
            buttonEcho.InitComponent();
        }

        public void Update()
        {
            buttonEcho.UpdateSystem();
        }

        private void SelectButton()
        {
            sprite.color = selectColor;
        }

        private void DeselectButton()
        {
            sprite.color = deselectColor;
        }
}
}
