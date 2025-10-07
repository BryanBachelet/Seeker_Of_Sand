using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Convet Stats  Behavior", menuName = "Spell/CustomBehavior/Convet Stats ")]
    public class ConvertStatToSize : BehaviorLevel
    {
        public override void Apply(SpellProfil spellProfil)
        {
            int projectile = spellProfil.GetIntStat(GuerhoubaGames.GameEnum.StatType.Projectile);
            int shootCount = spellProfil.GetIntStat(GuerhoubaGames.GameEnum.StatType.ShootNumber);


            AddingStatsData addingStatsData = new AddingStatsData();

            addingStatsData.BaseInit(GuerhoubaGames.GameEnum.StatType.SizeExplosion);
            spellProfil.gameEffectStats.AddToFloatStats((projectile + shootCount),addingStatsData);

            addingStatsData.BaseInit(GuerhoubaGames.GameEnum.StatType.Projectile);
            spellProfil.gameEffectStats.AddToIntStats( (-projectile + 1), addingStatsData);

            addingStatsData.BaseInit(GuerhoubaGames.GameEnum.StatType.ShootNumber);
            spellProfil.gameEffectStats.AddToIntStats((-shootCount +1), addingStatsData);
        }
    }
}
