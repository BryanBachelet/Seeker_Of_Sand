using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spell
{

    public class SpellContainer : MonoBehaviour
    {
        [SerializeField]
        private Spell m_spell;

        public void Start()
        {
            InitSpell();
        }
        public void InitSpell()
        {
            m_spell = SpellManager.GetInstance().GetRandomSpell();
        }

        public Spell GetSpell()
        {
            return m_spell;
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }

}