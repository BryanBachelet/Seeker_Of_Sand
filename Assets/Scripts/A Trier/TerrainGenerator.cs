using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using UnityEditor.EditorTools;
using UnityEngine.Rendering;
using BorsalinoTools;

public class TerrainGenerator : MonoBehaviour
{
    public GlobalSoundManager gsm;

    private const int maxPlayerSpell = 4;
    private const int frameCountBeforeDeactivation = 3;
    public Transform lastMapPlay;


    public RewardType rewardFirstRoom;
    public GameObject teleporterPrefab;
    public LayerMask groundLayer;
    public List<GameObject> terrainPool = new List<GameObject>();
    public GameObject BossTerrain; // Boss Terrain
    public List<GameObject> mapInstantiated = new List<GameObject>();
    public List<GameObject> previousMapList = new List<GameObject>();
    public List<GameObject> oldTerrain = new List<GameObject>();
    public int mapPoolCount;
    public int countRoomGeneration = 1;
    public static int roomGeneration_Static = 1;

    public bool generateNewTerrain;

    public int selectedTerrain = 0;

    public bool hasMerchantAppear;

    public GameObject player;
    private TeleporterBehavior playerTeleportorBehavior;

    public CameraFadeFunction cameraFadeFunction;
    private int lastMapIndexSelected = 0;

    [HideInInspector] public RoomManager currentRoomManager;
    public static RoomManager s_currentRoomManager;

    public RoomInfoUI roomInfoUI;
    public DayCyclecontroller dayController;
    public DayTimeController dayTimeController;
    private int lastNightCount = -1;

    private List<RewardType> rewardList = new List<RewardType>();
    private List<RoomType> roomTypeList = new List<RoomType>();

    public TMPro.TMP_Text roomGeneration_text;

    private bool isHealthBossRoom;
    public MiniMapControl miniMapControl;
    public MiniMapControl miniMap_IconControl;
    public Texture lastTextureCreated;

    [Header("Room Rewards variable")]
    [SerializeField] private float m_upgradeRewardPercent = 50;
    [SerializeField] private float m_artefactRewardPercent = 50;

    [Header("Debug Parameter")]
    public bool isOnlyBoss;
    [SerializeField] private bool m_activeRoomMangerDebug;
    [SerializeField] private bool m_activateTerrainGeneratorDebug;

    public void Start()
    {
        dayController.dayStartEvent += ResetRoomAtNewDay;
        dayTimeController.dayStartEvent += ResetRoomAtNewDay;
    }

    public void ResetRoomAtNewDay()
    {
        if (!roomTypeList.Contains(RoomType.Merchant))
        {
            if ((int)RoomType.Merchant < roomTypeList.Count)
            {
                roomTypeList.Insert((int)RoomType.Merchant, RoomType.Merchant);
            }
            else
            {
                roomTypeList.Add(RoomType.Merchant);
            }
        }
    }

    public void LaunchRoomGenerator()
    {
        InitRoomsDataList();

        // Retrive components
        currentRoomManager = lastMapPlay.GetComponentInChildren<RoomManager>();
        playerTeleportorBehavior = player.GetComponent<TeleporterBehavior>();
        if (cameraFadeFunction == null)
            cameraFadeFunction = Camera.main.GetComponent<CameraFadeFunction>();

        // Setup Variables at launch
        s_currentRoomManager = currentRoomManager;
        mapPoolCount = terrainPool.Count;
        previousMapList = mapInstantiated; // To verify

        currentRoomManager.InitComponent();


        SetupFirstRoom();

        GuerhoubaGames.SaveData.GameData.UpdateRunCount();

    }


    // Setup the possible room type and the reward possible
    public void InitRoomsDataList()
    {
        int sizeEnum = System.Enum.GetValues(typeof(RewardType)).Length;
        for (int i = 0; i < sizeEnum; i++)
        {
            rewardList.Add((RewardType)i);
        }
        roomTypeList.Add(RoomType.Event);
    }

