using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLocationID : MonoBehaviour
{
    [Header("Terrain")]
    [HideInInspector] private int terrainID;
    [HideInInspector] private string locationName;

    [Header("Player")]
    [SerializeField] private bool m_IsPlayer = false;
    [HideInInspector] private GameLayer m_gameLayer;
    [HideInInspector] private int currentTerrainID;
    static public string currentLocationName;
    [HideInInspector] private TerrainLocationID lastLocation;
    [SerializeField] private TMPro.TMP_Text locationText;

    public void Start()
    {
        m_gameLayer = GameLayer.instance;
    }
    public void Update()
    {
        if (!m_IsPlayer) return;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, m_gameLayer.groundLayerMask))
        {
            if (!hit.transform.GetComponent<TerrainLocationID>()) return;
            TerrainLocationID newterrainlocation = hit.transform.GetComponent<TerrainLocationID>();
            if (newterrainlocation != lastLocation)
            {
                ChangeCurrentLocation(newterrainlocation);
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);

        }
        else
        {

        }

    }
    public void ChangeCurrentLocation(TerrainLocationID terrainIDRef)
    {
        currentTerrainID = terrainIDRef.terrainID;
        currentLocationName = terrainIDRef.locationName;
        lastLocation = terrainIDRef;
        if (locationText) locationText.text = currentLocationName;
    }
}
