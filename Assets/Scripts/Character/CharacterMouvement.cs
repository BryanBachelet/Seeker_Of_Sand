using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{

    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMouvement : MonoBehaviour
    {
        public float speed = 10.0f;
        public float initialSpeed = 10.0f;
        private Rigidbody m_rigidbody;
        private Vector2 m_inputDirection;
        public Vector3 currentDirection { get; private set; }
        private void Start()
        {
            InitComponent();
        }

        private void InitComponent()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            initialSpeed = speed;
        }


        public void MoveInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) m_inputDirection = ctx.ReadValue<Vector2>();
            if (ctx.canceled) m_inputDirection = Vector2.zero;
        }

        public void FixedUpdate()
        {
            RaycastHit hit = new RaycastHit();
            Vector3 direction = new Vector3(0, 0, 1);
            float angle = 0;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 2.0f))
            {
                direction = Vector3.Cross(transform.right, hit.normal);
                angle = Vector3.Angle(transform.forward, direction);
            }
            if (angle >= 60 || m_inputDirection == Vector2.zero)
            {
                m_rigidbody.velocity = Vector3.zero;
                return;
            }
            Vector3 dir = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            float angleDir = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);
          
            Quaternion rot = Quaternion.AngleAxis(angleDir, Vector3.up);
            direction = rot * direction.normalized;
            transform.rotation *= Quaternion.AngleAxis(angleDir , Vector3.up);

            m_rigidbody.AddForce(direction * speed, ForceMode.Impulse);
            currentDirection = direction;
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, speed);

        }

    }
}