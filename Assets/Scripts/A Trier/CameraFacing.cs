using Klak.Wiring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraFacing : MonoBehaviour
{
    static public Camera mainCamera;
    //public GameObject player;
    //public Vector3 lookVector;
    // Start is called before the first frame update

    public bool faceCameraAxis = false;
    public bool faceMouseAxis = false;



    static public Vector3 mousePosition;
    
    public Vector3 multiplyAxis;
    void Start()
    {
        if(mainCamera == null) { mainCamera = Camera.main; }
    }

    // Update is called once per frame
    void Update()
    {
        if (faceCameraAxis)
        {
            if (mainCamera)
            {
                if (faceCameraAxis)
                {
                    transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
                }
                else
                {
                    transform.LookAt(mainCamera.transform.position, Vector3.up);
                }
                //transform.forward = mainCamera.transform.forward;

            }
            else
            {
                mainCamera = Camera.main;
            }
        }
        else if (faceMouseAxis)
        {
            mousePosition = Input.mousePosition;
            Vector3 positionToLook = mainCamera.ScreenToWorldPoint(mousePosition);
            transform.LookAt(positionToLook , multiplyAxis);
        }
        

    }
}
