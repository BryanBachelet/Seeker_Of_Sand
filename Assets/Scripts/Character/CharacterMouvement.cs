using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMOD.Studio;
using FMODUnity;
using Klak.Motion;
using UnityEngine.Profiling;
using Render.Camera;

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
        private CharacterProfile profile;
        private Render.Camera.CameraBehavior cameraPlayer;
        public float runSpeed = 80f;
        public float combatSpeed = 40f;
        [HideInInspector] public bool combatState;
        [HideInInspector] public float ratioSmoothMouvement = 10.0f;
        [HideInInspector] private float initialSpeed = 10.0f;
        [HideInInspector] private GameLayer m_gameLayer;
        [HideInInspector] private float m_groundDistance = 5;
        [HideInInspector] private float m_maxGroundSlopeAngle = 90;
        [SerializeField] private Animator m_CharacterAnim = null;
        [SerializeField] private Animator m_BookAnim = null;
        [SerializeField] private GameObject m_slidingEffect;
        [HideInInspector] public UnityEngine.VFX.VisualEffect m_slidingEffectVfx;
        [HideInInspector] public float m_SpeedReduce;
        private Rigidbody m_rigidbody;
        private Vector2 m_inputDirection;
        private Vector3 m_prevInputDirection;
        private float m_rotationTime;
        private Quaternion m_startRotation;

        public float distanceDetectObstacle = 3;
        [Header("Run State")]
        private float rotationLerpStepRunState = .5f;


        private bool m_onProjection;

        private ObjectState state;


        private float m_currentSlideSpeed;

        [Header("Glide Parameter")]
        [HideInInspector] private float m_glideSpeed = 4;
        [HideInInspector] private float m_gravityForce = 500;
        [HideInInspector] public float m_lastTimeShot = 0;

        [Header("Move Parameter")]
        [HideInInspector] private float m_accelerationSpeed = 150;
        private Vector3 m_velMovement;
        [HideInInspector] public bool activeCombatModeConstant = true;

        [Header("Slide Parameters")]
        [HideInInspector] public bool isSliding;
        public AnimationCurve accelerationCurve;
        [HideInInspector] private float maxSpeed = 120 ;
        [HideInInspector] private float maxSlope = 90;
        [HideInInspector] private float minSlope = 5.0f;
        [HideInInspector] private float minDecceleration = 10;
        [HideInInspector] private float angularSpeed = 200;
        [HideInInspector] private float timeAfterSliding = 0.2f;
        [HideInInspector] private float timeBeforeSliding = 0.1f;

        [HideInInspector] private float combatDeccelerationSpeed = 2400;
        [HideInInspector] private float turningRatioOfDecceleration = 5;

        private bool isSlidingIsActive;
        private bool m_isSlideInputActive;
        private float m_timerBeforeSliding;
        [HideInInspector] private float angleMinToRotateInSlide = 3;
        private float m_slope;
        private Vector3 m_groundNormal;
        private bool m_isSave;
        private Vector3 m_saveVeloctiy;
        private bool m_saveStateSliding;

        private SmoothFollow bookSmoothFollow;


        [HideInInspector] private Vector3 forwardDirection = new Vector3(-0.0131f, 0.303f, -0.947f);
        [HideInInspector] private bool m_isSlowdown;
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
            SpecialSpell,
        }

        [Header("Knockback Parameters")]
        [HideInInspector] private float m_knockBackPower = 500.0f;
        [HideInInspector] private float m_knockBackDuration = 0.15f;
        private float m_knockbackTimer;
        private bool m_applyKnockback;
        private float m_knockbackBaseGravityPower = 10.0f;
        private Vector3 m_directionKnockback;

        [HideInInspector] public MouvementState mouvementState;

        [HideInInspector] private SpeedData m_speedData = new SpeedData();
        private bool m_directionInputActive = false;

        public Vector3 currentDirection { get; private set; }

        private Character.CharacterAim m_characterAim;
        [SerializeField] private Transform m_avatarTransform;

        [Header("Debug Parameters")]
        [SerializeField] private bool m_activeDebug;

        private PlayerInput m_playerInput;
        private CharacterShoot m_characterShoot;
        private GuerhoubaGames.Input.DivideSchemeManager m_divideSchemeManager;
        private bool m_isSlideRotating;

        public bool updateProfilStateSpeedDebug = false;

        // temp
        private float timeToAccelerate = 0.75f;
        private float m_timerToAccelerate = 0.0f;
        
        public void InitComponentStat(CharacterStat stat)
        {
            runSpeed = runSpeed + stat.baseStat.speed;
            InitComponent();
        }
        private void InitComponent()
        {
            if (state == null)
            {
                state = new ObjectState();
                GameState.AddObject(state);
            }

            m_slidingEffectVfx = m_slidingEffect.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
            m_rigidbody = GetComponent<Rigidbody>();
            initialSpeed = runSpeed;
            m_characterAim = GetComponent<CharacterAim>();
            m_playerInput = GetComponent<PlayerInput>();
            m_characterShoot = GetComponent<CharacterShoot>();
            m_divideSchemeManager = GetComponent<GuerhoubaGames.Input.DivideSchemeManager>();
            if (m_BookAnim.GetComponent<SmoothFollow>()) { bookSmoothFollow = m_BookAnim.GetComponent<SmoothFollow>(); }
            if(m_gameLayer == null) { m_gameLayer = GameLayer.instance; }
            if(cameraPlayer == null) { cameraPlayer = Camera.main.GetComponent<CameraBehavior>(); }
        }

        private void Start()
        {
            profile = this.GetComponent<CharacterProfile>();
            m_speedData.referenceSpeed = new float[4];
            m_speedData.referenceSpeed[0] = 0;
            m_speedData.referenceSpeed[1] = runSpeed;
            m_speedData.referenceSpeed[2] = maxSpeed;
            m_speedData.referenceSpeed[3] = m_glideSpeed;
            m_speedData.IsFlexibleSpeed = false;
            m_speedData.currentSpeed = 0;

            m_speedData.direction = Vector3.zero;
            if (state == null)
            {
                InitComponent();
            }
            //combatState = true;
            //SetCombatMode(true);
            mouvementSoundInstance = RuntimeManager.CreateInstance(MouvementSoundReference);
            RuntimeManager.AttachInstanceToGameObject(mouvementSoundInstance, this.transform);
            mouvementSoundInstance.start();
            ActiveSlide();
        }

        public void MoveInput(InputAction.CallbackContext ctx)
        {

            if (ctx.started)
            {
                m_inputDirection = ctx.ReadValue<Vector2>();
                m_directionInputActive = true;
            }
            if (ctx.performed)
            {
                m_inputDirection = ctx.ReadValue<Vector2>();
                m_directionInputActive = true;
            }
            if (ctx.canceled)
            {
                m_inputDirection = Vector2.zero;
                m_directionInputActive = false;

                if (IsGamepad())
                {
                    if (m_divideSchemeManager.isAbleToChangeMap)
                    {
                        m_divideSchemeManager.ChangeToCombatActionMap();
                        CancelSlide();
                        ResetRun();
                    }
                }
                else
                {
                    CancelSlide();
                    ResetRun();
                }

            }

        }

        private bool IsGamepad()
        {
            return m_playerInput.currentControlScheme == "Gamepad";
        }


        public void ActiveSlide()
        {
            m_characterShoot.gsm.CanalisationParameterLaunch(1f, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);
            //m_characterShoot.gsm.CanalisationParameterStop();
            SlideActivation(true);
            m_CharacterAnim.SetBool("Running", true);
            m_BookAnim.SetBool("Running", true);
            m_CharacterAnim.SetBool("Casting", false);
            m_BookAnim.SetBool("Running", false);
            m_isSlideInputActive = true;
            m_timerToAccelerate = 0.0f;


        }

        public void ResetRun()
        {
            m_characterShoot.gsm.CanalisationParameterLaunch(1f, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);
            //m_characterShoot.gsm.CanalisationParameterStop();
            // SlideActivation(true);
            m_CharacterAnim.SetBool("Running", true);
            m_BookAnim.SetBool("Running", true);
            m_CharacterAnim.SetBool("Casting", false);

            m_BookAnim.SetBool("Running", false);
            m_isSlideInputActive = true;
            m_timerToAccelerate = 0.0f;
            m_timerBeforeSliding = 0.0f;
            isSliding = false;
            m_characterAim.vfxCast.SetFloat("Progress", 1);
            m_characterAim.vfxCastEnd.SetFloat("Progress", 1);
            ChangeState(MouvementState.Classic);
        }
        public void SlideInput(InputAction.CallbackContext ctx)
        {

            //if (ctx.started)
            //{
            //    if (!IsGamepad())
            //    {
            //        if (!m_isSlideInputActive)
            //        {
            //            m_isSlideInputActive = true;
            //            ActiveSlide();
            //            //if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(false); }
            //        }
            //        else
            //        {
            //            m_isSlideInputActive = false;
            //            SlideActivation(false);
            //            m_CharacterAnim.SetBool("Running", false);
            //            m_BookAnim.SetBool("Running", false);
            //            m_CharacterAnim.SetBool("Casting", true);
            //            m_characterShoot.gsm.CanalisationParameterLaunch(0.01f, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);

            //            m_BookAnim.SetBool("Casting", true);
            //            //if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(true);  }
            //        }
            //    }
            //    else
            //    {
            //        if (combatState)
            //        {
            //            m_isSlideInputActive = true;
            //            //m_characterShoot.gsm.CanalisationParameterStop();
            //            SlideActivation(true);
            //        }
            //        else
            //        {
            //            m_isSlideInputActive = false;
            //            SlideActivation(false);
            //        }
            //    }

            //}
            //if (ctx.canceled)
            //{
            //    if (!IsGamepad())
            //    {
            //        //m_isSlideInputActive = false;
            //        //SlideActivation(false);
            //        //m_CharacterAnim.SetBool("Running", false);
            //        //m_BookAnim.SetBool("Running", false);
            //        //m_CharacterAnim.SetBool("Casting", true);
            //        //m_BookAnim.SetBool("Running", true);
            //        //if (m_slidingEffectVfx.HasFloat("Rate")) m_slidingEffectVfx.SetFloat("Rate", 15);
            //    }
            //}
            if (!state.isPlaying)
            {
                ChangeState(MouvementState.None);
                m_isSlideInputActive = false;
            }
        }

        public void CancelSlide()
        {
            m_isSlideInputActive = false;
            SlideActivation(false);
            m_CharacterAnim.SetBool("Running", false);
            m_BookAnim.SetBool("Running", false);
            m_CharacterAnim.SetBool("Casting", true);
            m_BookAnim.SetBool("Casting", true);
            //if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(true); }

        }

        public void SlideActivation(bool isActive)
        {
            if (!activeCombatModeConstant) return;
            if (isActive)
            {
                SetCombatMode(false);
                m_characterShoot.DeactivateCanalisation();
                m_CharacterAnim.SetBool("Casting", false);
                m_BookAnim.SetBool("Casting", false);
                m_characterShoot.gsm.CanalisationParameterLaunch(1, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);
                MatchRotationAndDirection();
                if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(false); }
                //  cameraPlayer.BlockZoom(false);
                //DisplayNewCurrentState(1);

            }

            if (!isActive && combatState == false)
            {
                m_characterShoot.ActivateCanalisation();
                m_CharacterAnim.SetBool("Casting", true);
                m_BookAnim.SetBool("Casting", true);
                m_characterShoot.gsm.CanalisationParameterLaunch(0.01f, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);

                if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(true); }
                //DisplayNewCurrentState(0);
                //  cameraPlayer.BlockZoom(true);
                //SetCombatMode(true);
            }

        }

        public void SetCombatMode(bool state)
        {
            if (m_isSlideInputActive)
                combatState = false;
            else
                combatState = state;

            bookSmoothFollow.ChangeForBook(combatState);
            //if (!combatState) cameraPlayer.BlockZoom(false);
        }

        public void Update()
        {
            if (!state.isPlaying) return;
            if (updateProfilStateSpeedDebug) { updateProfilStateSpeedDebug = false; }

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
            if (m_activeDebug) Debug.Log("New State = " + newState);
            AfterChangeState(mouvementState, prevState);
        }

        public void BeforeChangeState(MouvementState prevState)
        {

            switch (prevState)
            {
                case MouvementState.None:

                    m_CharacterAnim.SetBool("Idle", false);
                    m_BookAnim.SetBool("Idle", false);
                    break;
                case MouvementState.Classic:
                    m_CharacterAnim.SetBool("Running", false);
                    m_BookAnim.SetBool("Running", false);
                    if (m_slidingEffectVfx.HasFloat("Rate")) m_slidingEffectVfx.SetFloat("Rate", 15);
                    break;
                case MouvementState.Slide:
                    m_CharacterAnim.SetBool("Sliding", false);
                    m_BookAnim.SetBool("Sliding", false);
                    //m_slidingEffect.SetActive(false);
                    //  m_timerBeforeSliding = 0.0f;
                    if (m_slidingEffectVfx.HasFloat("Rate")) m_slidingEffectVfx.SetFloat("Rate", 0);
                    break;
                case MouvementState.Glide:
                    m_CharacterAnim.SetBool("Shooting", false);
                    m_BookAnim.SetBool("Shooting", false);
                    break;
                case MouvementState.Knockback:
                    m_CharacterAnim.SetBool("Shooting", false);
                    m_BookAnim.SetBool("Shooting", false);
                    break;
                case MouvementState.Dash:
                    m_CharacterAnim.SetBool("Shooting", false);
                    m_BookAnim.SetBool("Shooting", false);

                    break;
                default:
                    break;
            }

        }

        public void AfterChangeState(MouvementState newState, MouvementState prevState)
        {


            switch (newState)
            {
                case MouvementState.None:

                    m_CharacterAnim.SetBool("Idle", true);
                    m_BookAnim.SetBool("Idle", true);
                    UpdateParameter(0f, "MouvementState");
                    break;
                case MouvementState.Classic:
                    if (m_isSlideInputActive)
                    {
                        m_CharacterAnim.SetBool("Running", true);
                        m_BookAnim.SetBool("Running", true);
                        UpdateParameter(0.10f, "MouvementState");
                    }

                    m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)newState]);
                    if (m_isSlowdown)
                    {
                        m_speedLimit = m_speedData.referenceSpeed[2];
                    }

                    break;

                case MouvementState.Slide:
                    m_CharacterAnim.SetBool("Sliding", true);
                    m_BookAnim.SetBool("Sliding", true);
                    UpdateParameter(1, "MouvementState");
                    //m_slidingEffect.SetActive(true);\
                    m_characterAim.vfxCast.SetFloat("Progress", 0);
                    m_characterAim.vfxCastEnd.SetFloat("Progress", 0);
                    if (m_slidingEffectVfx.HasFloat("Rate")) m_slidingEffectVfx.SetFloat("Rate", 100);

                    if (m_isSlowdown && prevState == MouvementState.Slide)
                    {
                        m_speedLimit = m_speedData.referenceSpeed[2];
                    }


                    break;
                case MouvementState.Glide:
                    //m_CharacterAnim.SetBool("Shooting", true);
                    UpdateParameter(0f, "MouvementState");
                    m_isSlowdown = IsFasterThanSpeedReference(m_speedData.referenceSpeed[(int)newState]);
                    if (m_isSlowdown && prevState == MouvementState.Slide)
                    {
                        m_speedLimit = m_speedData.referenceSpeed[2];
                    }

                    if (prevState == MouvementState.Classic)
                    {
                        m_speedLimit = m_speedData.referenceSpeed[1];
                    }
                    break;
                case MouvementState.Knockback:
                    m_CharacterAnim.SetBool("Shooting", false);
                    m_BookAnim.SetBool("Shooting", false);
                    UpdateParameter(0f, "MouvementState");
                    break;
                case MouvementState.Dash:
                    m_CharacterAnim.SetBool("Shooting", false);
                    m_BookAnim.SetBool("Shooting", false);
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

        public Vector3 GetDirection()
        {
            Vector3 horizontalDirection = new Vector3(forwardDirection.x, 0, forwardDirection.z);
            float angle = Vector3.SignedAngle(horizontalDirection.normalized, GetMouvementDirection(), Vector3.up);
            Vector3 forward = Quaternion.AngleAxis(angle, m_groundNormal.normalized) * forwardDirection;
            return forward;
        }
        public Vector3 OrientateWithSlopeDirection(Vector3 dir)
        {
            Vector3 direction = Quaternion.AngleAxis(m_slope, Vector3.right) * dir;
            return direction;
        }

        private void CheckPlayerMouvement()
        {
            if (mouvementState == MouvementState.Knockback || mouvementState == MouvementState.Dash || mouvementState == MouvementState.SpecialSpell) return;

            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
            
            inputDirection = cameraPlayer.TurnDirectionForCamera(inputDirection);

            if (IsObstacle())
            {
                Debug.Log("hit obstacle");
                m_speedData.currentSpeed = 0;
                m_velMovement = Vector3.zero;
                m_rigidbody.velocity = Vector3.zero;
                return;
            }
            else
            {
                Debug.Log("not hit obstacle");
            }

            RaycastHit hit = new RaycastHit();
            if (!OnGround(ref hit))
            {
                Debug.Log("NotGround");
                ChangeState(MouvementState.Glide);
                AirMove(inputDirection);
                m_timerBeforeSliding = 0;
                return;
            }
            else
            {
                Debug.Log("hit ground");
            }
                Vector3 direction = GetForwardDirection(hit.normal);
            Vector3 newDir = new Vector3(direction.x, 0, direction.z);
            if (combatState && inputDirection != Vector3.zero)
            {
                float angle = Vector3.SignedAngle(newDir, inputDirection, hit.normal.normalized);
                newDir = Quaternion.AngleAxis(angle, hit.normal.normalized) * direction;
            }

            m_groundNormal = hit.normal;
            forwardDirection = direction;
            m_speedData.direction = direction;

            m_slope = GetSlopeAngle(direction);
            if (GetSlopeAngleAbs(direction) >= m_maxGroundSlopeAngle)
            {
                Debug.Log("Slope superior");
                m_speedData.currentSpeed = 0;
                m_timerBeforeSliding = 0;
                ChangeState(MouvementState.None);
                return;
            }


            if (isSliding && !combatState && m_isSlideInputActive)
            {
                ChangeState(MouvementState.Slide);

                Slide(direction);
                return;
            }
            if (inputDirection == Vector3.zero && m_speedData.currentSpeed <= m_speedData.referenceSpeed[(int)mouvementState])
            {
                Debug.Log("Stop Move");
                m_speedData.currentSpeed = 0;
                m_timerBeforeSliding = 0;
                ChangeState(MouvementState.None);
                m_velMovement = Vector3.zero;
                currentDirection = Vector3.zero;

                if (IsGamepad())
                {
                    //if (m_divideSchemeManager.isAbleToChangeMap)
                    //{
                    //    m_divideSchemeManager.ChangeToCombatActionMap();

                    //}

                    CancelSlide();
                }
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
                Move(newDir);
                return;
            }



            if (m_slope > minSlope && m_isSlideInputActive)
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
            if (mouvementState == MouvementState.Dash || mouvementState == MouvementState.SpecialSpell)
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
                    m_knockbackTimer += Time.deltaTime;
                 

                    Vector3 vel = (m_directionKnockback.normalized * Mathf.Lerp(m_knockBackPower, 0.0f, m_knockbackTimer / m_knockBackDuration));
                    vel.y = m_rigidbody.velocity.y - m_knockbackBaseGravityPower;
                    m_rigidbody.velocity = vel;


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

            if (m_isSlowdown && mouvementState == MouvementState.Slide)
            {
                if (isSliding)
                {
                    currentRefSpeed = m_speedData.referenceSpeed[2];
                    m_velMovement = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
                }
            }

            if (mouvementState == MouvementState.Glide)
            {
                if (m_isSlowdown)
                {
                    currentRefSpeed = m_speedLimit;
                }
                m_rigidbody.AddForce(Vector3.down * m_gravityForce, ForceMode.Impulse);
                m_velMovement += Vector3.down * m_gravityForce * Time.deltaTime;
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, currentRefSpeed);
                m_velMovement = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
                m_speedData.currentSpeed = m_velMovement.magnitude;
                Debug.Log("gliding");
                return;
            }



            RaycastHit hit = new RaycastHit();
            float targetDistance = m_rigidbody.velocity.magnitude;

            Ray ray = new Ray(transform.position, m_velMovement.normalized);
            if (Physics.Raycast(ray, out hit, 20, m_gameLayer.groundLayerMask))
            {
                Vector3 direction = GetForwardDirection(hit.normal);
                float slopeAngle = GetSlopeAngleAbs(direction);
                if (slopeAngle >= 60)
                {
                    m_rigidbody.velocity = Vector3.zero;
                }
                else
                {
                    m_rigidbody.AddForce(m_velMovement, ForceMode.Impulse);
                    m_rigidbody.velocity = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
                    m_velMovement = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
                }

            }
            else
            {
                m_rigidbody.AddForce(m_velMovement, ForceMode.Impulse);
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
                m_velMovement = Vector3.ClampMagnitude(m_velMovement, currentRefSpeed);
            }

            m_rigidbody.velocity *= m_SpeedReduce;
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

            if (!combatState)
            {
                if (m_timerToAccelerate <= timeToAccelerate + 1)
                {
                    m_timerToAccelerate += Time.fixedDeltaTime;

                }
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
            return Physics.Raycast(transform.position, -Vector3.up, out hit, m_groundDistance, m_gameLayer.propsGroundLayerMask);
        }

        private bool IsObstacle()
        {
            return Physics.Raycast(transform.position, transform.forward, distanceDetectObstacle + m_velMovement.magnitude * Time.deltaTime, m_gameLayer.decoLayerMask);
        }

        private bool IsFasterThanSpeedReference(float speedReference)
        {
            return m_velMovement.magnitude > speedReference;
        }


        private void Move(Vector3 direction)
        {

            if (combatState)
            {
                m_speedData.referenceSpeed[(int)mouvementState] = combatSpeed + profile.stats.baseStat.speed;
            }
            else
            {
                m_speedData.referenceSpeed[(int)mouvementState] = Mathf.Clamp((m_timerToAccelerate / timeToAccelerate), 0, 1.0f) * (runSpeed - combatSpeed) + combatSpeed + profile.stats.baseStat.speed;
            }

            m_speedData.IsFlexibleSpeed = false;

            currentDirection = direction;


            m_velMovement += direction.normalized * (m_velMovement.magnitude / ratioSmoothMouvement + m_accelerationSpeed * Time.deltaTime);
            if (!m_isSlowdown)
            {

                m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedData.referenceSpeed[(int)mouvementState]);
            }
            else
            {

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

            // Decceleration of slide
            if (m_slope < minSlope)
            {
                float multiplyDecceleration = 1;
                if (m_isSlideRotating)
                {
                    multiplyDecceleration = turningRatioOfDecceleration;

                }
                m_currentSlideSpeed -= minDecceleration * multiplyDecceleration * Time.deltaTime;

            }

            m_currentSlideSpeed += accelerationCurve.Evaluate(m_slope / maxSlope) * Time.deltaTime;
            m_speedData.currentSpeed += m_currentSlideSpeed;

            if (m_speedData.currentSpeed < runSpeed)
            {
                Debug.Log("Stop sliding");
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

        public void UpdateSpeedData(int additionnalSpeed)
        {
            for (int i = 1; i < m_speedData.referenceSpeed.Length; i++)
            {
                m_speedData.referenceSpeed[i] += additionnalSpeed;
            }
        }
        public void OnDisable()
        {
            mouvementSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        #region Rotation

        private void RotateCharacter()
        {
            if (!m_directionInputActive) return;

            if (!combatState || m_characterShoot.m_aimModeState == AimMode.Automatic)
            {

                Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);
                if (m_prevInputDirection != inputDirection)
                {
                    m_startRotation = transform.rotation;
                    m_prevInputDirection = inputDirection;
                    m_rotationTime = 0.0f;

                }
                else
                {
                    m_rotationTime += rotationLerpStepRunState;
                }

                Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
                float angleDir = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

                Quaternion endRot = Quaternion.AngleAxis(angleDir, Vector3.up);

                transform.rotation = Quaternion.Slerp(m_startRotation, endRot, m_rotationTime);
                m_avatarTransform.localRotation = Quaternion.identity;  
            }

            if (combatState && m_characterShoot.m_aimModeState != AimMode.Automatic)
            {

                m_characterAim.FeedbackHeadRotation();
                Quaternion rotationFromHead = m_characterAim.GetTransformHead().rotation;
                m_avatarTransform.rotation = rotationFromHead;
            }

        }


        private void SlideRotationCharacter()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
            float angleDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
            m_isSlideRotating = false;
            if (Mathf.Abs(angleDir) > angleMinToRotateInSlide)
            {
                m_isSlideRotating = true;

            }
            angleDir = Mathf.Clamp(angleDir * Time.deltaTime, -angularSpeed * Time.deltaTime, angularSpeed * Time.deltaTime);


            transform.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
            m_avatarTransform.localRotation = Quaternion.identity;
        }

        private void MatchRotationAndDirection()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
            float angleDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);


            transform.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
            m_avatarTransform.localRotation = Quaternion.identity;
        }
        #endregion

        #region Knockback

        public void SetKnockback(Vector3 attackPosition, float powerKnockback = 50)
        {

            if (mouvementState == MouvementState.Knockback) return;

            Vector3 attackDirection = transform.position - attackPosition;
            attackDirection = attackDirection.normalized;
            attackDirection.y = 0.0f;
            m_directionKnockback = attackDirection.normalized * powerKnockback;
            m_applyKnockback = true;
            m_velMovement = Vector3.zero;
            m_rigidbody.velocity = Vector3.zero;
            ChangeState(MouvementState.Knockback);
        }

        #endregion


        public void StopRigidbody()
        {
            m_rigidbody.velocity = Vector3.zero;
        }

    }
}