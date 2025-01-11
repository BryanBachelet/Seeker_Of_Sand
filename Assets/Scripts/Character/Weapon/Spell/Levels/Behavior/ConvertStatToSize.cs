using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Convet Stats  Behavior", menuName = "Spell/CustomBehavior/Convet Stats ")]
    public class ConvertStatToSize : CustomStatBehavior
    {
        public override void Apply(SpellProfil spellProfil)
        {
            int projectile = spellProfil.GetIntStat(GuerhoubaGames.GameEnum.StatType.Projectile);
            int shootCount = spellProfil.GetIntStat(GuerhoubaGames.GameEnum.StatType.ShootNumber);

            spellProfil.AddToFloatStats(GuerhoubaGames.GameEnum.StatType.SizeExplosion, (projectile + shootCount));
            spellProfil.AddToIntStats(GuerhoubaGames.GameEnum.StatType.Projectile, (-projectile + 1));
            spellProfil.AddToIntStats(GuerhoubaGames.GameEnum.StatType.ShootNumber, (-shootCount +1));
        }
    }
}
