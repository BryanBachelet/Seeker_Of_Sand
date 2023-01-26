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
        [SerializeField] private LayerMask m_groundLayerMask;
        [SerializeField] private float m_groundDistance = 2.0f;
        [SerializeField] private float m_maxGroundSlopeAngle = 60f;
        
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

        public void Update()
        {
            RotateCharacter();
        }

        public void FixedUpdate()
        {
            RaycastHit hit = new RaycastHit();
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

     

            if (OnGround(ref hit))
            {
                Vector3 direction =  GetForwardDirection(hit.normal);
                if (GetSlopeAngle(direction) >= m_maxGroundSlopeAngle)
                {
                    m_rigidbody.velocity = Vector3.zero;
                    return;
                }
                if (inputDirection == Vector3.zero)
                {
                    m_rigidbody.velocity = Vector3.zero;
                    return;
                }
                Move(direction);
            }else
            {
                AirMove(inputDirection);
                if (inputDirection == Vector3.zero)
                {
                    m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 0);
                    return;
                }
            }
        }

        private bool OnGround(ref RaycastHit hit)
        {
            return Physics.Raycast(transform.position, -Vector3.up, out hit, m_groundDistance, m_groundLayerMask);
        }
        private void Move(Vector3 direction)
        {
            m_rigidbody.AddForce(direction * speed, ForceMode.Impulse);
            currentDirection = direction;
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, speed);
        }

        private Vector3 GetForwardDirection(Vector3 normal)
        {
            return Vector3.Cross(transform.right,normal);
        }

        private float GetSlopeAngle(Vector3 direction)
        {
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            return Mathf.Abs(Vector3.SignedAngle(rotTest * Vector3.forward, direction, transform.right));
        }

        private void AirMove(Vector3 direction)
        {
            m_rigidbody.AddForce(direction * speed, ForceMode.Impulse);
            Vector3 horizontalVelocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, speed);
            m_rigidbody.velocity = new Vector3(horizontalVelocity.x, m_rigidbody.velocity.y, horizontalVelocity.z);
        }

        #region Rotation

        private void RotateCharacter()
        {
            Vector3 dir = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            float angleDir = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);

            Quaternion rot = Quaternion.AngleAxis(angleDir, Vector3.up);
            transform.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
        }

        #endregion


    }
}