using GuerhoubaGames.Resources;
using SpellSystem;
using System;
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
        public Sprite[] raritySprite;
        [HideInInspector] public SpellProfil[] spellProfils;
        [SerializeField] private Image[] spellProfilsImage;
        [SerializeField] private Image[] spellCadreImage;

        public GameObject[] skillUiHolder = new GameObject[4];
        public GameObject[] referenceSkill = new GameObject[4];
        public List<GameObject> skillUiHolderTemp;
        public void OpenSpellExchangeUI()
        {
            GameState.ChangeState();

            panelUI.SetActive(true);
            spellProfils = spellBook.GetSpellsRotations();
            for (int i = 0; i < spellProfils.Length; i++)
            {
                skillUiHolderTemp.Add(Instantiate(referenceSkill[i], skillUiHolder[i].transform.position, skillUiHolder[i].transform.rotation, skillUiHolder[i].transform));
            }
        }

        public void CloseSpellExchangeUI()
        {
            GameState.ChangeState();
            for(int i = 0;i < skillUiHolderTemp.Count;i++)
            {
                Destroy(skillUiHolderTemp[i]);
            }
            skillUiHolderTemp.Clear();
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