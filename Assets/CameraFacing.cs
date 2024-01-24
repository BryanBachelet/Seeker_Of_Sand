using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    public Camera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(mainCamera)
        {
            transform.forward = mainCamera.transform.forward;
            transform.LookAt(mainCamera.transform.position, Vector3.up);
        }
        else
        {
            mainCamera = Camera.main;
        }

    }
}
