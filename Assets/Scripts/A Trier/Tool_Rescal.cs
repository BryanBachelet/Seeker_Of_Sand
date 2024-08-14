using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_Rescal : MonoBehaviour
{
    public float divideScale = 1;
    public bool activeRescale = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeRescale)
        {
            transform.localScale = transform.localScale / divideScale;
            activeRescale = false;
        }
    }
}
