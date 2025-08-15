using GuerhoubaGames.Artefact;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Artefact
{

    public class Ar_Bulle : MonoBehaviour
    {
        private ArtefactData m_artefactData;
        private float radiusEffect;
        public LayerMask enemyMask; 
        public GameObject bulleGO;

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


            ApplyEffect(m_artefactData.agent.transform.position);
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


            ApplyEffect(enemies[indexEnemy].transform.position);
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
                if (currentDistance < minDistance)
                {
                    indexEnemy = i;
                    minDistance = currentDistance;
                }
            }
            if (indexEnemy == -1)
            {
                Destroy(gameObject);
                return;
            }

            ApplyEffect(enemies[indexEnemy].transform.position);
        }

        public void ApplyEffect(Vector3 position)
        {
            GameObject instance = GameObject.Instantiate(bulleGO, position, new Quaternion(-90,0,0,0));

        }
    }
}