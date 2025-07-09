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
using BorsalinoTools;
using UnityEditor;
using GuerhoubaTools.Curves;
using static Character.CharacterMouvement;
using UnityEditor.Rendering;

namespace Character
{

    public struct SlopeData
    {
        public float slopeAngle;
        public float slopeAngleAbs;
        public Vector3 slopeDirection;
    }


    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMouvement : MonoBehaviour, CharacterComponent
    {
        private const int m_slopeMaxWalkable = 60;
        private const int m_slopeMinAngleAcceleration = 5;
        private CharacterProfile profile;
        private Render.Camera.CameraBehavior cameraPlayer;
        private SlopeData m_slopeData;

        [Header("Running Variables")]
        public float runningSpeed = 0;
        private float m_baseRunSpeed;
        [SerializeField] private float m_runningSpeedAccelerationDuration = 0.1f;
        [SerializeField] private float m_rotationDurationOnRun = .15f;
        [SerializeField] private float m_mouvementLagChangeDirection = 3.0f;
        [SerializeField] private float m_durationRotationToCursor = .3f;
        private float m_timerRotationToCursor = 0.0f;

        [Header("Running Slope Variables")]
        [SerializeField] private float m_slopeMaxGainSpeedRunning = 20;
        [SerializeField] private float m_slopeRunningGainSpeedLimit = 20;
        [SerializeField] private float m_slopeRunningLoseSpeedLimit = 20;

        private float m_slopeAddionnalRunningSpeed = 0;


        [Header("Combat Speed Variables")]
        public float m_combatReductionSpeedPercent = 0.45f;

        [Header("Sliding Variables")]
        [SerializeField] private float m_slidingThreshold = 100;
        [SerializeField] private float m_slidingBrake = 100;
        [SerializeField] private float m_slidingMaxSpeed = 200;
        [SerializeField] private float m_angularSpeed = 200;
        [SerializeField] private CurveObject m_slidingAcceleration;
        [SerializeField] private float m_combatSpeedTransitionDuration;
        [SerializeField] private float m_slidingBaseSpeedEnter = 120;
        [SerializeField] private float m_slideLimitSpeed = 0.0f;

        private float m_speedLimit;
        private float m_currentSpeed;
        private Vector3 m_currentDirection;

        
       
        [HideInInspector] private float initialSpeed = 10.0f;
        [HideInInspector] private GameLayer m_gameLayer;
        [HideInInspector] private float m_groundDistance = 12;
        [HideInInspector] private float m_maxGroundSlopeAngle = 90;
        [Space]
        [Header("Old Variables")]
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
        [HideInInspector] private float maxSpeed = 120;
        [HideInInspector] private float maxSlope = 90;
        [HideInInspector] private float minSlope = 5.0f;
        [HideInInspector] private float minDecceleration = 10;

        [HideInInspector] private float timeAfterSliding = 0.2f;
        [HideInInspector] private float timeBeforeSliding = 0.1f;

        [HideInInspector] private float combatDeccelerationSpeed = 2400;
        [HideInInspector] private float turningRatioOfDecceleration = 5;

        private bool isSlidingIsActive;
        private bool m_isSlideInputActive;
        private float m_timerBeforeSliding;
        [HideInInspector] private float angleMinToRotateInSlide = 3;
        private Vector3 m_groundNormal;
        private bool m_isSave;
        private Vector3 m_saveVeloctiy;
        private bool m_saveStateSliding;

        private SmoothFollow bookSmoothFollow;


        [HideInInspector] private Vector3 forwardDirection = new Vector3(-0.0131f, 0.303f, -0.947f);
        [HideInInspector] private bool m_isSlowdown;


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

        private bool m_directionInputActive = false;

        public Vector3 currentDirection { get; private set; }

        private Character.CharacterAim m_characterAim;
        [SerializeField] private Transform m_avatarTransform;

        [Header("Debug Parameters")]
        [SerializeField] private bool m_activeMouvementDebug;

        private PlayerInput m_playerInput;
        private CharacterShoot m_characterShoot;
        private GuerhoubaGames.Input.DivideSchemeManager m_divideSchemeManager;
        private bool m_isSlideRotating;

        public bool updateProfilStateSpeedDebug = false;


        private MouvementState m_prevState;
        // temp
        private float timeToAccelerate = 0.75f;
        private float m_timerToAccelerate = 0.0f;

        #region Stats functions
        public void InitComponentStat(CharacterStat stat)
        {
            //runningSpeed = stat.runSpeed.totalValue;
            //combatSpeed = stat.runSpeed.totalValue * stat.combatSpeed.percent + stat.combatSpeed.statsValue;
            //m_baseRunSpeed = runningSpeed;
            InitComponent();
        }

        public void UpdateComponentStat(CharacterStat stat)
        {
            //runningSpeed = stat.runSpeed.totalValue;
            //combatSpeed = stat.runSpeed.totalValue * stat.combatSpeed.modificatorPercent + stat.combatSpeed.statsValue;

        }
        #endregion

        private void InitComponent()
        {
            if (state == null)
            {
                state = new ObjectState();
                GameState.AddObject(state);
            }

            m_slidingEffectVfx = m_slidingEffect.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
            m_rigidbody = GetComponent<Rigidbody>();
            initialSpeed = runningSpeed;
            m_characterAim = GetComponent<CharacterAim>();
            m_playerInput = GetComponent<PlayerInput>();
            m_characterShoot = GetComponent<CharacterShoot>();
            m_divideSchemeManager = GetComponent<GuerhoubaGames.Input.DivideSchemeManager>();
            if (m_BookAnim.GetComponent<SmoothFollow>()) { bookSmoothFollow = m_BookAnim.GetComponent<SmoothFollow>(); }
            if (m_gameLayer == null) { m_gameLayer = GameLayer.instance; }
            if (cameraPlayer == null) { cameraPlayer = Camera.main.GetComponent<CameraBehavior>(); }

            timeSlideTransition = m_combatSpeedTransitionDuration;
        }

        private void Start()

        {
            profile = this.GetComponent<CharacterProfile>();
            m_speedLimit = 0;
            m_currentDirection = Vector3.zero;
            if (state == null)
            {
                InitComponent();
            }


            m_characterShoot.OnCombatStarting += SetSpeedLimit;
            m_characterShoot.OnCombatEnding += SetSpeedLimit;


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
                //m_directionInputActive = false;

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
            m_BookAnim.SetBool("Casting", false);
            m_isSlideInputActive = true;
            m_timerToAccelerate = 0.0f;


        }

        public void ResetRun()
        {
            m_characterShoot.gsm.CanalisationParameterLaunch(1f, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);
            //m_characterShoot.gsm.CanalisationParameterStop();
            // SlideActivation(true);
            m_CharacterAnim.SetBool("Running", false);
            m_BookAnim.SetBool("Running", false);
            m_CharacterAnim.SetBool("Casting", false);
            m_BookAnim.SetBool("Casting", false);
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
                m_characterShoot.SetCombatMode(CombatPlayerState.NONE);
                m_characterShoot.DeactivateCanalisation();
                m_CharacterAnim.SetBool("Casting", false);
                m_BookAnim.SetBool("Casting", false);
                m_characterShoot.gsm.CanalisationParameterLaunch(1, (float)m_characterShoot.m_characterSpellBook.GetSpecificSpell(m_characterShoot.m_currentIndexCapsule).tagData.element - 0.01f);
                MatchRotationAndDirection();
                if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(false); }
                //  cameraPlayer.BlockZoom(false);
                //DisplayNewCurrentState(1);

            }

