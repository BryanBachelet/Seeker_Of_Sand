using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using GuerhoubaGames.GameEnum;
using UnityEngine.VFX;
using GuerhoubaGames.UI;
using GuerhoubaGames;
using Enemies;
namespace Character
{

    public enum AimMode
    {
        Automatic,
        AimControls,
        FullControl,

    }

    public class CharacterAim : MonoBehaviour
    {

        [HideInInspector] private Camera m_camera;
        [SerializeField] private Transform m_transformHead;
        private GameLayer m_gameLayer;
        private LayerMask combineLayer;

        [Header("Aim Feedback")]
        private int distanceMinimumProjectorVisor = 12;
        [SerializeField] private GameObject projectorVisorObject;
        [SerializeField] private Texture2D m_cursorTex;
        [SerializeField] private RectTransform m_cursor;
        [SerializeField] private LineRenderer m_lineRenderer;
        [HideInInspector] private bool m_hasCloseTarget = false;
        public Vector2 offsetCursor;

        private PlayerInput m_playerInput;
        private CharacterShoot m_characterShoot;
        private CharacterMouvement m_characterMouvement;

        private bool search;

        private Vector2 m_aimInputValue = Vector2.zero;
        private Vector2 m_aimScreenPoint = Vector2.zero;
        private const float m_aimScreenToWorldDistance = 1000.0f;
        private Vector3 m_aimFinalPoint = Vector3.zero;
        private Vector3 m_aimFinalPointFeedbackOffSet_Y = new Vector3(0, 10, 0);
        private Vector3 m_aimPoint = Vector3.zero;
        private Vector3 m_aimFinalSnapPoint = Vector3.zero;
        private Vector3 m_rawAimPoint = Vector3.zero;
        private Vector3 m_aimDirection = Vector3.zero;
        private Vector3 m_aimFinalPointNormal = Vector3.zero;
        private Vector2 m_aimFinalPointUI = Vector3.zero;
        private Vector2 m_aimInputPointUI = Vector3.zero;
        private float m_aimPointToPlayerDistance = 0;
        private int m_numberOfPointForCurveTrajectory = 50;
        private bool m_isNewTarget = false;
        private bool m_isInRange = false;

        [HideInInspector] private float aimAssistDistance = 1;
        [HideInInspector] private float aimClosetEnemyRadiusDetection = 20;

        private Vector3 prevAimPosition;

        private bool m_exitCombatState = false;
        private Vector3 v3Ref = new Vector3(1, 0, 0);

        public VisualEffect vfxStartShot;
        public VisualEffect vfxCast;
        public GameObject gameObject_vfxCastEnd;
        [HideInInspector] public VisualEffect vfxCastEnd;

        [HideInInspector] public Vector3 lastRawPosition;
        [SerializeField] private Transform basePosition;

        private GameObject m_closestEnemy;
        [SerializeField] private UI_LifeTarget m_lifeTargetUI;

        private SkinnedMeshRenderer m_lastObjectPointed;
        private GameObject m_GO_lastObject;
        public int pointedObject = 20;
        private int previewMask;
        private void Start()
        {
            //Cursor.SetCursor(m_cursorTex, Vector2.zero, CursorMode.ForceSoftware);
            m_playerInput = GetComponent<PlayerInput>();
            m_characterShoot = GetComponent<CharacterShoot>();
            m_characterMouvement = GetComponent<CharacterMouvement>();
            vfxCastEnd = gameObject_vfxCastEnd.GetComponentInChildren<VisualEffect>();
            gameObject_vfxCastEnd.transform.SetParent(this.transform.parent);
            m_camera = Camera.main;
            m_gameLayer = GameLayer.instance;
            combineLayer = m_gameLayer.groundLayerMask + m_gameLayer.interactibleLayerMask;
        }


