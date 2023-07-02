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

    static private Vector2 posMaxOnScreen;
    public Vector2 posMaxOnScreenInput;
    public int distanceStartScaling;
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
        posMaxOnScreen = posMaxOnScreenInput;
        Vector3 direction = mainCam.WorldToScreenPoint(refGo.transform.position);

        myTransform.position = direction;
        
        if (myTransform.localPosition.x > posMaxOnScreen.x)
        {
            myTransform.localPosition = new Vector3(posMaxOnScreen.x, myTransform.localPosition.y, myTransform.localPosition.z);
            if(myTransform.localPosition.x > posMaxOnScreen.x + distanceStartScaling)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.x / (posMaxOnScreen.x + distanceStartScaling));
            }
        }
        else if(myTransform.localPosition.x < -posMaxOnScreen.x)
        {
            myTransform.localPosition = new Vector3(-posMaxOnScreen.x, myTransform.localPosition.y, myTransform.localPosition.z);
            if(myTransform.localPosition.x < -posMaxOnScreen.x - distanceStartScaling)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.x / (posMaxOnScreen.x - distanceStartScaling));
            }
        }
        if (myTransform.localPosition.y > posMaxOnScreen.y)
        {
            myTransform.localPosition = new Vector3(myTransform.localPosition.x, posMaxOnScreen.y, myTransform.localPosition.z);
            if (myTransform.localPosition.y > posMaxOnScreen.y - distanceStartScaling)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.y / (posMaxOnScreen.y + distanceStartScaling));
            }
        }
        else if (myTransform.localPosition.y < -posMaxOnScreen.y)
        {
            myTransform.localPosition = new Vector3(myTransform.localPosition.x, -posMaxOnScreen.y, myTransform.localPosition.z);
            if (myTransform.localPosition.y < -posMaxOnScreen.y - distanceStartScaling)
            {
                myTransform.localScale = initialLocalScale * 0.2f;
            }
            else
            {
                myTransform.localScale = Vector3.Lerp(initialLocalScale, initialLocalScale * 0.2f, myTransform.localPosition.y / (-posMaxOnScreen.y - distanceStartScaling));
            }
        }
        else
        {
            myTransform.localScale = initialLocalScale;
        }
    }

}
