using ExcelLibrary.BinaryFileFormat;
using GuerhoubaGames.GameEnum;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SpellSystem
{

    [CreateAssetMenu(fileName = "Spell Stats Level", menuName = "Spell/Spell Stats Level")]
    public class StatsLevel : LevelSpell
    {

        //[Header("Tag to Change")]
        //public TagData tagDataToChange;

        //[Space]
        //[Header("Stats to Change")]
        //public List<StatDataLevel> statDatas = new List<StatDataLevel>();
        //private StatType[] statTypes = new StatType[0];

        public GameEffectStats<StatDataLevel> gameEffectStats;

        public BehaviorLevel customStatBehavior;

        public void SetupStatLevel()
        {
            gameEffectStats.statTypes = new StatType[gameEffectStats.statDatas.Count];

            for (int i = 0; i < gameEffectStats.statDatas.Count; i++)
            {
                gameEffectStats.statTypes[i] = gameEffectStats.statDatas[i].stat;
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

            spellProfil.gameEffectStats.tagData.ChangeTag(gameEffectStats.tagData);
            spellProfil.UpdateStatistics();

            gameEffectStats.ChangeStats(gameEffectStats);

            if (customStatBehavior != null)
            {
                customStatBehavior.Apply(spellProfil);
            }
        }

        public StatsLevel()
        {
            LevelType = SpellLevelType.STATS;
        }
        public void OnValidate()
        { 

            //gameEffectStats.tagData = tagDataToChange;
            //gameEffectStats.statDatas = new List<StatData>(statDatas);
            //gameEffectStats.statTypes = statTypes;
            for (int i = 0; i < gameEffectStats.statDatas.Count; i++)
            {
                gameEffectStats.statDatas[i].isShowMultiply = true;
            }
            EditorUtility.SetDirty(this);

        }
    }


}
