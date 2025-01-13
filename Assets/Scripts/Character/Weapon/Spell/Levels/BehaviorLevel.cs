using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        /// <summary>
        ///  This function is call when a stat level has a custom behavior attach to them
        /// </summary>
        /// <param name="spellProfil"></param>
        public virtual void Apply(SpellProfil spellProfil) { }


        /// <summary>
        /// This functions is apply when an upgrade is taken. Use it carefully because it can do permannant modification to prefab
        /// </summary>
        /// <param name="profil"></param>
        public virtual void OnGain(SpellProfil profil)
        {

        }

        public virtual void OnLaunch()
        {

        }

        /// <summary>
        ///  This functions is active behavior that take projectile data into account to active a behavior. This will only use for projectile spell
        /// </summary>
        /// <param name="profil"></param>
        /// <param name="instance"></param>
        public virtual void OnProjectileShoot(ProjectileShootData profil, GameObject instance)
        {

        }
        /// <summary>
        /// This function is to active general behavior on a spell object like bounce or add bonus damage. This function will happen for every spell
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="spellProfil"></param>
        public virtual void ActiveInstanceBehavior(GameObject instance, SpellProfil spellProfil)
        {

        }

        /// <summary>
        ///  This function is call when a behavior is use in chain effect
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spellProfil"></param>
        /// <param name="ownerProfil"></param>
        public virtual void OnEffectChain(GameObject player, SpellProfil spellProfil,SpellProfil ownerProfil)
        {

        }
    }

}