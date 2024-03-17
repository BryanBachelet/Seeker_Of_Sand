using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Punketone : MonoBehaviour
{
    public GameObject fireVfxEffect;
    [SerializeField] private float HPMax;
    public float currentHP;
    public float percentHP;

    public LineRenderer lineRenderer;
    public int distanceToAbsorb = 10;
    public float distanceEntreVfx = 3;
    public float tempsEcouleLastFire = 0;
    public float tempsAvantAbsorb = 5;
    public float tempsCurrentAbsorb = 1;
    private Vector3 positionLastFire = Vector3.zero;
    public bool spawningFire = false;

    public float speed;
    public LayerMask groundLayer;

    public GameObject target;
    public Transform targetFocus;


    private Rigidbody m_rigidbody;
    private NavMeshAgent m_navMeshAgent;

    public AltarBehaviorComponent altarRefered;
    private ObjectHealthSystem objectHealthSystem;

    public GameObject deathDestroyVfx;
    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_navMeshAgent.Warp(transform.position);
        currentHP = HPMax;
        percentHP = currentHP / HPMax;
        objectHealthSystem = altarRefered.GetComponent<ObjectHealthSystem>();

        Move();

    }

    // Update is called once per frame
    void Update()
    {
        percentHP = currentHP / HPMax;
        if (target != null)
        {

            if (Vector3.Distance(transform.position, target.transform.position) < distanceToAbsorb)
            {
                Vector3[] position = new Vector3[2];
                lineRenderer.positionCount = position.Length;
                position[0] = transform.position + Vector3.up * 18;
                position[1] = targetFocus.position;
                lineRenderer.SetPositions(position);

                m_navMeshAgent.isStopped = true;
                tempsCurrentAbsorb += Time.deltaTime;
                if (tempsCurrentAbsorb > tempsAvantAbsorb)
                {
                    tempsCurrentAbsorb = 0;
                    objectHealthSystem.TakeDamage((int)10);
                }
            }
            else
            {

            }


            if (Vector3.Distance(transform.position, positionLastFire) > distanceEntreVfx)
            {


                RaycastHit hit;
                if (Physics.Raycast(transform.position + new Vector3(0, 20, 0), Vector3.down, out hit, Mathf.Infinity, groundLayer))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    GameObject newFire = Instantiate(fireVfxEffect, hit.point, transform.rotation);
                    newFire.transform.position = hit.point;
                    positionLastFire = hit.point;
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                }

            }

        }
        if (percentHP < 0.01f)
        {
            altarRefered.punketonHP.Remove(this);
            altarRefered.skeletonCount--;
            Destroy(this.gameObject);
        }
    }

    public void Move()
    {
        Vector3 directionToDestination = m_navMeshAgent.destination - transform.position;
        Vector3 directionToTarget = target.transform.position - transform.position;
        float dot = Vector3.Dot(directionToDestination.normalized, directionToTarget.normalized);
        float distancePos = Vector3.Distance(transform.position, target.transform.position);

        m_navMeshAgent.enabled = true;
        if (m_navMeshAgent.isOnNavMesh)
        {

            m_navMeshAgent.SetDestination(target.transform.position);
            //if(!m_navMeshAgent.hasPath)
            //{
            //    NavMeshPath path = new NavMeshPath();
            //    bool result = m_navMeshAgent.CalculatePath(target.transform.position, path);
            //    Debug.Log("New Path result" + result);
            //    Debug.Log("New Path status" + path.status); 
            //}
            Debug.Log("Test Debug Punketon");
        }
        if (!m_navMeshAgent.hasPath)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target.transform.position, out hit, 100.0f, NavMesh.AllAreas))
            {

                if (m_navMeshAgent.isOnNavMesh) m_navMeshAgent.SetDestination(hit.position);
                else
                {
                    if (NavMesh.SamplePosition(transform.position, out hit, 100.0f, NavMesh.AllAreas))
                    {
                        Debug.Log(name + "is not on the navMesh");
                        m_navMeshAgent.Warp(hit.position);
                    }
                }


            }
        }
    }

    public void OnDestroy()
    {
        Instantiate(deathDestroyVfx, transform.position, transform.rotation);
    }
}
