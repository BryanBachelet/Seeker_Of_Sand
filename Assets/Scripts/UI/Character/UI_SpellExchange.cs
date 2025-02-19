using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{


    public class UI_SpellExchange : MonoBehaviour
    {
        public Character.CharacterSpellBook spellBook;
        public GameObject panelUI;

        public void OpenSpellExchangeUI()
        {
            GameState.ChangeState();
            panelUI.SetActive(true);
        }

        public void CloseSpellExchangeUI()
        {
            GameState.ChangeState();
            panelUI.SetActive(false);
            spellBook.CloseUiExchange();
        }

        public void CancelAddingSpell()
        {

            CloseSpellExchangeUI();

        }

        public void ExchangeSpell(int index)
        {
            spellBook.ExchanceSpell(index);
            CloseSpellExchangeUI();

        }
    }
}