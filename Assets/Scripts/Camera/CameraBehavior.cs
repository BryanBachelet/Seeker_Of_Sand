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
            if (ctx.started)
            {
                float value = ctx.ReadValue<float>();
                if (value > 0) ChangeRotation(true);
                if (value < 0) ChangeRotation(false);
            }
        }

        private void ChangeRotation(bool state)
        {
            if (state) indexDirection++;
            else indexDirection--;

            indexDirection = indexDirection % maxDirection;
        }


        private void SetCameraRotation()
        {
            m_finalRotation = m_baseAngle + new Vector3(0.0f, angleValue * indexDirection, 0.0f);
            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalRotation += cameraEffects[i].GetEffectRot();
            }
        }
        private void SetCameraPosition()
        {
            m_finalPosition = m_targetTransform.position;
            m_finalPosition += Quaternion.Euler(0.0f, angleValue * indexDirection, 0.0f) *  m_cameraDirection.normalized * m_distanceToTarget;
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