    public void SetupFirstRoom()
    {
        currentRoomManager = lastMapPlay.GetComponentInChildren<RoomManager>();
        s_currentRoomManager = currentRoomManager;
        currentRoomManager.InitComponent();
        currentRoomManager.currentRoomType = RoomType.Free;
        currentRoomManager.rewardType = rewardFirstRoom;
        currentRoomManager.isRoomHasBeenDeactivated = true;
        currentRoomManager.hasEndReward = true;
        currentRoomManager.terrainGenerator = this;
        currentRoomManager.terrainIndex = -1;

        currentRoomManager.roomInfoUI = roomInfoUI;
        roomInfoUI.currentRoomManager = currentRoomManager;
        currentRoomManager.ActivateRoom(currentRoomManager.m_materialPreviewTRT);
        roomInfoUI.ActualizeRoomInfoInterface();


    }

    public void GenerateMap()
    {
        GenerateTerrain();
        AssociateNewReward();
    }


    public void GenerateBossMap()
    {
        int postionOffsetMap = TerrainGenerator.roomGeneration_Static % 2;
        int positionNewTerrain = 3000 * postionOffsetMap + mapInstantiated.Count;

        GameObject newTerrain;
        int randomTerrain = Random.Range(1, mapPoolCount);
        newTerrain = Instantiate(BossTerrain, transform.position + new Vector3(positionNewTerrain, 500, 1500 * 0), transform.rotation);

        mapInstantiated.Add(newTerrain);

        RoomManager roomManager = newTerrain.GetComponentInChildren<RoomManager>();
        roomManager.InitComponent();
        roomManager.terrainGenerator = this;
        roomManager.currentRoomType = RoomType.Boss;
        roomManager.rewardType = RewardType.SPELL;
        roomManager.healthReward = HealthReward.FULL;
        roomManager.isTimingPassing = false;
        roomManager.roomInfoUI = roomInfoUI;
        if (m_activeRoomMangerDebug)
        {
            roomManager.activeRoomManagerDebug = true;
        }

        GuerhoubaTools.LogSystem.LogMsg("New room with the type " + roomManager.currentRoomType.ToString() + " and the reward is " + roomManager.rewardType.ToString());
        isHealthBossRoom = true;
        roomManager.m_CRT.Initialize();
        roomManager.m_CRT.Update();
        StartCoroutine(roomManager.RoomDeactivation(3));

    }

    public void GenerateTerrain()
    {
        // Clear old map
        oldTerrain.Clear();
        oldTerrain.AddRange(previousMapList);

        previousMapList = mapInstantiated;
        mapInstantiated.Clear();

        int mapCountToGenerate = Random.Range(1, 4);

        // Setup next position of the new terrain
        int postionOffsetMap = TerrainGenerator.roomGeneration_Static % 2;
        int offsetXNewMaps = 3000 * postionOffsetMap + mapInstantiated.Count;


        List<GameObject> mapList = terrainPool;
        if (currentRoomManager.terrainIndex != -1 && currentRoomManager.terrainIndex < mapList.Count) 
            mapList.RemoveAt(currentRoomManager.terrainIndex);

        for (int i = 0; i < mapCountToGenerate; i++)
        {
            int idMap = -1;
            // Get new random map index 

            int randomTerrain = Random.Range(0, mapList.Count);

            
            GameObject mapInstance = Instantiate(mapList[randomTerrain], transform.position + new Vector3(offsetXNewMaps, 500, 1500 * i), transform.rotation);
            idMap = terrainPool.IndexOf(mapList[randomTerrain]);
            
            mapList.RemoveAt(randomTerrain);
            mapInstantiated.Add(mapInstance);

            RoomManager roomManager = mapInstance.GetComponentInChildren<RoomManager>();
            roomManager.InitComponent();
            roomManager.terrainGenerator = this;
            roomManager.currentRoomType = RoomType.Event;
            roomManager.healthReward = HealthReward.QUARTER;
            roomManager.terrainIndex = idMap;
            roomManager.hasEndReward = false;
            roomManager.isTimingPassing = true;

            // Setup the preview texture
            roomManager.previewCamera.gameObject.SetActive(true);
            RenderTexture custom_TRT = roomManager.previewCamera.targetTexture;
            roomManager.m_CRT.Initialize();
            roomManager.m_CRT.Update();

            if(m_activateTerrainGeneratorDebug)
            {
                ScreenDebuggerTool.AddMessage("Next map  : " + mapInstance.name,i,6);
            }

            GuerhoubaTools.LogSystem.LogMsg("New room with the type " + roomManager.currentRoomType.ToString() + "with map : " + mapInstance.name);

            StartCoroutine(roomManager.RoomDeactivation(frameCountBeforeDeactivation));
        }

        countRoomGeneration++;
        roomGeneration_Static = countRoomGeneration;
    }

