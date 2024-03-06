using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Character
{
    public class CharacterSpellBook : MonoBehaviour
    {
        [SerializeField] private int m_rotationSize = 4;
            private List<SpellSystem.Capsule> m_bookOfSpell = new List<SpellSystem.Capsule>();
        [SerializeField] public SpellSystem.Capsule[] m_spellsRotationArray = new SpellSystem.Capsule[4];


        //  Need to create copy from the spell place

        public void Start()
        {
            m_spellsRotationArray = new SpellSystem.Capsule[m_rotationSize];

        }

        #region Spell Rotation Function
        public SpellSystem.Capsule[] GetSpellsRotations()
        {
            return m_spellsRotationArray;
        }

        public void ChangeSpellOfRotation(int indexRotation, int spellIndex)
        {
            m_spellsRotationArray[indexRotation] = m_bookOfSpell[spellIndex];
        }

        public SpellSystem.Capsule GetSpellOfRotation(int spellIndex)
        {
            return m_spellsRotationArray[spellIndex];
        }

        #endregion

        #region Spell Inventory Functions

        public SpellSystem.Capsule GetSpecificSpell(int index)
        {
            return m_bookOfSpell[index];
        }

        public void AddSpell(SpellSystem.Capsule spell)
        {
            m_bookOfSpell.Add(spell);
            
        }

        public int GetSpellIndex(SpellSystem.Capsule spell) { return m_bookOfSpell.IndexOf(spell); }

        public void RemoveSpell(int index)
        {
            m_bookOfSpell.RemoveAt(index);
        }

        public SpellSystem.Capsule GetRandomSpellSimple()
        {
            int indexSpell = Random.Range(0, m_bookOfSpell.Count);
            return m_bookOfSpell[indexSpell];
        }

        public SpellSystem.Capsule[] GetAllSpells()
        {
            return m_bookOfSpell.ToArray();
        }
        public int GetSpellCount() { return m_bookOfSpell.Count; }
        #endregion



    }

}