using Character;
using GuerhoubaGames.Resources;
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
        private Character.CharacterShoot m_characterShoot;

        [SerializeField] private bool hasSound = false;
        [SerializeField] private int indexSound = 0;

        private bool m_hasBeenInit = false;
        private PullingMetaData m_pullingMetaData;


        public void InitArtefact()
        {
            m_characterShoot = m_artefactData.characterGo.GetComponent<Character.CharacterShoot>();
            radiusEffect = m_artefactData.radius;
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyHit) OnDirectTarget();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyRandomAround) AroundTargetRandom();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.ClosestEnemyAround) ClosestTarget();
            if (hasSound) GlobalSoundManager.PlayOneShot(indexSound, transform.position);
        }
        public void Start()
        {
            if (!m_hasBeenInit)
            {
                m_artefactData = GetComponent<ArtefactData>();
                m_pullingMetaData = GetComponent<PullingMetaData>();
                m_artefactData.OnSpawn += InitArtefact;
                m_hasBeenInit = true;
            }

            if (hasSound) GlobalSoundManager.PlayOneShot(indexSound, transform.position);
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
                DestroyObject();
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
                DestroyObject();
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
                DestroyObject();
                return;
            }


            Enemies.NpcHealthComponent healthComponent = enemies[indexEnemy].GetComponent<Enemies.NpcHealthComponent>();
            ApplyEffect(healthComponent);
        }

        private void ApplyEffect(Enemies.NpcHealthComponent targetHealthComponent)
        {
            DamageStatData damageStatData = new DamageStatData(m_damage, m_artefactData.objectType);
            if (targetHealthComponent) targetHealthComponent.ReceiveDamage(m_artefactData.nameArtefact, damageStatData, Vector3.up, 1, 0);
        }

        private void DestroyObject()
        {
            PullingMetaData pullingMetaData = GetComponent<PullingMetaData>();
            if (GamePullingSystem.instance != null && pullingMetaData != null)
            {
                GamePullingSystem.instance.ResetObject(this.gameObject, pullingMetaData.id);
            }
            else
            {
                
                Destroy(this.gameObject);
            }
        }


    }
}