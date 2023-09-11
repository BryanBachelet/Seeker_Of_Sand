using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLocationID : MonoBehaviour
{
    [Header("Terrain")]
    public int terrainID;
    public string locationName;

    [Header("Player")]
    [SerializeField] private bool m_IsPlayer = false;
    public LayerMask groundLayer;
    public int currentTerrainID;
    static public string currentLocationName;
    private TerrainLocationID lastLocation;
    [SerializeField] private TMPro.TMP_Text locationText;

    public void Update()
    {
        if (!m_IsPlayer) return;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundLayer))
        {
            if (!hit.transform.GetComponent<TerrainLocationID>()) return;
            TerrainLocationID newterrainlocation = hit.transform.GetComponent<TerrainLocationID>();
            if(newterrainlocation != lastLocation)
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
        locationText.text = currentLocationName;
    }
}
