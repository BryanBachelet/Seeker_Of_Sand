using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Artefact
{
    public class SpellLauncher : MonoBehaviour
    {
        private ArtefactData m_artefactData;
        private Character.CharacterShoot m_characterShoot ;
        private float radiusEffect;
        public LayerMask enemyMask;
        public void Start()
        {
            m_artefactData = GetComponent<ArtefactData>();
            radiusEffect = m_artefactData.radius;
            m_characterShoot = m_artefactData.characterGo.GetComponent<Character.CharacterShoot>();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyHit) OnDirectTarget();
            if (m_artefactData.entitiesTargetSystem == EntitiesTargetSystem.EnemyRandomAround) AroundTargetRandom();
        }


        private void OnDirectTarget()
        {
            if (!m_artefactData.agent) return;


            m_characterShoot.LaunchShootUniqueSpell(m_characterShoot.GetCurrentCapsuleIndex());
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

            m_characterShoot.LaunchShootUniqueSpell(m_characterShoot.GetCurrentCapsuleIndex());
        }
    }
}