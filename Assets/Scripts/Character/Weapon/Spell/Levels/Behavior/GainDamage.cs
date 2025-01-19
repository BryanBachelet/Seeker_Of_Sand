using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Target Projectile Level", menuName = "Spell/Gain Damage Level")]
    public class GainDamage : BehaviorLevel
    {
        public int damageGain;
        public override void OnEffectChain(GameObject player, SpellProfil spellProfil,SpellProfil ownerProfil)
        {
            base.OnEffectChain(player,spellProfil, ownerProfil);

            CharacterDamageComponent characterDamageComponent =  player.GetComponent<CharacterDamageComponent>();
            if (characterDamageComponent != null)
            {
                characterDamageComponent.m_damageStats.AddDamage(damageGain, ownerProfil.tagData.element, GuerhoubaGames.DamageType.TEMPORAIRE);
            }


        }
    }

}