using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapControl : MonoBehaviour
{
    [HideInInspector] private GameObject playerTarget;
    [SerializeField] private float[] y_distance;
    [HideInInspector] private int currentZoomLevel;
    [HideInInspector] private Vector3 maxDezoomPosition = new Vector3(-11327.8096f, 600, -2150.37207f);
    [HideInInspector] private Camera cameraminimap;

    [HideInInspector] private Texture2D discoveryMap;
    [HideInInspector] private Color[] colorMap;
    [HideInInspector] private Vector3 playerPosition;
    [HideInInspector] private Vector2 playerPositionMiniMap;
    [HideInInspector] private GameObject terrainPosition;
    [HideInInspector] private CameraMinimap_Data minimapData;
    [HideInInspector] private int diameterDiscovery = 50;
    [HideInInspector] private int radiusBrush = 25;

    [HideInInspector] private Material material;
    [SerializeField] private RawImage imageMiniMap;

    public Vector3 positionOffSet;
    public bool debugUpdatePosition = false;

    [SerializeField] private int indexReferenceCameraMinimap = 0;
    // Start is called before the first frame update
    void Start()
    {
        cameraminimap = this.GetComponent<Camera>();
        playerTarget = GameObject.Find("Player");
        //ResetDiscovery(terrainPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if(debugUpdatePosition)
        {
            debugUpdatePosition = false;
            UpdateMiniMapPosition();
        }
        if(terrainPosition == null) { return; }
        playerPosition = playerTarget.transform.position - terrainPosition.transform.position;
        playerPositionMiniMap = new Vector2((playerPosition.x / 1500) * 256, (playerPosition.z / 1500) * 256);
        UpdateDiscovery();
        if (currentZoomLevel == 0) { return; }
        else
        {
            transform.position = new Vector3(playerTarget.transform.position.x, playerTarget.transform.position.y + y_distance[currentZoomLevel], playerTarget.transform.position.z);
        }

    }

    public void ZoomIn()
    {
        if (currentZoomLevel >= y_distance.Length -1)
        {
            return;
        }
        else
        {
            currentZoomLevel += 1;
            transform.position = new Vector3(this.transform.position.x, playerTarget.transform.position.y + y_distance[currentZoomLevel], this.transform.position.z);
            cameraminimap.orthographicSize = y_distance[currentZoomLevel];
        }
        if (currentZoomLevel > y_distance.Length - 1) { currentZoomLevel = y_distance.Length - 1; }
    }

    public void zoomOut()
    {

        currentZoomLevel -= 1;
        if (currentZoomLevel <= 0)
        {
            currentZoomLevel = 0;
            transform.position = maxDezoomPosition;
            return;
        }
        else
        {
            transform.position = new Vector3(this.transform.position.x, playerTarget.transform.position.y + y_distance[currentZoomLevel], this.transform.position.z);
            cameraminimap.orthographicSize = 630;
        }
    }

    public void UpdateDiscovery()
    {
        int radius = Mathf.FloorToInt(diameterDiscovery / 2);

        for (int i = 0; i < diameterDiscovery; i++)
        {
            int posX = (int)playerPositionMiniMap.x + (-radius) + i;
            posX = Mathf.Clamp(posX, 0, 256);
            for (int j = 0; j < diameterDiscovery; j++)
            {
                int posY = (int)playerPositionMiniMap.y + (-radius) + j;
              
                posY = Mathf.Clamp(posY, 0, 256);
                Vector2 currentPos = new Vector2(posX, posY);
                float distance = Vector2.Distance(playerPositionMiniMap, currentPos);
                if (distance < radiusBrush)
                {
                    discoveryMap.SetPixel(posX, posY, Color.white);
                }
              
            }
           
        }
        
        discoveryMap.Apply();
        material.SetTexture("_DiscoveryMap", discoveryMap);

    }

    public void ResetDiscovery(GameObject NewterrainPosition)
    {
        minimapData = NewterrainPosition.GetComponent<CameraMinimap_Data>();
        terrainPosition = NewterrainPosition;
        //cameraminimap.transform.position = NewterrainPosition.transform.position + minimapData.SettingCameraMinimap_Position()[indexReferenceCameraMinimap];
        cameraminimap.transform.SetPositionAndRotation
            (NewterrainPosition.transform.position + minimapData.SettingCameraMinimap_Position()[indexReferenceCameraMinimap], 
            minimapData.SettingCameraMinimap_Rotation()[indexReferenceCameraMinimap]);
        cameraminimap.transform.position = new Vector3(cameraminimap.transform.position.x, minimapData.SettingCameraMinimap_Position()[indexReferenceCameraMinimap].y, cameraminimap.transform.position.z);
        maxDezoomPosition = cameraminimap.transform.position;
        discoveryMap = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        colorMap = discoveryMap.GetPixels();
        for (int i = 0; i < colorMap.Length; i++)
        {
            colorMap[i] = Color.white;
        }
        discoveryMap.SetPixels(colorMap);
        discoveryMap.Apply();
        material = imageMiniMap.material;
    }

    public void UpdateMiniMapPosition()
    {
        cameraminimap.transform.position = terrainPosition.transform.position + minimapData.SettingCameraMinimap_Position()[indexReferenceCameraMinimap];
    }
}
