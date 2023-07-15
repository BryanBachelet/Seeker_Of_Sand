using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{

    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMouvement : MonoBehaviour, CharacterComponent
    {
        public Render.Camera.CameraBehavior cameraPlayer;
        public float speed = 7.0f;
        public bool combatState;
        [HideInInspector] public float initialSpeed = 10.0f;
        [SerializeField] private LayerMask m_groundLayerMask;
        [SerializeField] private float m_groundDistance = 2.0f;
        [SerializeField] private float m_maxGroundSlopeAngle = 60f;
        [SerializeField] private Animator m_CharacterAnim = null;
        [SerializeField] private GameObject m_slidingEffect;
        [Range(0, 1)]
        [SerializeField] private float m_SpeedReduce;
        private Rigidbody m_rigidbody;
        private Vector2 m_inputDirection;

        private bool m_onProjection;

        
        public bool isSliding;
        private float m_currentSlideSpeed;
        [Header("Slide")]
        public float accelerationSlide = 3.0f;
        public float maxSpeed = 30.0f;
        public float maxSlope = 60.0f;
        public float minSlope = 5.0f;
        public float minDecceleration = 2.0f;
        public float angularSpeed;


        public Vector3 currentDirection { get; private set; }

        public void InitComponentStat(CharacterStat stat)
        {
            speed = stat.baseStat.speed;
            InitComponent();
        }
        private void InitComponent()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            if (m_CharacterAnim == null) { m_CharacterAnim = GameObject.Find("Avatar_Model").GetComponent<Animator>(); }
            initialSpeed = speed;
        }

        public void MoveInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_inputDirection = ctx.ReadValue<Vector2>();
                m_CharacterAnim.SetBool("Running", true);
                 m_CharacterAnim.SetBool("Idle", false);
            }
            if (ctx.canceled)
            {
                m_inputDirection = Vector2.zero;
               m_CharacterAnim.SetBool("Running", false);
                m_CharacterAnim.SetBool("Idle", true);
            }
        }

        public void Update()
        {

            if (!isSliding) RotateCharacter();
            else SlideRotationCharacter();
        }

        public void FixedUpdate()
        {
            RaycastHit hit = new RaycastHit();
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            if (OnGround(ref hit))
            {
                if (m_onProjection)
                {
                    return;
                }
                Vector3 direction = GetForwardDirection(hit.normal);
                if (GetSlopeAngleAbs(direction) >= m_maxGroundSlopeAngle)
                {
                    m_rigidbody.velocity = Vector3.zero;
                    return;
                }
                if (inputDirection == Vector3.zero && !isSliding)
                {
                    m_rigidbody.velocity = Vector3.zero;
                    return;
                }
                Slide(direction, GetSlopeAngle(direction));
                Move(direction);
            }
            else
            {
                Debug.Log("Air move ");
                if (m_onProjection)
                {
                    m_onProjection = false;
                }
                AirMove(inputDirection);
                if (inputDirection == Vector3.zero)
                {
                    // Possible probleme but necessary for the bump;
                    m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, m_rigidbody.velocity.y, m_rigidbody.velocity.z);
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
            if (isSliding) return;
            if (combatState)
            {
                m_rigidbody.AddForce(direction * speed * m_SpeedReduce, ForceMode.Impulse);
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, speed * m_SpeedReduce);
            }
            else
            {
                m_rigidbody.AddForce(direction * speed, ForceMode.Impulse);
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, speed);
            }
            currentDirection = direction;
        }

        private void Slide(Vector3 direction, float slope)
        {
           
            if (!combatState && slope > minSlope || !combatState && isSliding)
            {
          
                isSliding = true;
                m_CharacterAnim.SetBool("Sliding", true);
                m_slidingEffect.SetActive(true);
                if (slope < minSlope) m_currentSlideSpeed -= minDecceleration * Time.deltaTime;
                m_currentSlideSpeed += accelerationSlide * slope / maxSlope * Time.deltaTime;
                m_rigidbody.AddForce(direction * (speed + m_currentSlideSpeed), ForceMode.Impulse);
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, speed);
                if (m_currentSlideSpeed < -speed)
                {
                    isSliding = false;
                    m_CharacterAnim.SetBool("Sliding", false);
                    m_slidingEffect.SetActive(false);
                    m_currentSlideSpeed = 0.0f;
                }

            }
            else
            {
                isSliding = false;
                m_CharacterAnim.SetBool("Sliding", false);
                m_slidingEffect.SetActive(false);
                m_currentSlideSpeed = 0;
            }
            currentDirection = direction;
        }

        public void Projection(Vector3 dir, ForceMode mode)
        {

            m_rigidbody.AddForce(dir, mode);
            m_onProjection = true;
        }

        private Vector3 GetForwardDirection(Vector3 normal)
        {
            return Vector3.Cross(transform.right, normal);
        }

        private float GetSlopeAngleAbs(Vector3 direction)
        {
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            return Mathf.Abs(Vector3.SignedAngle(rotTest * Vector3.forward, direction, transform.right));
        }
        private float GetSlopeAngle(Vector3 direction)
        {
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            return Vector3.SignedAngle(rotTest * Vector3.forward, direction, transform.right);
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
            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            float angleDir = Vector3.SignedAngle(Vector3.forward, new Vector3(m_inputDirection.x, 0, m_inputDirection.y), Vector3.up);
            transform.rotation = Quaternion.AngleAxis(angleDir, Vector3.up);
        }


        private void SlideRotationCharacter()
        {
            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            float angleDir = Vector3.SignedAngle(transform.forward, new Vector3(m_inputDirection.x, 0, m_inputDirection.y), Vector3.up);
            angleDir = Mathf.Clamp(angleDir * Time.deltaTime, -angularSpeed * Time.deltaTime, angularSpeed * Time.deltaTime);
            transform.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
        }
        #endregion



    }
}