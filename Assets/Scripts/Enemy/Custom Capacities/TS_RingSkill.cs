using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
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
            
        private bool canBeLaunch =true;
        private NpcSpecialCapacities m_specialCapacities;
        private NpcHealthComponent m_npcHealthComponent;
        private NpcMetaInfos m_npcMetaInfo;
        private GuerhoubaGames.AI.BehaviorTreeComponent m_behaviorTreeComponent;
        private GameObject playerGO;

        #region Unity Functions

        public void Start()
        {
            m_specialCapacities = GetComponent<NpcSpecialCapacities>();
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_npcMetaInfo = GetComponent<NpcMetaInfos>();
            m_behaviorTreeComponent = GetComponent<GuerhoubaGames.AI.BehaviorTreeComponent>();
            playerGO = m_npcHealthComponent.targetData.target.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRingSkill();
            if(m_npcMetaInfo.state == NpcState.DEATH)
            {
                Destroy(ringInstance);
            }
        }

        #endregion


        public void ActiveRingSkill()
        {
            SetPositionRing();
            if (ringInstance == null)
            {
                ringInstance = Instantiate(ringGO, Vector3.zero,Quaternion.identity);
            }
            ringInstance.transform.position = new Vector3(centerRing.x, centerRing.y, centerRing.z);
        }

        private void SetPositionRing()
        {
            centerRing = transform.position;
        }

        public void UpdateRingSkill()
        {
            if (canBeLaunch) return;

            Vector3 pos = playerGO.transform.position;
            pos.y = centerRing.y;
            if (Vector3.Distance(playerGO.transform.position, centerRing) > radius)
            {
                canBeLaunch = true;
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

        public void OnDrawGizmosSelected()
        {
            if(activeDebug) Gizmos.DrawWireSphere(centerRing, radius);
        }
    }
}