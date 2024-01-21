using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tool_PositionningTerrain : MonoBehaviour
{
    public static bool activeDebug;
    public bool activeDebugDisplay;

    public LayerMask layerGround;
    public Vector3 directionPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeDebugDisplay)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position,directionPosition, out hit, Mathf.Infinity, layerGround))
            {
                Debug.DrawRay(transform.position,directionPosition * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                transform.position = hit.point;
                
            }
            else
            {
                Debug.DrawRay(transform.position,directionPosition * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
activeDebugDisplay = false;
        }
        
        Debug.DrawRay(transform.position,directionPosition * 1000, Color.white);
       
    }
}
