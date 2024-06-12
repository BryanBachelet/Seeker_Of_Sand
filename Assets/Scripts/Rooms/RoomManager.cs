using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;

public enum RoomType
{
    Free = 0,
    Event = 1,
    Enemy = 2
}

public enum RewardType
{
    UPGRADE = 0,
    SPELL = 1,
    ARTEFACT = 2,
    HEAL = 3,
}

public class RoomManager : MonoBehaviour
{
    public RoomType currentRoomType;
    /// <summary>
    /// Correspond to start room type
    /// </summary>
    [HideInInspector] public RoomType baseRoomType;
    public RewardType rewardType;

    public int enemyToKillCount = 0;
    public int currentCountOfEnemy;

    public Teleporter[] teleporterArray = new Teleporter[3];
    private int m_currentTeleporterCount = 0;

    private Enemies.EnemyManager m_enemyManager;
    private bool isRoomHasBeenValidate = true;
    private bool isTeleporterActive = true;
    private Chosereward choserewardRef;
    public TerrainGenerator terrainGenerator;

   [HideInInspector] public RoomInfoUI roomInfoUI;

    static RewardDistribution playerRewardDistribution;
   // static ObjectifAndReward_Ui_Function objAndReward_UI;


    public void RetriveComponent()
    {
        isRoomHasBeenValidate = false;
        isTeleporterActive = false;
        m_enemyManager = FindAnyObjectByType<Enemies.EnemyManager>();
        if (playerRewardDistribution == null)
        {
            playerRewardDistribution = GameObject.Find("Player").GetComponent<RewardDistribution>();
           
        }
    }
    public void ActivateRoom()
    {
        m_enemyManager.ResetAllSpawingPhasse();
        baseRoomType = currentRoomType;
        if (currentRoomType == RoomType.Enemy)
        {
            m_enemyManager.OnDeathSimpleEvent += CountEnemy;
            m_enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.DEBUG);
        }
           currentCountOfEnemy = 0;

        if (currentRoomType == RoomType.Free) m_enemyManager.isStopSpawn = true;
        else m_enemyManager.isStopSpawn = false;
   
        SetupRoomType();
    }

    // Setup room teleporter 
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
                int enemyCount = 20 + 20 * terrainGenerator.countRoomGeneration;
                enemyToKillCount = Random.Range(enemyCount / 2, enemyCount);
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
      
        GiveRoomReward();
        currentRoomType = RoomType.Free;
        roomInfoUI.ActualizeRoomInfoInterface();
        roomInfoUI.DeactivateMajorGoalInterface();
        m_enemyManager.isStopSpawn = true;
        m_enemyManager.DestroyAllEnemy();
        isRoomHasBeenValidate = true;
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
        playerRewardDistribution.GiveReward(rewardType);
       
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
        Debug.Log("Enemy Count" + progress);
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

}
