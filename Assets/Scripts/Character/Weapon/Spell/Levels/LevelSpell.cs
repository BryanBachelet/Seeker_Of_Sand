using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace SpellSystem
{
    public  class LevelSpell : ScriptableObject
    {
        public SpellLevelType LevelType;
        public bool isPermanent;

        public static void SetupLevelEffect(LevelSpell[] levelSpells)
        {
            for (int i = 0; i < levelSpells.Length; i++)
            {
                if (levelSpells[i] == null) continue;

                if (levelSpells[i].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpells[i]);
                    statsLevel.SetupStatLevel();
                }

                if (levelSpells[i].LevelType == SpellLevelType.CHAIN_EFFECT)
                {
                    ChainEffect chainEffectLevel = (ChainEffect)(levelSpells[i]);
                    chainEffectLevel.SetupLevelEffect();
                }
            }
        }

    }


    public enum BehaviorLevelType
    { 
        GENERAL = 0,   
        OBJECTS =1,
    }


    


}
