using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapControl : MonoBehaviour
{
    public GameObject playerTarget;
    public float[] y_distance;
    public int currentZoomLevel;
    public Vector3 maxDezoomPosition;
    private Camera cameraminimap;

    public Texture2D discoveryMap;
    private Color[] colorMap;
    public Vector3 playerPosition;
    public Vector2 playerPositionMiniMap;
    public GameObject terrainPosition;
    public int diameterDiscovery;

    private Material material;
    public RawImage imageMiniMap;
    // Start is called before the first frame update
    void Start()
    {
        cameraminimap = this.GetComponent<Camera>();
        ResetDiscovery(terrainPosition);

    }

    // Update is called once per frame
    void Update()
    {
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
        int[] brushDiscovery = new int[diameterDiscovery * diameterDiscovery];
        int radius = Mathf.FloorToInt(diameterDiscovery / 2);

        for (int i = 0; i < diameterDiscovery; i++)
        {
            int posX = (int)playerPositionMiniMap.x + (-radius) + i;
            for(int j = 0; j < diameterDiscovery; j++)
            {
                int posY = (int)playerPositionMiniMap.y + (-radius) + j;
                posX = Mathf.Clamp(posX, 0, 256);
                posY = Mathf.Clamp(posY, 0, 256);
                discoveryMap.SetPixel(posX, posY, Color.white);
            }

        }

        discoveryMap.Apply();
        material.SetTexture("_DiscoveryMap", discoveryMap);
    }

    public void ResetDiscovery(GameObject NewterrainPosition)
    {
        terrainPosition = NewterrainPosition;
        cameraminimap.transform.position = NewterrainPosition.transform.position + new Vector3(750,570,750);
        maxDezoomPosition = cameraminimap.transform.position;
        discoveryMap = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        colorMap = discoveryMap.GetPixels();
        for (int i = 0; i < colorMap.Length; i++)
        {
            colorMap[i] = Color.black;
        }
        discoveryMap.SetPixels(colorMap);
        discoveryMap.Apply();
        material = imageMiniMap.material;
    }

}
