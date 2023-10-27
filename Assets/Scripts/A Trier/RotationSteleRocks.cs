using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSteleRocks : MonoBehaviour
{
    public bool activeRotation;
    [SerializeField] private AnimationCurve RotationBehavior;
    [SerializeField] private float rotationspeed;
    [SerializeField] private Transform m_transform;
    private Vector3 initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        initialRotation = m_transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeRotation)
        {
            m_transform.eulerAngles = new Vector3(initialRotation.x, initialRotation.y, m_transform.eulerAngles.z + Time.deltaTime * RotationBehavior.Evaluate(Time.time) * rotationspeed);
        }
    }
}

