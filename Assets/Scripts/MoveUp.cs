using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MoveUp : MonoBehaviour
{
    public float distanceUp = 30;
    public float timeMovingUp = 5;
    private float timeEcoule = 0;

    public Vector3 rotation;
    private GameObject[] dragonHead = new GameObject[2];
    public Transform target;
    public GameObject areaPrefab;
    // Start is called before the first frame update
    void Start()
    {
        timeEcoule = 0;
        for (int i = 0; i < dragonHead.Length; i++)
        {
            dragonHead[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeEcoule += Time.deltaTime;


        transform.Rotate(rotation * Time.deltaTime);
        for(int i = 0; i < dragonHead.Length; i++)
        {
            dragonHead[i].transform.LookAt(target);
        }
        if(Vector3.Distance(target.position, transform.position) < 20)
        {
            Instantiate(areaPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (timeEcoule > timeMovingUp)
        {
            
            this.GetComponent<SmoothFollow>().enabled = true;
            //this.enabled = false;
        }
        else
        {
            transform.Translate(Vector3.up * distanceUp * Time.deltaTime);
        }
    }
}
