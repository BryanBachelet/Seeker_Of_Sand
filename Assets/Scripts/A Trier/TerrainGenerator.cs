using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;

public class TerrainGenerator : MonoBehaviour
{
    private const int maxPlayerSpell = 4;
    public Transform lastTerrainPlay;

    public GameObject teleporterPrefab;
    public LayerMask groundLayer;
    public List<GameObject> terrainPool = new List<GameObject>();
    public List<GameObject> terrainInstantiated = new List<GameObject>();
    public List<GameObject> previousTerrain = new List<GameObject>();
    public List<GameObject> oldTerrain = new List<GameObject>();
    public List<Teleporter> teleporter = new List<Teleporter>();
    public int poolNumber;
    public int countRoomGeneration = 1;
    public static int roomGeneration_Static = 1;

    public bool generateNewTerrain;

    public int selectedTerrain = 0;

    public bool hasMerchantAppear;

    public GameObject player;
    private TeleporterBehavior playerTeleportorBehavior;
    Transform transformReference;

    public CameraFadeFunction cameraFadeFunction;
    private int lastTerrainSelected = 0;

    private RoomManager currentRoomManager;
    public static RoomManager staticRoomManager;

    public RoomInfoUI roomInfoUI;
    public DayCyclecontroller dayController;

    private List<RewardType> rewardList = new List<RewardType>();
    private List<RoomType> roomTypeList = new List<RoomType>();

    public TMPro.TMP_Text roomGeneration_text;

    private bool isHealthBossRoom;

    public void Start()
    {
        dayController.dayStartEvent += ResetRoomAtNewDay;
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
        InitRoomDataList();
        poolNumber = terrainPool.Count;
        transformReference = lastTerrainPlay;
        previousTerrain = terrainInstantiated;
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        staticRoomManager = currentRoomManager;
        currentRoomManager.RetriveComponent();
        GenerateTerrain(0);
        AssociateNewReward(0);
        SetupFirstRoom();
        GuerhoubaGames.SaveData.GameData.UpdateRunCount();
        playerTeleportorBehavior = player.GetComponent<TeleporterBehavior>();
        if (cameraFadeFunction == null) { cameraFadeFunction = Camera.main.GetComponent<CameraFadeFunction>(); }
    }


    public void InitRoomDataList()
    {
        int size = System.Enum.GetValues(typeof(RewardType)).Length;
        for (int i = 0; i < size; i++)
        {
            rewardList.Add((RewardType)i);
        }

        size = System.Enum.GetValues(typeof(RoomType)).Length;

        for (int i = 0; i < size; i++)
        {
            roomTypeList.Add((RoomType)i);
        }

        roomTypeList.Remove(RoomType.Free);
    }

    public void SetupFirstRoom()
    {
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        staticRoomManager = currentRoomManager;
        currentRoomManager.RetriveComponent();
        currentRoomManager.currentRoomType = RoomType.Free;
        currentRoomManager.rewardType = RewardType.SPELL;
        currentRoomManager.isRoomHasBeenDeactivated = true;

        currentRoomManager.roomInfoUI = roomInfoUI;
        roomInfoUI.currentRoomManager = currentRoomManager;
        currentRoomManager.ActivateRoom();
        roomInfoUI.ActualizeRoomInfoInterface();

    }
    public void GenerateTerrain(int selectedTerrainNumber)
    {
        oldTerrain.Clear();
        oldTerrain.AddRange(previousTerrain);
        previousTerrain = terrainInstantiated;
        terrainInstantiated.Clear();
        int randomNextTerrainNumber = Random.Range(1, 4);
        int positionNewTerrain = 1500 * TerrainGenerator.roomGeneration_Static + terrainInstantiated.Count;

        GuerhoubaTools.LogSystem.LogMsg("The next hour is " + dayController.GetNextHour().ToString());


        
        if (dayController.IsNextRoomIsDay() && !isHealthBossRoom)
        {
            GameObject newTerrain;
            int randomTerrain = Random.Range(1, poolNumber);
            newTerrain = Instantiate(terrainPool[randomTerrain], transform.position + new Vector3(positionNewTerrain, 500, 1500 * 0), transform.rotation);

            terrainInstantiated.Add(newTerrain);

            RoomManager roomManager = newTerrain.GetComponentInChildren<RoomManager>();
            roomManager.RetriveComponent();
            roomManager.terrainGenerator = this;
            roomManager.currentRoomType = RoomType.Free;
            roomManager.rewardType = RewardType.HEAL;
            roomManager.healthReward = HealthReward.FULL;
            roomManager.isTimingPassing = false;

            GuerhoubaTools.LogSystem.LogMsg("New room with the type " + roomManager.currentRoomType.ToString() + " and the reward is " + roomManager.rewardType.ToString());
            isHealthBossRoom = true;
            roomManager.m_CRT.Update();
            StartCoroutine(roomManager.RoomDeactivation(3));
            return;
        }

        for (int i = 0; i < randomNextTerrainNumber; i++)
        {

            int indexRoomType = 0;
            if (roomGeneration_Static < 6) { indexRoomType = Random.Range(0, 2); }
            else { indexRoomType = Random.Range(0, roomTypeList.Count); }
            GameObject newTerrain;
            isHealthBossRoom = false;
            if (roomTypeList[indexRoomType] == RoomType.Merchant)
            {
                newTerrain = Instantiate(terrainPool[0], transform.position + new Vector3(positionNewTerrain, 500, 1500 * i), transform.rotation);

            }
            else
            {
                int randomTerrain = Random.Range(1, poolNumber);
                newTerrain = Instantiate(terrainPool[randomTerrain], transform.position + new Vector3(positionNewTerrain, 500, 1500 * i), transform.rotation);
            }

            terrainInstantiated.Add(newTerrain);

            RoomManager roomManager = newTerrain.GetComponentInChildren<RoomManager>();
            roomManager.RetriveComponent();
            roomManager.terrainGenerator = this;
            roomManager.currentRoomType = roomTypeList[indexRoomType];
            roomManager.healthReward = HealthReward.QUARTER;

            if (roomTypeList[indexRoomType] == RoomType.Merchant)
            {
                roomManager.rewardType = RewardType.MERCHANT;
                roomManager.isTimingPassing = false;
                roomTypeList.Remove(roomTypeList[indexRoomType]);
            }
            else
            {
                int indexReward = 0;
                if (i > 0)
                {
                    indexReward = Random.Range(0, rewardList.Count - 1);

                }
                else
                {
                    indexReward = Random.Range(0, rewardList.Count - 2);
                }

                roomManager.rewardType = rewardList[indexReward];
                roomManager.isTimingPassing = true;
            }


            GuerhoubaTools.LogSystem.LogMsg("New room with the type " + roomManager.currentRoomType.ToString() + " and the reward is " + roomManager.rewardType.ToString());
            RenderTexture custom_TRT = roomManager.previewCamera.targetTexture;
            roomManager.m_CRT.Update();
            StartCoroutine(roomManager.RoomDeactivation(3));
            //newTerrain.SetActive(false);
        }
        
  //      AssociateNewReward(selectedTerrainNumber);
        countRoomGeneration++;
        roomGeneration_Static = countRoomGeneration;
    }

