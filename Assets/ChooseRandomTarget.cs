using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRandomTarget : MonoBehaviour
{
    public List<GameObject> child = new List<GameObject>();

    public GameObject goFrom;
    public GameObject goToo;

    public float frequenceChange = 5;
    public float tempsEcouleLastChange;
    public bool change = false;


    public ElectricityDetection elecDetect;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            child.Add(this.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!change) return;

        if(tempsEcouleLastChange < frequenceChange)
        {
            tempsEcouleLastChange += Time.deltaTime;
        }
        else
        {
            ChooseNewTarget();
        }
    }

    public void ChooseNewTarget()
    {
        bool change = false;
        while (!change)
        {
            int newRandom = Random.Range(0, this.transform.childCount);
            int newRandom2 = Random.Range(0, this.transform.childCount);
            if(newRandom != newRandom2)
            {
                change = true;
                goFrom = this.transform.GetChild(newRandom).gameObject;
                goToo = this.transform.GetChild(newRandom2).gameObject;
                tempsEcouleLastChange = 0;
                elecDetect.ActiveChangementTarget(goFrom.transform, goToo.transform, this.transform);
            }
        }
    }
}
