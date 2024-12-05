using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;
using GuerhoubaTools;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.AI;

public class RoomManager : MonoBehaviour
{
    private const float distanceBeforeActivatingRooom = 30;
    public GameElement element = 0; 


    [HideInInspector] public Camera previewCamera;
    public CustomRenderTexture m_CRT;
    public Material m_materialPreviewTRT;
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

    [HideInInspector] public Enemies.EnemyManager m_enemyManager;
    public bool isRoomHasBeenValidate = true;
    private bool isTeleporterActive = true;
    [HideInInspector] public TerrainGenerator terrainGenerator;

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
    [HideInInspector] public TerrainDropGeneration dropGenerator;
    [HideInInspector] public BossRoom bossRoom;
    public bool isRoomHasBeenDeactivated;

    private DateTime m_startRoomChallengeTime;
    private DateTime m_EndRoomChallengeTime;
    private TimeSpan timeSpan;


    private bool m_isStartActivation = false;
    private bool m_isDistanceActivationDone = false;

    private Character.CharacterUpgrade m_characterUpgrade;

    static public int enemyMaxSpawnInRoon;

    private NavMeshData m_navMesh;
    public void RetriveComponent()
    {
        if (onCreateRoom != null) onCreateRoom.Invoke(currentRoomType, rewardType);
        isRoomHasBeenValidate = false;
        previewCamera = transform.parent.GetComponentInChildren<Camera>();
        isTeleporterActive = false;
        m_enemyManager = FindAnyObjectByType<Enemies.EnemyManager>();
        playerGO = GameObject.Find("Player");
        if(dropGenerator == null && transform.parent.GetComponentInChildren<TerrainDropGeneration>())
        {
            dropGenerator = transform.parent.GetComponentInChildren<TerrainDropGeneration>();
        }
        if (playerRewardDistribution == null)
        {

            playerRewardDistribution = playerGO.GetComponent<RewardDistribution>();

        }
        if (m_characterUpgrade == null)
        {
            m_characterUpgrade = playerGO.GetComponent<Character.CharacterUpgrade>();
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
        currentCountOfEnemy = 0;
        m_isStartActivation = true;
        m_startRoomChallengeTime = DateTime.Now;
        baseRoomType = currentRoomType;
        //if (currentRoomType == RoomType.Enemy)
        //{
        //    m_enemyManager.OnDeathSimpleEvent += CountEnemy;
        //    m_enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.DEBUG);
        //}


        //if (currentRoomType == RoomType.Enemy) m_enemyManager.isStopSpawn = false;
        //else
        m_enemyManager.isStopSpawn = true;

        SetupRoomType();
        previewCamera.gameObject.SetActive(false);
    }

    public void ActivateRoomAfterDistanceTP()
    {
        NavMeshHit hit;
        Vector3 playerPos = playerGO.transform.position;
        if (!NavMesh.SamplePosition(playerPos, out hit, 10, NavMesh.AllAreas)) return;
        if (currentRoomType == RoomType.Enemy && !m_isDistanceActivationDone && m_isStartActivation)
        {
            if ((teleporterSpawn.transform.position - playerGO.transform.position).magnitude > distanceBeforeActivatingRooom)
            {
                
                roomInfoUI.ActiveMajorGoalInterface();
                m_startRoomChallengeTime = DateTime.Now;
                baseRoomType = currentRoomType;
                if (currentRoomType == RoomType.Enemy)
                {
                    m_enemyManager.OnDeathSimpleEvent += CountEnemy;
                    m_enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.DEBUG);
                }
                m_isDistanceActivationDone = true;

                if (currentRoomType == RoomType.Enemy) m_enemyManager.isStopSpawn = false;


            }
        }
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
        m_enemyManager.countEnemySpawnMaximum = 0;
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
                    //obj.m_enemiesCountConditionToWin = (int)enemyCountCurve.Evaluate(TerrainGenerator.roomGeneration_Static);
                    obj.eventElementType = element;
                    obj.m_enemiesCountConditionToWin = (int)enemyCountCurve.Evaluate(m_characterUpgrade.avatarUpgradeList.Count + (int)m_characterUpgrade.GetComponent<CharacterArtefact>().artefactsList.Count * 3f);
                    enemyMaxSpawnInRoon = enemyToKillCount = obj.m_enemiesCountConditionToWin;
                    obj.roomInfoUI = roomInfoUI;
                }

                break;
            case RoomType.Enemy:
                DeactivateAltar();

                //int enemyCount = (int)enemyCountCurve.Evaluate(TerrainGenerator.roomGeneration_Static);
                //enemyToKillCount = UnityEngine.Random.Range(enemyCount / 2, enemyCount);
                enemyToKillCount = (int)enemyCountCurve.Evaluate(m_characterUpgrade.avatarUpgradeList.Count + (int)m_characterUpgrade.GetComponent<CharacterArtefact>().artefactsList.Count * 3f);
                enemyMaxSpawnInRoon = enemyToKillCount;
                break;
            case RoomType.Boss:
                DeactivateAltar();


                break;
            default:
                break;
        }
    }




    public void Update()
    {
        ActivateRoomAfterDistanceTP();

        if (!isRoomHasBeenValidate || !isRoomHasBeenDeactivated) return;

        if (playerRewardDistribution.isRewardSend && !isTeleporterActive)
        {
             playerGO.GetComponent<HealthPlayerComponent>().RestoreQuarter();
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
      
        if(dropGenerator != null) dropGenerator.DistributeCristalAtTheEnd();
        m_EndRoomChallengeTime = DateTime.Now;
        m_enemyManager.ActiveSpawnPhase(false, Enemies.EnemySpawnCause.DEBUG);
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
        playerRewardDistribution.GiveReward(rewardType, rewardPosition, healthReward,element);

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
                roomInfoUI.UpdateTextProgression(enemyToKillCount - currentCountOfEnemy, enemyToKillCount);
            }
        }


    }


    public IEnumerator RoomDeactivation(int frameCount)

    {

        int framePassed = 0;

        while (framePassed < frameCount)
        {
            yield return Time.deltaTime;
            framePassed++;
        }


        isRoomHasBeenDeactivated = true;
        transform.parent.gameObject.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        if (isActiveStartRotation)
        {
            Vector3 position = Quaternion.Euler(0, spawnAngle, 0) * Vector3.forward * -24 + teleporterSpawn.transform.position + Vector3.up * 10;
            Gizmos.DrawCube(position, new Vector3(7, 7, 7));

            Gizmos.DrawRay(position, (teleporterSpawn.transform.position - position).normalized * 100);
        }
    }

}