            if (!isActive && m_characterShoot.combatPlayerState == CombatPlayerState.NONE)
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


        public void Update()
        {
            if (!state.isPlaying) return;
            if (updateProfilStateSpeedDebug) { updateProfilStateSpeedDebug = false; }

            if (!isSliding) RotateCharacter();
            else SlideRotationCharacter();
        }


        public float GetCurrentSpeed()
        {
            return m_currentSpeed;
        }
        #region State

        public void ChangeState(MouvementState newState)
        {
            MouvementState prevState = mouvementState;
            m_prevState = mouvementState;
            if (newState == mouvementState) return;
            BeforeChangeState(mouvementState);
            mouvementState = newState;
            SetSpeedLimit();

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
                    timeSlideTransition  = 0;
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

                    m_isSlowdown = IsFasterThanSpeedReference(m_speedLimit);

                    break;

                case MouvementState.Slide:
                    m_CharacterAnim.SetBool("Sliding", true);
                    m_BookAnim.SetBool("Sliding", true);
                    UpdateParameter(1, "MouvementState");
                    //m_slidingEffect.SetActive(true);\
                    m_characterAim.vfxCast.SetFloat("Progress", 0);
                    m_characterAim.vfxCastEnd.SetFloat("Progress", 0);
                    if (m_slidingEffectVfx.HasFloat("Rate")) m_slidingEffectVfx.SetFloat("Rate", 100);



                    break;
                case MouvementState.Glide:
                    //m_CharacterAnim.SetBool("Shooting", true);
                    UpdateParameter(0f, "MouvementState");
                    m_isSlowdown = IsFasterThanSpeedReference(m_speedLimit);

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
            Vector3 direction = Quaternion.AngleAxis(m_slopeData.slopeAngle, Vector3.right) * dir;
            return direction;
        }

