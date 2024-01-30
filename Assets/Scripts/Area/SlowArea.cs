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


    private ObjectState state;
    public float m_DestroyAfterTime = 3;

    private bool hasToDestroy;
    private bool canDestroy;
    private int index;

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

        state = new ObjectState();
        GameState.AddObject(state);
        StartCoroutine(DestroyAfter(m_DestroyAfterTime));
    }

    public void Update()
    {
        if(hasToDestroy)
        {
            if (canDestroy && index >= 1)
                Destroy(this.gameObject);

            index++;
        }
    
    }



    public void OnTriggerEnter(Collider other)
    {
        if (hasToDestroy) return; 
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

    public void OnTriggerStay(Collider other)
    {
        if (!hasToDestroy) return;

        int value = m_layersAffect.value;
        if (value == (value | 1 << other.gameObject.layer))
        {
            Debug.Log("Test Slow Down");
            NavMeshAgent navmeshAgent = other.GetComponent<NavMeshAgent>();

            if (navmeshAgent)
            {
                navmeshAgent.speed *= m_slowUpRatio; ;
            }

        }
        canDestroy = true;
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

    public IEnumerator DestroyAfter(float time)
    {
        float duration = 0;
        while (duration < time)
        {
            yield return Time.deltaTime;

            if (state.isPlaying)
            {
                duration += Time.deltaTime;
            }
        }

        hasToDestroy = true;
    }


}
