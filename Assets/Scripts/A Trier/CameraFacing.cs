using Klak.Wiring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraFacing : MonoBehaviour
{
    static public Camera mainCamera;
    public Camera debugCamera;
    //public GameObject player;
    //public Vector3 lookVector;
    // Start is called before the first frame update

    public bool faceCameraAxis = false;
    public bool faceMouseAxis = false;

    public bool moveToCamera = false;

    static public Vector3 mousePosition;
    
    public Vector3 multiplyAxis;

    public float speed;
    private float step;
    void Start()
    {
        if(mainCamera == null) { mainCamera = Camera.main; }
        debugCamera = mainCamera;
        step = speed * Time.deltaTime;
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
                debugCamera = mainCamera;
            }
        }
        else if (faceMouseAxis)
        {
            mousePosition = Input.mousePosition;
            Vector3 positionToLook = mainCamera.ScreenToWorldPoint(mousePosition);
            transform.LookAt(positionToLook , multiplyAxis);
        }
        
        if(moveToCamera)
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, step);
        }

    }


}
