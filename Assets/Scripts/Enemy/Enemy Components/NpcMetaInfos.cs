using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{



    public enum NpcState
    {
        MOVE,
        IDLE,
        ATTACK,
        PREP_ATTACK,
        RECUPERATION,
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
        private NpcHealthComponent m_healthComponent;
        [HideInInspector] public NpcMouvementComponent moveComponent;
        [HideInInspector] public EnemyManager manager;
        [HideInInspector] public GuerhoubaGames.AI.BehaviorTreeComponent behaviorTreeComponent;
        public ObjectState m_objectGameState;
        private int m_previousNpcState;

        public void Awake()
        {
            m_healthComponent = GetComponent<NpcHealthComponent>();
            moveComponent = GetComponent<NpcMouvementComponent>();
            attackComponent = GetComponent<NpcAttackComponent>();
            behaviorTreeComponent = GetComponent<GuerhoubaGames.AI.BehaviorTreeComponent>();
        }

        public void Start()
        {
            if (behaviorTreeComponent)
            {
                behaviorTreeComponent.Init();
                behaviorTreeComponent.behaviorTree.blackboard.moveToObject = moveComponent.targetData.baseTarget.gameObject;
            }


            m_objectGameState = new ObjectState();
            GameState.AddObject(m_objectGameState);
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
            m_healthComponent.RestartObject(1);
            moveComponent.RestartObject();
        }

        public void SetPauseState()
        {
            m_previousNpcState = (int)state;
            state = NpcState.PAUSE;
        }
        public void RemovePauseState()
        {
            state = (NpcState)m_previousNpcState;
        }

        public void TeleportToPool()
        {
            manager.TeleportEnemyOut(this);
        }
    }
}