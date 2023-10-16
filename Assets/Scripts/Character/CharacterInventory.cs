using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Character
{


    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField] private int m_rotationSize = 4;
        [SerializeField] private List<Spell.Spell> m_bookOfSpell;
        [SerializeField] public Spell.Spell[] m_spellsRotationArray = new Spell.Spell[4];


        //  Need to create copy from the spell place

        public void Start()
        {
            m_spellsRotationArray = new Spell.Spell[m_rotationSize];
            
        }

        #region Spell Rotation Function
        public Spell.Spell[] GetSpellsRotations()
        {
            return m_spellsRotationArray;
        }

        public void ChangeSpellOfRotation(int indexRotation, int spellIndex)
        {
            m_spellsRotationArray[indexRotation] = m_bookOfSpell[spellIndex];
        }

        public Spell.Spell GetSpellOfRotation(int spellIndex)
        {
            return m_spellsRotationArray[spellIndex];
        }

        #endregion

        #region Spell Inventory Functions

        public void AddSpell(Spell.Spell spell)
        {
            m_bookOfSpell.Add(spell);
        }

        public void RemoveSpell(int index)
        {
            m_bookOfSpell.RemoveAt(index);
        }

        public Spell.Spell GetRandomSpellSimple()
        {
            int indexSpell = Random.Range(0, m_bookOfSpell.Count);
            return m_bookOfSpell[indexSpell];
        }

        public Spell.Spell[] GetAllSpells()
        {
            return m_bookOfSpell.ToArray();
        }

        #endregion

        // Need to move this function to spell manager.
        //public void LoadSpells()
        //{
        //    m_bookOfSpell.Clear();
        //    Spell.Spell[] spells =Resources.LoadAll<Spell.Spell>("SpellsIntances");
        //    for (int i = 0; i < spells.Length; i++)
        //    {
        //        m_bookOfSpell.Add(ScriptableObject.CreateInstance<Spell.Spell>());
        //    }
        //}


    }

}