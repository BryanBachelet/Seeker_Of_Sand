using Character;
using GuerhoubaGames.AI;
using GuerhoubaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public Transform centerTransform;
    public RoomManager roomManager;
    private GameObject m_bossInstance;
    [HideInInspector] public Enemies.EnemyManager enemyManager;
    public int bossLife = 200;
    [HideInInspector] public RoomInfoUI roomInfoUI;
    DayCyclecontroller dayCyclecontroller;
    // Boss components 
   private Enemies.NpcHealthComponent m_bossHealth;
    private BossCamera bossCamera;
    public void Start()
    {
        enemyManager = roomManager.m_enemyManager;
        roomInfoUI = roomManager.roomInfoUI;
        bossCamera = GetComponent<BossCamera>();
    }

    public void SpawnBossInstance()
    {
        GameState.ChangeState();
        GameObject bossInstance = enemyManager.SpawnBoss(centerTransform.position, Enemies.EnemyType.TWILIGHT_SISTER);
        dayCyclecontroller = enemyManager.m_dayController;
        dayCyclecontroller.UpdateDepthOfField(false);
        m_bossHealth = bossInstance.GetComponent<Enemies.NpcHealthComponent>();
        m_bossHealth.SetupLife(bossLife + 30 * enemyManager.m_characterUpgrade.avatarUpgradeList.Count + (int)enemyManager.m_characterUpgrade.GetComponent<CharacterArtefact>().artefactsList.Count * 3f);
        bossInstance.GetComponent<BehaviorTreeComponent>().isActivate = false;
        bossCamera.StartCamera(Camera.main,bossInstance.transform.GetChild(0), centerTransform.position);
        Vector3 posRef = enemyManager.AstrePositionReference.position;
        bossInstance.transform.GetChild(0).transform.position = posRef;

    }

    public void LaunchBoss()
    {
        GameState.ChangeState();

    }
    public void DisplayBossHealth()
    {
        roomInfoUI.ActiveMajorGoalInterface();
    }
    public void Update()
    {
        if(m_bossHealth)
        {
            float bossHealth_float = m_bossHealth.GetCurrentLife();
            roomInfoUI.ActualizeMajorGoalProgress(1.0f-(bossHealth_float / m_bossHealth.maxLife));
            roomInfoUI.UpdateTextProgression((int)bossHealth_float, (int)m_bossHealth.maxLife);
        }
    }

    public void EndRoomBoss()
    {
        roomManager.ValidateRoom();
        dayCyclecontroller.UpdateDepthOfField(true);
        DayCyclecontroller.m_nightCountGlobal++;
        enemyManager.gsm.UpdateParameter(0.1f, "Intensity");
    }
}
