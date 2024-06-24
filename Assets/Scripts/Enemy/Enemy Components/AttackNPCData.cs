using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{


    public class AttackNPCData : MonoBehaviour
    {
        [Header("Attack parameters")]
        public float prepationTime = 1;
        public float recoverTime = 1;
        public float contactTime = 1;
        public int damage = 1;
        public bool isStunLockingAttack = false;
        public bool isStopMovingAtPrep = false;

        public Collider attackCollider;
        public GameObject decalAttack;

    }

}