        /// <summary>
        /// Find where the player is pointing in the 3D world
        /// </summary>
        private void FindAimWorldPoint()
        {
            if (m_characterShoot.m_aimModeState == AimMode.Automatic)
            {
                Collider[] col = new Collider[0];
                Vector3 position = Vector3.zero;

                if (m_characterShoot.m_aimModeState == AimMode.Automatic)
                    position = transform.position;

                col = Physics.OverlapSphere(position, m_characterShoot.GetPodRange(), m_gameLayer.enemisLayerMask);
                if (col.Length > 0)
                {
                    m_hasCloseTarget = true;
                    //m_characterMouvement.SetCombatMode(true);
                    Vector3 nearestEnemyPos = NearestEnemy(col, position);
                    nearestEnemyPos = m_camera.WorldToScreenPoint(nearestEnemyPos);
                    Vector2 convertPos = new Vector2(nearestEnemyPos.x, nearestEnemyPos.y);
                    m_aimScreenPoint = convertPos;
                }
                else
                {
                    m_aimScreenPoint = m_aimInputValue;
                    m_hasCloseTarget = false;
                }

            }
            else
            {
                m_aimScreenPoint = m_aimInputValue;
            }

            Ray aimRay = m_camera.ScreenPointToRay(m_aimScreenPoint);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(aimRay, out hit, m_aimScreenToWorldDistance, combineLayer))
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
            v3Ref.Normalize();
            m_aimFinalPoint = VerifyAimTrajectory(m_characterShoot.GetSpellProfil());
            m_aimDirection = (m_aimFinalPoint - basePosition.position).normalized;
            m_aimPointToPlayerDistance = (m_aimFinalPoint - basePosition.position).magnitude;
            //if(m_aimPointToPlayerDistance < distanceMinimumProjectorVisor)
            //{
            Vector3 newpos = new Vector3(m_aimFinalPoint.x, 0, m_aimFinalPoint.z) - new Vector3(transform.position.x, 0, transform.position.z);
            newpos = newpos.normalized * distanceMinimumProjectorVisor;
            projectorVisorObject.transform.position = this.transform.position + newpos + new Vector3(0, 5, 0);
            //}
            //else
            //{
            lastRawPosition = m_rawAimPoint + new Vector3(0, 5, 0);
            gameObject_vfxCastEnd.transform.position = m_rawAimPoint + new Vector3(0, 5, 0);
            //}
            projectorVisorObject.transform.LookAt(this.transform);
            gameObject_vfxCastEnd.transform.LookAt(this.transform);
            m_aimInputPointUI = m_camera.WorldToScreenPoint(m_rawAimPoint);
            m_aimFinalPointUI = m_camera.WorldToScreenPoint(m_aimFinalPoint);

            m_isNewTarget = false;
        }

        #region Aim Test Function
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
        private Vector3 VerifyAimTrajectory(SpellSystem.SpellProfil spellProfil)
        {
            if (spellProfil.TagList.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE)
            {
                m_aimPoint = CheckCurveTrajectory();
                SnapAimFinalPoint();

                return m_aimPoint;
            }
            else
                return CheckLineTrajectory();
        }

        private Vector3 CheckLineTrajectory()
        {
            Ray aimTrajectoryRay = new Ray(transform.position, m_aimDirection);
            RaycastHit hit = new RaycastHit();
            //Debug.Log("Dist :" + m_characterShoot.GetPodRange());
            if (Physics.Raycast(aimTrajectoryRay, out hit, m_characterShoot.GetPodRange(), combineLayer))
            {

                //Debug.Log("Hit obstacle" + hit.collider.name);

                m_aimPoint = hit.point;
                m_aimFinalPointNormal = hit.normal;
                Collider[] nearestAimedEnemy = Physics.OverlapSphere(m_aimPoint, aimClosetEnemyRadiusDetection, m_gameLayer.enemisLayerMask);

                if (nearestAimedEnemy.Length > 0)
                {
                    m_aimFinalPointNormal = NearestEnemy(nearestAimedEnemy, m_aimFinalPointNormal);
                }
                else
                {
                    m_closestEnemy = null;
                }
            }

            return m_aimPoint;
        }

