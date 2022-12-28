using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Render.Camera
{
    public class CameraBehavior : MonoBehaviour
    {
        [SerializeField] private Transform m_targetTransform;
        [SerializeField] private float m_distanceToTarget;
        private Vector3 cameraDirection;
        // Start is called before the first frame update
        void Start()
        {
            cameraDirection =  transform.position - m_targetTransform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = m_targetTransform.position + cameraDirection.normalized * m_distanceToTarget;
        }
    }
}
