using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class dummy : MonoBehaviour
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
    public GameObject m_vfxHitFeedback;
    public HealthManager m_healthManager;
    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        currentHP = HPMax;
        percentHP = currentHP / HPMax;
    }

    // Update is called once per frame
    void Update()
    {
        percentHP = currentHP / HPMax;
        MoveAndAbsorbEnergy();
        if (percentHP < 0.01f)
        {
            //altarRefered.punketonHP.Remove(this);
            //altarRefered.skeletonCount--;
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
        if (m_navMeshAgent.isOnNavMesh) m_navMeshAgent.SetDestination(target.transform.position);
        if (!m_navMeshAgent.hasPath)
        {
            Debug.Log("Has hit");
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

    public void MoveAndAbsorbEnergy()
    {
        if (target != null)
        {

            if (Vector3.Distance(transform.position, target.transform.position) < distanceToAbsorb)
            {
                Vector3[] position = new Vector3[2];
                lineRenderer.positionCount = position.Length;
                position[1] = transform.position + new Vector3(0, transform.GetChild(0).position.y + 18, 0);
                position[0] = targetFocus.position;
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

                Move();
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
                    Debug.Log("Did Hit || " + hit.point);
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    Debug.Log("Did not Hit");
                }

            }

        }
    }

    public void ReceiveDamage(float damage, Vector3 direction, float power)
    {
        // VfX feedback
        m_healthManager.CallDamageEvent(transform.position + Vector3.up * 1.5f, damage);
        Instantiate(m_vfxHitFeedback, transform.position, Quaternion.identity);
        //m_entityAnimator.SetTrigger("TakeDamage");
        GlobalSoundManager.PlayOneShot(12, transform.position);
    }
    public void OnDestroy()
    {
        Instantiate(deathDestroyVfx, transform.position, transform.rotation);
    }
}
