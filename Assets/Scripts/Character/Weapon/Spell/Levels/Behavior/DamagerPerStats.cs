using GuerhoubaGames;
using UnityEngine;

namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Target Projectile Level", menuName = "Spell/Damage Per Stats")]
    public class DamagerPerStats : BehaviorLevel
    {
        public float damagePerStats =.1f ;

        public override void OnProjectileShoot(ProjectileShootData projectileEffectData, GameObject instance)
        {
            DamageCalculComponent DamageComponent = instance.GetComponent<DamageCalculComponent>();

            if (DamageComponent != null)
            {
                DamageComponent.damageStats.damageTemporaireGeneral = (int)(damagePerStats * (projectileEffectData.profil.GetSize()));
            }


        }
    }
}
