using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Render.Camera
{
    public class CameraBehavior : MonoBehaviour
    {
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


        private CameraEffect[] cameraEffects;
        // Start is called before the first frame update
        void Start()
        {
            cameraEffects = GetComponents<CameraEffect>();
            m_cameraDirection = transform.position - m_targetTransform.position;
            m_baseAngle = transform.rotation.eulerAngles;
        }

        void Update()
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

            SetCameraRotation();
            SetCameraPosition();
            Apply();
        }

        public float GetAngle()
        {
            return angleValue * indexDirection;
        }
        public void RotationInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started && !m_isLerping)
            {
                float value = ctx.ReadValue<float>();
                if (value > 0) ChangeRotation(true);
                if (value < 0) ChangeRotation(false);
            }
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
                if (prevIndex -1 == -maxDirection)
                    prevIndex = 1;

                m_prevAngle = angleValue * prevIndex;
                m_prevRot = new Vector3(0.0f, angleValue * prevIndex, 0.0f);
                m_isLerping = true;
            }

            if (state) indexDirection++;
            else indexDirection--;
            
            indexDirection = indexDirection % maxDirection;
            
            

            m_nextRot = new Vector3(0.0f, angleValue * indexDirection, 0.0f);
            m_nextAngle = angleValue * indexDirection;
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
