using GuerhoubaGames.GameEnum;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpellSystem
{

    [CreateAssetMenu(fileName = "Spell Stats Level", menuName = "Spell/Spell Stats Level")]
    public class StatsLevel : LevelSpell
    {
        [Header("Tag to Change")]
        public TagData tagDataToChange;

        [Header("Stats to Change")]
        [Space]
        public List<StatDataLevel> statDatas = new List<StatDataLevel>();
        private StatType[] statTypes = new StatType[0];

        public CustomStatBehavior customStatBehavior;

        public void SetupStatLevel()
        {
            statTypes = new StatType[statDatas.Count];

            for (int i = 0; i < statDatas.Count; i++)
            {
                statTypes[i] = statDatas[i].stat;
            }
        }

        private bool IsStatInt(StatType statsType)
        {
            return ((int)statsType) - ((1 * 1000)) < 1000;
        }
        private bool IsStatFloat(StatType statsType)
        {
            return ((int)statsType) - ((2 * 1000)) < 1000;
        }
        private bool IsStatBool(StatType statsType)
        {
            return ((int)statsType) - ((0 * 1000)) < 1000;
        }
        private bool IsStatString(StatType statsType)
        {
            return ((int)statsType) - ((3 * 1000)) < 1000;
        }

        public void Apply(SpellSystem.SpellProfil spellProfil)
        {

            spellProfil.tagData.ChangeTag(tagDataToChange);
            spellProfil.UpdateStatistics();

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (IsStatBool(statTypes[i]))
                {
                    spellProfil.ChangBoolValue(statTypes[i], statDatas[i].val_bool);
                    continue;
                }

                if (IsStatInt(statTypes[i]))
                {
                    spellProfil.AddToIntStats(statTypes[i], statDatas[i].val_int, statDatas[i].multiply);
                    continue;
                }
                if (IsStatFloat(statTypes[i]))
                {
                    spellProfil.AddToFloatStats(statTypes[i], statDatas[i].val_float, statDatas[i].multiply);
                    continue;
                }

                if (IsStatString(statTypes[i]))
                {
                    spellProfil.ChangeStringStats(statTypes[i], statDatas[i].val_string);
                    continue;
                }
            }

            if (customStatBehavior != null)
            {
                customStatBehavior.Apply(spellProfil);
            }
        }

        public StatsLevel()
        {
            LevelType = SpellLevelType.STATS;
        }
    }
}
