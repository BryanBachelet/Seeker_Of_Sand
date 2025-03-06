using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    [HideInInspector] public RoomManager m_roomManager;
    [HideInInspector] public EnemyManager m_enemyManager;

    public void SendSpawnerDesactivation()
    {
        m_enemyManager.DesactiveSpawner(this.gameObject);
    }
}
