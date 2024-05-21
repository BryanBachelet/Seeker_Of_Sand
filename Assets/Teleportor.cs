using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportor : MonoBehaviour
{
    public int TeleporterNumber;
    public bool usedTeleporter = false;

    public bool isSpawn;
    public bool activation = false;
    public bool teleportorIsActive = false;
    public Material socleMaterial;

    static private TerrainGenerator terrainGen;

    // Start is called before the first frame update
    void Start()
    {
        socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;
        activation = true;
        if(terrainGen == null)
        {
            terrainGen = GameObject.Find("TerrainGenerator").GetComponent<TerrainGenerator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activation)
        {
            activation = false;
            ActivationTeleportor();
        }
    }

    public void ActivationTeleportor()
    {
        teleportorIsActive = true;
        socleMaterial.SetFloat("_TEXMCOLINT", 50f);
    }

    public void DesactivationTeleportor()
    {
        teleportorIsActive = false;
        socleMaterial.SetFloat("_TEXMCOLINT", -200f);
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            terrainGen.SelectTerrain(TeleporterNumber);
            usedTeleporter = true;
        }
    }
}
