using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Transform lastTerrainPlay;

    public GameObject teleporterPrefab;
    public LayerMask groundLayer;
    public List<GameObject> terrainPool = new List<GameObject>();
    public List<GameObject> terrainInstantiated = new List<GameObject>();
    public List<GameObject> previousTerrain = new List<GameObject>();
    public List<Teleportor> teleporter = new List<Teleportor>();
    public int poolNumber;
    public int generation = 1;

    public bool generateNewTerrain;

    public int selectedTerrain = 0;

    public GameObject player;
    private teleporterBehavior playerTeleportorBehavior;
    Transform transformReference;

    public CameraFadeFunction cameraFadeFunction;
    private int lastTerrainSelected = 0;
    // Start is called before the first frame update
    void Start()
    {
        poolNumber = terrainPool.Count;
        transformReference = lastTerrainPlay;
        previousTerrain = terrainInstantiated;
        ActiveGenerationTerrain(0);
        AssociateNewReward(selectedTerrain);
        playerTeleportorBehavior = player.GetComponent<teleporterBehavior>();
        if(cameraFadeFunction == null) { cameraFadeFunction = Camera.main.GetComponent<CameraFadeFunction>(); }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GenerateTerrain(int selectedTerrainNumber)
    {
        previousTerrain = terrainInstantiated;
        terrainInstantiated.Clear();
        int randomNextTerrainNumber = Random.Range(1, 4);
        int positionNewTerrain = 1500 * generation + terrainInstantiated.Count;
        for(int i = 0; i < randomNextTerrainNumber; i++)
        {
            int randomTerrain = Random.Range(0, poolNumber);
            GameObject newTerrain = Instantiate(terrainPool[randomTerrain], transform.position + new Vector3(positionNewTerrain, 500, 1500 * i), transform.rotation);
            terrainInstantiated.Add(newTerrain);
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
            if (Physics.Raycast(transformReference.position + new Vector3(0,500,0), transformReference.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundLayer))
            {
                Debug.DrawRay(transformReference.position + new Vector3(0, 500, 0), transformReference.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                GameObject newTp = Instantiate(teleporterPrefab, hit.point + new Vector3(50 * i,5,0), transform.rotation, transformReference);
                Teleportor tpScript = newTp.GetComponent<Teleportor>();
                tpScript.TeleporterNumber = i;
                teleporter.Add(tpScript);
            }

        }


    }

    public void SelectTerrain(int selectedTerrain)
    {
        Teleportor teleportorAssociated = null;
        lastTerrainSelected = selectedTerrain;
        teleportorAssociated = terrainInstantiated[selectedTerrain].transform.GetChild(0).GetComponent<Teleportor>();
        transformReference = terrainInstantiated[selectedTerrain].transform;
        playerTeleportorBehavior.GetTeleportorData(teleportorAssociated);
        cameraFadeFunction.fadeInActivation = true;
        cameraFadeFunction.tpBehavior.disparitionVFX.Play();
    }

    public void ActiveGenerationTerrain(int selectedTerrainNumber)
    {
        selectedTerrain = selectedTerrainNumber;
        lastTerrainPlay = previousTerrain[selectedTerrain].transform;
        GenerateTerrain(selectedTerrainNumber);
        AssociateNewReward(selectedTerrainNumber);
    }

    public void DestroyPreviousTerrain()
    {
        for(int i = 0; i < previousTerrain.Count; i++)
        {
            Destroy(previousTerrain[i]);
        }
    }

}
