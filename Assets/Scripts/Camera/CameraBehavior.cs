using FMODUnity;
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
        [SerializeField] private Character.CharacterMouvement playerMove;
        [SerializeField] private Transform m_targetTransform;  
        [SerializeField] public float m_distanceToTarget = 80;
        [SerializeField] private Vector3 m_directionCamera;
        [SerializeField] private Vector3 m_angleCamera;
        [SerializeField] private Vector3 m_offsetCamera;
 




        const int maxDirection = 8;
        const float angleValue = 360.0f / maxDirection;
        private int indexDirection = 0;
        private float modifyTargetDistance;

        // Oriented Camera Variable 
        [Header("Oriented Camera Parameter")]
        public float cameraOrientedAngleSpeed = 5f;
        public int minFrameCamOriented = 5; 
        
        private bool m_isOrientedCamera;
        private Quaternion m_orientedCameraQuat;
        private Vector3 m_orientedAngle;
        private Vector2 m_stickDir2D;

        private Vector3 m_startOrientedInputValue = Vector3.zero;
        private Vector3 m_currentOrientedInputValue = Vector3.forward;
        private Vector3 m_targetOrientedInputValue = Vector3.zero;

        private float m_startOrientedInputAngle = 0.0f;
        private float m_currentOrientedInputAngle = 0.0f;
        private float m_targetOrientedInputAngle = 0.0f;

        private int frameToEndLerp;
        private int frameCount;

        // -----------  

        private Vector3 m_finalPosition;
        private Vector3 m_finalRotation;

        private float m_nextAngle;
        private Vector3 m_nextRot;
        private float m_prevAngle;
        private Vector3 m_prevRot;
        private bool m_isLerping = false;
        private float m_lerpTime = 0.3f;
        private float m_lerpTimer = 0.0f;

        [Space]
        public CameraMode cameraMode = CameraMode.HIGH_VIEW;
        private CameraEffect[] cameraEffects;
        [SerializeField] private Texture2D[] m_cursorTex = new Texture2D[2];

        [Header("Cameara Collider Parameters")]
        [HideInInspector] private GameLayer m_gameLayer;
        // -- Test Camera Zoom ---- 

        private float m_zoomInputGamepad = 0.0f;
        private bool m_IsGamepad = true;

        [Header("Camera Zoom High Block State parameters")]
        private bool m_isZoomBlock = false;
        [HideInInspector] private float m_maxZoomBlock = 0f;
        [HideInInspector] private float m_transitionDuration = 2;
        [HideInInspector] private float m_minZoomBlock = .85f;
        private bool m_isDezoomingAutomatily;

        private float m_inputZoomValue;
        private float m_slopeAngle;
        private float m_prevSlopeAngle;
        private float m_nextSlopeAngle;

        [HideInInspector] private float m_thresholdAngle = 4.0f;

        // -------------

        // -------- Test Rotation Camera Mouse ---------

        [Header("Camera Mouse Parameters")]
        [HideInInspector] private float m_mousDeltaThreshold = 3.0f;
        [HideInInspector] private float m_maxMouseDeltaSpeed = 200;
        [HideInInspector] private float m_minMouseDeltaSpeed = 5.0f;
        [HideInInspector] private float m_mouseSensibility = 0.8f;
        [HideInInspector] private float m_gamepadSensibility = 30;
        [HideInInspector] private float m_maxAngularSpeed = 720;

        [HideInInspector] private float m_sensibility = 0.8f;
        [HideInInspector] private float m_angleSpeed = 0.5f;
        // ------------------------------


        // Free Rotation Variable
        [Header("Free Rotation Variables")]
        [HideInInspector] private float m_angularSpeed = 200;
        [SerializeField] private AnimationCurve angularSpeedAcceleration;
        [HideInInspector] private bool m_inverseCameraController = false;
        [HideInInspector] private bool m_activateHeightDirectionMode = false;
        [HideInInspector] private bool m_mouseInputActivate = true;

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

        static public Vector3 mousePositionOnScreen;
        // Start is called before the first frame update
        void Start()
        {
            cameraMode = CameraMode.HIGH_VIEW;
            m_gameLayer = GameLayer.instance;

            m_playerInputComponent = m_targetTransform.GetComponent<PlayerInput>();
            m_characterShootComponent = m_targetTransform.GetComponent<Character.CharacterShoot>();

            initialAngularSpeed = m_angularSpeed;
           
            cameraEffects = GetComponents<CameraEffect>();

            // Setup cursor state 
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
          

                if (!m_activateHeightDirectionMode && m_isRotationInputPress) FreeRotation(m_mouseDeltaValue);
                if (m_isOrientedCamera && m_isRotationInputPress) OrientedCameraRotation();

                 SetCameraRotation();
                SetCameraPosition();
                CheckCameraTerrain();

                Apply();
            }


        }

        private bool IsGamepad()
        {
            return m_playerInputComponent.currentControlScheme == "Gamepad";
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


        public float GetAngle()
        {
            return m_currentAngle;
        }

        #region Camera Rotation Functions


        public void RotationCameraOrientedInput(InputAction.CallbackContext ctx)
        {
            if (!IsGamepad()) return;

            if (ctx.performed && this.enabled)
            {
                // 1. Need to verified to reset lerp 
                // 2. Determine Speed to lerp;
                // 2. Lerp system 

                m_isOrientedCamera = true;
                Vector2 stickDir2D = ctx.ReadValue<Vector2>();
                if (stickDir2D.magnitude < .25f)
                {
                    m_isOrientedCamera = true;
                    m_mouseDeltaValue = 0.0f;
                    m_isRotationInputPress = false;
                    return;
                }
                m_stickDir2D = stickDir2D.normalized;
                m_isRotationInputPress = true;





            }

            if (ctx.canceled && this.enabled)
            {
                m_mouseDeltaValue = 0.0f;
                m_isRotationInputPress = false;
                m_isOrientedCamera = true;
            }
        }


        public void RotationAimInput(InputAction.CallbackContext ctx)
        {
            if (!GameState.IsPlaying()) return;
            if (m_mouseInputActivate && this.enabled && ctx.performed)
            {
                int value = 1;
                if (m_inverseCameraController) value = -1;
                m_isOrientedCamera = false;

                if (!IsGamepad())
                {
                    m_mouseDeltaValue = value * (m_mouseSensibility * m_sensibility) * ctx.ReadValue<float>();
                }
                else
                {
                    m_mouseDeltaValue = value * (m_gamepadSensibility * m_sensibility) * ctx.ReadValue<float>();
                    //Debug.Log("Is gamepad ");

                }


                if (Mathf.Abs(m_mouseDeltaValue) < m_mousDeltaThreshold) m_mouseDeltaValue = 0;
                //if (m_activeDebugMouseRotation) Debug.Log("Mouse Delta = " + m_mouseDeltaValue.ToString());

            }
            else
            {
                m_mouseDeltaValue = 0;
            }


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

        private void OrientedCameraRotation()
        {
            Vector3 stickDir3D = new Vector3(m_stickDir2D.x, 0, m_stickDir2D.y);
            Vector3 deltaInputDirecton = m_targetOrientedInputValue + stickDir3D;

            if (deltaInputDirecton.magnitude > .1f)
            {
                m_targetOrientedInputValue = -stickDir3D;
                Vector3 cameraBaseDirection = new Vector3(0.0f, 0.0f, m_directionCamera.z);

                m_startOrientedInputValue = m_orientedCameraQuat * cameraBaseDirection.normalized;
                m_startOrientedInputAngle = Vector3.SignedAngle(-Vector3.forward,m_startOrientedInputValue, Vector3.up);

                float angle = Vector3.SignedAngle(m_startOrientedInputValue, m_targetOrientedInputValue, Vector3.up);

                frameToEndLerp = (int)(Mathf.Abs(angle) / cameraOrientedAngleSpeed);
                frameToEndLerp = Mathf.Clamp(frameToEndLerp, minFrameCamOriented, 1000);
                frameCount = 0;
                
            }else
            {
                if (frameCount <= frameToEndLerp)
                {
                    frameCount++;
                    float ratio = ((float)frameCount / (float)frameToEndLerp);
                    ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);
                    m_currentOrientedInputAngle = m_startOrientedInputAngle + Vector3.SignedAngle(m_startOrientedInputValue, m_targetOrientedInputValue, Vector3.up) * ratio;
                }

            }



            m_orientedAngle.y = m_currentOrientedInputAngle;
            m_currentAngle = m_currentOrientedInputAngle;
            m_orientedCameraQuat = Quaternion.Euler(0, m_currentOrientedInputAngle, 0);

        }


        private void FreeRotation(float sign)
        {
            if (m_isOrientedCamera) return;

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
            m_prevAngle = angle;
            m_nextAngle = angle;
            m_currentAngle = angle;
            m_prevRot = new Vector3(0.0f, m_currentAngle, 0.0f);
            m_nextRot = new Vector3(0.0f, m_currentAngle, 0.0f);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);
            SetCameraPosition();
            transform.position = m_finalPosition;
            setupAutoCam = false;
        }

        #endregion

        private void CheckCameraTerrain()
        {
            RaycastHit hit = new RaycastHit();
            RaycastHit hit2 = new RaycastHit();

            Vector3 direction = (transform.position+ Vector3.down) - m_targetTransform.position;
            Vector3 dir2 = (m_targetTransform.position +  m_directionCamera.normalized * m_distanceToTarget + Vector3.down) - m_targetTransform.position;

            bool hasHit2 =    Physics.Raycast(m_targetTransform.position + m_directionCamera.normalized * m_distanceToTarget + Vector3.down, -dir2, out hit2, m_distanceToTarget + 5f, m_gameLayer.groundLayerMask);
            bool hasHit = Physics.Raycast(m_targetTransform.position, direction, out hit, m_distanceToTarget + 5f, m_gameLayer.groundLayerMask);
            if (hasHit && (hit2.point -hit.point).magnitude >0.5f /*|| hasHit && !hasHit2*/)
            {
                modifyTargetDistance = ( m_targetTransform.position - hit.point).magnitude;
                 SetCameraRotation();
                SetCameraPosition(true);
            }
        }

        private void SetCameraRotation()
        {
          

            if (!m_isOrientedCamera)
            {
                m_finalRotation = m_angleCamera + Vector3.Lerp(m_prevRot, m_nextRot, m_lerpTimer / m_lerpTime);
            }
            else
            {
                m_finalRotation = m_angleCamera + m_orientedAngle;
            }

            if (cameraEffects == null) return;
            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalRotation += cameraEffects[i].GetEffectRot();
            }
        }
        private void SetCameraPosition(bool IsDistanceTargetModify = false)
        {
            if (!m_isOrientedCamera)
            {
                m_finalPosition = m_targetTransform.position;
                float distance = m_distanceToTarget;
                if(IsDistanceTargetModify)
                {
                    distance = modifyTargetDistance;
                }
                m_finalPosition += Quaternion.Euler(0.0f, Mathf.Lerp(m_prevAngle, m_nextAngle, m_lerpTimer / m_lerpTime), 0.0f) * m_directionCamera.normalized * distance;
                //if (cameraMode == CameraMode.THIRD_VIEW) m_finalPosition += m_baseOffset;
            }
            else
            {
                m_finalPosition = m_targetTransform.position;
                m_finalPosition += m_orientedCameraQuat * m_directionCamera.normalized * m_distanceToTarget;
            }
            if (cameraEffects == null) return;
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
            transform.position += m_offsetCamera;
        }

        public void UpdateSettingRotationSpeed(float sensibility)
        {
            m_sensibility = sensibility;
        }

        public void UpdateSettingAngleSpeed(float angleSpeed)
        {
            m_angleSpeed = angleSpeed;
        }
        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(hitPoint, normalDebug * 100);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(collsionRayDebug.origin, collsionRayDebug.direction * 100);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_targetTransform.position, directionDebug * m_distanceToTarget);
        }

        public void ChangeLerpForTP()
        {
           
        }


        public void OnValidate()
        {
            SetCameraRotation();
            SetCameraPosition();
            Apply();
            m_directionCamera = m_directionCamera.normalized;
        }


    }

}
