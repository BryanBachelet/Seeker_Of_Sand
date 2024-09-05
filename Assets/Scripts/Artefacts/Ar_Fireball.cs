using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artefact
{
    public class Ar_Fireball : MonoBehaviour
    {
        public int m_damage;
        public float lifeTimeProjectile;
        public float speed;
        public int piercing;
        private ArtefactData m_artefactData;
        [Header("Around Parameter")]
        private float radiusEffect;
        public LayerMask enemyMask;
        public GameObject fireBallGO;
        private Character.CharacterShoot m_characterShoot;


        private bool m_hasBeenInit = false;
        public void Start()
        {
            m_hasBeenInit = true;
            m_artefactData = GetComponent<ArtefactData>();
            m_characterShoot = m_artefactData.characterGo.GetComponent<Character.CharacterShoot>();
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

        private void ApplyEffect(Vector3 agentPosition)
        {
            Vector3 direction = agentPosition - m_artefactData.characterGo.transform.position;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, direction);
            Vector3 posOffset = new Vector3(0, 5, 0);
            GameObject instance = GameObject.Instantiate(fireBallGO, m_artefactData.characterGo.transform.position + posOffset , rot);
            ProjectileData data = new ProjectileData();
            data.direction = direction;
            data.damage = m_damage;
            data.life = lifeTimeProjectile;
            data.speed = speed;
            data.piercingMax = piercing;
            data.characterShoot = m_characterShoot;
            data.nameFragment = m_artefactData.nameArtefact;
            
            data.objectType = GuerhoubaGames.GameEnum.CharacterObjectType.FRAGMENT;
            instance.GetComponent<Projectile>().SetFragmentDirectProjectile(data);
        }
    }
}
