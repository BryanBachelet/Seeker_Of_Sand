using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Spawn Object Behavior", menuName = "Spell/CustomBehavior/Spawn Object Behavior")]
    public class SpawmObject : BehaviorLevel
    {
        public GameObject objectToSpawn;
        public bool isArea = true;

        public override void Apply(SpellProfil profil)
        {

            if (isArea)
            {
                profil.objectToSpawn.GetComponent<AreaOneHitBehavior>().ObjectToSpawnAtDeath = objectToSpawn;
            }
        }

    }
}