    private int[] GenerateRoomReward()
    {
        int[] rewardAssociated = new int[3];
        for (int i = 0; i < rewardAssociated.Length; i++)
        {
            int drawPercent = UnityEngine.Random.Range(0, 100);
            if (drawPercent < m_upgradeRewardPercent)
            {
                rewardAssociated[i] = (int)RewardType.UPGRADE;
                continue;
            }
            if (drawPercent > m_artefactRewardPercent)
            {
                rewardAssociated[i] = (int)RewardType.ARTEFACT;
            }

        }
        return rewardAssociated;
    }

    public void AssociateNewReward()
    {

        for (int i = 0; i < mapInstantiated.Count; i++)
        {
            currentRoomManager.teleporterArray[i].TeleporterNumber = i;

            RoomManager roomManagerNextMap = mapInstantiated[i].GetComponentInChildren<RoomManager>();
            roomManagerNextMap.rewardAssociated = GenerateRoomReward();

            TeleporterFeebackController tpFeedback = currentRoomManager.teleporterArray[i].GetComponentInChildren<TeleporterFeebackController>();
            tpFeedback.rewardToUse = (int)roomManagerNextMap.rewardType;
            tpFeedback.eventReward = roomManagerNextMap.rewardAssociated;
            tpFeedback.ChangeRewardID(tpFeedback.rewardToUse, roomManagerNextMap.m_materialPreviewTRT);
            tpFeedback.ChangeColorVFX(GeneralTools.GetElementalArrayIndex(roomManagerNextMap.element, true));
        }


    }

    public void SelectTerrain(int selectedMapIndex)
    {
        // Deactive previous map
        RoomManager previousRoomManager = currentRoomManager; ;
        currentRoomManager.DeactivateRoom();


        //Set new current map 
        lastMapPlay = currentRoomManager.transform.parent.transform;
        currentRoomManager = mapInstantiated[selectedMapIndex].GetComponentInChildren<RoomManager>();
        s_currentRoomManager = currentRoomManager;

        currentRoomManager.roomInfoUI = roomInfoUI;
        roomInfoUI.currentRoomManager = currentRoomManager;

        lastMapIndexSelected = selectedMapIndex;
        mapInstantiated[selectedMapIndex].SetActive(true);

        Teleporter teleportorAssociated = mapInstantiated[selectedMapIndex].transform.GetComponentInChildren<Teleporter>();
        playerTeleportorBehavior.GetTeleportorData(teleportorAssociated);
        RoomManager roomManager = mapInstantiated[selectedMapIndex].GetComponentInChildren<RoomManager>();


        playerTeleportorBehavior.nextTerrainNumber = selectedMapIndex;
        roomManager.portalPrevious.material = currentRoomManager.m_materialPreviewTRT;
        cameraFadeFunction.LaunchFadeIn(true, 0.5f);
        cameraFadeFunction.tpBehavior.disparitionVFX.Play();
        cameraFadeFunction.tpBehavior.isTimePassing = roomManager.isTimingPassing;
        cameraFadeFunction.tpBehavior.specialRoomID = roomManager.specialID;

        ////dayController.UpdateTimeByStep();
        //roomGeneration_text.text = "Room " + TerrainGenerator.roomGeneration_Static;

        miniMapControl.ResetDiscovery(roomManager.miniMapCameraPosition.gameObject);
        miniMap_IconControl.ResetDiscovery(roomManager.miniMapCameraPosition.gameObject);

        currentRoomManager.ActivateRoom(previousRoomManager.m_materialPreviewTRT);
        roomInfoUI.ActualizeRoomInfoInterface();

        if (roomManager.currentRoomType == RoomType.Boss) { gsm.UpdateParameter(1, "BossAmbiant"); }
        else { gsm.UpdateParameter(0, "BossAmbiant"); }

        if (m_activateTerrainGeneratorDebug)
        {
            ScreenDebuggerTool.AddMessage(" map Selected : " + mapInstantiated[selectedMapIndex].name);
        }

    }


    public void GetTexturePreviousMap(Texture texture)
    {
        lastTextureCreated = texture;
    }


    public void DestroyPreviousTerrain()
    {
        for (int i = 0; i < previousMapList.Count; i++)
        {
            Destroy(previousMapList[i]);
        }
    }

}
