using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Enemies
{
    public class JumperAttackComponent : MonoBehaviour
    {
        [Header("Attack Parameters")]
        public float damage = 5;
        public float jumpDuration = 1;
        public float angleOfJump = 15;
        public float jumpDistance = 10;

        public float recuperationDistance = 20;// Find better name;

        [Header("Technical Parameters")]
        public float groundMaxDistance = 3;

        private bool m_isOnGround;
        public LayerMask m_layerMask;
        public LayerMask m_layerObstaclePlayer;
        public Transform m_basePlayer;

        private bool m_isFalling;
        private Vector3 m_direction;

        private Rigidbody m_rigidbody;
        private CapsuleCollider m_capsuleCollider;
        private CurveBehavior m_curveBehavior;
        private Transform m_target;
        private NavMeshAgent m_agent;
        private NpcHealthComponent m_npcHealthComponent;

        // -------- Temps --------------
        private bool m_onlyOnce;
        private Vector3 dest;

        public void Start()
        {
            InitComponent();
        }

        private void InitComponent()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_curveBehavior = new CurveBehavior();
            m_agent = GetComponent<NavMeshAgent>();
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_capsuleCollider = GetComponent<CapsuleCollider>();
            m_target = m_npcHealthComponent.targetData.target;
        }


        public void Update()
        {
            if (m_npcHealthComponent.npcState == NpcState.MOVE && Vector3.Distance(transform.position, m_target.position) < jumpDistance / 2.0f)
            {
                if (IsPlayerVisible())
                {
                    m_npcHealthComponent.npcState = NpcState.ATTACK;
                    StartAttack();

                    return;
                }

            }

            if (m_npcHealthComponent.npcState == NpcState.ATTACK)
            {
                AttackJumper();
                return;
            }
            if (m_npcHealthComponent.npcState == NpcState.RECUPERATION && m_agent.remainingDistance <= 0.5f)
            {
                m_npcHealthComponent.npcState = NpcState.MOVE;
            }


        }


        private void AttackJumper()
        {
            transform.position += m_curveBehavior.UpdateCurveBehavior();
            if (m_curveBehavior.IsCurveFinish())
            {
                EndAttack();
            }

        }

        private void FallingTrajector()
        {
            if (!m_isFalling) return;

            RaycastHit hit = new RaycastHit();
            m_direction = (m_target.position - m_basePlayer.position).normalized;
            if (Physics.Raycast(m_basePlayer.position, Vector3.down, out hit, 2.0f, m_layerMask))
            {
                m_npcHealthComponent.npcState = NpcState.MOVE;
                m_rigidbody.isKinematic = true;
                m_rigidbody.velocity = Vector3.zero;
                EndAttack();
                Debug.Log("End Test Rigidbody");
            }
        }



        private bool IsPlayerVisible()
        {
            Vector3 direction = (m_target.position - transform.position).normalized;
            return Physics.Raycast(transform.position, direction, jumpDistance, m_layerObstaclePlayer);
        }

        private void StartAttack()
        {
            m_agent.enabled = false;
            m_capsuleCollider.isTrigger = true;
            m_isFalling = false;

            Vector3 finalDashPos = Vector3.zero;
            RaycastHit hit = new RaycastHit();
            m_direction = (m_target.position - m_basePlayer.position).normalized;
            if (Physics.Raycast(m_basePlayer.position, m_direction, out hit, jumpDistance, m_layerMask))
            {
                m_isOnGround = true;
                finalDashPos = hit.point + Vector3.up * 2.0f;
            }
            else
            {
                finalDashPos = (m_target.position - transform.position).normalized * jumpDistance + transform.position;
                m_isOnGround = Physics.Raycast(finalDashPos, Vector3.down, out hit, groundMaxDistance, m_layerMask);
            }

            dest = Vector3.zero;


            if (!m_isOnGround)
            {
                dest += finalDashPos + Vector3.down * groundMaxDistance;
            }
            else
            {
                dest = hit.point;
            }

            NavMeshHit hitTest = new NavMeshHit();
            NavMesh.SamplePosition(dest, out hitTest, Mathf.Infinity, NavMesh.AllAreas);
            

            CurveInfo info = new CurveInfo();
            info.duration = jumpDuration;
            info.startPosition = transform.position;
            info.angleOfTrajectory = angleOfJump;
            info.destination = hitTest.position;
            m_curveBehavior.SendCurveInfo(info);

        }

        private void EndAttack()
        {
          
            m_agent.enabled = true;
            NavMeshHit hitTest = new NavMeshHit();
            NavMesh.SamplePosition(transform.position, out hitTest, Mathf.Infinity, NavMesh.AllAreas);
            


            m_capsuleCollider.isTrigger = false;
            m_npcHealthComponent.npcState = NpcState.RECUPERATION;
            RaycastHit hit = new RaycastHit();
            Vector3 dirTransform = m_direction;
            dirTransform.y = 0;


            Vector3 recupFinalPos = transform.position + Vector3.up * 100 + dirTransform.normalized * recuperationDistance;
            dest = recupFinalPos;
            if (Physics.Raycast(recupFinalPos, Vector3.down, out hit, Mathf.Infinity, m_layerMask))
            {
                NavMesh.SamplePosition(hit.point, out hitTest, Mathf.Infinity, NavMesh.AllAreas);
                m_agent.destination = hitTest.position ;
                m_agent.velocity = (m_agent.destination - transform.position).normalized * m_agent.speed;
                dest = hitTest.position;
                m_agent.isStopped = false;


            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, dest);
        }

    }
}