        private Vector3 CheckCurveTrajectory()
        {
            SpellSystem.SpellProfil spellProfil = m_characterShoot.GetSpellProfil();
            float speed = GetSpeed(m_aimPointToPlayerDistance, spellProfil) * Mathf.Sin(spellProfil.GetFloatStat(StatType.AngleTrajectory) * Mathf.Deg2Rad); // Angle
            float gravity = GetGravitySpeed(0, m_aimPointToPlayerDistance, spellProfil);
            float height = ((speed * speed) / (2 * gravity));

            Vector3 dir = m_aimDirection.normalized;
            Vector3 dirRight = Quaternion.AngleAxis(90, Vector3.up) * dir;
            Vector3 normalDirection = Quaternion.AngleAxis(-90, dirRight) * dir;

            Vector3 newPos = transform.position + dir.normalized * m_aimPointToPlayerDistance * 0.5f;
            Vector3 heightestPoint = newPos + normalDirection.normalized * height;

            Ray aimTrajectoryRay = new Ray(transform.position, (heightestPoint - transform.position).normalized);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(aimTrajectoryRay, out hit, (heightestPoint - transform.position).magnitude, combineLayer))
            {
                Debug.Log("Hit obstacle" + hit.collider.name);
                m_aimPoint = hit.point;
                m_aimFinalPointNormal = hit.normal;
                return m_aimPoint;
            }

            aimTrajectoryRay = new Ray(heightestPoint, (m_aimPoint - heightestPoint).normalized);
            if (Physics.Raycast(aimTrajectoryRay, out hit, (m_aimPoint - heightestPoint).magnitude, combineLayer))
            {
                //Debug.Log("Hit obstacle" + hit.collider.name);
                m_aimPoint = hit.point;
                m_aimFinalPointNormal = hit.normal;
                return m_aimPoint;
            }

