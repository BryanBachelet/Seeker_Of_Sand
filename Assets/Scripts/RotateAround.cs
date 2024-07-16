using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public bool rotationActive;
    public GameObject targetToRotateAround;
    public float speedRotation;
    public Transform m_transform;

    public Vector3 axisRotation;
    public Vector3 offSet;
    public float angleRotation;

    public float distanceWithTarget;
    // Start is called before the first frame update
    void Start()
    {
        if(m_transform == null) { m_transform = this.GetComponent<Transform>(); }
    }

    // Update is called once per frame
    void Update()
    {
        if(rotationActive)
        {
            Vector3 distance = transform.position - targetToRotateAround.transform.position;
            Vector3 NewPosition = targetToRotateAround.transform.position + distance.normalized * distanceWithTarget;
            m_transform.RotateAround(NewPosition + offSet, axisRotation, angleRotation);
            SetDistance(NewPosition + offSet);
        }
    }

    private void OnValidate()
    {

    }

    private void SetDistance(Vector3 applyDistance)
    {
        transform.position = applyDistance;
    }
}
