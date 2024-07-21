using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Character
{
    public class CharacterSpellBook : MonoBehaviour
    {
        [SerializeField] private int m_rotationSize = 4;
        private List<SpellSystem.SpellProfil> m_bookOfSpell = new List<SpellSystem.SpellProfil>();
        [SerializeField] public SpellSystem.SpellProfil[] m_spellsRotationArray = new SpellSystem.SpellProfil[4];
        UI_Inventory ui_inventory;

        //  Need to create copy from the spell place

        public void Start()
        {
            m_spellsRotationArray = new SpellSystem.SpellProfil[m_rotationSize];
            ui_inventory = GameState.m_uiManager.GetComponent<UIDispatcher>().uiInventory;

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

        public void ActualizeUI()
        {
            ui_inventory.ActualizeInventory();
        }

        #endregion



    }

}