using GuerhoubaGames.GameEnum;
using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace SpellSystem
{


    [CreateAssetMenu(fileName = "Chain Effect Level", menuName = "Spell/Chain Effect Level")]
    public class ChainEffect : LevelSpell
    {

        public int countSpell = 1;
        public bool isUseOnThisSpell = false;
        private int useCount;
        [HideInInspector] public bool hasBeenAdd;
        [HideInInspector] public SpellProfil launchSpellProfil;
        public bool consumeEffectAfterShot;
        public ChainEffect()
        {
            LevelType = SpellLevelType.CHAIN_EFFECT;
        }

        public LevelSpell[] levelSpells;

        public void SetupLevelEffect()
        {
            for (int i = 0; i < levelSpells.Length; i++)
            {
                if (levelSpells[i] == null) return;

                if (levelSpells[i].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpells[i]);
                    statsLevel.SetupStatLevel();
                }


            }
        }

        public void Reset()
        {
            useCount = 0;
        }

        public void Apply(GameObject player, SpellProfil spell, bool afterShot)
        {

            if (afterShot != consumeEffectAfterShot) return;

            if (hasBeenAdd)
            {
                if (!isUseOnThisSpell)
                {

                    hasBeenAdd = false;
                    return;

                }
                else
                {
                    useCount--;
                }
            }

            if (useCount >= countSpell) return;

            for (int i = 0; i < levelSpells.Length; i++)
            {
                if (levelSpells[i].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpells[i]);
                    statsLevel.Apply(spell);
                }
                if (levelSpells[i].LevelType == SpellLevelType.BEHAVIOR)
                {
                    BehaviorLevel statsLevel = (BehaviorLevel)(levelSpells[i]);
                    statsLevel.OnEffectChain(player, spell, launchSpellProfil);
                }
            }

            useCount++;
        }

        public bool IsFinish()
        {
            return useCount >= countSpell;
        }
    }
}