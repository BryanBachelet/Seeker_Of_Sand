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
        [HideInInspector] public Animator m_entityAnimator;
        private EnemyManager m_enemyManager;
        [SerializeField] private Animator m_EnemyAnimatorDissolve;

        public SkinnedMeshRenderer m_SkinMeshRenderer;
        [SerializeField] private List<Material> m_materialList = new List<Material>();        // Hit damage
        public int[] materialCutout;
        public int[] materialEmissive;
        public AnimationCurve cutoutProgress;
        public AnimationCurve emissiveProgress;
        private float deathTimer;

        private bool death = false;
        private MaterialPropertyBlock _propBlock;
        public GameObject death_vfx;
        void Awake()
        {
            InitComponent();
        }
        private void InitComponent()
        {
            m_healthSystem = new HealthSystem();
            m_healthSystem.Setup(m_maxLife);
            m_entityAnimator = GetComponentInChildren<Animator>();
            _propBlock = new MaterialPropertyBlock();
            for (int i = 0; i < m_SkinMeshRenderer.materials.Length; i ++)
            {
                m_materialList.Add(m_SkinMeshRenderer.materials[i]);
            }


        }

        private void Update()
        {
            if(death)
            {

                float progressDeath = 1 - (timeBeforeDestruction + deathTimer - Time.time) / 2;
                float cutoutValue = progressDeath;
                float emissiveValue = progressDeath;
                for (int i = 0; i < materialCutout.Length; i++)
                {
                    m_materialList[materialCutout[i]].SetFloat("_Cutout", cutoutProgress.Evaluate(cutoutValue));
                }
                for (int i = 0; i < materialCutout.Length; i++)
                {
                   //m_SkinMeshRenderer.GetPropertyBlock(_propBlock, 0);
                   //_propBlock.SetColor("_EmissiveColor", Color.white * emissiveProgress.Evaluate(emissiveValue));
                   //m_SkinMeshRenderer.SetPropertyBlock(_propBlock, 0);
                    m_materialList[materialCutout[i]].SetColor("_EmissiveColor", Color.gray * emissiveProgress.Evaluate(emissiveValue));
                }
            }
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
            //m_entityAnimator.SetTrigger("TakeDamage");
            GlobalSoundManager.PlayOneShot(12, transform.position);

            if (m_healthSystem.health > 0) return;

            GetDestroy(direction, power);
        }

        public void GetDestroy(Vector3 direction, float power)
        {
            if (npcState == NpcState.DEATH) return;

            if (hasDeathAnimation) m_entityAnimator.SetTrigger("Death");
            npcState = NpcState.DEATH;
            this.gameObject.layer = 16;
            //destroyEvent.Invoke(direction, power);
          
            m_enemyManager.SpawnExp(transform.position, xpToDrop);
            m_enemyManager.IncreseAlterEnemyCount(this);
            if(!death)
            {
                deathTimer = Time.time;
            }
            
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            m_enemyManager.DeathEnemy();
            death = true;
            //m_EnemyAnimatorDissolve.SetBool("Dissolve", true);
            yield return new WaitForSeconds(timeBeforeDestruction /2);
            Instantiate(death_vfx, transform.position, transform.rotation);
            yield return new WaitForSeconds(timeBeforeDestruction / 2);
           
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
