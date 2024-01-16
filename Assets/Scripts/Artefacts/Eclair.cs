using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artefact
{
    public class Eclair : MonoBehaviour
    {
        public float m_damage;
        private ArtefactData m_artefactData;
        [Header("Around Parameter")]
        public float radiusEffect;
        public LayerMask enemyMask;
        public void Start()
        {
            m_artefactData = GetComponent<ArtefactData>();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyHit) OnDirectTarget();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyRandomAround) AroundTargetRandom();
        }


        private void OnDirectTarget()
        {
            if (!m_artefactData.agent) return;

            Enemies.NpcHealthComponent healthComponent = m_artefactData.agent.GetComponent<Enemies.NpcHealthComponent>();
            healthComponent.ReceiveDamage(m_damage, Vector3.up, 1);
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
            healthComponent.ReceiveDamage(m_damage, Vector3.up, 1);
        }
    }
}