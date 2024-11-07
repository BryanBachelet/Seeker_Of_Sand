using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDropGeneration : MonoBehaviour
{
    public LayerMask groundLayer;
    [Range(1, 4)]
    public int dropQuantity;
    public int random = 1;
    public float rangeFromCenter = 25;

    private Vector3 raycastdirection;
    public Vector3 m_DropAreaPosition;

    public GameObject[] cristalDropObject;

    public bool generateCristal = false;
    private RoomManager associateRoomManager;
    // Start is called before the first frame update
    void Start()
    {
        associateRoomManager = transform.parent.transform.GetComponentInChildren<RoomManager>();
        GenerateRandomCristal();
    }

    private void Update()
    {
        if(generateCristal)
        {
            generateCristal = false;
            GenerateRandomCristal();
        }
    }
    public void GenerateRandomCristal()
    {
        raycastdirection = new Vector3(0, -25, 0);
        int dropToGenerate = dropQuantity + Random.Range(-random, random);
        int randomCristalType = (int)associateRoomManager.element ;
        if(randomCristalType == 0) { randomCristalType = Random.Range(1, 5); }
        if (dropToGenerate > 0)
        {
            for (int i = 0; i < dropToGenerate; i++)
            {
                Vector2 rnd = Random.insideUnitCircle * rangeFromCenter;

                RaycastHit hit;
                Vector3 newPosition = transform.position + new Vector3(rnd.x, 100, rnd.y);
                if (Physics.Raycast(newPosition, raycastdirection * 150, out hit, Mathf.Infinity, groundLayer))
                {
                    Debug.DrawRay(newPosition, raycastdirection * hit.distance, Color.cyan);
                    m_DropAreaPosition = hit.point + new Vector3(0, -5, 0);
                    Instantiate(cristalDropObject[randomCristalType -1], m_DropAreaPosition, transform.rotation,transform.parent);

                }
                else
                {
                    Debug.DrawRay(newPosition, raycastdirection * 1000, Color.white);

                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        //Vector2 rnd = Random.insideUnitCircle * rangeFromCenter;
        RaycastHit hit;
        Vector3 newPosition = transform.position + new Vector3(0, 100, 0);
        if (Physics.Raycast(newPosition, raycastdirection * 150, out hit, Mathf.Infinity, groundLayer))
        {
            Debug.DrawRay(newPosition, raycastdirection * hit.distance, Color.cyan);
            Gizmos.DrawRay(newPosition, raycastdirection * hit.distance);
            m_DropAreaPosition = hit.point + new Vector3(0, -5, 0);
            //Instantiate(cristalDropObject[randomCristalType], m_DropAreaPosition, transform.rotation);

        }
        else
        {
            Debug.DrawRay(newPosition, raycastdirection * 1000, Color.white);

        }
    }
    // Update is called once per frame
}
