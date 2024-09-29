using GuerhoubaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public Transform centerTransform;
    public RoomManager roomManager;
    private GameObject m_bossInstance;
    private Enemies.EnemyManager enemyManager;
    public int bossLife = 200;
    [HideInInspector] public RoomInfoUI roomInfoUI;

    // Boss components 
   private Enemies.NpcHealthComponent m_bossHealth;

    public void Start()
    {
        enemyManager = roomManager.m_enemyManager;
        roomInfoUI = roomManager.roomInfoUI;
      
    }

    public void SpawnBossInstance()
    {
        GameObject bossInstance = enemyManager.SpawnBoss(centerTransform.position, Enemies.EnemyType.TWILIGHT_SISTER);
        m_bossHealth = bossInstance.GetComponent<Enemies.NpcHealthComponent>();
        m_bossHealth.SetupLife(bossLife);
        roomInfoUI.ActiveMajorGoalInterface();
    }

    public void Update()
    {
        if(m_bossHealth)
        {
            roomInfoUI.ActualizeMajorGoalProgress(1.0f-(m_bossHealth.GetCurrentLife() / m_bossHealth.maxLife));
        }
    }

    public void EndRoomBoss()
    {
        roomManager.ValidateRoom();
    }
}
