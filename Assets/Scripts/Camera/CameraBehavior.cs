using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Render.Camera
{
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
        private bool attachedToTrain = false;


        private CameraEffect[] cameraEffects;
        public Transform sun;

        // Free Rotation Variable
        [Header("Free Rotation Variables")]
        [SerializeField] private float m_angularSpeed = 10;
        private float initialAngularSpeed;
        [SerializeField] private AnimationCurve angularSpeedAcceleration;
        private float timeLastRotationInput;
        private float m_currentAngle;
        [SerializeField] private bool m_activateHeightDirectionMode = false;
        [SerializeField] private bool m_mouseInputActivate = false;
        private bool m_isRotationInputPress;
        private float m_signValue;




        // Start is called before the first frame update
        void Start()
        {
            initialAngularSpeed = m_angularSpeed;
            cameraEffects = GetComponents<CameraEffect>();
            m_cameraDirection = transform.position - m_targetTransform.position;
            m_baseAngle = transform.rotation.eulerAngles;
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

                if (!m_activateHeightDirectionMode && m_isRotationInputPress) FreeRotation(m_signValue);

                SetCameraRotation();
                SetCameraPosition();
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

        }

        public float GetAngle()
        {
            return m_currentAngle;
        }
        public void RotationInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && !m_mouseInputActivate)
            {

                float value = ctx.ReadValue<float>();
                m_signValue = value;
                m_isRotationInputPress = true;
                timeLastRotationInput = Time.time;
                if (value > 0)
                {
                    if (m_activateHeightDirectionMode && !m_isLerping) ChangeRotation(true);

                }
                if (value < 0)
                {
                    if (m_activateHeightDirectionMode && !m_isLerping) ChangeRotation(false);
                }
            }

            if (ctx.canceled && !m_mouseInputActivate)
            {
                m_isRotationInputPress = false;
                float value = ctx.ReadValue<float>();
            }
        }

        public void RotationAimInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && m_mouseInputActivate)
            {
                m_signValue = ctx.ReadValue<Vector2>().x;
            }
        }

        public void RotationMouseInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && m_mouseInputActivate)
            {
                m_isRotationInputPress = true;
          
            }
            if (ctx.canceled && m_mouseInputActivate)
            {
                m_isRotationInputPress = false;
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
            Debug.Log("Angular speed [" + m_angularSpeed + "]");
            m_prevAngle = m_currentAngle;
            m_prevRot = new Vector3(0.0f, m_currentAngle, 0.0f);

            m_currentAngle += sign * m_angularSpeed * Time.deltaTime;

            m_nextAngle = m_currentAngle;
            m_nextRot = new Vector3(0.0f, m_currentAngle, 0.0f);
            if (m_isLerping)
            {
                m_lerpTimer = m_lerpTime / 1.5f;
            }
            else m_lerpTimer = 0.0f;
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
            m_finalPosition += Quaternion.Euler(0.0f, Mathf.Lerp(m_prevAngle, m_nextAngle, m_lerpTimer / m_lerpTime), 0.0f) * m_cameraDirection.normalized * m_distanceToTarget; ;

            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalPosition += cameraEffects[i].GetEffectPos();
            }

        }
        void Apply()
        {
            transform.position = m_finalPosition;
            transform.rotation = Quaternion.Euler(m_finalRotation);
        }
    }
}
