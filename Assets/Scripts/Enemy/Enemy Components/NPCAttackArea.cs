using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{
    public class NPCAttackArea : MonoBehaviour
    {
        private ObjectState gameState;

        [HideInInspector] public int damage;
        public float sizeArea;
        public bool isOneShotAttack;
        public AreaType type;
        [HideInInspector] public Transform target;
        public bool ActiveDebug = false;

        void Start()
        {
            if (isOneShotAttack)
            {
                ApplyDamageCircle();
            }
        }

        public void ApplyDamageCircle()
        {
            if (type != AreaType.CIRCLE) return;

            // Set Size 

            if (Vector3.Distance(transform.position, target.position) < sizeArea)
            {
                HealthPlayerComponent hpPlayer = target.GetComponent<HealthPlayerComponent>();
                hpPlayer.GetLightDamage(damage, transform.position);
            }
        }


        public void OnDrawGizmos()
        {
            if (ActiveDebug)
            {
                if (type == AreaType.CIRCLE) Gizmos.DrawWireSphere(transform.position, sizeArea);

            }
        }
    }
}
