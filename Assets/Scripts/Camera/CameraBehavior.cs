using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Render.Camera
{
    public class CameraBehavior : MonoBehaviour
    {
        [SerializeField] private Transform m_targetTransform;
        [SerializeField] private float m_distanceToTarget;
        [HideInInspector] public Vector3 m_offsetPos;

        private Vector3 m_cameraDirection;
        private Vector3 m_baseAngle;

        private Vector3 m_finalPosition;
        private Vector3 m_finalRotation;

        private CameraShake m_cameraShake;
        private CameraAimOffset m_cameraOffset;
        // Start is called before the first frame update
        void Start()
        {
            m_cameraShake = GetComponent<CameraShake>();
            m_cameraOffset = GetComponent<CameraAimOffset>();
            m_cameraDirection =  transform.position - m_targetTransform.position;
            m_baseAngle = transform.rotation.eulerAngles;
        }

        void Update()
        {
            SetCameraRotation();
            SetCameraPosition();
            Apply();
        }

        private void SetCameraRotation()
        {
            m_finalRotation = m_baseAngle;
            m_finalRotation += m_cameraShake.GetShakeEuleurAngle();
        }
        private void SetCameraPosition()
        {
            m_finalPosition = m_targetTransform.position;
            m_finalPosition += m_cameraDirection.normalized * m_distanceToTarget;
            m_finalPosition += m_cameraShake.GetShakeOffset();
            m_finalPosition += m_cameraOffset.GetAimOffset();

        }
        void Apply()
        {
            transform.position = m_finalPosition;
            transform.rotation =Quaternion.Euler( m_finalRotation);
        }
    }
}
