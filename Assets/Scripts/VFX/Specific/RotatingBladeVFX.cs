using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using SpellSystem;

namespace GuerhoubaGames.VFX
{
    public class RotatingBladeVFX : MonoBehaviour
    {
        public AreaMeta areaMeta;
        private SpellProfil spellProfil;
        public VisualEffect visualEffect;
        void Start()
        {
            spellProfil = areaMeta.areaData.spellProfil;

            int hitNumber = spellProfil.GetIntStat(GameEnum.StatType.HitNumber);
            float hitFrequency = spellProfil.GetFloatStat(GameEnum.StatType.HitFrequency);
            visualEffect.SetFloat("Lifetime", hitNumber * hitFrequency);
            visualEffect.SetFloat("HitParSeconde", hitFrequency);
            visualEffect.SetInt("SwordNumber", (int)(hitNumber * hitFrequency));

        }


    }
}
