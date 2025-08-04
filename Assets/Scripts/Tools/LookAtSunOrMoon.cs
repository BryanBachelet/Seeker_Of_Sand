using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSunOrMoon : MonoBehaviour
{
    public Vector3 lookAtRotatingPoint;
    public Vector3 rotation;
    public AnimationCurve rotationByHour;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(rotationByHour.Evaluate(DayCyclecontroller.staticTimeOfTheDay), 30, 0));    
    }
}
