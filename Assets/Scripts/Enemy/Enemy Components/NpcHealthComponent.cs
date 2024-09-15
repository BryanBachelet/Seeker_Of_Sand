using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMOD.Studio;
using FMODUnity;
namespace Enemies
{


    [Serializable]
    public struct TargetData
    {
        public Transform target;
        public Transform baseTarget;
        public bool isMoving;
    }

    public class NpcHealthComponent : MonoBehaviour
    {
        public int indexEnemy = 0;
        [Header("Health Parameter")]
        public float m_maxLife;
        public float gainPerMinute = 2;
        public int spawnMinute;
        private float m_baseLife;
        public GameObject m_vfxHitFeedback;
        public float timeBeforeDestruction;
        public bool hasDeathAnimation = false;

        [Header("NPC State")]
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

        [HideInInspector] public NpcMetaInfos m_npcInfo;

        public bool IsDebugActive = false;

        public bool isMassed = true;
        public EventReference moveSoundAssociated;
        public EventInstance moveSoundInstance;
        public int indexDestroySound;
        public AnimationCurve maxHealthEvolution;

        public GameObject dissonancePrefabObject;
        void Awake()
        {
            InitComponent();
        }
        private void InitComponent()
        {
            m_healthSystem = new HealthSystem();
            m_npcInfo = GetComponent<NpcMetaInfos>();
            m_entityAnimator = GetComponentInChildren<Animator>();
            _propBlock = new MaterialPropertyBlock();
            if(!isMassed)
            {
                moveSoundInstance = RuntimeManager.CreateInstance(moveSoundAssociated);
                moveSoundInstance.start();
                moveSoundInstance.setVolume(0);
                //moveSoundInstance.setVolume(0);
            }
            for (int i = 0; i < m_SkinMeshRenderer.materials.Length; i++)
            {
                m_materialList.Add(m_SkinMeshRenderer.materials[i]);
            }
            RestartObject(1);

        }

        public void SetMinuteLife(int min)
        {
            spawnMinute = min;
        }

        private void Update()
        {
            if (death)
            {

                float progressDeath = 1 - (timeBeforeDestruction + deathTimer - Time.time) / 2;
                float cutoutValue = progressDeath;
                float emissiveValue = progressDeath;
                for (int i = 0; i < materialCutout.Length; i++)
                {
                    //m_materialList[materialCutout[i]].SetFloat("_Cutout", cutoutProgress.Evaluate(cutoutValue));
                }
                for (int i = 0; i < materialCutout.Length; i++)
                {
                    //m_SkinMeshRenderer.GetPropertyBlock(_propBlock, 0);
                    //_propBlock.SetColor("_EmissiveColor", Color.white * emissiveProgress.Evaluate(emissiveValue));
                    //m_SkinMeshRenderer.SetPropertyBlock(_propBlock, 0);
                    //m_materialList[materialCutout[i]].SetColor("_EmissiveColor", Color.gray * emissiveProgress.Evaluate(emissiveValue));
                }
            }
            if(!isMassed)
            {
                moveSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            }

        }

        public void SetTarget(Transform targetTransform,Transform baseTranform)
        {
            targetData.target = targetTransform;
            targetData.baseTarget = baseTranform;
            NpcMouvementComponent npcMouvement = GetComponent<NpcMouvementComponent>();
            npcMouvement.SetTarget(targetData);


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

        public void ReceiveDamage(string nameDamage,DamageStatData damageStat, Vector3 direction, float power, int element)
        {
            m_healthSystem.ChangeCurrentHealth(-damageStat.damage);
            GameStats.instance.AddDamageSource(nameDamage, damageStat);
            // VfX feedback
            m_healthManager.CallDamageEvent(transform.position + Vector3.up * 1.5f, damageStat.damage, element);
            Instantiate(m_vfxHitFeedback, transform.position, Quaternion.identity);
            //m_entityAnimator.SetTrigger("TakeDamage");
            Debug.Log("TakeDamage");
            GlobalSoundManager.PlayOneShot(12, transform.position);

            if (m_healthSystem.health > 0) return;

            GetDestroy(direction, power);
        }

        public void GetDestroy(Vector3 direction, float power)
        {
            if (m_npcInfo.state == NpcState.DEATH) return;

            if (hasDeathAnimation) m_entityAnimator.SetTrigger("Death");
            m_npcInfo.state = NpcState.DEATH;
            

            if (!isMassed)
            {
                //moveSoundInstance.setVolume(0);
            }

            this.gameObject.layer = 16;
          if(destroyEvent != null) destroyEvent.Invoke(direction, power);

            m_enemyManager.EnemyHasDied(this, xpToDrop);

            if (!death)
            {
                deathTimer = Time.time;
            }

            //StartCoroutine(Death());
          if(gameObject.activeSelf)  StartCoroutine(TeleportToPool());
        }


        private IEnumerator TeleportToPool()
        {
            m_enemyManager.DeathEnemy();
            death = true;
            if (!isMassed)
            {
                moveSoundInstance.setVolume(0);
                Debug.Log("Pause le son 2 le son !!");
            }
            GlobalSoundManager.PlayOneShot(indexDestroySound, transform.position);
            GameObject dissonanceInstance = Instantiate(dissonancePrefabObject, transform.position, transform.rotation);
            ExperienceMouvement ExperienceMove = dissonanceInstance.GetComponent<ExperienceMouvement>();
            ExperienceMove.m_playerPosition = TerrainGenerator.staticRoomManager.rewardPosition;
            //m_EnemyAnimatorDissolve.SetBool("Dissolve", true);
            yield return new WaitForSeconds(timeBeforeDestruction / 2);
            Instantiate(death_vfx, transform.position, transform.rotation);
            m_npcInfo.TeleportToPool();

        }

        public void SetPauseState()
        {
            m_previousNpcState = (int)m_npcInfo.state;
            m_npcInfo.state = NpcState.PAUSE;
        }
        public void RemovePauseState()
        {
            m_npcInfo.state = (NpcState)m_previousNpcState;
        }

        public void RestartObject(int playerLevel)
        {
            if (IsDebugActive)
            {
                Debug.Log("Setup current max life : " + (m_maxLife + spawnMinute * gainPerMinute));
            }
            m_healthSystem.Setup(m_maxLife + spawnMinute * gainPerMinute);
            m_healthSystem.Setup(maxHealthEvolution.Evaluate(TerrainGenerator.roomGeneration_Static));
            m_healthSystem.Setup(maxHealthEvolution.Evaluate(playerLevel));
            death = false;
            if (hasDeathAnimation) m_entityAnimator.ResetTrigger("Death");
            m_npcInfo.state = NpcState.MOVE;
            if (!isMassed)
            {
                moveSoundInstance.setVolume(1);
                moveSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            }

            this.gameObject.layer = 6;
        }
    }
}
