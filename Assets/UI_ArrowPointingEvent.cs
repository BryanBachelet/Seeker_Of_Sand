using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ArrowPointingEvent : MonoBehaviour
{
    float CameraFieldOfView;
    Camera mainCam;
    public float PullbackForScale = 1;
    public GameObject refGo;
    public GameObject player;

    RectTransform myTransform;
    Vector3 initialLocalScale;
    // Start is called before the first frame update
    void Start()
    {
        CameraFieldOfView = Camera.main.fieldOfView;
        mainCam = Camera.main;
        myTransform = this.GetComponent<RectTransform>();
        initialLocalScale = myTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAdjustArrowPosition();
    }

    void UpdateAdjustArrowPosition()
    {

        Vector3 direction = mainCam.WorldToScreenPoint(refGo.transform.position);

        myTransform.position = direction;
        Debug.Log(myTransform.localPosition.x);
        if (myTransform.localPosition.x > 800)
        {
            myTransform.localPosition = new Vector3(800, myTransform.localPosition.y, myTransform.localPosition.z);
            if(myTransform.localPosition.x > 2500)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.x / 1600);
            }
        }
        else if(myTransform.localPosition.x < -800)
        {
            myTransform.localPosition = new Vector3(-800, myTransform.localPosition.y, myTransform.localPosition.z);
            if(myTransform.localPosition.x < -2500)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.x / (-1600));
            }
        }
        if (myTransform.localPosition.y > 500)
        {
            myTransform.localPosition = new Vector3(myTransform.localPosition.x, 500, myTransform.localPosition.z);
            if (myTransform.localPosition.y > 2100)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.y / 1600);
            }
        }
        else if (myTransform.localPosition.y < -500)
        {
            myTransform.localPosition = new Vector3(myTransform.localPosition.x, -500, myTransform.localPosition.z);
            if (myTransform.localPosition.y < -2100)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.y / (-1600));
            }
        }
        else
        {
            myTransform.localScale = initialLocalScale;
        }
    }

}
