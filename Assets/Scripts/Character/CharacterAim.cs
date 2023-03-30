using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{

    public class CharacterAim : MonoBehaviour
    {

        [SerializeField] [Range(0.0f, 1.0f)] private float inputAim = 0.6f;
        [SerializeField] private Camera m_camera;
        private PlayerInput m_playerInput;
        private Vector2 m_aimInputValue;
        private Vector2 m_aimValueTransform;
        private CharacterMouvement m_characterMouvement;
        private CharacterShoot m_characterShoot;
        [SerializeField] private Texture2D m_cursorTex;
        [SerializeField] private LineRenderer m_lineRenderer;
        [SerializeField] private Transform m_transformHead;
        [SerializeField] private LayerMask m_aimLayer;
        [SerializeField] private RectTransform m_cursor;
        public GameObject projectorVisorObject;
        private Ray cameRay;

        private Vector3 mouseHitPointWorldSpace;
        private bool search;

        private Vector2 m_aimScreenPoint = Vector2.zero;
        private const float aimScreenToWorldDistance = 1000.0f;
        private Vector3 m_aimFinalPoint = Vector3.zero;
        private Vector3 m_aimPoint = Vector3.zero;
        private Vector3 m_rawAimPoint = Vector3.zero;
        private Vector3 m_aimDirection = Vector3.zero;
        private Vector3 m_aimFinalPointNormal = Vector3.zero;
        private Vector2 m_aimFinalPointUI = Vector3.zero;
        private Vector2 m_aimInputPointUI = Vector3.zero;
        private float m_aimPointToPlayerDistance = 0;
        private int m_numberOfPointForCurveTrajectory = 50;
        private bool m_isNewTarget = false;
        private bool m_isInRange = false;

        private void Start()
        {
            Cursor.SetCursor(m_cursorTex, Vector2.zero, CursorMode.Auto);
            //Cursor.visible = false;
            m_playerInput = GetComponent<PlayerInput>();
            m_characterMouvement = GetComponent<CharacterMouvement>();
            m_characterShoot = GetComponent<CharacterShoot>();
        }


        /// <summary>
        /// Find where the player is pointing in the 3D world
        /// </summary>
        private void FindAimWorldPoint()
        {
            m_aimScreenPoint = m_aimInputValue;
            Ray aimRay = m_camera.ScreenPointToRay(m_aimScreenPoint);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(aimRay, out hit, aimScreenToWorldDistance, m_aimLayer))
            {
                m_rawAimPoint = m_aimPoint = hit.point;

                m_isNewTarget = true;
                m_aimFinalPointNormal = hit.normal;
            }
        }

        private void CalculateAimInformation()
        {
            if (!m_isNewTarget) return;

            CheckAimPointDistance();

            m_aimFinalPoint = VerifyAimTrajectory();
            m_aimDirection = (m_aimFinalPoint - transform.position).normalized;
            m_aimPointToPlayerDistance = (m_aimFinalPoint - transform.position).magnitude;
            m_aimInputPointUI = m_camera.WorldToScreenPoint(m_rawAimPoint);
            m_aimFinalPointUI = m_camera.WorldToScreenPoint(m_aimFinalPoint);

            m_isNewTarget = false;
        }

        private void CheckAimPointDistance()
        {
            m_isInRange = true;
            m_aimDirection = (m_aimPoint - transform.position).normalized;
            m_aimPointToPlayerDistance = (m_aimPoint - transform.position).magnitude;

            if (m_aimPointToPlayerDistance <= m_characterShoot.GetPodRange()) return;

            m_aimPointToPlayerDistance = m_characterShoot.GetPodRange();
            m_isInRange = false;
            m_aimPoint = transform.position + m_aimDirection * m_characterShoot.GetPodRange();

        }

        /// <summary>
        ///   Check if any obstacle on the aim trajectory and adapt the aim final point
        /// </summary>
        private Vector3 VerifyAimTrajectory()
        {
            Ray aimTrajectoryRay = new Ray(transform.position, m_aimDirection);
            RaycastHit hit = new RaycastHit();
            Debug.Log("Dist :" + m_characterShoot.GetPodRange());
            if (Physics.Raycast(aimTrajectoryRay, out hit, m_characterShoot.GetPodRange(), m_aimLayer))
            {

                Debug.Log("Hit obstacle" + hit.collider.name);

                m_aimPoint = hit.point;
                m_aimFinalPointNormal = hit.normal;
            }

            return m_aimPoint;
        }

        private Vector3 CheckLineTrajectory()
        {
            Ray aimTrajectoryRay = new Ray(transform.position, m_aimDirection);
            RaycastHit hit = new RaycastHit();
            Debug.Log("Dist :" + m_characterShoot.GetPodRange());
            if (Physics.Raycast(aimTrajectoryRay, out hit, m_characterShoot.GetPodRange(), m_aimLayer))
            {

                Debug.Log("Hit obstacle" + hit.collider.name);

                m_aimPoint = hit.point;
                m_aimFinalPointNormal = hit.normal;
            }

            return m_aimPoint;
        }

        private Vector3 CheckCurveTrajectory()
        {
            return m_aimPoint;
        }

        /// <summary>
        /// Get the current aiming direction
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAimDirection() { return m_aimDirection; }
        
        /// <summary>
        /// Give the final location of aiming or the location of the collision if the sight is obstructed
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAimFinalPoint() { return m_aimFinalPoint; }

        /// <summary>
        /// Get the location pointed by the mouse in the 3D world space
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAimInputPoint() { return m_rawAimPoint; }

        /// <summary>
        /// Send the normal of the final location
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNormalFinalPoint() { return m_aimFinalPointNormal; }

        /// <summary>
        /// Get the final location of aiming in the screen space
        /// </summary>
        /// <returns></returns>
        public Vector2 GetAimFinalPointUI() { return m_aimFinalPointNormal; }

        /// <summary>
        /// Give the screen location of mouse 
        /// </summary>
        /// <returns></returns>
        public Vector2 GetAimInputPointUI() { return m_aimFinalPointNormal; }


        private void Update()
        {
            // == New Version ===
            FindAimWorldPoint();
            CalculateAimInformation();
            AimFeedback();
            // == old version
            if (m_cursor != null)
            {
                m_cursor.position = Input.mousePosition;
            }
            if (search) search = false;
        }

        public Transform GetTransformHead()
        {
            return m_transformHead;
        }
        private void AimFeedback()
        {
             m_lineRenderer.SetPosition(0, transform.position);
            if (m_characterShoot.GetPod().trajectory == TrajectoryType.LINE)
            {
                m_lineRenderer.positionCount = 2;
                Vector3 direction2d = new Vector3(m_aimDirection.x, 0, m_aimDirection.z);
                float angleDir = Vector3.SignedAngle(m_transformHead.forward, direction2d.normalized, Vector3.up);
                m_transformHead.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
                RaycastHit hit = new RaycastHit();
                float distance = m_characterShoot.weaponStat.range;
                if (Physics.Raycast(transform.position, m_aimDirection.normalized * distance, out hit, m_aimLayer))
                {
                    distance = (hit.point - transform.position).magnitude;
                }

                m_lineRenderer.SetPosition(1, m_aimFinalPoint);
            }
            else
            {

                FeedbackCurveTrajectory(m_characterShoot.GetPod());
            }
            Cursor.visible = true;

        }


        private void FeedbackCurveTrajectory(CapsuleStats stats)
        {
            float ratio = ((stats.lifetime-0.1f) / m_numberOfPointForCurveTrajectory);

            Vector3 position = transform.position;
            m_lineRenderer.positionCount = m_numberOfPointForCurveTrajectory;
            m_lineRenderer.SetPosition(0, position);

            Vector3 dir = new Vector3(m_aimDirection.x, 0, m_aimDirection.z);
            
            for (int i = 1; i < m_numberOfPointForCurveTrajectory; i++)
            {
                Vector3 newPos = Vector3.zero;
                newPos = dir.normalized *  stats.GetSpeed(m_aimPointToPlayerDistance) * Mathf.Cos(45 * Mathf.Deg2Rad) * ratio;
                newPos.y = (-stats.GetGravitySpeed(transform.position.y-m_aimFinalPoint.y, m_aimPointToPlayerDistance) * ratio * i + stats.GetSpeed(m_aimPointToPlayerDistance) * Mathf.Cos(45 * Mathf.Deg2Rad)) * ratio;
                m_lineRenderer.SetPosition(i, position + newPos);
                position += newPos;
            }
        }

        public void AimInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_aimInputValue = ctx.ReadValue<Vector2>();

            }
            if (ctx.canceled) m_aimInputValue = Vector2.zero;

            m_aimDirection = GetAimPoint();
        }

        public Vector3 GetAim()
        {
            return (GetAimDestination() - transform.position).normalized;
        }

        public float GetAimMagnitude() { return m_aimValueTransform.magnitude; }


        public Vector3 GetAimDestination()
        {
            if (search) return mouseHitPointWorldSpace;
            Vector3 aimValue = new Vector2(m_aimInputValue.x, m_aimInputValue.y);
            search = true;
            Ray aimRay = m_camera.ScreenPointToRay(aimValue);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(aimRay, out hit, 1000.0f, m_aimLayer.value))
            {
                mouseHitPointWorldSpace = hit.point;
                return mouseHitPointWorldSpace;
            }
            else
            {
                mouseHitPointWorldSpace = transform.position + GetAim() * m_characterShoot.weaponStat.range;
                return transform.position + GetAim() * m_characterShoot.weaponStat.range;
            }
        }



        private Vector3 GetAimPoint()
        {
            if (!IsGamepad())
            {
                Resolution currentResolution = Screen.currentResolution;
                m_aimValueTransform = new Vector2(m_aimInputValue.x, m_aimInputValue.y);
                Ray aimRay = m_camera.ScreenPointToRay(m_aimValueTransform);
                m_aimValueTransform = new Vector2(m_aimValueTransform.x - (currentResolution.width / 2.0f), m_aimValueTransform.y - (currentResolution.height / 2.0f));
                m_aimValueTransform = new Vector2(m_aimValueTransform.x / (currentResolution.width / 2.0f), m_aimValueTransform.y / (currentResolution.height / 2.0f));


                cameRay = aimRay;
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(cameRay, out hit, 150.0f, m_aimLayer.value))
                {
                    if (projectorVisorObject)                                               // Decal Projector for visor positionning 
                    {                                                                       //
                        projectorVisorObject.transform.position = hit.point;                // 
                    }                                                                       //
                    return ((hit.point + Vector3.up) - transform.position);

                }
                return m_characterMouvement.currentDirection;
            }
            return m_aimInputValue;
        }

        private bool IsGamepad()
        {
            return m_playerInput.currentControlScheme == "Gamepad";
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, GetAimDestination());
            Vector3 aimValue = new Vector2(m_aimInputValue.x, m_aimInputValue.y);
            Ray aimRay = m_camera.ScreenPointToRay(aimValue);
            Gizmos.DrawRay(m_camera.transform.position, aimRay.direction * 1000.0f);
        }

    }
}
