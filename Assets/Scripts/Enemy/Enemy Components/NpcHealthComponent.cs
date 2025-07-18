using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMOD.Studio;
using FMODUnity;
using GuerhoubaGames.Resources;
using GuerhoubaTools.Gameplay;
using GuerhoubaGames;

namespace Enemies
{


    [Serializable]
    public struct TargetData
    {
        public Transform target;
        public Transform baseTarget;
        public bool isMoving;
    }

    public class NpcHealthComponent : MonoBehaviour, IDamageReceiver
    {
        public int indexEnemy = 0;
        [Header("Health Parameter")]
        public float maxLife;
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
        public Animator m_entityAnimator;
        private EnemyManager m_enemyManager;
        [SerializeField] private Animator m_EnemyAnimatorDissolve;
        [SerializeField] private string m_DeathTagState = "Death"; //Name of the tag of the death State
        private float deathStateDuration; // Duration of the death animation identified by the death tag



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
        public HitEffectHighLight m_HitEffectHighLight;
        public Action OnDeathEvent;

        public GameObject healthHolder;
        public GameObject healthBarFill;
        public GameObject healthBackground;
        public SpriteRenderer healthBar;
        public Vector3 posLowHealth;
        public Vector3 posFullHealth;

        public bool isHealthDisplay = false;

        private Object_HealthDisplay m_objectHealthDisplay;
        public TrailRenderer m_trailRenderer;
        public GameObject lastDissonanceInstantiated;
        public AfflictionManager m_afflictionManager;

        void Awake()
        {
            InitComponent();
            healthHolder.SetActive(isHealthDisplay);
        }
        private void InitComponent()
        {
            m_healthSystem = new HealthSystem();
            m_npcInfo = GetComponent<NpcMetaInfos>();
            m_entityAnimator = GetComponentInChildren<Animator>(false);
            m_objectHealthDisplay = GetComponentInChildren<Object_HealthDisplay>();
            m_afflictionManager = GetComponent<AfflictionManager>();
            _propBlock = new MaterialPropertyBlock();
            if (!isMassed)
            {
                moveSoundInstance = RuntimeManager.CreateInstance(moveSoundAssociated);
                moveSoundInstance.start();
                moveSoundInstance.setVolume(0);
                //moveSoundInstance.setVolume(0);
            }
            if (m_SkinMeshRenderer != null)
            {
                for (int i = 0; i < m_SkinMeshRenderer.materials.Length; i++)
                {
                    m_materialList.Add(m_SkinMeshRenderer.materials[i]);
                }

            }
            if (this.GetComponent<HitEffectHighLight>() != null) { m_HitEffectHighLight = this.GetComponent<HitEffectHighLight>(); m_HitEffectHighLight.m_objectHealthDisplay = m_objectHealthDisplay; }

            if(deathStateDuration == 0 && hasDeathAnimation) { SearchDeathOnAnimator(); }
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
                deathTimer += Time.deltaTime;
                float progressDeath = deathTimer / timeBeforeDestruction;
                float cutoutValue = progressDeath;
                float emissiveValue = progressDeath;
                ResetTrail();
                for (int i = 0; i < materialCutout.Length; i++)
                {
                    m_materialList[materialCutout[i]].SetFloat("_Cutout", cutoutProgress.Evaluate(cutoutValue));
                }
                for (int i = 0; i < materialCutout.Length; i++)
                {
                    //m_SkinMeshRenderer.GetPropertyBlock(_propBlock, 0);
                    //_propBlock.SetColor("_EmissiveColor", Color.white * emissiveProgress.Evaluate(emissiveValue));
                    //m_SkinMeshRenderer.SetPropertyBlock(_propBlock, 0);
                    //m_materialList[materialCutout[i]].SetColor("_EmissiveColor", Color.gray * emissiveProgress.Evaluate(emissiveValue));
                }
            }
            if (!isMassed)
            {
                moveSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            }

        }

