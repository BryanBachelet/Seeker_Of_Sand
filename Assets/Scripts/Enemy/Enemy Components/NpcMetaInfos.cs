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
        FREEZE,
        DEATH,
        PAUSE,
        TERRIFY,
    }
    /// <summary>
    /// This class is the interface between enemy and the rest of the game
    /// </summary>
    public class NpcMetaInfos : MonoBehaviour
    {
        public string nameNpc;
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

        private EntityModifier m_entityModifier;

        public  Action OnStart;
        public void Awake()
        {
            m_healthComponent = GetComponent<NpcHealthComponent>();
            moveComponent = GetComponent<NpcMouvementComponent>();
            attackComponent = GetComponent<NpcAttackComponent>();
            specialCapacities = GetComponent<NpcSpecialCapacities>();
            behaviorTreeComponent = GetComponent<GuerhoubaGames.AI.BehaviorTreeComponent>();
            m_entityModifier = GetComponent<EntityModifier>();

            
        }

        public void Start()
        {
          
            m_objectGameState = new ObjectState();
            GameState.AddObject(m_objectGameState);
            
            m_entityModifier.OnStartFreeze += ActivateFreezeState;
            m_entityModifier.EndFreeze += DeactivateFreezeState;
            m_entityModifier.OnStartTerrify += ActivateTerrifyState;
            m_entityModifier.OnEndTerrify += DeactivateTerrifyState;

            if (behaviorTreeComponent && m_objectGameState.isPlaying)
            {
                behaviorTreeComponent.Init();
                behaviorTreeComponent.behaviorTree.blackboard.moveToObject = moveComponent.targetData.baseTarget.gameObject;
                OnStart?.Invoke();
            }
            
        }

        public void OnDestroy()
        {
            m_entityModifier.OnStartFreeze -= ActivateFreezeState;
            m_entityModifier.EndFreeze -= DeactivateFreezeState;
            m_entityModifier.OnStartTerrify -= ActivateFreezeState;
            m_entityModifier.OnEndTerrify -= DeactivateFreezeState;
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
            m_healthComponent.ResetTrail();
            transform.rotation = Quaternion.identity;
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

        public void ActivateFreezeState()
        {
            m_previousNpcState = (int)state;
            state = NpcState.FREEZE;

            if (behaviorTreeComponent)
            {
                behaviorTreeComponent.isActivate = false;
            }

        }

        private void DeactivateFreezeState()
        {
            state = (NpcState) m_previousNpcState;
            if (behaviorTreeComponent)
            {
                behaviorTreeComponent.isActivate = true;
            }
        }

        public void ActivateTerrifyState()
        {
            m_previousNpcState = (int)state;
            state = NpcState.TERRIFY;

            if (behaviorTreeComponent)
            {
                behaviorTreeComponent.isActivate = false;
            }

        }

        public void DeactivateTerrifyState()
        {
            m_previousNpcState = (int)state;
            state = NpcState.TERRIFY;

            if (behaviorTreeComponent)
            {
                behaviorTreeComponent.isActivate = false;
            }

        }


    }
}