using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform m_target;
    private NavMeshAgent m_navAgent;

    private void Start()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        m_navAgent.destination = m_target.position;
    }

    public void GetDestroy()
    {
        // Call Enemy Manager
    }
}
