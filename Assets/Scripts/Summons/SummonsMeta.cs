using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public struct SummonData
    {
        public SpellProfil spellProfil;
        public CharacterSummonManager characterSummonManager;
    }

    public class SummonsMeta : MonoBehaviour
    {
        public SummonData summonData;
        public Action OnSpecialSkill;

        public void OnDestroy()
        {
            summonData.characterSummonManager.RemoveSummon(summonData.spellProfil.id, this.gameObject);
        }
    }

}