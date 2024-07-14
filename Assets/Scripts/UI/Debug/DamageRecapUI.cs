using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace GuerhoubaGames.UI
{

    public class DamageRecapUI : MonoBehaviour
    {
        public GameObject background;
        public TMP_Text damageText;

        public void ShowUI()
        {
            background.SetActive(!background.activeSelf);
        }

        public void UpdateText(string text)
        {
            damageText.text = text;
        }
    }
}
