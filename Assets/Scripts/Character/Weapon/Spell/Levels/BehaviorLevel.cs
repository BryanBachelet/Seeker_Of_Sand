using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpellSystem
{

    public struct ProjectileShootData
    {
        public SpellProfil profil;
        public Vector3 position;
        public Quaternion rotation;
        public ProjectileData projectileData;
    }


    [CreateAssetMenu(fileName = "Behavior Level", menuName = "Spell/Behavior Level")]
    public class BehaviorLevel : LevelSpell
    {

        public BehaviorLevel()
        {
            LevelType = SpellLevelType.BEHAVIOR;
        }


        public virtual void OnGain()
        {

        }

        public virtual void OnLaunch()
        {

        }

        public virtual void OnProjectileShoot(ProjectileShootData profil)
        {

        }

        public virtual void OnUpgradeGain()
        {

        }
    }

}