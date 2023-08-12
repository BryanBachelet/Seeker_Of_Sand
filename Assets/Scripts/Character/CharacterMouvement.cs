using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Character
{
    [Serializable]
    public class SpeedData
    {
        public float currentSpeed;
        public float[] referenceSpeed;
        public bool IsFlexibleSpeed;
        public Vector3 direction;
    }



    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMouvement : MonoBehaviour, CharacterComponent
    {
        public Render.Camera.CameraBehavior cameraPlayer;
        public float runSpeed = 7.0f;
        public bool combatState;
        [HideInInspector] public float initialSpeed = 10.0f;
        [SerializeField] private LayerMask m_groundLayerMask;
        [SerializeField] private float m_groundDistance = 2.0f;
        [SerializeField] private float m_maxGroundSlopeAngle = 60f;
        [SerializeField] private Animator m_CharacterAnim = null;
        [SerializeField] private GameObject m_slidingEffect;
        [Range(0, 1)]
        [SerializeField] public float m_SpeedReduce;
        private Rigidbody m_rigidbody;
        private Vector2 m_inputDirection;

        private bool m_onProjection;

        private ObjectState state;


        public bool isSliding;
        private float m_currentSlideSpeed;

        [Header("Glide Parameter")]
        [SerializeField] private float m_glideSpeed = 4;

        [Header("Slide Parameters")]
        public float accelerationSlide = 3.0f;
        public float maxSpeed = 30.0f;
        public float maxSlope = 60.0f;
        public float minSlope = 5.0f;
        public float minDecceleration = 2.0f;
        public float angularSpeed;
        public float timeAfterSliding = 0.5f;
        public float timeBeforeSliding = 0.3f;
        public float combatDeccelerationSpeed = 6.0f;
        private bool isSlidingIsActive;
        private float m_timerBeforeSliding;

        private float m_slope;
        private bool m_isSave;
        private Vector3 m_saveVeloctiy;
        private bool m_saveStateSliding;

        public enum MouvementState
        {
            None,
            Classic,
            Slide,
            Glide,
        }

        public MouvementState mouvementState;

        [SerializeField] private SpeedData m_speedData = new SpeedData();

        public Vector3 currentDirection { get; private set; }

        public void InitComponentStat(CharacterStat stat)
        {
            runSpeed = stat.baseStat.speed;
            InitComponent();
        }
        private void InitComponent()
        {
            state = new ObjectState();
            GameState.AddObject(state);


            m_rigidbody = GetComponent<Rigidbody>();
            if (m_CharacterAnim == null) { m_CharacterAnim = GameObject.Find("Avatar_Model").GetComponent<Animator>(); }
            initialSpeed = runSpeed;
        }

        private void Start()
        {
            m_speedData.referenceSpeed = new float[4];
            m_speedData.referenceSpeed[0] = 0;
            m_speedData.referenceSpeed[1] = runSpeed;
            m_speedData.referenceSpeed[2] = maxSpeed;
            m_speedData.referenceSpeed[3] = m_glideSpeed;
            m_speedData.IsFlexibleSpeed = false;
            m_speedData.currentSpeed = 0;

            m_speedData.direction = Vector3.zero;
        }

        public void MoveInput(InputAction.CallbackContext ctx)
        {

            // ========== Need to be clean ================
            if (ctx.performed)
            {
                m_inputDirection = ctx.ReadValue<Vector2>();

            }
            if (ctx.canceled)
            {
                m_inputDirection = Vector2.zero;
             
            }
            if (!state.isPlaying)
            {
                ChangeState(MouvementState.None);
            }
        }

        public void Update()
        {
            if (!state.isPlaying) return;
            if (!isSliding) RotateCharacter();
            else SlideRotationCharacter();
        }

        #region State

        public void ChangeState(MouvementState newState)
        {
            if (newState == mouvementState) return;
            BeforeChangeState(mouvementState);
            mouvementState = newState;
            AfterChangeState(mouvementState);
        }

        public void BeforeChangeState(MouvementState prevState)
        {
        
            switch (prevState)
            {
                case MouvementState.None:

                    m_CharacterAnim.SetBool("Idle", false);
                    break;
                case MouvementState.Classic:
                    m_CharacterAnim.SetBool("Running", false);
                    break;
                case MouvementState.Slide:
                    m_CharacterAnim.SetBool("Sliding", false);
                    m_slidingEffect.SetActive(false);
                    break;
                case MouvementState.Glide:
                    m_CharacterAnim.SetBool("Shooting", false);
                    break;
                default:
                    break;
            }
        }

        public void AfterChangeState(MouvementState newState)
        {

           
            switch (newState)
            {
                case MouvementState.None:
                   
                    m_CharacterAnim.SetBool("Idle", true);
                    break;
                case MouvementState.Classic:
                    m_CharacterAnim.SetBool("Running", true);
                    break;
                case MouvementState.Slide:
                    m_CharacterAnim.SetBool("Sliding", true);
                    m_slidingEffect.SetActive(true);
                    break;
                case MouvementState.Glide:
                    m_CharacterAnim.SetBool("Shooting", true);
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void CheckPlayerMouvement()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

            RaycastHit hit = new RaycastHit();
            if (!OnGround(ref hit))
            {
                ChangeState(MouvementState.Glide);
                AirMove(inputDirection);
                m_timerBeforeSliding = 0;
                return;
            }
            Vector3 direction = GetForwardDirection(hit.normal);
            m_speedData.direction = direction;

            m_slope = GetSlopeAngle(direction);
            if (GetSlopeAngleAbs(direction) >= m_maxGroundSlopeAngle)
            {

                m_speedData.currentSpeed = 0;
                m_timerBeforeSliding = 0;
                ChangeState(MouvementState.None);
                return;
            }


            if (isSliding && !combatState)
            {
                ChangeState(MouvementState.Slide);
                Slide(direction);
                return;
            }
            if (inputDirection == Vector3.zero && m_speedData.currentSpeed <= m_speedData.referenceSpeed[(int)mouvementState])
            {
                m_speedData.currentSpeed = 0;
                m_timerBeforeSliding = 0;
                ChangeState(MouvementState.None);
                return;
            }


            if (combatState)
            {
                isSliding = false;
                ChangeState( MouvementState.Classic);
                Move(direction);
                return;
            }



            if (m_slope > minSlope)
            {
                if (m_timerBeforeSliding < timeBeforeSliding)
                {
                    m_timerBeforeSliding += Time.deltaTime;
                    if (m_timerBeforeSliding >= timeBeforeSliding)
                        ChangeState(MouvementState.Slide);
                    ChangeState(MouvementState.Classic);
                    Move(direction);
                    return;
                }
                ChangeState(MouvementState.Slide);
                Slide(direction);
                return;
            }

            ChangeState(MouvementState.Classic);
            Move(direction);
            return;
        }


        /// <summary>
        ///  Function that apply the final result of our velocity calcul to our rigidbody
        /// </summary>
        private void ApplyVelocity()
        {


            float currentRefSpeed = m_speedData.referenceSpeed[(int)mouvementState];
            if (m_speedData.currentSpeed > currentRefSpeed)
            {
                if (!combatState) m_speedData.currentSpeed -= minDecceleration * Time.deltaTime;
                if (combatState) m_speedData.currentSpeed -= combatDeccelerationSpeed * Time.deltaTime;
            }
            if (!m_speedData.IsFlexibleSpeed && m_speedData.currentSpeed < currentRefSpeed)
                m_speedData.currentSpeed = currentRefSpeed;

            m_rigidbody.AddForce(m_speedData.direction * m_speedData.currentSpeed, ForceMode.Impulse);
            if (mouvementState == MouvementState.Glide)
            {
                Vector3 horizontalVelocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, runSpeed);
                m_rigidbody.velocity = new Vector3(horizontalVelocity.x, m_rigidbody.velocity.y, horizontalVelocity.z);
                return;
            }
            m_rigidbody.AddForce(m_speedData.direction * m_speedData.currentSpeed, ForceMode.Impulse);
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_speedData.direction * m_speedData.currentSpeed, currentRefSpeed);



        }


        public void FixedUpdate()
        {

            if (!state.isPlaying && m_isSave) return;

            if (!state.isPlaying && !m_isSave)
            {
                StartPause();
                return;
            }
            if (state.isPlaying && m_isSave)
            {
                EndPause();
            }

            CheckPlayerMouvement();
            ApplyVelocity();

        }

        private void StartPause()
        {
            m_isSave = true;
            m_saveVeloctiy = m_rigidbody.velocity;
            m_rigidbody.velocity = Vector3.zero;
            m_saveStateSliding = isSliding;
        }

        private void EndPause()
        {
            m_rigidbody.velocity = m_saveVeloctiy;
            isSliding = m_saveStateSliding;
            m_isSave = false;

        }
        private bool OnGround(ref RaycastHit hit)
        {
            return Physics.Raycast(transform.position, -Vector3.up, out hit, m_groundDistance, m_groundLayerMask);
        }
        private void Move(Vector3 direction)
        {
            
            if (combatState)
            {
                m_speedData.referenceSpeed[(int)mouvementState] = runSpeed * m_SpeedReduce;
            }
            else
            {
                m_speedData.referenceSpeed[(int)mouvementState] = runSpeed;
            }
            m_speedData.IsFlexibleSpeed = false;

            currentDirection = direction;
        }

        private void Slide(Vector3 direction)
        {
            m_speedData.IsFlexibleSpeed = true;
            m_speedData.direction = direction;
            currentDirection = direction;

            m_currentSlideSpeed = 0;
            isSliding = true;
            if (m_slope < minSlope) m_currentSlideSpeed -= minDecceleration * Time.deltaTime;
            m_currentSlideSpeed += accelerationSlide * m_slope / maxSlope * Time.deltaTime;
            m_speedData.currentSpeed += m_currentSlideSpeed;

            if (m_speedData.currentSpeed < runSpeed && m_slope < minSlope)
            {
                isSliding = false;
                m_currentSlideSpeed = 0.0f;
                m_timerBeforeSliding = 0;
            }


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
            m_speedData.direction = direction;
            m_speedData.IsFlexibleSpeed = false;

            //m_rigidbody.AddForce(direction * runSpeed, ForceMode.Impulse);
            //Vector3 horizontalVelocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
            //horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, runSpeed);
            //m_rigidbody.velocity = new Vector3(horizontalVelocity.x, m_rigidbody.velocity.y, horizontalVelocity.z);
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