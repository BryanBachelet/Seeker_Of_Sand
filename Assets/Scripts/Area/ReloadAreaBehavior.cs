using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{

    [Serializable]
    public class ReloadAreaBehavior : GeneralSpellBehavior
    {
        private AreaMeta m_areaMeta;
        private AreaDotBehavior m_areaDotBehavior;
        public void Awake()
        {
            m_areaMeta = GetComponent<AreaMeta>();
            m_areaMeta.OnSpawn += Init;
            m_areaDotBehavior = GetComponent<AreaDotBehavior>();
        }

        public void Init()
        {
            GameState.m_enemyManager.OnDeathEvent += ResetArea;
        }

        public void ResetArea(Vector3 position, EntitiesTrigger tag, GameObject objectHit, float distance)
        {
            if (!isActive) return;
            m_areaDotBehavior.hitCount--;
            m_areaDotBehavior.hitCount = Mathf.Clamp(m_areaDotBehavior.hitCount, 0,int.MaxValue);
        }

        public void OnDestroy()
        {
            GameState.m_enemyManager.OnDeathEvent -= ResetArea;
        }
    } 
}
