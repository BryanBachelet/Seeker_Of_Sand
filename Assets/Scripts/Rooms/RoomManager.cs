using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;
using GuerhoubaTools;
using UnityEngine.Rendering.HighDefinition;

public class RoomManager : MonoBehaviour
{
    public RoomType currentRoomType;
    /// <summary>
    /// Correspond to start room type
    /// </summary>
    [HideInInspector] public RoomType baseRoomType;
    public RewardType rewardType;
    public HealthReward healthReward;
    public bool isTimingPassing;
    public int enemyToKillCount = 0;
    public int currentCountOfEnemy;

    public Teleporter[] teleporterArray = new Teleporter[3];
    private int m_currentTeleporterCount = 0;

    private Enemies.EnemyManager m_enemyManager;
    public bool isRoomHasBeenValidate = true;
    private bool isTeleporterActive = true;
    private Chosereward choserewardRef;
    public TerrainGenerator terrainGenerator;

    [HideInInspector] public RoomInfoUI roomInfoUI;

    static RewardDistribution playerRewardDistribution;


    public bool isActiveStartRotation;
    public float spawnAngle;
    public GameObject teleporterSpawn;
    public Transform rewardPosition;

    public Action<RoomType, RewardType> onActivateRoom;
    public Action<RoomType, RewardType> onDeactivateRoom;
    public Action<RoomType, RewardType> onCreateRoom;

    public AnimationCurve enemyCountCurve;

    public float timeReset = 0.2f;

    public float timerReset = 0.0f;

    private GameObject playerGO;

    private bool rewardGenerated = false;


     private DateTime m_startRoomChallengeTime; 
     private DateTime m_EndRoomChallengeTime; 
    private TimeSpan timeSpan;

    public void RetriveComponent()
    {
        if (onCreateRoom != null) onCreateRoom.Invoke(currentRoomType, rewardType);
        isRoomHasBeenValidate = false;
        isTeleporterActive = false;
        m_enemyManager = FindAnyObjectByType<Enemies.EnemyManager>();
        playerGO = GameObject.Find("Player");
        if (playerRewardDistribution == null)
        {

            playerRewardDistribution = playerGO.GetComponent<RewardDistribution>();

        }



        RoomInterface[] interfacesArray = transform.parent.GetComponentsInChildren<RoomInterface>();
        for (int i = 0; i < interfacesArray.Length; i++)
        {
            interfacesArray[i].SetupRoomOptions();
        }
    }
    public void ActivateRoom()
    {

        m_enemyManager.ResetAllSpawingPhasse();
        m_enemyManager.ResetSpawnStat();
        if (!rewardGenerated) GiveRoomReward(); rewardGenerated = true;

        if (onActivateRoom != null) onActivateRoom.Invoke(currentRoomType, rewardType);


        if (isActiveStartRotation)
        {
            Camera.main.GetComponent<Render.Camera.CameraBehavior>().SetupCamaraAnglge(spawnAngle);
        }


        m_startRoomChallengeTime = DateTime.Now;
        baseRoomType = currentRoomType;
        if (currentRoomType == RoomType.Enemy)
        {
            m_enemyManager.OnDeathSimpleEvent += CountEnemy;
            m_enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.DEBUG);
        }
        currentCountOfEnemy = 0;

        if (currentRoomType == RoomType.Enemy) m_enemyManager.isStopSpawn = false;
        else m_enemyManager.isStopSpawn = true;

