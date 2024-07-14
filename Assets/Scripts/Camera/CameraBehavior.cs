using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Render.Camera
{

    public enum CameraMode
    {
        THIRD_VIEW = 0,
        HIGH_VIEW = 1,
    }

    public class CameraBehavior : MonoBehaviour
    {
        [SerializeField] private Transform cameraTrainTransform;
        [SerializeField] private Character.CharacterMouvement playerMove;
        [SerializeField] private Transform m_targetTransform;
        [SerializeField] private float m_distanceToTarget;
        [HideInInspector] public Vector3 m_offsetPos;

        private Vector3 m_cameraDirection;
        private Vector3 m_baseAngle;
        const int maxDirection = 8;
        const float angleValue = 360.0f / maxDirection;
        private int indexDirection = 0;

        private Vector3 m_finalPosition;
        private Vector3 m_finalRotation;

        private float m_nextAngle;
        private Vector3 m_nextRot;
        private float m_prevAngle;
        private Vector3 m_prevRot;
        private bool m_isLerping = false;
        private float m_lerpTime = 0.3f;
        private float m_lerpTimer = 0.0f;

        public CameraMode cameraMode = CameraMode.HIGH_VIEW;
        private CameraEffect[] cameraEffects;
        public Transform sun;
        [SerializeField] private Texture2D[] m_cursorTex = new Texture2D[2];

        [Header("Cameara Collider Parameters")]
        public LayerMask obstacleLayerMask;
        // -- Test Camera Zoom ---- 

        [Header("Camera Zoom parameter")]
        [SerializeField] private float m_maxDistance = 10;
        [SerializeField] private float m_minDistance = 2;
        [SerializeField] private Vector3 m_maxAngle;
        [SerializeField] private Vector3 m_minAngle;
        [SerializeField] private float m_currentLerpValue = 1;
        [SerializeField] private float m_keyboardZoomSensibility = 1.0f;
        [SerializeField] private float m_gamepadZoomSensibility = 7.0f;
        [SerializeField] private bool m_activeCameraZoomDebug = false;
        [SerializeField] private Vector3 m_baseOffset;
        [SerializeField] private float m_valueMinToStartSlope = 0.8f;

        private float m_zoomInputGamepad = 0.0f;
        private bool m_IsGamepad = true;

        [Header("Camera Zoom High Block State parameters")]
        private bool m_isZoomBlock = false;
        [SerializeField] private float m_maxZoomBlock = 0.15f;
        [SerializeField] private float m_transitionDuration = 2;
        [SerializeField] private float m_minZoomBlock = .85f;
        private bool m_isDezoomingAutomatily;

        private float m_inputZoomValue;
        private float m_slopeAngle;
        private float m_prevSlopeAngle;
        private float m_nextSlopeAngle;

        [SerializeField] private float m_thresholdAngle = 4.0f;

        // -------------

        // -------- Test Rotation Camera Mouse ---------

        [Header("Camera Mouse Parameters")]
        [SerializeField] private float m_mousDeltaThreshold = 3.0f;
        [SerializeField] private float m_maxMouseDeltaSpeed = 500;
        [SerializeField] private float m_minMouseDeltaSpeed = 5.0f;
        [SerializeField] private float m_mouseSensibility = 1.0f;
        [SerializeField] private float m_gamepadSensibility = 1.0f;
        [SerializeField] private float m_maxAngularSpeed = 360;

        // ------------------------------


        // Free Rotation Variable
        [Header("Free Rotation Variables")]
        [SerializeField] private float m_angularSpeed = 10;
        [SerializeField] private AnimationCurve angularSpeedAcceleration;
        [SerializeField] private bool m_inverseCameraController = false;
        [SerializeField] private bool m_activateHeightDirectionMode = false;
        [SerializeField] private bool m_mouseInputActivate = true;

        private float initialAngularSpeed;
        private float timeLastRotationInput;
        private float m_currentAngle;
        private bool m_isRotationInputPress;
        private float m_mouseDeltaValue;

        private PlayerInput m_playerInputComponent;
        private Character.CharacterShoot m_characterShootComponent;
        private bool m_isActiveAutomaticDezoom = false;

        private Vector2 m_registerMousePositionRotation = Vector3.zero;

        public LayerMask maskGround;

        // Debug Value 
        private Vector3 normalDebug;
        private Vector3 hitPoint;
        private Vector3 directionDebug;
        private Ray collsionRayDebug;

        private bool setupAutoCam;

        // Start is called before the first frame update
        void Start()
        {
            cameraMode = CameraMode.HIGH_VIEW;
            m_playerInputComponent = m_targetTransform.GetComponent<PlayerInput>();
            m_characterShootComponent = m_targetTransform.GetComponent<Character.CharacterShoot>();
            initialAngularSpeed = m_angularSpeed;
            cameraEffects = GetComponents<CameraEffect>();
            m_cameraDirection = transform.position - m_targetTransform.position;
            m_baseAngle = transform.rotation.eulerAngles;
            Cursor.lockState = CursorLockMode.Confined;
        }




        void Update()
        {
            if (playerMove.mouvementState != Character.CharacterMouvement.MouvementState.Train)
            {
                if (m_isLerping)
                {
                    if (m_lerpTimer > m_lerpTime)
                    {
                        m_prevRot = m_nextRot;
                        m_prevAngle = m_nextAngle;
                        m_lerpTimer = 0.0f;
                        m_isLerping = false;
                    }
                    else
                    {
                        m_lerpTimer += Time.deltaTime;
                    }

                }
                // ------------ Camera zoom ----------
                CameraZoom();
                // ------------------------

                if (!m_activateHeightDirectionMode && m_isRotationInputPress) FreeRotation(m_mouseDeltaValue);

                SetCameraRotation();
                SetCameraPosition();
                CheckCameraTerrain();

                Apply();
            }
            else
            {
                //transform.parent = cameraTrainTransform;
                transform.position = cameraTrainTransform.position;
                Vector3 directionsun = sun.position;
                directionsun.y = m_targetTransform.position.y;
                transform.LookAt(directionsun);
            }


            //if (m_characterShootComponent.m_aimModeState != Character.AimMode.FullControl)
            //{
            //    m_isActiveAutomaticDezoom = false; 
            //}
            //else
            //{
            //    m_isActiveAutomaticDezoom = false; 
            //}
        }

        private bool IsGamepad()
        {
            return m_playerInputComponent.currentControlScheme == "Gamepad";
        }

        #region Camera Zoom Functions

        public void InputZoom(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_zoomInputGamepad = ctx.ReadValue<float>();
                if (!IsGamepad())
                {
                    UpdateZoomValue(m_keyboardZoomSensibility);
                }

            }

            if (ctx.canceled)
            {
                if (IsGamepad()) m_zoomInputGamepad = 0.0f;
                //m_inputZoomValue = m_inputZoomSensibility * ctx.ReadValue<float>();
            }
        }

        private void UpdateZoomValue(float sensibility)
        {
            m_inputZoomValue = sensibility * m_zoomInputGamepad;
            if (m_activeCameraZoomDebug) Debug.Log("Zoom Input value = " + m_inputZoomValue);

            if (m_isDezoomingAutomatily) return;

            if (!m_isZoomBlock) m_currentLerpValue += m_inputZoomValue;
            m_currentLerpValue = Mathf.Clamp(m_currentLerpValue, 0.0f, 1.0f);
            if (m_isZoomBlock) m_currentLerpValue = Mathf.Clamp(m_currentLerpValue, 0.0f, m_maxZoomBlock);

            if (m_currentLerpValue > 0.6f) cameraMode = CameraMode.THIRD_VIEW;
            else cameraMode = CameraMode.HIGH_VIEW;
        }

        private void CameraZoom()
        {

            if (IsGamepad())
            {
                UpdateZoomValue(m_gamepadZoomSensibility);
            }



            m_prevSlopeAngle = m_slopeAngle;
            float angle = playerMove.GetSlope();
            if (angle > m_thresholdAngle)
            {
                m_nextSlopeAngle = angle;
            }

            m_slopeAngle = Mathf.Lerp(m_prevSlopeAngle, m_nextSlopeAngle, 0.2f);
            Vector3 slopeAngle = new Vector3(0, 0.0f, 0);


            if (m_currentLerpValue > m_valueMinToStartSlope) slopeAngle = new Vector3(m_slopeAngle / 2, 0.0f, 0);
            m_baseAngle = Vector3.Lerp(m_maxAngle, m_minAngle, m_currentLerpValue) + slopeAngle;
            m_distanceToTarget = Mathf.Lerp(m_maxDistance, m_minDistance, m_currentLerpValue);
            m_cameraDirection = Quaternion.Euler(m_baseAngle) * -Vector3.forward;
            m_distanceToTarget = CheckGroundCamera(m_cameraDirection);

        }

        // Check if they is a ground obstacle
        public float CheckGroundCamera(Vector3 direction)
        {
            if (cameraMode != CameraMode.THIRD_VIEW) return m_distanceToTarget;

            direction = Quaternion.Euler(0.0f, m_nextAngle, 0.0f) * direction;
            directionDebug = direction;
            RaycastHit hit = new RaycastHit();
            float targetDistance = m_distanceToTarget;
            Ray ray = new Ray(m_targetTransform.position, direction.normalized);
            collsionRayDebug = ray;
            if (Physics.Raycast(ray, out hit, targetDistance, obstacleLayerMask))
            {
                float distance = Vector3.Distance(m_targetTransform.position, hit.point) - 0.3f;
                return distance;
            }

            return m_distanceToTarget;
        }


        private Vector3 GetForwardDirection(Vector3 normal)
        {
            return Vector3.Cross(transform.right, normal);
        }

        private float GetSlopeAngle(Vector3 direction)
        {
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            return Vector3.SignedAngle(rotTest * Vector3.forward, direction, Vector3.right);
        }

        IEnumerator DeZoomCamera()
        {

            // Setup a bool 
            m_isDezoomingAutomatily = true;
            float zoonDelta = m_currentLerpValue - m_maxZoomBlock;
            float speed = zoonDelta * (1 / m_transitionDuration);
            while (m_currentLerpValue > m_maxZoomBlock)
            {
                m_currentLerpValue -= speed * Time.deltaTime;
                yield return Time.deltaTime;
            }
            m_isDezoomingAutomatily = false;
            //Reset bool

        }

        IEnumerator ZoomCloseCamera()
        {
            m_isDezoomingAutomatily = true;
            float zoonDelta = m_minZoomBlock - m_currentLerpValue;
            float speed = zoonDelta * (1 / m_transitionDuration);
            while (m_currentLerpValue < m_minZoomBlock)
            {
                m_currentLerpValue += speed * Time.deltaTime;
                yield return Time.deltaTime;
            }
            m_isDezoomingAutomatily = false;
        }

        public void BlockZoom(bool state)
        {
            if (state == m_isZoomBlock) return;

            m_isZoomBlock = state;

            //Debug.Log("Zoom");
            if (!m_isZoomBlock || !m_isActiveAutomaticDezoom) return;

            StartCoroutine(DeZoomCamera());

        }


        #endregion

        public float GetAngle()
        {
            return m_currentAngle;
        }

        #region Camera Rotation Functions


        public void RotationAimInput(InputAction.CallbackContext ctx)
        {
            if (m_mouseInputActivate && this.enabled)
            {

                int value = 1;
                if (m_inverseCameraController) value = -1;

                if (!IsGamepad()) m_mouseDeltaValue = value * m_mouseSensibility * ctx.ReadValue<float>();
                else m_mouseDeltaValue = value * m_gamepadSensibility * ctx.ReadValue<float>();

                if (Mathf.Abs(m_mouseDeltaValue) < m_mousDeltaThreshold) m_mouseDeltaValue = 0;
                //if (m_activeDebugMouseRotation) Debug.Log("Mouse Delta = " + m_mouseDeltaValue.ToString());

            }
            else
            {

            }

            //if (ctx.canceled && m_mouseInputActivate && this.enabled)
            //{
            //    //Cursor.lockState = CursorLockMode.None;
            //    Debug.Log("Unlock Cursor");
            //    //m_mouseDeltaValue = 0.0f;
            //    //if (m_activeDebugMouseRotation) Debug.Log("Mouse Delta = " + m_mouseDeltaValue.ToString());
            //}
        }

        public void RotationMouseInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && m_mouseInputActivate)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //Cursor.visible = false;
                Cursor.SetCursor(m_cursorTex[0], Vector2.zero, CursorMode.ForceSoftware);
                m_isRotationInputPress = true;
                m_registerMousePositionRotation = Mouse.current.position.value;

            }
            if (ctx.canceled && m_mouseInputActivate)
            {
                Cursor.SetCursor(m_cursorTex[1], Vector2.zero, CursorMode.ForceSoftware);
                m_isRotationInputPress = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Mouse.current.WarpCursorPosition(m_registerMousePositionRotation);
            }

        }

        public Vector3 TurnDirectionForCamera(Vector3 direction)
        {
            return Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * direction;
        }


        private void ChangeRotation(bool state)
        {
            if (m_isLerping)
            {
                m_lerpTimer = m_lerpTime / 1.5f;
            }
            else m_lerpTimer = 0.0f;


            if (!m_isLerping)
            {
                int prevIndex = indexDirection;
                if (prevIndex + 1 == maxDirection)
                    prevIndex = -1;
                if (prevIndex - 1 == -maxDirection)
                    prevIndex = 1;

                m_prevAngle = angleValue * prevIndex;
                m_prevRot = new Vector3(0.0f, angleValue * prevIndex, 0.0f);
                m_isLerping = true;
            }

            if (state) indexDirection++;
            else indexDirection--;

            indexDirection = indexDirection % maxDirection;
            m_currentAngle = angleValue * indexDirection;
            m_nextRot = new Vector3(0.0f, angleValue * indexDirection, 0.0f);
            m_nextAngle = angleValue * indexDirection;
        }


        private void FreeRotation(float sign)
        {
            if (sign == 0) return;
            sign = Mathf.Sign(sign);
            float deltaInputMove = Time.time - timeLastRotationInput;

            if (deltaInputMove < 1)
            {
                m_angularSpeed = angularSpeedAcceleration.Evaluate(deltaInputMove);
            }
            else
            {
                m_angularSpeed = initialAngularSpeed;
            }
            float ratio = Mathf.Clamp(Mathf.Abs(m_mouseDeltaValue), m_minMouseDeltaSpeed, m_maxMouseDeltaSpeed) / m_maxMouseDeltaSpeed;

            float angularSpeed = m_maxAngularSpeed * ratio;
            m_prevAngle = m_currentAngle;
            m_prevRot = new Vector3(0.0f, m_currentAngle, 0.0f);

            m_currentAngle += sign * angularSpeed * Time.deltaTime;

            m_nextAngle = m_currentAngle;
            m_nextRot = new Vector3(0.0f, m_currentAngle, 0.0f);
            if (m_isLerping)
            {
                m_lerpTimer = m_lerpTime / 1.5f;
            }
            else m_lerpTimer = 0.0f;
        }

        public void SetupCamaraAnglge(float angle)
        {
            setupAutoCam = true;
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        #endregion

        private void CheckCameraTerrain()
        {
            RaycastHit hit = new RaycastHit();

            Vector3 direction = transform.position - m_targetTransform.position;
            if (Physics.Raycast(m_targetTransform.position, direction, out hit, direction.magnitude + 5f, maskGround))
            {

                m_prevAngle = m_currentAngle;
                m_prevRot = new Vector3(0.0f, m_currentAngle, 0.0f);

                m_currentAngle += 1 * 60 * Time.deltaTime;

                m_nextAngle = m_currentAngle;
                m_nextRot = new Vector3(0.0f, m_currentAngle, 0.0f);
                m_lerpTimer = 0.1f;

                SetCameraRotation();
                SetCameraPosition();
            }
        }

        private void SetCameraRotation()
        {
            m_finalRotation = m_baseAngle + Vector3.Lerp(m_prevRot, m_nextRot, m_lerpTimer / m_lerpTime);
            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalRotation += cameraEffects[i].GetEffectRot();
            }
        }
        private void SetCameraPosition()
        {
            m_finalPosition = m_targetTransform.position;
            m_finalPosition += Quaternion.Euler(0.0f, Mathf.Lerp(m_prevAngle, m_nextAngle, m_lerpTimer / m_lerpTime), 0.0f) * m_cameraDirection.normalized * m_distanceToTarget;
            if (cameraMode == CameraMode.THIRD_VIEW) m_finalPosition += m_baseOffset;
            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalPosition += cameraEffects[i].GetEffectPos();
            }

        }
        void Apply()
        {
            transform.position = m_finalPosition;

            if (setupAutoCam)
            {
                setupAutoCam = false;
                return;
            }
            transform.rotation = Quaternion.Euler(m_finalRotation);
        }


        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(hitPoint, normalDebug * 100);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(collsionRayDebug.origin, collsionRayDebug.direction * 100);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_targetTransform.position, directionDebug * m_distanceToTarget);
        }
    }
}
