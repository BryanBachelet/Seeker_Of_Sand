using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{
    public class NPCAttackArea : MonoBehaviour
    {
        private ObjectState gameState;


        public bool ActiveDebug = false;
        private NpcAttackMeta m_npcAttackMeta;
        private AttackObjMetaData attackObj;


        public void Awake()
        {
            m_npcAttackMeta = GetComponent<NpcAttackMeta>();
           
            m_npcAttackMeta.OnStart += InitAttack;
        }

        void InitAttack()
        {
            attackObj = m_npcAttackMeta.attackObjMetaData;
            if (attackObj.isOneShoot)
            {
                ApplyDamageCircle();
            }
        }

        public void ApplyDamageCircle()
        {
            if (attackObj.typeArea != AreaType.CIRCLE) return;

            if (Vector3.Distance(transform.position, attackObj.target.position) < attackObj.size)
            {
                HealthPlayerComponent hpPlayer = attackObj.target.GetComponent<HealthPlayerComponent>();
                AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
                attackDamageInfo.attackName = attackObj.nameAttack;
                attackDamageInfo.position = transform.position;
                attackDamageInfo.damage = attackObj.damage;
                hpPlayer.GetLightDamage(attackDamageInfo);
            }
        }


        public void OnDrawGizmos()
        {
            if (ActiveDebug)
            {
                if (attackObj.typeArea == AreaType.CIRCLE) Gizmos.DrawWireSphere(transform.position, attackObj.size);

            }
        }
    }
}