        SetupRoomType();

    }

    public void DeactivateRoom()
    {
        if (onDeactivateRoom != null) onDeactivateRoom.Invoke(currentRoomType, rewardType);
        DeactivateAltar();
    }

    // Setup room teleporter 
    public void SetupTeleporter(int teleporterCount)
    {
        m_currentTeleporterCount = teleporterCount;
        for (int i = 0; i < m_currentTeleporterCount; i++)
        {
            teleporterArray[i].gameObject.SetActive(true);
            teleporterArray[i].enemyManager = m_enemyManager;
        }

    }
    public void AddTeleporter(Teleporter newInstance)
    {
        newInstance.DesactivationTeleportor();

        newInstance.enemyManager = m_enemyManager;
        if (m_currentTeleporterCount > 2)
        {
            Debug.Log("Too much teleporter");
            return;
        }


        teleporterArray[m_currentTeleporterCount] = newInstance;
        m_currentTeleporterCount++;

    }


    public void SetupRoomType()
    {
        switch (currentRoomType)
        {
            case RoomType.Free:
                ValidateRoom();
                DeactivateAltar();

                break;
            case RoomType.Merchant:
                ValidateRoom();
                DeactivateAltar();
                break;
            case RoomType.Event:

                AltarBehaviorComponent obj = transform.parent.GetComponentInChildren<AltarBehaviorComponent>();
                if (obj != null)
                {
                    obj.ResetAltar();
                    obj.roomInfoUI = roomInfoUI; ;
                }

                break;
            case RoomType.Enemy:
                DeactivateAltar();
                roomInfoUI.ActiveMajorGoalInterface();
                int enemyCount = (int)enemyCountCurve.Evaluate(TerrainGenerator.roomGeneration_Static);
                enemyToKillCount = UnityEngine.Random.Range(enemyCount / 2, enemyCount);
                break;
            default:
                break;
        }
    }

    public void Update()
    {
        if (!isRoomHasBeenValidate) return;

        if (playerRewardDistribution.isRewardSend && !isTeleporterActive)
        {
            ActivateTeleporters();
            isTeleporterActive = true;
        }


    }
    public void ValidateRoom()
    {
        if (isRoomHasBeenValidate) return;

        //GiveRoomReward();
        if ((int)currentRoomType < (int)RoomType.Free) currentRoomType = RoomType.Free;
        roomInfoUI.ActualizeRoomInfoInterface();
        roomInfoUI.DeactivateMajorGoalInterface();
        m_enemyManager.isStopSpawn = true;
        m_enemyManager.DestroyAllEnemy();
        isRoomHasBeenValidate = true;
        playerGO.GetComponent<HealthPlayerComponent>().RestoreQuarter();
        m_EndRoomChallengeTime = DateTime.Now;

        timeSpan = m_EndRoomChallengeTime - m_startRoomChallengeTime;
        LogSystem.LogMsg("Duration of the room is " + timeSpan.ToString());

    }

    #region Room Validation Functions
    private void ActivateTeleporters()
    {
        for (int i = 0; i < m_currentTeleporterCount; i++)
        {
            teleporterArray[i].ActivationTeleportor();
            //teleporterArray[i].GetComponentInChildren<TeleporterFeebackController>().activeChange = true;
        }
    }

    private void GiveRoomReward()
    {
        playerRewardDistribution.GiveReward(rewardType, rewardPosition, healthReward);

    }


    #endregion


    public void DeactivateAltar() // Temp function 
    {
        AltarBehaviorComponent obj = transform.parent.GetComponentInChildren<AltarBehaviorComponent>();
        if (obj != null)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void CountEnemy()
    {
        currentCountOfEnemy++;
        float progress = (float)currentCountOfEnemy / (float)enemyToKillCount;
        //Debug.Log("Enemy Count" + progress);
        if (currentRoomType == RoomType.Enemy)
        {
            if (progress >= 1)
            {
                ValidateRoom();

            }
            else
            {
                roomInfoUI.ActualizeMajorGoalProgress(progress);
            }
        }


    }

    private void OnDrawGizmosSelected()
    {
        if (isActiveStartRotation)
        {
            Vector3 position = Quaternion.Euler(0, spawnAngle, 0) * Vector3.forward * -24 + teleporterSpawn.transform.position + Vector3.up *10;
            Gizmos.DrawCube(position, new Vector3(7, 7, 7));

            Gizmos.DrawRay(position, (teleporterSpawn.transform.position-position).normalized *100);
        }
    }

}