            return m_aimPoint;

        }

        private void SnapAimFinalPoint()
        {
            Ray aimTrajectoryRay = new Ray(m_aimPoint, Vector3.down);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(aimTrajectoryRay, out hit, Mathf.Infinity, combineLayer))
            {
                if (Vector3.Distance(m_aimPoint, hit.point) > 1.0f)
                {
                    m_aimPoint = hit.point;
                }
            }
        }


        #endregion

        #region Public Functions

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

        /// <summary>
        /// Give the range of between the avatar and the final aim point
        /// </summary>
        /// <returns></returns>
        public float GetRangeFromPlayerToFinalPoint() { return m_aimPointToPlayerDistance; }

        public Transform GetTransformHead() { return m_transformHead; }

        #endregion 

        private void Update()
        {
            FindAimWorldPoint();
            CalculateAimInformation();
            AimFeedback();
            //projectorVisorObject.transform.position = GetAimFinalPoint();
            prevAimPosition = GetAimFinalPoint();

            if (m_closestEnemy != null)
            {
                IDamageReceiver damageReceiver = m_closestEnemy.GetComponent<IDamageReceiver>();
                if (damageReceiver != null)
                {
                    damageReceiver.GetAfflictionManager().ShowAfflictionUI();
                    m_lifeTargetUI.ActiveLifeTarget(damageReceiver.GetLifeRatio(), damageReceiver.GetName());
                    ActiveOutlineForEnemi();
                }
            }
            else
            {
                m_lifeTargetUI.DeactiveLifeTarget();
                ResetOutlineForEnemi();
            }
            UpdateCursorPosition();
            if (search) search = false;
        }


        private void UpdateCursorPosition()
        {
            if (m_cursor != null)
            {
                m_cursor.localPosition = m_aimInputValue + offsetCursor;
            }
        }

        private void AimFeedback()
        {
            if (m_characterShoot.IsCombatMode())
            {
                m_exitCombatState = false;
                // FeedbackHeadRotation();
            }
            else if (!m_exitCombatState)
            {
                m_exitCombatState = true;
                FeedbackHeadRotationSlide();
            }


        }

        private void FeedbackLinearTrajectory()
        {
            m_lineRenderer.positionCount = 2;
            m_lineRenderer.SetPosition(1, m_aimFinalPoint);
        }

        public void FeedbackHeadRotation()
        {
            Vector3 direction2d = new Vector3(m_aimDirection.x, 0, m_aimDirection.z);
            float angleDir = Vector3.SignedAngle(m_transformHead.forward, direction2d.normalized, Vector3.up);
            m_transformHead.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
        }

        private void FeedbackHeadRotationSlide()
        {
            Vector3 direction2d = new Vector3(m_aimDirection.x, 0, m_aimDirection.z);
            float angleDir = Vector3.SignedAngle(m_transformHead.forward, m_characterShoot.m_CharacterMouvement.GetMouvementDirection(), Vector3.up);
            m_transformHead.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);

        }


        private void FeedbackCurveTrajectory(SpellSystem.SpellProfil spellProfil)
        {
            float ratio = ((spellProfil.GetFloatStat(StatType.TrajectoryTimer)) / (m_numberOfPointForCurveTrajectory + 1));

            Vector3 position = transform.position;

            m_lineRenderer.positionCount = m_numberOfPointForCurveTrajectory + 1;
            m_lineRenderer.SetPosition(0, position);

            Vector3 dir = m_aimDirection.normalized;
            Vector3 dirRight = Quaternion.AngleAxis(90, Vector3.up) * dir;
            Vector3 normalDirection = Quaternion.AngleAxis(-90, dirRight) * dir;

            for (int i = 1; i < m_numberOfPointForCurveTrajectory + 1; i++)
            {
                Vector3 newPos = Vector3.zero;
                newPos = dir.normalized * GetSpeed(m_aimPointToPlayerDistance, spellProfil) * Mathf.Cos(spellProfil.gameEffectStats.GetFloatStat(StatType.AngleTrajectory) * Mathf.Deg2Rad) * ratio; // Angle
                newPos += normalDirection * (-GetGravitySpeed(0, m_aimPointToPlayerDistance, spellProfil) * ratio * i + GetSpeed(m_aimPointToPlayerDistance, spellProfil) * Mathf.Sin(spellProfil.gameEffectStats.GetFloatStat(StatType.AngleTrajectory) * Mathf.Deg2Rad)) * ratio;
                m_lineRenderer.SetPosition(i, position + newPos);
                position += newPos;
            }
        }

        public void AimInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_aimInputValue = ctx.ReadValue<Vector2>();

                if (IsGamepad())
                {
                    Vector2 tempVec2 = m_aimInputValue;
                    m_aimInputValue.x = (tempVec2.x / 2.0f + 0.5f) * 1920;
                    m_aimInputValue.y = (tempVec2.y / 2.0f + 0.5f) * 1080;

                }
            }


            if (ctx.canceled)
            {
                if (!IsGamepad())
                {
                    m_aimInputValue = Vector2.zero;
                }
            }

        }



        public Vector3 NearestEnemy(Collider[] enemysPos, Vector3 position)
        {
            Vector3 closestPosition = Vector3.zero;
            float distance = 10000.0f;
            int highestPriority = -1;
            for (int i = 0; i < enemysPos.Length; i++)
            {
                float currentDistance = Vector3.Distance(enemysPos[i].transform.position, position);

                int currentPriority = 0;

                DamageInfoObject damageInfoObject = enemysPos[i].GetComponent<DamageInfoObject>();
                if (damageInfoObject)
                    currentPriority = damageInfoObject.priorityTarget;

                if (currentPriority >= highestPriority && currentDistance < distance)
                {
                    highestPriority = currentPriority;
                    distance = currentDistance;
                    closestPosition = enemysPos[i].transform.position;
                    m_closestEnemy = enemysPos[i].gameObject;
                }
            }
            return closestPosition;
        }
        private bool IsGamepad()
        {
            return m_playerInput.currentControlScheme == "Gamepad";
        }

        public bool HasCloseTarget()
        {
            return m_hasCloseTarget;
        }


        public float GetSpeed(float rangeGive, SpellSystem.SpellProfil spellProfil)
        {
            float speed = (rangeGive / (spellProfil.GetFloatStat(StatType.TrajectoryTimer) * Mathf.Cos(spellProfil.GetFloatStat(StatType.AngleTrajectory)) * Mathf.Deg2Rad));
            return speed;
        }

        public float GetVerticalSpeed(float rangeGive, SpellSystem.SpellProfil spellProfil)
        {
            return GetSpeed(rangeGive, spellProfil) * Mathf.Sign(spellProfil.GetFloatStat(StatType.AngleTrajectory) * Mathf.Deg2Rad);
        }

        public float GetGravitySpeed(float height, float rangeGive, SpellSystem.SpellProfil spellProfil)
        {
            float speed = GetSpeed(rangeGive, spellProfil);
            float angle = spellProfil.GetFloatStat(StatType.AngleTrajectory) * Mathf.Deg2Rad;
            float gravity = 2 * (speed * Mathf.Sin(angle) * spellProfil.GetFloatStat(StatType.TrajectoryTimer) + height);
            gravity = gravity / (spellProfil.GetFloatStat(StatType.TrajectoryTimer)) * (spellProfil.GetFloatStat(StatType.TrajectoryTimer));
            return gravity;
        }



        private void OnDrawGizmos()
        {

            if (Application.isPlaying)
            {
                SpellSystem.SpellProfil spellProfil = m_characterShoot.GetSpellProfil();
                if (spellProfil.TagList.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE)
                {
                    float speed = GetSpeed(m_aimPointToPlayerDistance, spellProfil) * Mathf.Sin(spellProfil.GetFloatStat(StatType.AngleTrajectory) * Mathf.Deg2Rad); // TODO 
                    float gravity = GetGravitySpeed(0, m_aimPointToPlayerDistance, spellProfil);
                    float height = ((speed * speed) / (2 * gravity));
                    Vector3 dir = m_aimDirection.normalized;
                    Vector3 dirRight = Quaternion.AngleAxis(90, Vector3.up) * dir;
                    Vector3 normalDirection = Quaternion.AngleAxis(-90, dirRight) * dir;
                    float angleTest = Vector3.SignedAngle(normalDirection, Vector3.up, dir);
                    if (angleTest != 0)
                    {
                        normalDirection = Quaternion.AngleAxis(angleTest, dir) * normalDirection;
                    }


                    Vector3 newPos = transform.position + dir.normalized * m_aimPointToPlayerDistance * 0.5f;
                    Vector3 heightestPoint = newPos + normalDirection.normalized * height;

                    Ray aimTrajectoryRay = new Ray(transform.position, (heightestPoint - transform.position).normalized);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, m_aimFinalPoint);
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(aimTrajectoryRay.origin, aimTrajectoryRay.direction * (heightestPoint - transform.position).magnitude);
                    Gizmos.DrawLine(heightestPoint, m_aimPoint);

                    Gizmos.DrawWireSphere(GetAimFinalPoint(), aimAssistDistance);
                    Gizmos.DrawSphere(prevAimPosition, aimAssistDistance);
                }
            }
        }

        public void ActiveOutlineForEnemi()
        {
            if (m_GO_lastObject != null)
            {
                ResetOutlineForEnemi();
            }
            m_GO_lastObject = m_closestEnemy;
            if(m_closestEnemy.GetComponent<NpcHealthComponent>())
            {
                m_lastObjectPointed = m_closestEnemy.GetComponent<NpcHealthComponent>().m_SkinMeshRenderer;
            }
            if (m_lastObjectPointed)
            {
                previewMask = (int)m_lastObjectPointed.gameObject.layer;
                m_lastObjectPointed.gameObject.layer = (int)pointedObject;
            }
           
        }

        public void ResetOutlineForEnemi()
        {
            if (m_lastObjectPointed != null) 
            { 
                m_lastObjectPointed.gameObject.layer = previewMask; 
            }
            m_GO_lastObject = null;
            m_lastObjectPointed = null;
        }

    }
}
