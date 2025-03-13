using GuerhoubaGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Active Event Behavior", menuName = "Spell/CustomBehavior/Active Event Behavior")]
    public class ActiveEventArtefact : BehaviorLevel
    {
        public bool activeOnContact;
        public bool activeOnDeath;
        public bool activeOnHit;

        public override void Apply(SpellProfil spellProfil)
        {
            if (activeOnContact) spellProfil.OnContact = true;
            if (activeOnDeath) spellProfil.OnHit = true;
            if (activeOnHit) spellProfil.OnDeath = true;
        }


        public override void ActiveInstanceBehavior(GameObject instance, SpellProfil spellProfil)
        {

            DamageCalculComponent DamageComponent = instance.GetComponent<DamageCalculComponent>();

            if (DamageComponent != null)
            {
                if (activeOnContact) DamageComponent.damageStats.OnContact = true;
                if (activeOnDeath) DamageComponent.damageStats.OnHit = true;
                if (activeOnHit) DamageComponent.damageStats.OnDeath = true;
            }

        }
    }
}