using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class positioningCameraPreview : MonoBehaviour
{
    public GameObject target;
    public bool changeFocus = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(changeFocus) transform.LookAt(target.transform, Vector3.up); changeFocus = false;
    }
}