    public void AssociateNewReward(int selectedTerrainNumber)
    {
        Debug.Log("NextTerrainSelected : " + lastTerrainSelected);
        int terrainSelected = selectedTerrainNumber;
        teleporter.Clear();

        for (int i = 0; i < terrainInstantiated.Count; i++)
        {

            currentRoomManager.teleporterArray[i].TeleporterNumber = i;
            RoomManager roomManager = terrainInstantiated[i].GetComponentInChildren<RoomManager>();
            TeleporterFeebackController tpFeedback = currentRoomManager.teleporterArray[i].GetComponentInChildren<TeleporterFeebackController>();
            tpFeedback.rewardToUse = (int)roomManager.rewardType;
            tpFeedback.ChangeRewardID(tpFeedback.rewardToUse, roomManager.m_materialPreviewTRT);
            tpFeedback.ChangeColorVFX(roomManager.element);
        }

        currentRoomManager.SetupTeleporter(terrainInstantiated.Count);


    }

    public void SelectTerrain(int selectedTerrain)
    {
        Teleporter teleportorAssociated = null;
        lastTerrainSelected = selectedTerrain;
        teleportorAssociated = terrainInstantiated[selectedTerrain].transform.GetComponentInChildren<Teleporter>();
        transformReference = terrainInstantiated[selectedTerrain].transform;
        terrainInstantiated[selectedTerrain].SetActive(true);
        playerTeleportorBehavior.GetTeleportorData(teleportorAssociated);
        RoomManager roomManager = terrainInstantiated[selectedTerrain].GetComponentInChildren<RoomManager>();
        playerTeleportorBehavior.nextTerrainNumber = selectedTerrain;
        cameraFadeFunction.fadeInActivation = true;
        cameraFadeFunction.tpBehavior.disparitionVFX.Play();
        cameraFadeFunction.tpBehavior.isTimePassing = roomManager.isTimingPassing;
        //dayController.UpdateTimeByStep();
        roomGeneration_text.text = "Room " + TerrainGenerator.roomGeneration_Static;

    }

    public void ActiveGenerationTerrain(int selectedTerrainNumber)
    {
        Character.CharacterShoot shootComponent = player.GetComponent<Character.CharacterShoot>();
        if (shootComponent.spellIndexGeneral.Count >= maxPlayerSpell && rewardList.Contains(RewardType.SPELL))
        {
            rewardList.Remove(RewardType.SPELL);
        }

        for (int i = 0; i < oldTerrain.Count; i++)
        {
            NavMeshSurface navSurf = oldTerrain[i].GetComponent<NavMeshSurface>();
            if (navSurf != null)
            {
                navSurf.RemoveData();
                navSurf.enabled = false;
            }

            oldTerrain[i].SetActive(false);
        }



        selectedTerrain = selectedTerrainNumber;
        lastTerrainPlay = previousTerrain[selectedTerrain].transform;
        currentRoomManager.DeactivateRoom();
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        staticRoomManager = currentRoomManager;

        GenerateTerrain(selectedTerrainNumber);
        AssociateNewReward(selectedTerrainNumber);

        currentRoomManager.roomInfoUI = roomInfoUI;
        roomInfoUI.currentRoomManager = currentRoomManager;
        currentRoomManager.ActivateRoom();
        roomInfoUI.ActualizeRoomInfoInterface();


    }

    public void DestroyPreviousTerrain()
    {
        for (int i = 0; i < previousTerrain.Count; i++)
        {
            Destroy(previousTerrain[i]);
        }
    }

}