        private void CheckPlayerMouvement()
        {
            if (mouvementState == MouvementState.Knockback || mouvementState == MouvementState.Dash || mouvementState == MouvementState.SpecialSpell) return;

            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

            inputDirection = cameraPlayer.TurnDirectionForCamera(inputDirection);

            if (IsObstacle())
            {
                if (m_activeMouvementDebug)
                {
                    ScreenDebuggerTool.AddMessage(" Cancel Speed Obstacle ", 2, Color.red);
                }

                m_currentSpeed = 0;
                m_velMovement = Vector3.zero;
                m_rigidbody.velocity = Vector3.zero;
                return;
            }


            RaycastHit hit = new RaycastHit();
            if (!OnGround(ref hit))
            {
                if (m_activeMouvementDebug)
                {
                    ScreenDebuggerTool.AddMessage("On Glide ", 2, Color.red);
                }
                ChangeState(MouvementState.Glide);
                AirMove(inputDirection);
                m_timerBeforeSliding = 0;
                return;
            }
            else
            {
                if (Vector3.Distance(hit.point, transform.position) > 7)
                {
                    if (m_activeMouvementDebug)
                    {
                        ScreenDebuggerTool.AddMessage("Replacement ", 2, Color.red);
                    }
                    transform.position += -Vector3.up * (Vector3.Distance(hit.point, transform.position) - 7);
                }
            }

            Vector3 direction = GetForwardDirection(hit.normal);
            Vector3 newDir = new Vector3(direction.x, 0, direction.z);
            if (m_characterShoot.IsCombatMode() && inputDirection != Vector3.zero || mouvementState == MouvementState.Classic && inputDirection != Vector3.zero)
            {
                float angle = Vector3.SignedAngle(newDir, inputDirection, hit.normal.normalized);
                newDir = Quaternion.AngleAxis(angle, hit.normal.normalized) * direction;
            }

            m_groundNormal = hit.normal;
            forwardDirection = direction;
            m_currentDirection = direction;


            m_slopeData.slopeAngle = GetSlopeAngle(direction);
            m_slopeData.slopeDirection = direction;
            m_slopeData.slopeAngleAbs = GetSlopeAngleAbs(direction);
            if (m_slopeData.slopeAngleAbs >= m_maxGroundSlopeAngle)
            {
                Debug.Log("Slope superior");
                m_currentSpeed = 0;
                m_timerBeforeSliding = 0;
                if (m_activeMouvementDebug)
                {
                    ScreenDebuggerTool.AddMessage("Cancel Speed Slope ", 2, Color.red);
                }
                ChangeState(MouvementState.None);
                return;
            }


            if (isSliding && !m_characterShoot.IsCombatMode() && m_isSlideInputActive)
            {
                ChangeState(MouvementState.Slide);

                Slide(direction);
                return;
            }
            if (inputDirection == Vector3.zero && m_currentSpeed <= m_speedLimit)
            {
                if (m_activeMouvementDebug)
                {
                    ScreenDebuggerTool.AddMessage("Cancel Speed Input ", 2, Color.red);
                }
                m_currentSpeed = 0;
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


            if (m_characterShoot.IsCombatMode())
            {
                if(mouvementState == MouvementState.Slide)
                {
                    m_slideLimitSpeed = m_currentSpeed;
                }
                isSliding = false;
                ChangeState(MouvementState.Classic);
                Move(newDir);
                return;
            }



            if (m_currentSpeed >= m_slidingThreshold && m_isSlideInputActive)
            {
                //if (m_timerBeforeSliding < timeBeforeSliding)
                //{
                //    m_timerBeforeSliding += Time.deltaTime;

                //    if (m_timerBeforeSliding >= timeBeforeSliding)
                //        ChangeState(MouvementState.Slide);

                //    ChangeState(MouvementState.Classic);
                //    Move(direction);
                //    return;
                //}
                ChangeState(MouvementState.Slide);
                Slide(direction);
                return;
            }

            ChangeState(MouvementState.Classic);
            Move(newDir);
            return;
        }

        public float GetSlope()
        {
            return m_slopeData.slopeAngle;
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



            if (m_isSlowdown && mouvementState == MouvementState.Classic)
            {
                //m_speedLimit -= minDecceleration * Time.deltaTime;
                //m_speedLimit -= combatDeccelerationSpeed * Time.deltaTime;
                m_isSlowdown = IsFasterThanSpeedReference(m_speedLimit);


            }

            if (m_isSlowdown && mouvementState == MouvementState.Slide)
            {
                if (isSliding)
                {

                    m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
                }
            }

            if (mouvementState == MouvementState.Glide)
            {

                m_rigidbody.AddForce(Vector3.down * m_gravityForce, ForceMode.Impulse);
                m_velMovement += Vector3.down * m_gravityForce * Time.deltaTime;
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, m_speedLimit);
                m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
                m_currentSpeed = Mathf.Clamp(m_velMovement.magnitude, 0, m_speedLimit);
                return;
            }



            RaycastHit hit = new RaycastHit();
            float targetDistance = m_rigidbody.velocity.magnitude;

            Ray ray = new Ray(transform.position, m_velMovement.normalized);
            if (Physics.Raycast(ray, out hit, 20, m_gameLayer.groundLayerMask))
            {
                Vector3 direction = GetForwardDirection(hit.normal);

                if (m_slopeData.slopeAngleAbs >= m_slopeMaxWalkable)
                {
                    m_rigidbody.velocity = Vector3.zero;
                }
                else
                {
                    m_rigidbody.AddForce(m_velMovement, ForceMode.Impulse);
                    m_rigidbody.velocity = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
                    m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
                }

            }
            else
            {
                m_rigidbody.AddForce(m_velMovement, ForceMode.Impulse);
                m_rigidbody.velocity = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
                m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
            }

            m_rigidbody.velocity *= m_SpeedReduce;
        }

