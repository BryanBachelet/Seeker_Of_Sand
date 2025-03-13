using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    [HideInInspector] public RoomManager m_roomManager;
    [HideInInspector] public EnemyManager m_enemyManager;
    [HideInInspector] public ObjectHealthSystem m_healthSystem;
    [HideInInspector] public SpawnerAnimation m_spawnerAnimation;
    public void SendSpawnerDesactivation()
    {
        m_enemyManager.DesactiveSpawner(this.gameObject);
        if(this.GetComponent<Collider>() !=  null) { this.GetComponent<Collider>().enabled = false; }
    }

    public void UpdatePulse(float ratio)
    {
        if(m_spawnerAnimation == null) { m_spawnerAnimation = this.GetComponent<SpawnerAnimation>(); }
        m_spawnerAnimation.UpdatePulseFrequency(ratio);
    }
}
