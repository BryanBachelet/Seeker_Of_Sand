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
        public AttackType typeAttack;

        [HideInInspector] public int indexCollider;
        [HideInInspector] public int indexProjectileGO;
        [HideInInspector] public int indexDecals;



    }

}