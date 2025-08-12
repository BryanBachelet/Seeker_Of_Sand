using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
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

    public List<CristalHealth> cristalHealths = new List<CristalHealth>();
    // Start is called before the first frame update
    void Start()
    {
        associateRoomManager = transform.parent.transform.GetComponentInChildren<RoomManager>();
        if(generateCristal)
        {
            GenerateRandomCristal();
        }

    }

    private void Update()
    {

    }
    public void GenerateCristal(Transform transform)
    {
        GenerateRandomCristal();
    }
    public void GenerateRandomCristal()
    {
        if(associateRoomManager == null) { associateRoomManager = transform.parent.transform.GetComponentInChildren<RoomManager>(); }
        raycastdirection = new Vector3(0, -25, 0);
        int dropToGenerate = 3;
        GameElement randomCristalType = GeneralTools.GetFirstBaseElement(associateRoomManager.element);
        int indexElement = GeneralTools.GetElementalArrayIndex(randomCristalType);

            for (int i = 0; i < dropToGenerate; i++)
            {
                Vector2 rnd = Random.insideUnitCircle * rangeFromCenter;

                //RaycastHit hit;
                //Vector3 newPosition = transform.position + new Vector3(rnd.x, 100, rnd.y);
                //if (Physics.Raycast(newPosition, raycastdirection * 150, out hit, Mathf.Infinity, groundLayer))
                //{
                //    Debug.DrawRay(newPosition, raycastdirection * hit.distance, Color.cyan);
                  //  m_DropAreaPosition = hit.point + new Vector3(0, -5, 0);
                    GameObject cristalHealth = Instantiate(cristalDropObject[indexElement], this.transform.position, transform.rotation, transform.parent);
                    cristalHealths.Add(cristalHealth.GetComponent<CristalHealth>());

                //}
                //else
                //{
                //    Debug.DrawRay(newPosition, raycastdirection * 1000, Color.white);
                //
                //}
            }
        StartCoroutine(DelayDistributionCristal());
    }
    IEnumerator DelayDistributionCristal()
    {
        yield return new WaitForSeconds(1);
        DistributeCristalAtTheEnd();
    }
    public void DistributeCristalAtTheEnd()
    {
        for(int i = 0; i < cristalHealths.Count; i++)
        {
            cristalHealths[i].ReceiveHit(1);
        }
    }

   
    // Update is called once per frame
}
