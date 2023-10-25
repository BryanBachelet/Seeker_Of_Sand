using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Spell
{

    public class SpellManager : MonoBehaviour
    {

        #region Singleton
        // ========== Singleton =================
        private static SpellManager m_instance;
        public static SpellManager GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new SpellManager();
            }
            return m_instance;
        }
        #endregion


        public void Awake()
        {
            m_instance = this;
        }

        public List<Spell> m_spellsList = new List<Spell>();



        public void LoadSpells()
        {
            m_spellsList.Clear();
            Spell[] spells = Resources.LoadAll<Spell>("SpellsIntances");
            for (int i = 0; i < spells.Length; i++)
            {
                m_spellsList.Add(ScriptableObject.Instantiate(spells[i] ));
            }
        }

        public Spell GetRandomSpell()
        {
            int indexSpell = UnityEngine.Random.Range(0, m_spellsList.Count);
            return m_spellsList[indexSpell];
        }

        public Spell[] GetRandomSpells(int count)
        {
            Spell[] spellArray = new Spell[count];
            for (int i = 0; i < count; i++)
            {
                spellArray[i] = GetRandomSpell();
            }

            return spellArray;
        }

        public Spell GetSpell(int index)
        {
            return m_spellsList[index];
        }

        /// <summary>
        /// Function not done
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Spell GetSpell(string name)
        {


            return m_spellsList[0];
        }

        public Spell[] GetAllSpells()
        {
            return m_spellsList.ToArray();
        }

        public bool IsThisSpellLootable(Spell spell)
        {
            return true;
        }
 
    }
}
