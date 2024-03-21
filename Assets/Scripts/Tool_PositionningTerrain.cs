using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tool_PositionningTerrain : MonoBehaviour
{
    public bool changeRotationAlso = false;
    public bool activeDebugDisplay;
    public bool activeDebugDisplayParent;


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
            if (Physics.Raycast(transform.position, directionPosition, out hit, Mathf.Infinity, layerGround))
            {
                Debug.DrawRay(transform.position, directionPosition * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                Quaternion rotationProp = Quaternion.identity;
                if (changeRotationAlso)
                {
                    rotationProp = Quaternion.FromToRotation(Vector3.up, hit.normal) * new Quaternion(-90, 0, 0, 0);
                    transform.localRotation = rotationProp;
                }
                transform.position = hit.point;
            }
            else
            {
                Debug.DrawRay(transform.position, directionPosition * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
            activeDebugDisplay = false;
        }


        if(activeDebugDisplayParent)
        {
            
            for(int i = 0; i < transform.childCount; i++)
            {
                RaycastHit hit;
                Transform childTransform = transform.GetChild(i);
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(childTransform.position, directionPosition, out hit, Mathf.Infinity, layerGround))
                {
                    Debug.DrawRay(childTransform.position, directionPosition * hit.distance, Color.yellow);
                    Debug.Log("Did Hit");
                    Quaternion rotationProp = Quaternion.identity;
                    if (changeRotationAlso)
                    {
                        rotationProp = Quaternion.FromToRotation(Vector3.up, hit.normal) * new Quaternion(90,0,0,0); 
                        childTransform.localRotation = rotationProp;
                    }
                    childTransform.position = hit.point;
                    //RotateProps(childTransform);
                }
            }


            activeDebugDisplayParent = false;
        }

        Debug.DrawRay(transform.GetChild(0).position, directionPosition * 1000, Color.white);
    }

    public void RotateProps(Transform propsToPosition)
    {

    }
}