        public void SetTarget(Transform targetTransform, Transform baseTranform)
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
            int layerEnemy = LayerMask.NameToLayer("Enemy");
            Tools.ChangeLayerGameObject(layerEnemy, gameObject);
        }

        public void SetInitialData(HealthManager healthManager, EnemyManager enemyManager)
        {
            m_healthManager = healthManager;
            m_enemyManager = enemyManager;
        }

        public void ReceiveDamage(string nameDamage, DamageStatData damageStat, Vector3 direction, float power, int element, int additionnal)
        {
            m_healthSystem.ChangeCurrentHealth(-damageStat.damage);
            GameStats.instance.AddDamageSource(nameDamage, damageStat);
            // VfX feedback
            Vector3 positionOnScreen = transform.position + new Vector3(0,5,0);
            m_healthManager.CallDamageEvent(positionOnScreen, damageStat.damage + additionnal, element);
            if (m_HitEffectHighLight) { m_HitEffectHighLight.ReceiveHit(); }

            GameObject vfxHitInstance = GamePullingSystem.SpawnObject(m_vfxHitFeedback, transform.position, Quaternion.identity);

            //m_entityAnimator.SetTrigger("TakeDamage");
            GlobalSoundManager.PlayOneShot(12, transform.position);
            m_objectHealthDisplay.UpdateLifeBar(m_healthSystem.health / m_healthSystem.maxHealth);

            if (m_healthSystem.health > 0) return;


            GetDestroy(direction, power);
        }

        

        public void GetDestroy(Vector3 direction, float power)
        {
            if (m_npcInfo.state == NpcState.DEATH) return;

            if (hasDeathAnimation)
            {

                m_EnemyAnimatorDissolve.SetBool("DeathBool", true);

            }
            m_npcInfo.state = NpcState.DEATH;

            m_npcInfo.behaviorTreeComponent.isActivate = false;

            if (!isMassed)
            {
                //moveSoundInstance.setVolume(0);
            }


            Tools.ChangeLayerGameObject(16, gameObject);

            if (destroyEvent != null) destroyEvent.Invoke(direction, power);
            OnDeathEvent?.Invoke();
            m_enemyManager.EnemyHasDied(this, xpToDrop);

            if (!death)
            {
                //deathTimer = Time.time;
            }

            //StartCoroutine(Death());
            if (gameObject.activeSelf)
            {
                StartCoroutine(TeleportToPool());
            }
        }


        private IEnumerator TeleportToPool()
        {
            m_enemyManager.DeathEnemy();
            death = true;
            deathTimer = 0;
            if (!isMassed)
            {
                moveSoundInstance.setVolume(0);
                Debug.Log("Pause le son 2 le son !!");
            }
            if (m_enemyManager.GenerateDissonance())
            {
                GlobalSoundManager.PlayOneShot(indexDestroySound, transform.position);
                lastDissonanceInstantiated = GamePullingSystem.SpawnObject(dissonancePrefabObject,transform.position,transform.rotation, m_enemyManager.transform) ;
                ExperienceMouvement ExperienceMove = lastDissonanceInstantiated.GetComponent<ExperienceMouvement>();
                ExperienceMove.dissonanceValue = 1;
                //ExperienceMove.m_playerPosition = TerrainGenerator.staticRoomManager.rewardPosition;
                ExperienceMove.m_playerPosition = m_enemyManager.m_playerTranform;

            }

            //m_EnemyAnimatorDissolve.SetBool("Dissolve", true)
            yield return new WaitForSeconds(timeBeforeDestruction);
            GamePullingSystem.SpawnObject(death_vfx, transform.position, transform.rotation);
            m_npcInfo.TeleportToPool();

        }

        public void SetPauseState()
        {
            m_previousNpcState = (int)m_npcInfo.state;
            m_npcInfo.state = NpcState.PAUSE;
        }
        public void RemovePauseState()
        {
         if(m_npcInfo)   m_npcInfo.state = (NpcState)m_previousNpcState;
        }

        public void RestartObject(int playerLevel)
        {
            if (IsDebugActive)
            {
                Debug.Log("Setup current max life : " + (maxLife + spawnMinute * gainPerMinute));
            }

            NpcMouvementComponent npcMove = this.GetComponent<NpcMouvementComponent>();
            for (int i = 0; i < materialCutout.Length; i++)
            {
                m_materialList[materialCutout[i]].SetFloat("_Cutout", 0);
            }
            npcMove.lastTimeSeen = Time.time;
            ResetTrail();
            npcMove.lastTimeCheck = npcMove.lastTimeSeen;
            npcMove.lastPosCheck = this.transform.position;
            //m_healthSystem.Setup(maxLife + spawnMinute * gainPerMinute);
            //m_healthSystem.Setup(maxHealthEvolution.Evaluate(TerrainGenerator.roomGeneration_Static));
            m_healthSystem.Setup(maxHealthEvolution.Evaluate(playerLevel));
            m_objectHealthDisplay.UpdateLifeBar(m_healthSystem.health / m_healthSystem.maxHealth);
            death = false;
            if (hasDeathAnimation)
            {
                m_EnemyAnimatorDissolve.SetBool("DeathBool", false);
            }
            m_npcInfo.state = NpcState.IDLE;
            if (!isMassed)
            {
                moveSoundInstance.setVolume(1);
                moveSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            }

            this.gameObject.layer = 6;
        }

        public void SetupLife(float value)
        {
            maxLife = value;
            m_healthSystem.Setup(value);
        }

        public float GetCurrentLife()
        {
            return m_healthSystem.health;
        }

        public float GetCurrentLifePercent()
        {
            return m_healthSystem.percentHealth;
        }

        public void SearchDeathOnAnimator()
        {


            AnimationClip[] clips = m_EnemyAnimatorDissolve.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == "Death")
                {
                    deathStateDuration = clips[i].length;
                    timeBeforeDestruction = deathStateDuration;
                }
            }
        }

        public void ResetTrail()
        {
            if (m_trailRenderer != null) { m_trailRenderer.Clear(); }
        }

        public float GetLifeRatio()
        {
            return m_healthSystem.percentHealth;
        }

        public string GetName()
        {
            return m_npcInfo.nameNpc;
        }

        public AfflictionManager GetAfflictionManager()
        {
            return m_afflictionManager;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool IsDead()
        {
            return m_npcInfo.state == NpcState.DEATH ;
        }

        public int GetLastingLife()
        {
            return (int)m_healthSystem.health;
        }
    }
}
