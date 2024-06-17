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

    public bool generateNewTerrain;

    public int selectedTerrain = 0;

    public GameObject player;
    private TeleporterBehavior playerTeleportorBehavior;
    Transform transformReference;

    public CameraFadeFunction cameraFadeFunction;
    private int lastTerrainSelected = 0;

    private RoomManager currentRoomManager;

    public RoomInfoUI roomInfoUI;
    public DayCyclecontroller dayController;

    private List<RewardType> rewardList = new List<RewardType>();
    private List<RoomType> roomTypeList = new List<RoomType>();

    public TMPro.TMP_Text roomGeneration_text;



    public void LaunchRoomGenerator()
    {
        InitRoomDataList();
        poolNumber = terrainPool.Count;
        transformReference = lastTerrainPlay;
        previousTerrain = terrainInstantiated;
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        currentRoomManager.RetriveComponent();
        GenerateTerrain(0);
        AssociateNewReward(0);
        SetupFirstRoom();
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
    }

    public void SetupFirstRoom()
    {
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        currentRoomManager.RetriveComponent();
        currentRoomManager.currentRoomType = RoomType.Free;
        currentRoomManager.rewardType = RewardType.SPELL;


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
        int positionNewTerrain = 1500 * countRoomGeneration + terrainInstantiated.Count;

        if (currentRoomManager.currentRoomType == RoomType.Free)
            roomTypeList.Remove(currentRoomManager.currentRoomType);
        for (int i = 0; i < randomNextTerrainNumber; i++)
        {
            int randomTerrain = Random.Range(0, poolNumber);
            GameObject newTerrain = Instantiate(terrainPool[randomTerrain], transform.position + new Vector3(positionNewTerrain, 500, 1500 * i), transform.rotation);
            terrainInstantiated.Add(newTerrain);

            RoomManager roomManager = newTerrain.GetComponentInChildren<RoomManager>();
            roomManager.RetriveComponent();
            roomManager.terrainGenerator = this;

            int indexRoomType = 0;
            indexRoomType = Random.Range(0, roomTypeList.Count);
            indexRoomType = 3; // Temp
            roomManager.currentRoomType = roomTypeList[indexRoomType];

            if (indexRoomType == 3)
            {
                roomManager.rewardType = rewardList[4];
            }
            else
            {
                int indexReward = 0;
                if (i > 0)
                {
                    indexReward = Random.Range(0, rewardList.Count);
                }
                else
                {
                    indexReward = Random.Range(0, rewardList.Count - 1);
                }

                roomManager.rewardType = rewardList[indexReward];
            }
        }

        if (currentRoomManager.currentRoomType == RoomType.Free)
            roomTypeList.Add(currentRoomManager.currentRoomType);
        //AssociateNewReward(selectedTerrainNumber);
        countRoomGeneration++;
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
            tpFeedback.ChangeRewardID(tpFeedback.rewardToUse);
        }

        currentRoomManager.SetupTeleporter(terrainInstantiated.Count);


    }

    public void SelectTerrain(int selectedTerrain)
    {
        Teleporter teleportorAssociated = null;
        lastTerrainSelected = selectedTerrain;
        teleportorAssociated = terrainInstantiated[selectedTerrain].transform.GetComponentInChildren<Teleporter>();
        transformReference = terrainInstantiated[selectedTerrain].transform;
        playerTeleportorBehavior.GetTeleportorData(teleportorAssociated);
        playerTeleportorBehavior.nextTerrainNumber = selectedTerrain;
        cameraFadeFunction.fadeInActivation = true;
        cameraFadeFunction.tpBehavior.disparitionVFX.Play();
        //dayController.UpdateTimeByStep();
        roomGeneration_text.text = "Room " + countRoomGeneration;

    }

    public void ActiveGenerationTerrain(int selectedTerrainNumber)
    {
        Character.CharacterShoot shootComponent = player.GetComponent<Character.CharacterShoot>();
        if (shootComponent.capsuleIndex.Count >= maxPlayerSpell && rewardList.Contains(RewardType.SPELL))
        {
            rewardList.Remove(RewardType.SPELL);
        }

        for (int i = 0; i < oldTerrain.Count; i++)
        {
            oldTerrain[i].GetComponent<NavMeshSurface>().RemoveData();
            oldTerrain[i].GetComponent<NavMeshSurface>().enabled = false;
            oldTerrain[i].SetActive(false);
        }

        selectedTerrain = selectedTerrainNumber;
        lastTerrainPlay = previousTerrain[selectedTerrain].transform;
        currentRoomManager.DeactivateAltar();
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();


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
