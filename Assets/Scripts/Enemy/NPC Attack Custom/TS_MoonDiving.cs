using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

        }

        public override void ActivePrepPhase()
        {
            base.ActivePrepPhase();
        }

        public override bool UpdatePrepPhase()
        {
            return true;

        }

        public override bool UpdateContactPhase()
        {

            UpdateNextDive();
            UpdateCurrentDive();

            if (m_attackDoneCount == diveNumber) return true;
            return false;

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
            divePosition[m_diveStartCount] = CalculateAttackPosition();
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
        
        private Vector3 CalculateAttackPosition()
        {
            Vector3 playerPosition = customAttackData.targetTransform.position;

            Debug.Log("Attack Prediction position of Moon divin not implement");

            return playerPosition;
        }
        
        private void LaunchAttack(int index)
        {
            // 1. TP Boss
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
