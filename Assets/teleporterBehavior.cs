using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterBehavior : MonoBehaviour
{
    public Teleportor lastTeleportor;
    public Vector3 lastTpPosition;
    public bool activationTP;
    public Teleportor nextTeleporter;
    public Vector3 nextTpPosition;
    private int nextTerrainNumber = 0;

    public CameraFadeFunction cameraFadeFunction;
    public TerrainGenerator terrainGen;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraFadeFunction == null) { cameraFadeFunction = Camera.main.GetComponent<CameraFadeFunction>(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetTeleportorData(Teleportor tpObject)
    {
        nextTeleporter = tpObject;
        nextTpPosition = tpObject.transform.position;
        nextTerrainNumber = tpObject.TeleporterNumber;

    }

    public void ActivationTeleportation()
    {
        this.gameObject.transform.position = nextTpPosition + new Vector3(0, 10, 0);
        cameraFadeFunction.fadeOutActivation = true;
        terrainGen.ActiveGenerationTerrain(nextTerrainNumber);
    }
}
