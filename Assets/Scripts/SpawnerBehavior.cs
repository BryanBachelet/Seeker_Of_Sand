using Enemies;
using GuerhoubaGames.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    [HideInInspector] public RoomManager m_roomManager;
    [HideInInspector] public EnemyManager m_enemyManager;
    [HideInInspector] public ObjectHealthSystem m_healthSystem;
    [HideInInspector] public SpawnerAnimation m_spawnerAnimation;

    [SerializeField] private int m_dissonanceCount = 30;
    [SerializeField] private GameObject dissonancePrefabObject;
    public void SendSpawnerDesactivation()
    {
        gameObject.SetActive(false);
        m_enemyManager.DesactiveSpawner(this.gameObject);
        GameObject dissonanceInstance = GamePullingSystem.SpawnObject(dissonancePrefabObject, transform.position, transform.rotation);
        ExperienceMouvement ExperienceMove = dissonanceInstance.GetComponent<ExperienceMouvement>();
        ExperienceMove.dissonanceValue = m_dissonanceCount;
        //ExperienceMove.m_playerPosition = TerrainGenerator.staticRoomManager.rewardPosition;
        ExperienceMove.m_playerPosition = m_enemyManager.m_playerTranform;
        if (this.GetComponent<Collider>() !=  null) { this.GetComponent<Collider>().enabled = false; }
    }

    public void UpdatePulse(float ratio)
    {
        if(m_spawnerAnimation == null) { m_spawnerAnimation = this.GetComponent<SpawnerAnimation>(); }
        m_spawnerAnimation.UpdatePulseFrequency(ratio);
    }

}