        public void FixedUpdate()
        {

            UpdateDebug();
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

            if (!m_characterShoot.IsCombatMode())
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
            currentDirection = direction;

            m_currentSpeed += m_speedLimit * (1 / m_runningSpeedAccelerationDuration) * Time.deltaTime;
            m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, m_speedLimit);

            m_velMovement += direction.normalized * (m_currentSpeed / m_mouvementLagChangeDirection);



            m_velMovement = Vector3.ClampMagnitude(m_velMovement, m_speedLimit);
            m_currentSpeed = Mathf.Clamp(m_velMovement.magnitude, 0, m_speedLimit);

            SetSpeedLimit();
            if (m_characterShoot.IsCombatMode()) return;

            if (m_slopeData.slopeAngle > m_slopeMinAngleAcceleration)
            {
                m_slopeAddionnalRunningSpeed += m_slopeRunningGainSpeedLimit * Time.deltaTime;
                m_slopeAddionnalRunningSpeed = Mathf.Clamp(m_slopeAddionnalRunningSpeed, 0, m_slopeMaxGainSpeedRunning);
                SetSpeedLimit();
            }
            else
            {
                m_slopeAddionnalRunningSpeed -= m_slopeRunningLoseSpeedLimit * Time.deltaTime;
                m_slopeAddionnalRunningSpeed = Mathf.Clamp(m_slopeAddionnalRunningSpeed, 0, m_slopeMaxGainSpeedRunning);
                SetSpeedLimit();
            }

        }

        private void Slide(Vector3 direction)
        {

            m_currentDirection = direction;
            currentDirection = direction;

            m_currentSlideSpeed = 0;
            isSliding = true;

            //// Decceleration of slide
            //if (m_slopeData.slopeAngle < minSlope)
            //{
            //    float multiplyDecceleration = 1;
            //    if (m_isSlideRotating)
            //    {
            //        multiplyDecceleration = turningRatioOfDecceleration;

            //    }
            //    m_currentSlideSpeed -= minDecceleration * multiplyDecceleration * Time.deltaTime;

            //}
            Vector3 inputMvt = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

            if (inputMvt.z < 0)
            {
                m_currentSlideSpeed -= m_slidingBrake * Time.deltaTime;
                m_currentSpeed += m_currentSlideSpeed;
            }
            else
            {
                m_currentSlideSpeed += m_slidingAcceleration.Evaluate(m_slopeData.slopeAngle) * Time.deltaTime;
                m_currentSpeed += m_currentSlideSpeed;
            }
           

            if (m_currentSpeed < runningSpeed)
            {
                Debug.Log("Stop sliding");
                isSliding = false;
                m_currentSlideSpeed = 0.0f;
                m_timerBeforeSliding = 0;
            }
            m_velMovement = direction.normalized * m_currentSpeed;
            m_currentSpeed = Mathf.Clamp(m_velMovement.magnitude, 0, m_speedLimit);
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
            m_currentDirection = direction;

            m_currentSpeed = Mathf.Clamp(m_velMovement.magnitude, 0, m_speedLimit);
            if (m_isSlowdown)
            {
                m_isSlowdown = IsFasterThanSpeedReference(m_speedLimit);
            }
        }

        public void UpdateParameter(float parameterValue, string parameterName)
        {
            mouvementSoundInstance.setParameterByName(parameterName, parameterValue);
        }

        public void UpdateSpeedData(int additionnalSpeed)
        {

        }
        public void OnDisable()
        {
            mouvementSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        #region Rotation

        private void RotateCharacter()
        {


            if (!m_characterShoot.IsCombatMode() && m_inputDirection != Vector2.zero)
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
                    m_rotationTime += (1 / m_rotationDurationOnRun) * Time.deltaTime;
                }

                Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
                float angleDir = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

                Quaternion endRot = Quaternion.AngleAxis(angleDir, Vector3.up);

                transform.rotation = Quaternion.Slerp(m_startRotation, endRot, m_rotationTime);
                m_avatarTransform.localRotation = Quaternion.identity;
                m_timerRotationToCursor = 0;
            }

            if (m_characterShoot.IsCombatMode() && m_characterShoot.m_aimModeState != AimMode.Automatic || m_inputDirection == Vector2.zero)
            {
                m_prevInputDirection = Vector3.zero;
                m_characterAim.FeedbackHeadRotation();
                Quaternion rotationFromHead = m_characterAim.GetTransformHead().rotation;

                if (m_inputDirection == Vector2.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotationFromHead, m_timerRotationToCursor / m_durationRotationToCursor);

                    m_timerRotationToCursor += Time.deltaTime;
                    m_timerRotationToCursor = Mathf.Clamp(m_timerRotationToCursor, 0, m_durationRotationToCursor);

                }
                else
                {
                    m_avatarTransform.rotation = rotationFromHead;
                }

            }

        }


        private void SlideRotationCharacter()
        {
            Vector3 inputDirection = new Vector3(m_inputDirection.x, 0, m_inputDirection.y);

            Vector3 dir = Quaternion.Euler(0, cameraPlayer.GetAngle(), 0) * inputDirection;
            float angleDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
            //float angleDir = m_slidingMaxRotation.Evaluate(m_currentSpeed) * m_inputDirection.x;
            m_isSlideRotating = false;
            if (Mathf.Abs(angleDir) > angleMinToRotateInSlide)
            {
                m_isSlideRotating = true;

            }
            angleDir = Mathf.Clamp(angleDir * Time.deltaTime, -m_angularSpeed * Time.deltaTime, m_angularSpeed * Time.deltaTime);


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

        private float timeSlideTransition;

        public void SetSpeedLimit()
        {
            float result = 0;
          
            switch (mouvementState)
            {
                case MouvementState.None:
                    break;
                case MouvementState.Classic:

                    
                    result = runningSpeed + m_slopeAddionnalRunningSpeed;

         
                    if (m_characterShoot.IsCombatMode())
                    {
                        timeSlideTransition += Time.deltaTime * 0.5f;
                        result = Mathf.Clamp(runningSpeed * m_combatReductionSpeedPercent + m_slideLimitSpeed * ((m_combatSpeedTransitionDuration - timeSlideTransition) / m_combatSpeedTransitionDuration), runningSpeed * m_combatReductionSpeedPercent, m_slideLimitSpeed) ;
                    }

                    break;
                case MouvementState.Slide:
                    m_slopeAddionnalRunningSpeed = 0;
                    m_currentSpeed = m_slidingBaseSpeedEnter;
                    result = m_slidingMaxSpeed;
                    break;
                default:

                    break;
            }

            m_speedLimit = result;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(transform.position, -Vector3.up * m_groundDistance);
        }

        public void UpdateDebug()
        {
            if (!m_activeMouvementDebug) return;
            ScreenDebuggerTool.AddMessage("Current Speed : " + m_currentSpeed.ToString());
            ScreenDebuggerTool.AddMessage("Speed Limits : " + m_speedLimit.ToString());
            ScreenDebuggerTool.AddMessage("Current State : " + mouvementState.ToString());
            ScreenDebuggerTool.AddMessage("Combat State : " + m_characterShoot.combatPlayerState.ToString());
            ScreenDebuggerTool.AddMessage("Player Velocity : " + m_velMovement);
            ScreenDebuggerTool.AddMessage("Timer Rotation Cursor : " + m_timerRotationToCursor / m_durationRotationToCursor);
            ScreenDebuggerTool.AddMessage("Slope Angle : " + m_slopeData.slopeAngle);
        }
    }
}
