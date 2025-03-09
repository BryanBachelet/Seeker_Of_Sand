using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ElectricityController : MonoBehaviour
{
    public Transform[] elecPosition = new Transform[4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elecPosition[1].position = Vector3.Lerp(elecPosition[0].position, elecPosition[3].position, 0.33f);
        elecPosition[2].position = Vector3.Lerp(elecPosition[0].position, elecPosition[3].position, 0.66f);
    }
}
