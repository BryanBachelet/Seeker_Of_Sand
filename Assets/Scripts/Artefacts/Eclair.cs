using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artefact
{
    public class Eclair : MonoBehaviour
    {
        public int m_damage;
        private ArtefactData m_artefactData;
        [Header("Around Parameter")]
        private float radiusEffect;
        public LayerMask enemyMask;
        public void Start()
        {
            m_artefactData = GetComponent<ArtefactData>();
            radiusEffect = m_artefactData.radius;
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyHit) OnDirectTarget();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyRandomAround) AroundTargetRandom();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.ClosestEnemyAround) ClosestTarget();
        }


        private void OnDirectTarget()
        {
            if (!m_artefactData.agent) return;

            Enemies.NpcHealthComponent healthComponent = m_artefactData.agent.GetComponent<Enemies.NpcHealthComponent>();
            ApplyEffect(healthComponent);
        }

        private void AroundTargetRandom()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, radiusEffect, enemyMask);

            if (enemies.Length == 0)
            {
                Destroy(gameObject);
                return;
            }

            int indexEnemy = Random.Range(0, enemies.Length);

            Enemies.NpcHealthComponent healthComponent = enemies[indexEnemy].GetComponent<Enemies.NpcHealthComponent>();
            ApplyEffect(healthComponent);
        }

        private void ClosestTarget()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, radiusEffect, enemyMask);

            if (enemies.Length == 0)
            {
                Destroy(gameObject);
                return;
            }

            int indexEnemy = -1;
            float minDistance = 10000;
            for (int i = 0; i < enemies.Length; i++)
            {
                float currentDistance = Vector3.Distance(m_artefactData.agent.transform.position, enemies[i].transform.position);
                if(currentDistance < minDistance)
                {
                    indexEnemy = i;
                    minDistance = currentDistance;
                }
            }
            if (indexEnemy ==-1)
            {
                Destroy(gameObject);
                return;
            }


            Enemies.NpcHealthComponent healthComponent = enemies[indexEnemy].GetComponent<Enemies.NpcHealthComponent>();
            ApplyEffect(healthComponent);
        }

        private void ApplyEffect(Enemies.NpcHealthComponent targetHealthComponent)
        {
            DamageStatData damageStatData = new DamageStatData(m_damage, m_artefactData.objectType);
            if(targetHealthComponent) targetHealthComponent.ReceiveDamage(m_artefactData.nameArtefact, damageStatData, Vector3.up, 1, 0);
        }
    }
}