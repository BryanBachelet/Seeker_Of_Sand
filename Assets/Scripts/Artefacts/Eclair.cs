using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artefact
{
    public class Eclair : MonoBehaviour
    {
        public float m_damage;
        private ArtefactData m_artefactData;

        public void Start()
        {
            
            m_artefactData = GetComponent<ArtefactData>();
            if (!m_artefactData.agent) return;
            
            Enemies.NpcHealthComponent healthComponent = m_artefactData.agent.GetComponent<Enemies.NpcHealthComponent>();
            healthComponent.ReceiveDamage(m_damage, Vector3.up, 1);
        }
    }
}