using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{

    [System.Serializable]
    public struct AttackNPCData
    {

        public enum AttackPosition
        {
            Target,
            Self,
        }

        public NPCCustomAttack customAttack;

        public string nameAttack;
        [Header("Attack parameters")]
        public float prepationTime;
        public float recoverTime;
        public float contactTime;
        public float coooldownAttack;

        public bool isTriggerGeneralCooldown;

        public bool isStartinAttackSequence;
        public bool isEndingAttackSequence;


        [Tooltip("Periods during the ai is going to rotate to follow the player")]
        public float rotationTime;
        public int damage;
        public float attackRange;
        public bool isHeavyAttack;
        public bool isStunLockingAttack;
        public bool isStopMovingAtPrep;
        [Tooltip("Allow the AI to constantly rotate during prep time")]
        public bool isFollowTarget;
        public AttackLaunchMoment launchMoment;
        public NPCMoveAttackObject attackMovement;
        [Tooltip("When we use UPDATE_CONTACT state for launching attack ")]
        public float updateTimeAttackLaunch;

        public AttackType typeAttack;
        [Tooltip("Only useful for the projectile type of attack")]
        public RangeAttackType rangeTypeAttack;
        public GameObject projectileLaunch;

        public AttackPosition postionToSpawnType;

        [Header("Area Parameters")]
       
        public AreaType shapeType;

        [Tooltip("Only for circle")]
        public float radius;
        [Tooltip("For all form except circle")]
        public Vector3 scale;

        [Header("Projectile Parameters")]
        public float rangeProjectile;
        public float durationProjectile;

        [Header("Raycast Parameters")]
        public Vector3 scaleRaycast;
        public LayerMask rayLayerMask;
        public float radiusRaycastExplosion;

        [HideInInspector] public int indexCollider;
        [HideInInspector] public int indexProjectileGO;
        [HideInInspector] public int indexDecals;


        public Vector3 GetScale()
        {
            if(shapeType== AreaType.CIRCLE)
            {
                return new Vector3(radius, radius, radius);
            }
            return scale;
        }
    }

}