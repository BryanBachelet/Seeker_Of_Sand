using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_behavior_Movement : MonoBehaviour
{
    public bool isFixePosition = true;
    public Vector3 position;
    public Quaternion rotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        transform.SetLocalPositionAndRotation(new Vector3(0,0,0), new Quaternion(0,0,0,0));
        position = transform.position;
        rotation = transform.rotation;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(isFixePosition)
        {
            transform.SetPositionAndRotation(position, rotation);
            return;
        }
    }
}
