using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructPart : MonoBehaviour
{
    public LayerMask layerOfInteraction;
    private float m_timeBeforeStop = 10;
    private float m_timerBeforeStop = 0.0f;


    public void Update()
    {
        if (m_timerBeforeStop > m_timeBeforeStop)
        {
            DeactivationInteraction();
        }
        else
        {
            m_timerBeforeStop += Time.deltaTime;
        }
    }

    public void DeactivationInteraction()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].includeLayers = layerOfInteraction;
            rigidbodies[i].isKinematic = true;
            colliders[i].enabled = false;
        }
    }
      
}
