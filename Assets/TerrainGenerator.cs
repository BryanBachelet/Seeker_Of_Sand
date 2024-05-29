using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

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
    static public int generation = 1;

    public bool generateNewTerrain;

    public int selectedTerrain = 0;

    public GameObject player;
    private TeleporterBehavior playerTeleportorBehavior;
    Transform transformReference;

    public CameraFadeFunction cameraFadeFunction;
    private int lastTerrainSelected = 0;

    private RoomManager currentRoomManager;

    public ObjectifAndReward_Ui_Function objAndReward;
    public DayCyclecontroller dayController;

    private List<RewardType> rewardList = new List<RewardType>();

    public TMPro.TMP_Text roomGeneration_text;
    // Start is called before the first frame update
    void Start()
    {
        InitValue();
        poolNumber = terrainPool.Count;
        transformReference = lastTerrainPlay;
        previousTerrain = terrainInstantiated;
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        GenerateTerrain(0);
        AssociateNewReward(0);
        SetupFirstRoom();
        playerTeleportorBehavior = player.GetComponent<TeleporterBehavior>();
        if (cameraFadeFunction == null) { cameraFadeFunction = Camera.main.GetComponent<CameraFadeFunction>(); }
    }


    public void InitValue()
    {
        for (int i = 0; i < 4; i++)
        {
            rewardList.Add((RewardType)i);
        }
    }

    public void SetupFirstRoom()
    {
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();
        currentRoomManager.RetriveComponent();
        currentRoomManager.roomType = RoomType.Free;
        currentRoomManager.rewardType = RewardType.SPELL;

        for (int i = 0; i < teleporter.Count; i++)
        {
            currentRoomManager.AddTeleporter(teleporter[i]);
        }

        currentRoomManager.ActivateRoom();
        objAndReward.currentRoomManager = currentRoomManager;
        objAndReward.UpdateObjectifAndReward();

    }
    public void GenerateTerrain(int selectedTerrainNumber)
    {
        oldTerrain.Clear();
        oldTerrain.AddRange(previousTerrain);
        previousTerrain = terrainInstantiated;
        terrainInstantiated.Clear();
        int randomNextTerrainNumber = Random.Range(1, 4);
        int positionNewTerrain = 1500 * generation + terrainInstantiated.Count;

        for (int i = 0; i < randomNextTerrainNumber; i++)
        {
            int randomTerrain = Random.Range(0, poolNumber);
            GameObject newTerrain = Instantiate(terrainPool[randomTerrain], transform.position + new Vector3(positionNewTerrain, 500, 1500 * i), transform.rotation);
            terrainInstantiated.Add(newTerrain);

            RoomManager roomManager = newTerrain.GetComponentInChildren<RoomManager>();
            roomManager.RetriveComponent();
            roomManager.roomType = (RoomType)Random.Range(0, 3);

            int indexReward = Random.Range(0, rewardList.Count);
            roomManager.rewardType = rewardList[indexReward];


        }
        //AssociateNewReward(selectedTerrainNumber);
        generation++;
    }

    public void AssociateNewReward(int selectedTerrainNumber)
    {
        Debug.Log("NextTerrainSelected : " + lastTerrainSelected);
        int terrainSelected = selectedTerrainNumber;
        teleporter.Clear();

        for (int i = 0; i < terrainInstantiated.Count; i++)
        {
            //Transform transformReference = lastTerrainPlay;
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transformReference.position + new Vector3(0, 500, 0), transformReference.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundLayer))
            {
                Debug.DrawRay(transformReference.position + new Vector3(0, 500, 0), transformReference.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                GameObject newTp = Instantiate(teleporterPrefab, hit.point + new Vector3(50 * i, 5, 0), transform.rotation, transformReference);
                Teleporter tpScript = newTp.GetComponent<Teleporter>();
                TeleporterFeebackController tpFeedback = tpScript.GetComponentInChildren<TeleporterFeebackController>();
                RoomManager roomManager = terrainInstantiated[i].GetComponentInChildren<RoomManager>();
                tpFeedback.rewardToUse = (int)roomManager.rewardType;
                tpFeedback.ChangeRewardID(tpFeedback.rewardToUse);
                tpScript.TeleporterNumber = i;
                teleporter.Add(tpScript);
                if (generation > 1)
                {
                    currentRoomManager.AddTeleporter(tpScript);
                }


            }

        }


    }

    public void SelectTerrain(int selectedTerrain)
    {
        Teleporter teleportorAssociated = null;
        lastTerrainSelected = selectedTerrain;
        teleportorAssociated = terrainInstantiated[selectedTerrain].transform.GetChild(0).GetComponent<Teleporter>();
        transformReference = terrainInstantiated[selectedTerrain].transform;
        playerTeleportorBehavior.GetTeleportorData(teleportorAssociated);
        playerTeleportorBehavior.nextTerrainNumber = selectedTerrain;
        cameraFadeFunction.fadeInActivation = true;
        cameraFadeFunction.tpBehavior.disparitionVFX.Play();
        //dayController.UpdateTimeByStep();
        roomGeneration_text.text = "Room " + generation;

    }

    public void ActiveGenerationTerrain(int selectedTerrainNumber)
    {
        Character.CharacterShoot shootComponent = player.GetComponent<Character.CharacterShoot>();
        if(shootComponent.capsuleIndex.Count >= maxPlayerSpell && rewardList.Contains(RewardType.SPELL))
        {
            rewardList.Remove(RewardType.SPELL);
        }

        for (int i = 0; i < oldTerrain.Count; i++)
        {
            oldTerrain[i].GetComponent <NavMeshSurface>().RemoveData();
            oldTerrain[i].GetComponent <NavMeshSurface>().enabled =false;
            oldTerrain[i].SetActive(false); 
        }

        selectedTerrain = selectedTerrainNumber;
        lastTerrainPlay = previousTerrain[selectedTerrain].transform;
        currentRoomManager = lastTerrainPlay.GetComponentInChildren<RoomManager>();




        GenerateTerrain(selectedTerrainNumber);
        AssociateNewReward(selectedTerrainNumber);
        currentRoomManager.ActivateRoom();
        objAndReward.currentRoomManager = currentRoomManager;
        objAndReward.UpdateObjectifAndReward();
      

    }

    public void DestroyPreviousTerrain()
    {
        for (int i = 0; i < previousTerrain.Count; i++)
        {
            Destroy(previousTerrain[i]);
        }
    }

}
