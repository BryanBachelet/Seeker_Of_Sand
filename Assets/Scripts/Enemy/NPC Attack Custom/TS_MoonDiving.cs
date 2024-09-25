using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Enemies
{
    [CreateAssetMenu(fileName = "CustomAttackData", menuName = "Enemmis/Attack/Custom/TS Moon Diving", order = 1)]
    public class TS_MoonDiving : NPCCustomAttack
    {

        public float attackRadius = 30;

        [Tooltip("Time before the attack is launch")]
        public float prepDiveDuration;
        private float[] m_prepPhaseTime;

        [Tooltip("Time between attack ")]
        public float timeBetweenDive;
        private float m_timerBetweenDive;

        [Tooltip("Time to recover after the attack")]
        public float recoverAttackTime;
        private float m_recoverAttackTimer;

        private int m_currentDiveCount;
        private int m_diveStartCount;

        public int diveNumber = 3;
        private int m_attackDoneCount;

        private Vector3[] divePosition;

        public override void ResetAttack()
        {
            divePosition = new Vector3[diveNumber];
            m_prepPhaseTime = new float[diveNumber];
            m_diveStartCount = 0;
            m_currentDiveCount = 0;
            m_attackDoneCount = 0;
            m_recoverAttackTimer = 0;

        }


        public override void ActiveContactPhase()
        {
            customAttackData.npcAttacksComp.GetComponent<NpcMouvementComponent>().StopMouvement();
            customAttackData.npcAttacksComp.GetComponent<NpcMetaInfos>().state = NpcState.ATTACK;
        }

        public override bool UpdateContactPhase()
        {

            UpdateNextDive();
            UpdateCurrentDive();

            if (m_attackDoneCount == diveNumber) return true;
            return false;

        }

        public override bool UpdateRecoverPhase()
        {
            if (m_recoverAttackTimer > recoverAttackTime)
            {
                m_recoverAttackTimer = 0.0f;
                return true;
            }
            else
            {
                m_recoverAttackTimer += Time.deltaTime;
                return false;
            }

        }

        #region Attack Functions

        private void UpdateNextDive()
        {
            if (m_diveStartCount >= diveNumber) return;
            if (m_timerBetweenDive > timeBetweenDive)
            {
                LaunchPreviewAttack();
            }
            else
            {
                m_timerBetweenDive += Time.deltaTime;
            }
        }

        private void LaunchPreviewAttack()
        {
            m_timerBetweenDive = 0.0f;
            m_currentDiveCount++;
            // Determine attack
            divePosition[m_diveStartCount] = CalculateAttackPosition(m_diveStartCount);
            // Spanw Decal and store position
            AttackInfoData attackDamageInfo = new AttackInfoData();
            attackDamageInfo.duration = prepDiveDuration;
            attackDamageInfo.phase = GuerhoubaGames.GameEnum.AttackPhase.CONTACT;
            attackDamageInfo.positionAttack = divePosition[m_diveStartCount];
            attackDamageInfo.attackIndex = customAttackData.attackIndex;
            attackDamageInfo.radius = attackRadius;

            customAttackData.npcAttackFeedback.SpawnFeedbacks(attackDamageInfo);

            m_diveStartCount++;
        }

        private void UpdateCurrentDive()
        {
            // Attack Part
            for (int i = m_attackDoneCount; i < (m_attackDoneCount + m_currentDiveCount); i++)
            {
                float attackPrepTime = m_prepPhaseTime[i];

                if (attackPrepTime > prepDiveDuration)
                {
                    LaunchAttack(i);
                }
                else
                {
                    m_prepPhaseTime[i] += Time.deltaTime;

                }

            }
        }

        private Vector3 CalculateAttackPosition(int tpCount)
        {
            Vector3 playerPosition = customAttackData.targetTransform.position;

            Character.CharacterMouvement characterMouvement = customAttackData.targetTransform.GetComponent<Character.CharacterMouvement>();
            Vector3 playerDirection = characterMouvement.currentDirection;
            if((tpCount % 2) == 0)  playerPosition += playerDirection.normalized * 10;

            return playerPosition;
        }

        private void LaunchAttack(int index)
        {
            // 1. TP Boss
            NavMeshAgent navMeshAgent = customAttackData.npcAttacksComp.GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(divePosition[index]);
            // 2. Do Damage


            if (Vector3.Distance(divePosition[index], customAttackData.targetTransform.position) < attackRadius)
            {
                HealthPlayerComponent hpPlayer = customAttackData.targetTransform.GetComponent<HealthPlayerComponent>();
                AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
                attackDamageInfo.attackName = customAttackData.name;
                attackDamageInfo.position = customAttackData.ownTransform.position;
                attackDamageInfo.damage = customAttackData.damage;
                hpPlayer.GetLightDamage(attackDamageInfo);
            }

            // 3.Launch Animation

            m_attackDoneCount++;
            m_currentDiveCount--;
        }


        #endregion

    }
}
