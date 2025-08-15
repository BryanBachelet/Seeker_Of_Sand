using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GuerhoubaGames.Enemies
{
    public class TS_RingSkill : SpecialCapacity
    {
        [Header("Ring Object Parameters")]
        public GameObject ringGO;
        private GameObject ringInstance;
        public Transform centerPosition;

        private Vector3 centerRing;

        [Header("Ring Stats Parameters")]
        public float radius = 200;
        public float skillLaunchDuration = 1;
        private float m_skillLaunchDuration = 0.0f;

        [Space] public bool activeDebug;

        private bool canBeLaunch = true;
        private NpcSpecialCapacities m_specialCapacities;
        private NpcHealthComponent m_npcHealthComponent;
        private NpcMetaInfos m_npcMetaInfo;
        private NpcAttackComponent m_npcAttackComponent;
        private GuerhoubaGames.AI.BehaviorTreeComponent m_behaviorTreeComponent;
        private GameObject playerGO;
        private VisualEffect ringVFX;
        private VisualEffect[] allRingVFX;
        private bool m_activeRingIntensity;

        #region Unity Functions

        public void Start()
        {
            m_specialCapacities = GetComponent<NpcSpecialCapacities>();
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_npcHealthComponent.OnDeathEvent += ResetOnDeath;
            m_npcMetaInfo = GetComponent<NpcMetaInfos>();
            m_behaviorTreeComponent = GetComponent<GuerhoubaGames.AI.BehaviorTreeComponent>();
            playerGO = m_npcHealthComponent.targetData.target.gameObject;
            m_npcAttackComponent = GetComponent<NpcAttackComponent>();
        }

        // Update is called once per frame
        void Update()
        {

            if (m_npcMetaInfo.state != NpcState.DEATH)
            {
                UpdateRingSkill();
            }


        }

        #endregion


        public void ResetOnDeath()
        {
            canBeLaunch = true;
            if (ringInstance != null)
            {
                allRingVFX = ringInstance.GetComponentsInChildren<VisualEffect>();
                for (int i = 0; i < allRingVFX.Length; i++)
                {
                    if (allRingVFX[i] != null)
                    Destroy(allRingVFX[i].gameObject);
                }
                Destroy(ringInstance);
            }
        }

        public void UpgradeRingVisual()
        {
            m_activeRingIntensity = true;
            VisualEffect[] ringInstanceVFXIntensity = ringInstance.GetComponentsInChildren<VisualEffect>();
            for (int i = 0; i < ringInstanceVFXIntensity.Length; i++)
            {
                if (ringInstanceVFXIntensity[i].HasFloat("IntensityFactor")) ringInstanceVFXIntensity[i].SetFloat("IntensityFactor", 4);
            }
        }

        public void ActiveRingSkill()
        {
            SetPositionRing();
            if (ringInstance == null)
            {
                ringInstance = Instantiate(ringGO, Vector3.zero, Quaternion.identity);
                ringVFX = ringInstance.GetComponentInChildren<VisualEffect>();
                ringVFX.SetFloat("Radius", radius * 2);
                ringVFX.gameObject.transform.position = new Vector3(0, 0 - radius, 0);
            }
            ringInstance.transform.position = new Vector3(centerRing.x, centerRing.y, centerRing.z);
        }

        private void SetPositionRing()
        {
            centerRing = transform.position;
            GlobalSoundManager.PlayOneShot(55, centerRing);
        }

        public void UpdateRingSkill()
        {
            if (canBeLaunch || m_npcMetaInfo.state == NpcState.ATTACK || m_npcMetaInfo.state == NpcState.SPECIAL_CAPACITIES || m_npcAttackComponent.isInAttackSequence == true) return;

            Vector3 pos = playerGO.transform.position;
            pos.y = centerRing.y;
            if (Vector3.Distance(playerGO.transform.position, centerRing) > radius)
            {
                canBeLaunch = true;
                GlobalSoundManager.PlayOneShot(54, centerRing);
                m_specialCapacities.TriggerSpecialCapacityBehavior(indexSpecialCapacity);
                // Trigger Teleport;
                Destroy(ringInstance);
            }

        }

        public override void ActivateSkill()
        {
            Debug.Log("ActivateSkill");
            canBeLaunch = false;
            ActiveRingSkill();
        }

        public override void UpdateSkill(float deltaTime)
        {
            if (m_skillLaunchDuration > skillLaunchDuration)
            {
                m_skillLaunchDuration = 0;
                m_specialCapacities.OnFinish.Invoke(true);
            }
            else
            {
                m_skillLaunchDuration += deltaTime;
            }

        }

        public override bool CanLaunchSkill()
        {
            return canBeLaunch;
        }

    }
}