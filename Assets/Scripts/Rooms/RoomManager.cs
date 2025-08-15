using BorsalinoTools;
using GuerhoubaGames;
using GuerhoubaGames.Character;
using GuerhoubaGames.Enemies;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.UI;
using Render.Camera;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




public class RoomManager : MonoBehaviour
{
    [SerializeField] private float distanceBeforeActivatingRooom = 30;
    public GameElement element = 0;

    public Transform miniMapCameraPosition;
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
    public int specialID = -1; //-1 signifie que la salle n'est pas spécial. 0 = Salle Marchand, 1 = Salle Boss
    public int enemyToKillCount = 0;
    public int currentCountOfEnemy;
    [HideInInspector] public int terrainIndex;

    public Teleporter[] teleporterArray = new Teleporter[3];
    private int m_currentTeleporterCount = 0;

    [HideInInspector] public EnemyManager m_enemyManager;
    public bool isRoomHasBeenValidate = true;
    private bool isTeleporterActive = true;
    [HideInInspector] public TerrainGenerator terrainGenerator;

    [HideInInspector] public RoomInfoUI roomInfoUI;


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
    static public float progress = 0;

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

    private CharacterUpgrade m_characterUpgrade;

    static public int enemyMaxSpawnInRoon;

    private NavMeshData m_navMesh;
    private CameraBehavior m_cameraBehavior;

    public const int maxEventActive = 3;
    public int eventActive = 0;
    private TerrainActivationManager m_terrainActivation;

    public int[] rewardAssociated;
    public bool hasEndReward;

    [Header("Object Rooms Variables")]
    public AltarBehaviorComponent[] altarBehaviorComponents = new AltarBehaviorComponent[3];
    public DissonanceHeartBehavior dissonanceHeartBehavior;
    public PedestalRoomBehavior pedestalRoomBehavior;


    #region Spawner Parameter
    [Header("Spawner Parameter")]
    public GameObject spawnerPrefab;
    public float rangeSpawner;
    public float rangeSpawner_Min;
    public int quantitySpawner = 1;
    public GameObject[] spawnerList;
    public LayerMask groundLayer;

    public TeleporterFeebackController teleporterFeedback;
    public MeshRenderer portalPrevious;
    [SerializeField] private int specialRoomID = -1;

    public bool delayUpdate = true;

    [HideInInspector] public bool isFirstNightRoom = false;
     
    private int m_currentChampionAlive;

    #endregion

    [Header("Debug Variables")]
    [HideInInspector] public bool activeRoomManagerDebug;
    [HideInInspector] public bool activeFastRoom;

    public void InitComponent()
    {
        if (onCreateRoom != null) onCreateRoom.Invoke(currentRoomType, rewardType);
        isRoomHasBeenValidate = false;
        previewCamera = transform.parent.GetComponentInChildren<Camera>();
        isTeleporterActive = false;
        m_enemyManager = FindAnyObjectByType<EnemyManager>();
        playerGO = GameObject.Find("Player");
        if (dropGenerator == null && transform.parent.GetComponentInChildren<TerrainDropGeneration>())
        {
            dropGenerator = transform.parent.GetComponentInChildren<TerrainDropGeneration>();
        }

        if (m_characterUpgrade == null)
        {
            m_characterUpgrade = playerGO.GetComponent<CharacterUpgrade>();
        }
        if (m_cameraBehavior == null)
        {
            m_cameraBehavior = Camera.main.GetComponent<CameraBehavior>();
        }

        RoomInterface[] interfacesArray = transform.parent.GetComponentsInChildren<RoomInterface>();
        for (int i = 0; i < interfacesArray.Length; i++)
        {
            interfacesArray[i].SetupRoomOptions();
        }
        delayUpdate = false;
    }

    public void ActivateRoom(Material previousMat)
    {
     

        ResetObjectifData();

        StartCoroutine(CloseEnterPortal(3.5f));

        if (onActivateRoom != null) onActivateRoom.Invoke(currentRoomType, rewardType);

        if (pedestalRoomBehavior)
            pedestalRoomBehavior.roomManager = this;

        if (isActiveStartRotation)
        {
            m_cameraBehavior.SetupCamaraAnglge(spawnAngle);
        }

        m_isStartActivation = true;
        m_startRoomChallengeTime = DateTime.Now;
        baseRoomType = currentRoomType;
        if (quantitySpawner > 0) { GenerateSpawner(); }
        //if (currentRoomType == RoomType.Enemy)
        //{
        //    m_enemyManager.OnDeathSimpleEvent += CountEnemy;
        //    m_enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.DEBUG);
        //}


        //if (currentRoomType == RoomType.Enemy) m_enemyManager.isStopSpawn = false;
        //else
        //m_enemyManager.isStopSpawn = true;
        //m_cameraBehavior.ResetZoom();
        SetupRoomType();
        teleporterFeedback.previewMeshPlane.material = new Material(previousMat);
        previewCamera.gameObject.SetActive(false);
    }

