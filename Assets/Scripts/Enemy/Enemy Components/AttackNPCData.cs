using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{

    [System.Serializable]
    public struct AttackNPCData
    {
        public string nameAttack;
        [Header("Attack parameters")]
        public float prepationTime;
        public float recoverTime;
        public float contactTime;
        public int damage;
        public float attackRange;
        public bool isStunLockingAttack;
        public bool isStopMovingAtPrep;
        public bool isHeavyAttack;
        public bool isFollowTarget;
        public AttackLaunchMoment launchMoment;
        public NPCMoveAttackObject attackMovement;
        [Tooltip("When we use UPDATE_CONTACT state for launching attack ")]
        public float updateTimeAttackLaunch;

        public AttackType typeAttack;
        [Tooltip("Only useful for the projectile type of attack")]
        public RangeAttackType rangeTypeAttack;
        
        


        [Header("Area Parameters")]
        public float radius;
        public AreaType shapeType;

        [Header("Projectile Parameters")]
        public float rangeProjectile;
        public float durationProjectile;


        [HideInInspector] public int indexCollider;
        [HideInInspector] public int indexProjectileGO;
        [HideInInspector] public int indexDecals;



    }

}