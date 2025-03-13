using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.GameEnum;
using SpellSystem;
using GuerhoubaGames.UI;
using SeekerOfSand.Tools;

namespace Character
{
    public class CharacterSpellBook : MonoBehaviour
    {
        [SerializeField] private int m_rotationSize = 4;
        private List<SpellSystem.SpellProfil> m_bookOfSpell = new List<SpellSystem.SpellProfil>();
        [SerializeField] public SpellSystem.SpellProfil[] m_spellsRotationArray = new SpellSystem.SpellProfil[4];
        UI_Inventory ui_inventory;


        [HideInInspector] public SpellProfil tempSpell;
        private CharacterShoot m_characterShoot;
        public UI_SpellExchange m_spellExchangeUI;
        public int m_currentSpellInRotationCount;

        private CristalInventory m_cristalInventory;
        //  Need to create copy from the spell place

        public void Awake()
        {
            m_spellsRotationArray = new SpellSystem.SpellProfil[m_rotationSize];
            ui_inventory = GameState.m_uiManager.GetComponent<UIDispatcher>().uiInventory;
            m_characterShoot= GetComponent<CharacterShoot>();
            m_cristalInventory = GetComponent<CristalInventory>();

        }

        #region Spell Rotation Function
        public SpellSystem.SpellProfil[] GetSpellsRotations()
        {
            return m_spellsRotationArray;
        }

        public void ChangeSpellOfRotation(int indexRotation, int spellIndex)
        {
            m_spellsRotationArray[indexRotation] = m_bookOfSpell[spellIndex];
        }

        public GameElement[] GetElementSpellInRotation()
        {
            GameElement[] gameElements = new GameElement[m_currentSpellInRotationCount];

            for (int i = 0; i < gameElements.Length; i++)
            {
                if (m_spellsRotationArray[i] != null)
                    gameElements[i] = m_spellsRotationArray[i].tagData.element;
                else
                    gameElements[i] = GameElement.NONE;

            }
            return gameElements;
        }

        public SpellSystem.SpellProfil GetSpellOfRotation(int spellIndex)
        {
            return m_spellsRotationArray[spellIndex];
        }

        #endregion

        #region Spell Inventory Functions

        public SpellSystem.SpellProfil GetSpecificSpell(int index)
        {
            return m_bookOfSpell[index];
        }

        public void AddSpell(SpellSystem.SpellProfil spell)
        {
            m_bookOfSpell.Add(spell);

        }
        public void AddTempSpell(SpellProfil spell)
        {
            tempSpell = spell;
        }

        public int GetSpellIndex(SpellSystem.SpellProfil spell) { return m_bookOfSpell.IndexOf(spell); }

        public void RemoveSpell(int index)
        {
            m_bookOfSpell.RemoveAt(index);
        }

        public SpellSystem.SpellProfil GetRandomSpellSimple()
        {
            int indexSpell = Random.Range(0, m_bookOfSpell.Count);
            return m_bookOfSpell[indexSpell];
        }

        public SpellSystem.SpellProfil[] GetAllSpells()
        {
            return m_bookOfSpell.ToArray();
        }
        public int GetSpellCount() { return m_bookOfSpell.Count; }

        public void ReplaceSpell(SpellSystem.SpellProfil prevSpell, SpellSystem.SpellProfil spellToAdd)
        {
            m_bookOfSpell[GetSpellIndex(prevSpell)] = spellToAdd;
        }
        public void ActualizeUI()
        {
            ui_inventory.ActualizeInventory();
        }

        #endregion


        #region Spell UI Exchange Functions

        public void OpenUIExchange()
        {
            m_spellExchangeUI.OpenSpellExchangeUI();
        }

        public void ExchanceSpell(int index)
        {
            m_characterShoot.ExchangeRotationSpellWithNew(index);
        }

        public void CloseUiExchange()
        {
           
        }

        public void TradeSpellWithCristal()
        {
            int Indexelement = GeneralTools.GetElementalArrayIndex(tempSpell.tagData.element);
            m_cristalInventory.AddCristalCount(Indexelement, 30);
        }
        #endregion
    }

}