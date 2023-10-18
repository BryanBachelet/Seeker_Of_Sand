using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Enemies
{

    public enum NpcState
    {
        MOVE,
        ATTACK,
        PREP_ATTACK,
        RECUPERATION,
        DEATH,
        PAUSE,
    }

    [Serializable]
    public struct TargetData
    {
        public Transform target;
        public bool isMoving;
    }

    public class NpcHealthComponent : MonoBehaviour
    {
        [Header("Health Parameter")]
        public float m_maxLife;
        public GameObject m_vfxHitFeedback;
        public float timeBeforeDestruction;
        public bool hasDeathAnimation = false;

        [Header("NPC State")]
        public NpcState npcState;
        public ObjectState m_objectGameState;
        public int xpToDrop;
        private int m_previousNpcState;


        public TargetData targetData;
        public bool m_hasChangeTarget;

        public delegate void DestroyEvent(Vector3 direction, float power);
        public event DestroyEvent destroyEvent;

        private HealthSystem m_healthSystem;
        private HealthManager m_healthManager;
        private Animator m_entityAnimator;
        private EnemyManager m_enemyManager;
        [SerializeField] private Animator m_EnemyAnimatorDissolve;

        // Hit damage

        void Awake()
        {
            InitComponent();
        }
        private void InitComponent()
        {
            m_healthSystem = new HealthSystem();
            m_healthSystem.Setup(m_maxLife);
            m_entityAnimator = GetComponentInChildren<Animator>();


        }

        public void ResetTarget()
        {
            m_hasChangeTarget = true;
            NpcMouvementComponent npcMouvement = GetComponent<NpcMouvementComponent>();
            npcMouvement.SetTarget(targetData);
        }

        public void SetInitialData(HealthManager healthManager, EnemyManager enemyManager)
        {
            m_healthManager = healthManager;
            m_enemyManager = enemyManager;
        }

        public void ReceiveDamage(float damage, Vector3 direction, float power)
        {
            m_healthSystem.ChangeCurrentHealth(-damage);

            // VfX feedback
            m_healthManager.CallDamageEvent(transform.position + Vector3.up * 1.5f, damage);
            Instantiate(m_vfxHitFeedback, transform.position, Quaternion.identity);
            m_entityAnimator.SetTrigger("TakeDamage");
            GlobalSoundManager.PlayOneShot(12, transform.position);

            if (m_healthSystem.health > 0) return;

            GetDestroy(direction, power);
        }

        public void GetDestroy(Vector3 direction, float power)
        {
            if (hasDeathAnimation) m_entityAnimator.SetTrigger("Death");
            npcState = NpcState.DEATH;
            this.gameObject.layer = 16;
            destroyEvent.Invoke(direction, power);
          
            m_enemyManager.SpawnExp(transform.position, xpToDrop);
            m_enemyManager.IncreseAlterEnemyCount(this);
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            m_healthManager.m_serieController.RefreshSeries(false);
            m_EnemyAnimatorDissolve.SetBool("Dissolve", true);
            yield return new WaitForSeconds(timeBeforeDestruction);
           
            m_enemyManager.DestroyEnemy(this);
        }

        public void SetPauseState()
        {
            m_previousNpcState = (int)npcState;
            npcState = NpcState.PAUSE;
        }
        public void RemovePauseState()
        {
            npcState = (NpcState)m_previousNpcState;
        }


    }
}
