using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLightingStrike : MonoBehaviour
{
    public GameObject prefabThunder;
    private Vector3 strikePosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CallLightingStrike(Vector3 position)
    {
        strikePosition = position;
        Instantiate(prefabThunder, strikePosition, Quaternion.identity);
    }
}
