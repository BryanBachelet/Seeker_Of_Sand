using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Enemies
{



    public enum NpcState
    {
        MOVE,
        IDLE,
        ATTACK,
        PREP_ATTACK,
        RECUPERATION,
        SPECIAL_CAPACITIES,
        SPECIAL_ACTION,
        DEATH,
        PAUSE,
    }
    /// <summary>
    /// This class is the interface between enemy and the rest of the game
    /// </summary>
    public class NpcMetaInfos : MonoBehaviour
    {
        public EnemyType type;
        public NpcState state;
        public NpcAttackComponent attackComponent;
        public NpcSpecialCapacities specialCapacities;
        [HideInInspector] public NpcHealthComponent m_healthComponent;
        [HideInInspector] public NpcMouvementComponent moveComponent;
        [HideInInspector] public EnemyManager manager;
        [HideInInspector] public GuerhoubaGames.AI.BehaviorTreeComponent behaviorTreeComponent;
        public ObjectState m_objectGameState;
        private int m_previousNpcState;

      public  Action OnStart;
        public void Awake()
        {
            m_healthComponent = GetComponent<NpcHealthComponent>();
            moveComponent = GetComponent<NpcMouvementComponent>();
            attackComponent = GetComponent<NpcAttackComponent>();
            specialCapacities = GetComponent<NpcSpecialCapacities>();
            behaviorTreeComponent = GetComponent<GuerhoubaGames.AI.BehaviorTreeComponent>();
        }

        public void Start()
        {
          
            m_objectGameState = new ObjectState();
            GameState.AddObject(m_objectGameState);

            if (behaviorTreeComponent && m_objectGameState.isPlaying)
            {
                behaviorTreeComponent.Init();
                behaviorTreeComponent.behaviorTree.blackboard.moveToObject = moveComponent.targetData.baseTarget.gameObject;
                OnStart?.Invoke();
            }
            
        }

        public void Update()
        {
            if(!m_objectGameState.isPlaying && state != NpcState.PAUSE)
            {
                SetPauseState();
            }
            if (m_objectGameState.isPlaying && state == NpcState.PAUSE)
            {
                RemovePauseState(); 
            }
        }

        public void RestartEnemy()
        {
            attackComponent.ResetComponent();

           
            if(behaviorTreeComponent.isFirstSpawn)
            {
                behaviorTreeComponent.Init();
            }
            else
            {
                behaviorTreeComponent.isActivate = true;
            }

        }

        public void SetPauseState()
        {
            m_previousNpcState = (int)state;
            state = NpcState.PAUSE;

            if (behaviorTreeComponent)
            {
                if(type==EnemyType.CHAMAN)
                {

                    Debug.Log("Test");
                }

                behaviorTreeComponent.isActivate = false;
                
            }
        }
        public void RemovePauseState()
        {
            state = (NpcState)m_previousNpcState;
            if (behaviorTreeComponent)
            {
                behaviorTreeComponent.Init();
                behaviorTreeComponent.behaviorTree.blackboard.moveToObject = moveComponent.targetData.baseTarget.gameObject;
                OnStart?.Invoke();
            }
        }

        public void TeleportToPool()
        {
            manager.TeleportEnemyOut(this);
        }
    }
}