    public void ResetObjectifData()
    {
        m_enemyManager.ResetAllSpawingPhasse();
        m_enemyManager.ResetSpawnStat();
        progress = 0;

        currentCountOfEnemy = 0;
        m_enemyManager.countEnemySpawnMaximum = 0;

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


                //roomInfoUI.ActiveMajorGoalInterface();
                m_startRoomChallengeTime = DateTime.Now;
                baseRoomType = currentRoomType;
                if (isFirstNightRoom) RunManager.instance.LaunchNightCountdown();
                if (currentRoomType == RoomType.Enemy)
                {

                    //m_enemyManager.OnDeathSimpleEvent += CountEnemy;
                    m_enemyManager.ActiveSpawnPhase(true,   EnemySpawnCause.DEBUG);
                    //m_cameraBehavior.isZoomActive = false;
                }
                m_isDistanceActivationDone = true;

                if (currentRoomType == RoomType.Enemy) m_enemyManager.isStopSpawn = false;


            }
        }


    }

    public IEnumerator CloseEnterPortal(float time)
    {
        yield return new WaitForSeconds(time);
        teleporterFeedback.GetComponentInParent<Animator>().SetBool("Open", false);
    }

    public void DeactivateRoom()
    {
        if (onDeactivateRoom != null) onDeactivateRoom.Invoke(currentRoomType, rewardType);
        DeactiveEvents();
        m_enemyManager.DestroyAllEnemy();
        StartCoroutine(DestroyRoom());
    }

    IEnumerator DestroyRoom()
    {
        yield return new WaitForSeconds(3);
        GameObject.Destroy(this.gameObject.transform.parent.gameObject);
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
        for (int i = m_currentTeleporterCount; i < teleporterArray.Length; i++)
        {
            teleporterArray[i].gameObject.SetActive(false);
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
                DeactiveEvents();
                break;
            case RoomType.Merchant:
                ValidateRoom();
                DeactiveEvents();
                break;
            case RoomType.Event:
                ActivateEvents();
                eventActive = altarBehaviorComponents.Length;
                dissonanceHeartBehavior.roomManager = this;
                for (int i = 0; i < altarBehaviorComponents.Length; i++)
                {
                    if (altarBehaviorComponents[i] != null)
                    {
                        altarBehaviorComponents[i].indexEvent = i;
                        altarBehaviorComponents[i].ResetAltar();
                        //obj.m_enemiesCountConditionToWin = (int)enemyCountCurve.Evaluate(TerrainGenerator.roomGeneration_Static);
                        altarBehaviorComponents[i].eventElementType = element;
                        altarBehaviorComponents[i].m_enemiesCountConditionToWin = (int)enemyCountCurve.Evaluate(m_characterUpgrade.avatarUpgradeList.Count + (int)m_characterUpgrade.GetComponent<CharacterArtefact>().artefactsList.Count * 3f);
                        enemyMaxSpawnInRoon = enemyToKillCount = altarBehaviorComponents[i].m_enemiesCountConditionToWin;
                        altarBehaviorComponents[i].roomInfoUI = roomInfoUI;
                        roomInfoUI.UpdateTextProgression(altarBehaviorComponents[i].m_enemiesCountConditionToWin, altarBehaviorComponents[i].m_enemiesCountConditionToWin);


                    }

                }
              


                break;
            case RoomType.Enemy:
                DeactiveEvents();
                ActiveNightRoom();
                dissonanceHeartBehavior.roomManager = this;
                //int enemyCount = (int)enemyCountCurve.Evaluate(TerrainGenerator.roomGeneration_Static);
                ////enemyToKillCount = UnityEngine.Random.Range(enemyCount / 2, enemyCount);
                //enemyToKillCount = (int)enemyCountCurve.Evaluate(m_characterUpgrade.avatarUpgradeList.Count + (int)m_characterUpgrade.GetComponent<CharacterArtefact>().artefactsList.Count * 3f);
                //enemyMaxSpawnInRoon = enemyToKillCount;
                //roomInfoUI.UpdateTextProgression(enemyToKillCount, enemyToKillCount);
                break;
            case RoomType.Boss:
                DeactiveEvents();


                break;
            default:
                break;
        }
    }

    public int[] GenerateRewardForRoom()
    {
        rewardAssociated = new int[3];
        for (int i = 0; i < rewardAssociated.Length; i++)
        {
            int randomReward = UnityEngine.Random.Range(0, 100);
            if (randomReward < 50)
            {
                rewardAssociated[i] = 0;
            }
            else if (randomReward >= 50 && randomReward < 75)
            {
                rewardAssociated[i] = 2;
            }
            else
            {
                rewardAssociated[i] = 2;
            }
        }
        return rewardAssociated;

    }

    public void Update()
    {
        ActivateRoomAfterDistanceTP();
        if (delayUpdate) return;
        if (!isRoomHasBeenValidate || !isRoomHasBeenDeactivated) return;
        if (hasEndReward && !rewardGenerated) GiveRoomReward(); rewardGenerated = true;
        if ((hasEndReward || !hasEndReward) && !isTeleporterActive)
        {
            OpenPortals();
        }


    }

    public void OpenPortals()
    {
        // Restor player life
        if (currentRoomType != RoomType.Boss) playerGO.GetComponent<HealthPlayerComponent>().RestoreQuarter();
        else playerGO.GetComponent<HealthPlayerComponent>().RestoreFullLife();

        terrainGenerator.GenerateMap();


        for (int i = 0; i < terrainGenerator.mapInstantiated.Count; i++)
        {
            SetupTeleporter(terrainGenerator.mapInstantiated.Count);
        }
        ActivateTeleporters();

        isTeleporterActive = true;
    }

    public void FinishAllEvent()
    {
        if (dissonanceHeartBehavior.dissonanceHeartState == DissonanceHeartBehavior.DissonanceHeartState.PROTECTED)
            dissonanceHeartBehavior.RemoveProtection();
        else
            ValidateRoom();
    }

    public void ValidateRoom()
    {
        if (isRoomHasBeenValidate) return;

        if (pedestalRoomBehavior)
            pedestalRoomBehavior.ActivatePedestal();

        if ((int)currentRoomType < (int)RoomType.Free) currentRoomType = RoomType.Free;
        roomInfoUI.ActualizeRoomInfoInterface();
        roomInfoUI.DeactivateMajorGoalInterface();

        m_enemyManager.DestroyAllEnemy();
        isRoomHasBeenValidate = true;

        m_EndRoomChallengeTime = DateTime.Now;
        m_enemyManager.ActiveSpawnPhase(false, EnemySpawnCause.DEBUG);
        timeSpan = m_EndRoomChallengeTime - m_startRoomChallengeTime;


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
        RunManager.SpawnReward(rewardType, rewardPosition.position, element);
    }


    #endregion



    //private void CountEnemy()
    //{
    //    currentCountOfEnemy++;
    //    progress = (float)currentCountOfEnemy / (float)enemyToKillCount;
    //    //Debug.Log("Enemy Count" + progress);
    //    if (currentRoomType == RoomType.Enemy)
    //    {
    //        if (progress >= 1)
    //        {
    //            ValidateRoom();
    //        }
    //        else
    //        {
    //            roomInfoUI.ActualizeMajorGoalProgress(progress);
    //            roomInfoUI.UpdateTextProgression(enemyToKillCount - currentCountOfEnemy, enemyToKillCount);
    //        }
    //    }


    //}

    private void CountChampionEliminate()
    {
        m_currentChampionAlive--;

        if (activeRoomManagerDebug)
            ScreenDebuggerTool.AddMessage("Champion Eliminate. " + m_currentChampionAlive + " still alive");

        int[] dataObjectif =
{
            (maxEventActive - m_currentChampionAlive),
            maxEventActive
        };
        roomInfoUI.UpdateRoomInfoDisplay(dataObjectif, null);

        if (m_currentChampionAlive<=0)
        {
            FinishAllEvent();
            m_enemyManager.OnChampionDeathEvent -= CountChampionEliminate;
            return;
        }

    }

    public int EventValidate() 
    {
        eventActive--;

        dropGenerator.GenerateCristal(this.transform);

        int[] dataObjectif =
        {
            (maxEventActive - eventActive),
            maxEventActive
        };
        roomInfoUI.UpdateRoomInfoDisplay(dataObjectif, null);

        if (eventActive <= 0)
        {
            FinishAllEvent();
          
            return rewardAssociated[eventActive];
        }

        ResetObjectifData();
        return rewardAssociated[eventActive];

    }

    public int CheckEventNumber()
    {
        int data = maxEventActive - eventActive;

        return data;
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

    #region Spawner Functions
    public void GenerateSpawner()
    {
        return;

        float radius = rangeSpawner / 2;
        float radiusMin = rangeSpawner_Min / 2;
        List<GameObject> spawnerTemp = new List<GameObject>();
        for (int i = 0; i < quantitySpawner; i++)
        {
            RaycastHit hit;
            Vector3 positionVariant = FindCorruptSpawnerPosition(radiusMin, radius);

            if (Physics.Raycast(this.transform.position + new Vector3(0, 150, 0) + positionVariant, -Vector3.up, out hit, 250, groundLayer))
            {
                GameObject lastSpawnerCreated = Instantiate(spawnerPrefab, hit.point, Quaternion.identity, this.transform);
                SpawnerBehavior lastSpawnerBehavior = lastSpawnerCreated.GetComponent<SpawnerBehavior>();
                lastSpawnerBehavior.m_roomManager = this;
                lastSpawnerBehavior.m_enemyManager = m_enemyManager;
                lastSpawnerBehavior.m_healthSystem = lastSpawnerCreated.GetComponent<ObjectHealthSystem>();


                spawnerTemp.Add(lastSpawnerCreated);

            }
        }
        spawnerList = new GameObject[spawnerTemp.Count];
        for (int j = 0; j < spawnerTemp.Count; j++)
        {
            spawnerList[j] = spawnerTemp[j];
        }
        int[] dataObjectifOptional = { 0, spawnerList.Length };
        roomInfoUI.UpdateRoomInfoDisplay(null, dataObjectifOptional);
        m_enemyManager.GetDataSpawner(spawnerList);
    }

    public void SetupSpawnGate(Material materialPreview)
    {
        teleporterFeedback.previewMeshPlane.material = materialPreview;
    }

    public Vector3 FindCorruptSpawnerPosition(float rangeMin, float rangeMax)
    {
        Vector3 position = Vector3.zero;
        int randomPosition = UnityEngine.Random.Range(0, 4);
        if (randomPosition == 0)
        {
            position = new Vector3(UnityEngine.Random.Range(rangeMin, rangeMax), 0, UnityEngine.Random.Range(rangeMin, rangeMax));
        }
        else if (randomPosition == 1)
        {
            position = new Vector3(-UnityEngine.Random.Range(rangeMin, rangeMax), 0, UnityEngine.Random.Range(rangeMin, rangeMax));
        }
        else if (randomPosition == 2)
        {
            position = new Vector3(UnityEngine.Random.Range(rangeMin, rangeMax), 0, -UnityEngine.Random.Range(rangeMin, rangeMax));
        }
        else if (randomPosition == 3)
        {
            position = new Vector3(-UnityEngine.Random.Range(rangeMin, rangeMax), 0, -UnityEngine.Random.Range(rangeMin, rangeMax));
        }
        return position;
    }
    #endregion

    #region Event Functions

    public void ActivateEvents()
    {
        for (int i = 0; i < altarBehaviorComponents.Length; i++)
        {
            altarBehaviorComponents[i].gameObject.SetActive(true);
            altarBehaviorComponents[i].isFastEvent = activeFastRoom;
            altarBehaviorComponents[i].LaunchInit();
        }


    }

    private void DeactiveEvents()
    {
        for (int i = 0; i < altarBehaviorComponents.Length; i++)
        {
            altarBehaviorComponents[i].gameObject.SetActive(false);
        }
    }

    private void ActiveNightRoom()
    {
        if (RunManager.instance.dayStep != DayStep.NIGHT) return;
        for (int i = 0; i < altarBehaviorComponents.Length; i++)
        {
            m_enemyManager.SpawnChampion(altarBehaviorComponents[i].transform.position);
            m_currentChampionAlive++;
        }

        m_enemyManager.OnChampionDeathEvent += CountChampionEliminate;
    }
    public void ResetEvents()
    {
        if (activeRoomManagerDebug)
            ScreenDebuggerTool.AddMessage("Event Reset");

        // Close portal 
        for (int i = 0; i < m_currentTeleporterCount; i++)
        {
            teleporterArray[i].DesactivationTeleportor();
        }
        isTeleporterActive = false;
        isRoomHasBeenValidate = false;

        // Remove all map generate
        terrainGenerator.ClearMapGenerate();
        currentRoomType = RoomType.Event;

        eventActive = altarBehaviorComponents.Length;
        int[] dataObjectif =
        {
            (maxEventActive - eventActive),
            maxEventActive
        };
        roomInfoUI.UpdateRoomInfoDisplay(dataObjectif, null);
        //roomInfoUI.ActualizeRoomInfoInterface();
        //roomInfoUI.DeactivateMajorGoalInterface();

        for (int i = 0; i < altarBehaviorComponents.Length; i++)
        {
            altarBehaviorComponents[i].ResetAltarEvent();
        }
    }

    #endregion

}
