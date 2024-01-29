using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowArea : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private float m_slowPercent;
    public LayerMask m_layersAffect;
    public LayerMask m_groundLayer;
    public float areaRadius = 5;

    private float m_slowReduceRatio;
    private float m_slowUpRatio;

    public void Awake()
    {
        m_slowReduceRatio = 1-( m_slowPercent / 100.0f);
        m_slowUpRatio = 1.0f / m_slowReduceRatio;
        transform.localScale = Vector3.one * areaRadius;

    }

    public void Start()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, m_groundLayer))
        {
            transform.position = hit.point;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        int value = m_layersAffect.value;
        if (value == (value | 1 << other.gameObject.layer))
        {
            Debug.Log("Test Slow Down");
            NavMeshAgent navmeshAgent = other.GetComponent<NavMeshAgent>();

            if (navmeshAgent)
            {
                navmeshAgent.speed *= m_slowReduceRatio;
            }

        }
    }

    public void OnTriggerExit(Collider other)
    {
        int value = m_layersAffect.value;
        if (value == (value | 1 << other.gameObject.layer))
        {
            NavMeshAgent navmeshAgent = other.GetComponent<NavMeshAgent>();

            if (navmeshAgent)
            {
                navmeshAgent.speed *= m_slowUpRatio;
            }

        }
    }


}
