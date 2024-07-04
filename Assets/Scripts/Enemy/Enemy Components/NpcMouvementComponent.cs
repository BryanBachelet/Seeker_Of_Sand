using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class NpcMouvementComponent : MonoBehaviour
    {
        public bool selected = false;
        private const float slowSpeed = .7f;
        private const int facingAngle = 10;
        private const int minAngleToFace = 10;
        public float minTargetDistance = 1;
        [Header("Movement Parameters")]
        public float speed = 5;
        public float speedThreshold = 1;
        [HideInInspector] public bool isSlow;
        public Transform  baseTransform;
        private float m_baseSpeed;
        private float m_currentSpeed;

        [Header("Slope Parameters")]
        public bool isAffectedBySlope = false;
        [Range(1.0f, 3.00f)] public float maxVariation = 1.5f;
        [Range(0.001f, 1.00f)] public float minVariation = 0.5f;
        public float m_maxAngle = 60.0f;

        [SerializeField] private float m_groundDistance = 3.0f;
        [SerializeField] private LayerMask m_groundLayerMask;

        [Header("Navmesh Parameters")]
        public float timeBetweenNavRefresh;
        public float minDistanceToFullyActive;
        [SerializeField] private float m_distanceBeforeRepositionning = 400;
        [SerializeField] private bool m_isAlwaysUpdate;


        private bool m_isPauseActive;
        private float m_directionMinDot = 0.45f;

        private Rigidbody m_rigidbody;
        private NavMeshAgent m_navMeshAgent;
        private NpcMetaInfos m_npcInfo;
        private NpcHealthComponent m_npcHealthComponent;
        public EnemyManager enemiesManager;

        public TargetData targetData;
        public bool isDebugActive = false;
        
        [HideInInspector] public bool isGoingAway;
        [HideInInspector] public float awayDistance;
        private Vector3 fleePosition;

        public void Start()
        {
            InitComponent();
            m_npcHealthComponent.destroyEvent += OnDeath;
            RestartObject();
        }

        public void InitComponent()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_npcInfo = GetComponent<NpcMetaInfos>();
        }

        public void SetTarget(TargetData target)
        {
            targetData = target;
            if (m_navMeshAgent == null) m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_navMeshAgent.enabled = true;
            bool state = m_navMeshAgent.SetDestination(targetData.target.position);

            if (!m_navMeshAgent.hasPath)
            {

                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetData.target.position, out hit, 100.0f, NavMesh.AllAreas))
                {
                    state = m_navMeshAgent.SetDestination(hit.position);

                }
            }


        }

        public void Update()
        {
            if (m_npcInfo.state == NpcState.PAUSE)
            {
                if (!m_isPauseActive)
                {
                    SetupPause();
                    m_isPauseActive = true;
                }
                return;
            }
            m_isPauseActive = false;

            if (m_npcInfo.state == NpcState.MOVE)
            {
              
                if(isGoingAway)
                {
                    if (m_navMeshAgent.isActiveAndEnabled && m_navMeshAgent.isStopped) m_navMeshAgent.isStopped = false;
                    if (isAffectedBySlope)
                    {
                        m_navMeshAgent.speed = CalculateSlopeSpeed();
                        if (isSlow) m_navMeshAgent.speed *= slowSpeed;
                    }
                    else
                    {
                        m_navMeshAgent.speed = speed;
                        if (isSlow) m_navMeshAgent.speed *= slowSpeed;
                    }

                    if(IsOutsideRange(awayDistance) && !IsFacingTarget())
                    {
                        RotateToTarget();
                    }

                    // Compute the distance
                    ComputeFleePosition();
                    Move(fleePosition);
                    return;
                }
    
                if (!IsInRange())
                {
                    if (m_navMeshAgent.isActiveAndEnabled && m_navMeshAgent.isStopped) m_navMeshAgent.isStopped = false;

                    if (isAffectedBySlope)
                    {
                        m_navMeshAgent.speed = CalculateSlopeSpeed();
                        if (isSlow) m_navMeshAgent.speed *= slowSpeed;
                    }
                    else
                    {
                        m_navMeshAgent.speed = speed;
                        if (isSlow) m_navMeshAgent.speed *= slowSpeed;
                    }

                    MoveToTarget();
                    if (targetData.isMoving) MoveToTarget();
                }
                else
                {
                    
                    m_navMeshAgent.isStopped = true;
                    m_navMeshAgent.velocity = Vector3.zero;
                    m_rigidbody.velocity = Vector3.zero;
                }

                
            }

        }

        public void ComputeFleePosition(bool isDirect = false)
        {
            if(isDirect)
            {
                fleePosition = targetData.target.position + (baseTransform.position - targetData.target.position).normalized * awayDistance;
                return;
            }

            if (Vector3.Distance(targetData.target.position, fleePosition) < awayDistance)
            {
                Vector3 tempPosition = targetData.target.position + (baseTransform.position - targetData.target.position).normalized * awayDistance;
                fleePosition = tempPosition;
            }
            
        }

        public void StopMouvement()
        {
             m_navMeshAgent.isStopped = true;
                    m_navMeshAgent.velocity = Vector3.zero;
                    m_rigidbody.velocity = Vector3.zero;
        }

        public bool IsInRange()
        {
                float distance = Vector3.Distance(baseTransform.position, targetData.baseTarget.position);

            return distance  < minTargetDistance;
        }

        public bool IsOutsideRange(float outRange)
        {
            float distance = Vector3.Distance(baseTransform.position, targetData.baseTarget.position);
           
            if (outRange - distance < 0.5f) 
                distance = outRange;

            return distance >= outRange;
        }

        public bool IsFacingTarget()
        {
            Vector3 tarDir = targetData.target.position - baseTransform.position;
            float angle = Vector3.SignedAngle(baseTransform.forward, tarDir,Vector3.up);
            angle = Mathf.Abs(angle);

            return angle < facingAngle;

        }


        public void RotateToTarget(float duration = 1)
        {
            Vector3 tarDir = targetData.target.position - baseTransform.position;
            tarDir.y = 0;
            float angle = Vector3.SignedAngle(baseTransform.forward, tarDir.normalized, Vector3.up);
            float worldAngle = Vector3.SignedAngle(Vector3.forward, tarDir, Vector3.up);
            if (Mathf.Abs(angle) < minAngleToFace)
            {
                baseTransform.rotation = Quaternion.Euler(baseTransform.rotation.eulerAngles.x, worldAngle, baseTransform.rotation.eulerAngles.z);
            } 
            else
            {
                baseTransform.rotation *= Quaternion.Euler(0, Mathf.Sign(angle)* 360 * Time.deltaTime, 0);
            }
           
        }

     

        public void SetupPause()
        {
            if (m_navMeshAgent.isActiveAndEnabled)
            {
                m_rigidbody.velocity = Vector3.zero;
                m_navMeshAgent.isStopped = !m_navMeshAgent.isStopped;
            }
        }

        public float CalculateSlopeSpeed()
        {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(transform.position, -Vector3.up, out hit, m_groundDistance, m_groundLayerMask);
            Vector3 direction = Vector3.Cross(transform.right, hit.normal);
            float currentSlope = GetSlopeAngle(direction);

            float ratio = maxVariation;
            if (currentSlope < 0) ratio = -minVariation;
            float ratioSlope = (currentSlope / m_maxAngle);
            float ratioSpeed = Mathf.Lerp(0, ratio, Mathf.Abs(ratioSlope));
            float speed = m_baseSpeed + m_baseSpeed * ratioSpeed;
            return speed;
        }

        private float GetSlopeAngle(Vector3 direction)
        {
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            return Vector3.SignedAngle(rotTest * Vector3.forward, direction, transform.right);
        }


        public void MoveToTarget()
        {

            if (selected)
            {
                Debug.Log("Lets dig");
            }

            Vector3 directionToDestination = m_navMeshAgent.destination - transform.position;
            Vector3 directionToTarget = targetData.target.position - transform.position;
            float dot = Vector3.Dot(directionToDestination.normalized, directionToTarget.normalized);
            float distancePos = Vector3.Distance(transform.position, targetData.target.position);

            // Repositionning enemi when to far 
            if (distancePos > m_distanceBeforeRepositionning)
            {

                m_navMeshAgent.enabled = false;
                if (enemiesManager.ReplaceFarEnemy(this.gameObject))
                {
                    distancePos = Vector3.Distance(transform.position, targetData.target.position);
                    m_navMeshAgent.destination = targetData.target.position;
                    m_navMeshAgent.nextPosition = transform.position;

                    return;

                }
            }
            if (distancePos > minDistanceToFullyActive && dot > m_directionMinDot && !m_isAlwaysUpdate)
            {
                return;
            }

            m_navMeshAgent.enabled = true;
            if (m_navMeshAgent.isOnNavMesh) m_navMeshAgent.SetDestination(targetData.target.position);
            if (!m_navMeshAgent.hasPath)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetData.target.position, out hit, 100.0f, NavMesh.AllAreas))
                {

                    if (m_navMeshAgent.isOnNavMesh) m_navMeshAgent.SetDestination(hit.position);
                    else
                    {
                        if (NavMesh.SamplePosition(transform.position, out hit, 100.0f, NavMesh.AllAreas))
                        {
                            if(isDebugActive) Debug.Log(name + "is not on the navMesh");
                            m_navMeshAgent.Warp(hit.position);
                        }
                    }
                }
            }
        }


        public void Move(Vector3 position)
        {
            m_navMeshAgent.enabled = true;
            if (m_navMeshAgent.isOnNavMesh) m_navMeshAgent.SetDestination(position);
            if (!m_navMeshAgent.hasPath)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, 100.0f, NavMesh.AllAreas))
                {

                    if (m_navMeshAgent.isOnNavMesh) m_navMeshAgent.SetDestination(hit.position);
                    else
                    {
                        if (NavMesh.SamplePosition(transform.position, out hit, 100.0f, NavMesh.AllAreas))
                        {
                            if (isDebugActive) Debug.Log(name + "is not on the navMesh");
                            m_navMeshAgent.Warp(hit.position);
                        }
                    }
                }
            }
        }


        public void OnDeath(Vector3 direction, float power)
        {

            m_navMeshAgent.enabled = false;

            if (!m_npcHealthComponent.hasDeathAnimation)
            {

                m_rigidbody.isKinematic = false;
                m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
             
            }

            this.enabled = false;
        }

        public void RestartObject()
        {
            SetTarget(m_npcHealthComponent.targetData);
            m_baseSpeed = Random.Range(speed - speedThreshold, speed + speedThreshold);
            m_navMeshAgent.speed = m_baseSpeed;
            m_navMeshAgent.destination = (targetData.target.position);
        }

        public void OnDrawGizmosSelected()
        {
         
        }
    }
}
