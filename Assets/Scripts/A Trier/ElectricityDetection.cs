using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityDetection : MonoBehaviour
{
    public GameObject pos1;
    private Vector3 pos1Last = Vector3.zero;
    public GameObject pos4;
    private Vector3 pos4Last = Vector3.zero;

    public GameObject pos2;
    public GameObject pos3;

    public float distanceChangement;
    public float speedMovement;
    public Vector3 lastSavePos;

    public int variationRnd;
    public Vector3 offSetVariation1;
    public Vector3 offSetVariation2;

    public bool activeByDistance = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeByDistance) return;
        if(Vector3.Distance(lastSavePos, transform.position) > distanceChangement)
        {
            lastSavePos = transform.position;
            ActiveChangementDistance();
        }
        else
        {
            pos1.transform.position = pos1Last;
            pos4.transform.position = pos4Last;
            pos2.transform.position = this.transform.position + new Vector3(0,10,0);
            pos3.transform.position = this.transform.position + new Vector3(0, 5, 0);
        }
    }

    public void ActiveChangementDistance()
    {
        int rnd1 = Random.Range(-variationRnd, variationRnd);
        
        int rnd2 = Random.Range(-variationRnd, variationRnd);
        pos1Last = transform.position + offSetVariation1 + new Vector3(rnd2, 0, rnd1);
        pos4Last = transform.position + offSetVariation2 + new Vector3(rnd1, 0, rnd2);
    }

    public void ActiveChangementTarget(Transform transformStart, Transform transformEnd, Transform transformReference)
    {
        int rnd1 = Random.Range(-variationRnd, variationRnd);

        int rnd2 = Random.Range(-variationRnd, variationRnd);
        Vector3 newPositionPos2and3 = transformStart.position - transformEnd.position;
        pos1Last = transformStart.position;
        pos4Last = transformEnd.position;
        pos1.transform.position = pos1Last;
        pos4.transform.position = pos4Last;
        pos2.transform.position = transformReference.position + newPositionPos2and3 / 3;
        pos3.transform.position = transformReference.position + newPositionPos2and3 / 3 * 2;
        pos2.transform.position += new Vector3(-rnd1, rnd2 / 2, rnd1);
        pos3.transform.position += new Vector3(rnd2, rnd1 / 2, -rnd2);
    }
}
