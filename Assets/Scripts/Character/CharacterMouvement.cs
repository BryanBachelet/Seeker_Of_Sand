using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMOD.Studio;
using FMODUnity;

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
        [SerializeField] private Transform positionInTrain;
        public float runSpeed = 7.0f;
        public bool combatState;
        [HideInInspector] public float initialSpeed = 10.0f;
        [SerializeField] private LayerMask m_groundLayerMask;
        [SerializeField] private LayerMask m_objstacleLayer;
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
        [SerializeField] private float m_gravityForce = 50;
        [HideInInspector] public float m_lastTimeShot = 0;
        [SerializeField] private float m_TimeAutoWalk = 2;

        [Header("Move Parameter")]
        [SerializeField] private float m_accelerationSpeed = 4.0f;
        private Vector3 m_velMovement;

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


        public Vector3 forwardDirection;
        public bool m_isSlowdown;
        private float m_speedLimit;

        public EventInstance mouvementSoundInstance;
        public EventReference MouvementSoundReference;
        public enum MouvementState
        {
            None,
            Classic,
            Slide,
            Glide,
            Train,
            Knockback,
            Dash,
        }

        [Header("Knockback Parameters")]
        [SerializeField] private float m_knockBackPower = 50.0f;
        [SerializeField] private float m_knockBackDuration = 1.0f;
        private float m_knockbackTimer;
        private bool m_applyKnockback;
        private float m_knockbackBaseGravityPower = 10.0f;
        private Vector3 m_directionKnockback;


        public MouvementState mouvementState;
       

        [SerializeField] private SpeedData m_speedData = new SpeedData();

        public Vector3 currentDirection { get; private set; }

        [Header("Debug Parameters")]
        [SerializeField] private bool m_activeDebug;

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

            mouvementSoundInstance = RuntimeManager.CreateInstance(MouvementSoundReference);
            RuntimeManager.AttachInstanceToGameObject(mouvementSoundInstance, this.transform);
            mouvementSoundInstance.start();
            Debug.Log("Mouvement Sound Started");
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
            if (mouvementState == MouvementState.Train)
            {
                transform.position = positionInTrain.localPosition;
                m_rigidbody.velocity = Vector3.zero;
                m_rigidbody.useGravity = false;
                return;
            }
            if (!isSliding) RotateCharacter();
            else SlideRotationCharacter();
        }


        public float GetCurrentSpeed()
        {
            return m_speedData.currentSpeed;
        }
        #region State

        public void ChangeState(MouvementState newState)
        {
            MouvementState prevState = mouvementState;
            if (newState == mouvementState) return;
            BeforeChangeState(mouvementState);
            mouvementState = newState;
           if(m_activeDebug) Debug.Log("New State = " + newState);
            AfterChangeState(mouvementState, prevState);
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
                case MouvementState.Knockback:
                    m_CharacterAnim.SetBool("Shooting", false);
                    break;
                case MouvementState.Dash:
                    m_CharacterAnim.SetBool("Shooting", false);
                    break;
                default:
                    break;
            }
        }

        public void AfterChangeState(MouvementState newState , MouvementState prevState)
        {


            switch (newState)
            {
                case MouvementState.None:

                    m_CharacterAnim.SetBool("Idle", true);
                    UpdateParameter(0f, "MouvementState");
                    break;
                case MouvementState.Classic:
                    m_CharacterAnim.SetBool("Running", true);
                    UpdateParameter(0.10f, "MouvementState");
                    m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)newState]);
                    if (m_isSlowdown)
                    {
                        m_speedLimit = m_speedData.referenceSpeed[2];
                    }

                    break;

                case MouvementState.Slide:
                    m_CharacterAnim.SetBool("Sliding", true);
                    UpdateParameter(1, "MouvementState");
                    m_slidingEffect.SetActive(true);

                    break;
                case MouvementState.Glide:
                    //m_CharacterAnim.SetBool("Shooting", true);
                    UpdateParameter(0f, "MouvementState");
                    m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)newState]);
                    if (m_isSlowdown && prevState == MouvementState.Slide)
                    {
                        m_speedLimit = m_speedData.referenceSpeed[2];
                    }
                    break;
                case MouvementState.Knockback:
                    m_CharacterAnim.SetBool("Shooting", false);
                    UpdateParameter(0f, "MouvementState");
                    break;
                case MouvementState.Dash:
                    m_CharacterAnim.SetBool("Shooting", false);
                    UpdateParameter(0f, "MouvementState");
                    break;
                default:
                    break;
            }
        }

        public Vector3 GetMouvementDirection()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            inputDirection = cameraPlayer.TurnDirectionForCamera(inputDirection);
            return inputDirection;
        }

        #endregion

        private void CheckPlayerMouvement()
        {
            if (mouvementState == MouvementState.Knockback || mouvementState == MouvementState.Dash) return;
            
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            inputDirection = cameraPlayer.TurnDirectionForCamera(inputDirection);

            if (IsObstacle())
            {
                m_speedData.currentSpeed = 0;
                m_velMovement = Vector3.zero;
                m_rigidbody.velocity = Vector3.zero;

            }

            RaycastHit hit = new RaycastHit();
            if (!OnGround(ref hit))
            {
                ChangeState(MouvementState.Glide);
                AirMove(inputDirection);
                m_timerBeforeSliding = 0;
                return;
            }
            Vector3 direction = GetForwardDirection(hit.normal);
            forwardDirection = direction;
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
                m_velMovement = Vector3.zero;
                if (m_rigidbody.velocity.magnitude > 0)
                {
                    m_rigidbody.velocity = m_rigidbody.velocity.normalized * (m_rigidbody.velocity.magnitude - (2 * Time.deltaTime));
                }

                return;
            }


            if (combatState)
            {
                isSliding = false;
                ChangeState(MouvementState.Classic);
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


        public float GetSlope()
        {
            return m_slope;
        }

        /// <summary>
        ///  Function that apply the final result of our velocity calcul to our rigidbody
        /// </summary>
        private void ApplyVelocity()
        {
            if (mouvementState == MouvementState.Dash)
            {
                m_rigidbody.velocity = Vector3.zero;
                //m_velMovement = Vector3.zero;
                //m_speedData.currentSpeed = 0.0f;
                return;
            }
                if (mouvementState == MouvementState.Knockback)
            {
                if (m_applyKnockback)
                {
                    m_velMovement = Vector3.zero;
                    m_rigidbody.velocity = Vector3.zero;
                    m_rigidbody.velocity += (m_directionKnockback);

                    m_applyKnockback = false;

                    m_knockbackTimer = 0;
                }
                if (m_knockbackTimer > m_knockBackDuration)
                {
                    mouvementState = MouvementState.Classic;
                    m_rigidbody.velocity = Vector3.zero;
                    m_knockbackTimer = 0;
                    m_directionKnockback = Vector3.zero;
                }
                else
                {
                    Vector3 vel = (m_directionKnockback.normalized * Mathf.Lerp(m_knockBackPower, 0.0f, m_knockbackTimer / m_knockBackDuration));
                    vel.y = m_rigidbody.velocity.y - m_knockbackBaseGravityPower;
                    m_rigidbody.velocity = vel;
                    m_knockbackTimer += Time.deltaTime;
                }

                return;
            }


            float currentRefSpeed = m_speedData.referenceSpeed[(int)mouvementState];
            if (m_isSlowdown && mouvementState == MouvementState.Classic)
            {
                m_speedLimit -= minDecceleration * Time.deltaTime;
                m_speedLimit -= combatDeccelerationSpeed * Time.deltaTime;
                currentRefSpeed = m_speedLimit;
                m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)mouvementState]);
            }

            if (mouvementState == MouvementState.Glide)
            {
                if (m_isSlowdown)
                {
                    currentRefSpeed = m_speedLimit;
                }
                m_rigidbody.AddForce(Vector3.down*m_gravityForce, ForceMode.Impulse);
                m_velMovement += Vector3.down * m_gravityForce * Time.deltaTime;
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, currentRefSpeed);
                m_velMovement = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
                m_speedData.currentSpeed = m_velMovement.magnitude;
                return;
            }
            m_rigidbody.AddForce(m_velMovement, ForceMode.Impulse);
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
            m_velMovement = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);

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

        private bool IsObstacle()
        {
            return Physics.Raycast(transform.position, transform.forward, 3, m_objstacleLayer);
        }

     

        private bool IsFasterThanSpeedReference(float speedReference)
        {
            return m_velMovement.magnitude > speedReference;
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


            if (!m_isSlowdown)
            {
                m_velMovement += direction.normalized  * m_accelerationSpeed * Time.deltaTime;
                m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedData.referenceSpeed[(int)mouvementState]);
            }
            else
            {

                m_velMovement += direction.normalized * m_accelerationSpeed * Time.deltaTime;
                m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)mouvementState]);
            }
            m_speedData.currentSpeed = m_velMovement.magnitude;
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
            m_velMovement = direction.normalized * m_speedData.currentSpeed;
            m_speedData.currentSpeed = m_velMovement.magnitude;
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
            m_speedData.currentSpeed = m_velMovement.magnitude;
            if (m_isSlowdown)
            {
                m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)mouvementState]);
            }
        }

        public void UpdateParameter(float parameterValue, string parameterName)
        {
            mouvementSoundInstance.setParameterByName(parameterName, parameterValue);
        }

        public void OnDisable()
        {
            mouvementSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        #region Rotation

        private void RotateCharacter()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            

            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
            float angleDir = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            transform.rotation = Quaternion.AngleAxis(angleDir, Vector3.up);
        }


        private void SlideRotationCharacter()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
           
            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
            float angleDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
            angleDir = Mathf.Clamp(angleDir * Time.deltaTime, -angularSpeed * Time.deltaTime, angularSpeed * Time.deltaTime);
            transform.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
        }
        #endregion



        #region Knockback

        public void SetKnockback(Vector3 attackPosition)
        {

            if (mouvementState == MouvementState.Knockback) return;

            Vector3 attackDirection = transform.position - attackPosition;
            attackDirection = attackDirection.normalized;
            attackDirection.y = 0.0f;
            m_directionKnockback = attackDirection.normalized * 50.0f;
            m_applyKnockback = true;
            m_velMovement = Vector3.zero;
            m_rigidbody.velocity = Vector3.zero;
            ChangeState(MouvementState.Knockback);
        }

        #endregion

    }
}