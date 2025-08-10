using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using SeekerOfSand.UI;
using GuerhoubaTools.Gameplay;
using GuerhoubaGames.GameEnum;
using UnityEngine.VFX;
using SpellSystem;
using GuerhoubaGames.Resources;
using Klak.Motion;
using SeekerOfSand.Tools;
using GuerhoubaGames;
using Render.Camera;
using System;
using GuerhoubaGames.SaveData;

namespace Character
{
    public enum CombatPlayerState
    {
        NONE =0,
        COMBAT =1,
    }

    public class CharacterShoot : MonoBehaviour
    {

        private float m_timeBetweenRotation = 0.5f;
        [HideInInspector] private GameObject m_cameraObject;
        [HideInInspector] public GlobalSoundManager gsm;
        [HideInInspector] public int projectileNumber;

        [Header("Shoot Parameters")]
        [HideInInspector] private float baseTimeBetweenSpell = 0.15f;
        [HideInInspector] private float reloadTime = 0.25f;
        [HideInInspector] private float baseCanalisationTime = 0.1f;

        public bool activeRandom = false;

        [Header("Spell Composition Parameters")]
        [HideInInspector] public int[] spellEquip;
        [HideInInspector] public int maxSpellIndex = 4;
         public List<int> spellIndexGeneral;
        [HideInInspector] private List<int> spellIndexSpecific = new List<int>(4);
        [SerializeField] private SpellAttribution[] spellAttribution = new SpellAttribution[4];

        [HideInInspector] public int m_currentIndexCapsule = 0;
        private int m_currentRotationIndex = 0;

        [Header("Shoot Component Setup")]
        public SpellManager m_spellManger;
        [SerializeField] private Transform avatarTransform;
        [SerializeField] private Transform bookTransform;

        private SpellSystem.SpellProfil currentCloneSpellProfil;
        [HideInInspector] public List<SpellSystem.SpellProfil> spellProfils = new List<SpellSystem.SpellProfil>(4);
        public SpellSystem.SpellProfil weaponStat { get { return currentCloneSpellProfil; } private set { } }
        private int currentShotNumber;

        [HideInInspector] private Render.Camera.CameraShake m_cameraShake;
        [HideInInspector] private Render.Camera.CameraBehavior m_cameraBehavior;

        private CharacterAim m_characterAim;
        [HideInInspector] public CharacterMouvement m_CharacterMouvement; // Add reference to move script
        private Buff.BuffsManager m_buffManager;
        private CharacterProfile m_chracterProfil;
        private Character.CharacterUpgrade m_characterUpgrade;
        private Rigidbody m_rigidbody;

        private Animator m_CharacterAnimator;
        private Animator m_BookAnimator;

        public Vector3 m_spawnPointSpell;


        [Header("Shoot Infos")]
        [HideInInspector] public GameElement lastElement;
        [HideInInspector] public AimMode m_aimModeState = AimMode.FullControl;
        private bool isCasting;
        private bool IsRealoadingSpellRotation;


        [Header("Shoot Feedback")]
        [HideInInspector] private float m_shakeDuration = 0.15f;
        public GameObject[] vfxElementSign = new GameObject[4];
        private GameObject lastElementToUse;


        private bool isDirectSpellLaunchActivate = false;

        private float m_timerBetweenSpell;
        private float m_reloadTimer;
        private float m_timerBetweenShoot;

        private bool m_canShoot;
        private bool m_canEndShot;
        private bool m_isShooting;
        private bool m_shootInput;
        private bool m_shootInputActive;
        private bool m_isReloading;

        private GuerhoubaGames.GameEnum.BuffType m_currentType;
        private PauseMenu pauseScript;
        private ObjectState state;

        private float m_lastTimeShot = 0;

        [HideInInspector] public CharacterSpellBook m_characterSpellBook;
        [HideInInspector] public CharacterChainEffect m_characterChainEffect;
        private CharacterDash m_characterDash;

        public delegate void OnHit(Vector3 position, EntitiesTrigger tag, GameObject objectHit, GameElement element);
        public event OnHit onHit = delegate { };

        [Header("UI Object")]
        public GameObject uiManager;
        private UI_PlayerInfos m_uiPlayerInfos;
        [SerializeField] public List<Image> icon_Sprite;
        [SerializeField] public List<Image> spell_rarity;
        [SerializeField] public List<Transform> rankPoint;
        [SerializeField] public List<Image> m_spellGlobalCooldown;
        [SerializeField] public List<TextMeshProUGUI> m_TextSpellGlobalCooldown;

        [SerializeField] private GameResources m_gameResources;


        [Header("Spell Unique ")]
        public Coroutine[] m_spellCouroutine;

        private DropInventory m_dropInventory;

        private bool m_hasCancel;
        private bool m_activeSpellLoad;
        private bool m_hasBeenLoad;

        [Header("Debug")]
        public int specifiqueSpellStart = 0;
        public bool chooseBuild = false;

        private Texture currentPreviewDecalTexture; // Inspector, set texture named "box"
        private Texture currentPreviewDecalEndTexture; // Inspector, set texture named "box"

        private Texture m_initialPreviewDecal;

        private GameObject areaInstance;

        public enum CanalisationBarType
        {
            ByPart,
            Continious,
        }

        [Header("CanalisationBar")]
        [HideInInspector] private CanalisationBarType m_canalisationType;
        private float m_totalCanalisationDuration;
        private float m_totalLaunchingDuration;
        private float m_canalisationTimer;
        private float m_spellLaunchTime;
        private float m_deltaTimeFrame;
        private bool m_test;

        // Stacking Variable
        private ClockTimer[] m_stackingClock;
        private int[] m_currentStack = new int[4];
        private Image[] m_clockImage;
        private TMPro.TMP_Text[] m_textStack = new TMP_Text[4];

        private bool hasCanalise = false;
        private bool hasStartShoot = true;
        public bool hasShootBlock = false;

        public VisualEffect[] vfxUISign = new VisualEffect[4];

        private CharacterSummonManager m_characterSummmonManager;

        private SmoothFollow bookSmoothFollow;
        private CharacterDamageComponent m_characterDamageComponent;

        [HideInInspector] public CombatPlayerState combatPlayerState;
        public Action OnCombatStarting;
        public Action OnCombatEnding;


        #region playerFeedback(Cape)
        public SkinnedMeshRenderer skinedMeshRender;
        private Material m_Mat_capeSkinedMesh;
        private Material m_Mat_capucheSkinedMesh;
        private Material[] m_Mat_Cape_Flamme = new Material[2];
        public int indexMat;
        public int indexMatCapuche;
        public int[] indexMat_Cape_Flame = new int[2];
        [ColorUsage(true, true)] public Color[] capColorByElement = new Color[3];
        [ColorUsage(true, true)] public Color[] capFalmeColorByElement = new Color[3];
        #endregion
        #region Unity Functions

        private void Awake()
        {
            m_spellCouroutine = new Coroutine[100];
            m_initialPreviewDecal = currentPreviewDecalTexture;
            skinedMeshRender = avatarTransform.GetComponentInChildren<SkinnedMeshRenderer>();
            m_Mat_capeSkinedMesh = skinedMeshRender.materials[indexMat]; //Feedback Cape Color Element
            m_Mat_Cape_Flamme[0] = skinedMeshRender.materials[10];
            m_Mat_Cape_Flamme[1] = skinedMeshRender.materials[11];
            m_Mat_capucheSkinedMesh = skinedMeshRender.materials[indexMatCapuche];
        }



        private void Start()
        {
            state = new ObjectState();
            GameState.AddObject(state);

            m_dropInventory = this.GetComponent<DropInventory>();
            if (chooseBuild)
            {
                GenerateNewBuildSpecificStart(specifiqueSpellStart);
            }
            else if (activeRandom && !chooseBuild)
            {
                GenerateNewBuild();
            }
            InitComponents();
            InitCapsule();
            InitSpriteSpell();
            InitStacking();
            for (int i = 0; i < spellIndexGeneral.Count; i++)
            {
                m_dropInventory.AddNewItem(spellIndexGeneral[i]);
            }

            // Init Variables
            m_currentRotationIndex = 0;
            m_currentIndexCapsule = spellEquip[0];
            currentCloneSpellProfil = spellProfils[m_currentIndexCapsule].Clone();
            m_canShoot = true;
            m_activeSpellLoad = true;
            SpellSystem.SpellProfil spellProfil = spellProfils[m_currentRotationIndex];
            spellAttribution[m_currentIndexCapsule].AcquireSpellData(currentCloneSpellProfil);
            int maxStack = GetMaxStack(spellProfil);
            //m_uiPlayerInfos.ActiveSpellCanalisationUIv2(maxStack, icon_Sprite[m_currentRotationIndex]);
            m_deltaTimeFrame = Time.deltaTime;

            float cananisationTime = 0;

            if (currentCloneSpellProfil.gameEffectStats.HasStats(StatType.SpellCanalisation))
                cananisationTime = currentCloneSpellProfil.GetFloatStat(StatType.SpellCanalisation);

            m_totalCanalisationDuration = cananisationTime + baseCanalisationTime + m_deltaTimeFrame;

            m_aimModeState = AimMode.FullControl;
            if (m_canalisationType == CanalisationBarType.ByPart)
                m_totalLaunchingDuration = (m_currentStack[m_currentRotationIndex]);
        }

        private void Update()
        {
            if (!state.isPlaying) { return; } // Block Update during pause state

            if (IsCombatMode())
            {
                UpdateAvatarModels();
            }
            UpdateFullControlAimMode();
            UpdateStackingSystem();
            CountingCooldownBetweenSpell();
            ReloadWeapon(m_timeBetweenRotation);
        }

        #endregion

        // Update the avatar model rotation
        private void UpdateAvatarModels()
        {
            if (m_aimModeState == AimMode.Automatic && !m_characterAim.HasCloseTarget()) return;

            m_characterAim.FeedbackHeadRotation();
            Quaternion rotationFromHead = m_characterAim.GetTransformHead().rotation;
            avatarTransform.rotation = rotationFromHead;
            bookTransform.rotation = rotationFromHead;
        }

        #region Aim Mode Functions

       
        private void UpdateFullControlAimMode()
        {
            if (m_aimModeState != AimMode.FullControl) return;

            if (!IsCombatMode() || hasShootBlock) return;

            if (m_CharacterMouvement.mouvementState == CharacterMouvement.MouvementState.SpecialSpell) return;


            bool isValidCanalisation = ShotCanalisation();
            if (!m_shootInputActive) m_CharacterMouvement.m_SpeedReduce = 1;
            if (IsRealoadingSpellRotation || !m_shootInputActive || !hasCanalise) return;

            InitShot();




            if (m_activeSpellLoad)
                return;

            if (!m_isShooting)
            {

                if (currentCloneSpellProfil.TagList.mouvementBehaviorType == MouvementBehavior.Dash)
                {
                    m_characterDash.SpellDash(currentCloneSpellProfil.GetFloatStat(StatType.MouvementTravelTime), currentCloneSpellProfil.GetFloatStat(StatType.DistanceDash));
                }
                m_isShooting = true;
                if (m_canalisationType == CanalisationBarType.ByPart) m_spellLaunchTime = m_totalLaunchingDuration;


                if (currentCloneSpellProfil.TagList.spellNatureType == SpellNature.PROJECTILE)
                {
                    if (!isDirectSpellLaunchActivate) m_timerBetweenShoot = currentCloneSpellProfil.GetFloatStat(StatType.TimeBetweenShot);

                    if (m_canalisationType == CanalisationBarType.Continious) m_totalLaunchingDuration = m_currentStack[m_currentRotationIndex] * currentCloneSpellProfil.GetFloatStat(StatType.TimeBetweenShot);

                }

                if (currentCloneSpellProfil.TagList.spellNatureType == SpellNature.AREA)
                {
                    if (!isDirectSpellLaunchActivate) m_timerBetweenShoot = currentCloneSpellProfil.GetFloatStat(StatType.SpellFrequency);

                    if (m_canalisationType == CanalisationBarType.Continious) m_totalLaunchingDuration = m_currentStack[m_currentRotationIndex] * currentCloneSpellProfil.GetFloatStat(StatType.SpellFrequency);

                }
                if (!isDirectSpellLaunchActivate) Shoot();
                return;
            }


            if (currentCloneSpellProfil.TagList.spellNatureType == SpellNature.PROJECTILE) UpdateMultipleShoot(StatType.TimeBetweenShot);
            if (currentCloneSpellProfil.TagList.spellNatureType == SpellNature.AREA) UpdateMultipleShoot(StatType.SpellFrequency);

            if (currentCloneSpellProfil.TagList.spellNatureType == SpellNature.MULTI_HIT_AREA) Shoot();
            if (currentCloneSpellProfil.TagList.spellNatureType == SpellNature.SUMMON) Shoot();

            // m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
        }
        #endregion

        #region Start Functions
        public void InitComponents()
        {
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>();
            m_characterDash = GetComponent<CharacterDash>();
            m_characterSummmonManager = GetComponent<CharacterSummonManager>();
            pauseScript = GetComponent<PauseMenu>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_buffManager = GetComponent<Buff.BuffsManager>();
            m_characterChainEffect = GetComponent<CharacterChainEffect>();
            m_chracterProfil = GetComponent<CharacterProfile>();
            m_characterUpgrade = GetComponent<CharacterUpgrade>();
            m_characterSpellBook = GetComponent<CharacterSpellBook>();
            m_uiPlayerInfos = uiManager.GetComponent<UI_PlayerInfos>();
            m_CharacterAnimator = avatarTransform.GetComponent<Animator>();
            m_BookAnimator = bookTransform.GetComponent<Animator>();
            m_characterDamageComponent = GetComponent<CharacterDamageComponent>();
            m_clockImage = m_uiPlayerInfos.ReturnClock();
            m_textStack = m_uiPlayerInfos.ReturnStack();
            if (m_BookAnimator.GetComponent<SmoothFollow>()) m_BookAnimator.GetComponent<SmoothFollow>();
            if(m_cameraObject == null) { m_cameraObject = Camera.main.gameObject; }
            if(gsm == null) { gsm = m_cameraObject.GetComponentInChildren<GlobalSoundManager>(); }
            if(m_cameraShake == null) { m_cameraShake = m_cameraObject.GetComponent<CameraShake>(); }
            if(m_cameraBehavior == null) { m_cameraBehavior = m_cameraObject.GetComponent<CameraBehavior>(); }

            bookSmoothFollow = FindObjectOfType<SmoothFollow>();
        }

        private void InitSpriteSpell()
        {
            ActiveIcon();
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                //---m_spellGlobalCooldown[i].sprite = icon_Sprite[i].sprite;

            }
            if (m_currentType == BuffType.DAMAGE_SPELL)
            {
                currentCloneSpellProfil = spellProfils[m_currentIndexCapsule];
            }
        }



        public void InitCapsule()
        {
            // Add Spell in Spell List
            // Get the spell stats for each spell
            for (int i = 0; i < spellIndexGeneral.Count; i++)
            {
                if (spellIndexGeneral[i] == -1) continue;

                m_characterSpellBook.AddSpell(m_spellManger.spellProfils[spellIndexGeneral[i]].Clone(true));
                spellAttribution[i].AcquireSpellData(m_spellManger.spellProfils[spellIndexGeneral[i]]);
                spellIndexSpecific.Add(i);
                CreatePullObject(m_characterSpellBook.GetSpecificSpell(m_characterSpellBook.GetSpellCount() - 1));
                SpellManager.RemoveSpecificSpellFromSpellPool(spellIndexGeneral[i]);

            }


            SpellManager.RemoveSpecificSpellFromSpellPool(0);
            SpellManager.RemoveSpecificSpellFromSpellPool(1);
            SpellManager.RemoveSpecificSpellFromSpellPool(2);
            SpellManager.RemoveSpecificSpellFromSpellPool(3);


            //  Set to Spell Equip
            spellEquip = new int[4];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (i >= spellIndexGeneral.Count)
                    spellEquip[i] = -1;
                else
                {
                    spellEquip[i] = i;
                    spellProfils.Add(m_characterSpellBook.GetSpecificSpell(i));
                    m_characterSpellBook.m_spellsRotationArray[i] = (m_characterSpellBook.GetSpecificSpell(i));
                    spellAttribution[i].AcquireSpellData(m_characterSpellBook.m_spellsRotationArray[i]);
                    m_characterSpellBook.m_currentSpellInRotationCount++;
                    //---icon_Sprite[i].transform.parent.gameObject.SetActive(true);
                }
            }
            m_currentIndexCapsule = spellEquip[0];
            ChangeVfxOnUI(m_currentIndexCapsule);
            maxSpellIndex = Mathf.Clamp(spellIndexGeneral.Count, 0, 4);
            m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();

            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
        }

        private void GenerateNewBuild()
        {
            for (int i = 0; i < 1; i++)
            {
                int RndCapsule = 0;
                if (i == 0)
                {
                    RndCapsule = UnityEngine.Random.Range(0, 3);

                }
                else if (i == 1)
                {
                    RndCapsule = UnityEngine.Random.Range(9, 13);
                }
                else if (i == 2)
                {
                    RndCapsule = UnityEngine.Random.Range(4, 7);
                }
                spellIndexGeneral.Add(RndCapsule);
            }
        }

        private void GenerateNewBuildSpecificStart(int index)
        {
            spellIndexGeneral.Add(index);
        }
        private void InitStacking()
        {
            m_stackingClock = new ClockTimer[4];
            for (int i = 0; i < maxSpellIndex; i++)
            {
                m_stackingClock[i] = new ClockTimer();
                m_stackingClock[i].ActiaveClock();
                m_stackingClock[i].SetTimerDuration(spellProfils[i].GetFloatStat(StatType.StackDuration), m_clockImage[i], m_textStack[i]);

            }
        }
        #endregion


        public float GetPodRange() { return currentCloneSpellProfil.GetFloatStat(StatType.Range); }
        public SpellSystem.SpellProfil GetSpellProfil() { return currentCloneSpellProfil; }

        #region Inputs Functions
        public void ShootInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && state.isPlaying)
            {
                StartCasting();
                //gsm.CanalisationParameterLaunch(0.01f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);
                m_shootInput = true;
                m_shootInputActive = true;
                m_lastTimeShot = Time.time;
                m_hasCancel = false;
                SetCombatMode(CombatPlayerState.COMBAT);
            }
            if (ctx.canceled && state.isPlaying)
            {
                m_shootInput = false;
                m_shootInputActive = false;
                CancelShoot();
                //gsm.CanalisationParameterLaunch(1, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);
                m_CharacterMouvement.m_SpeedReduce = 1;
                SetCombatMode(CombatPlayerState.NONE);
                //m_CharacterMouvement.ActiveSlide();
            }
        }
        #endregion

        #region Shoot Function

        private void InitShot()
        {
            if (m_isShooting && m_activeSpellLoad || hasStartShoot) return;

            m_deltaTimeFrame = Time.deltaTime;
            hasStartShoot = true;
            currentPreviewDecalTexture = currentCloneSpellProfil.previewDecal_mat;
            currentPreviewDecalEndTexture = currentCloneSpellProfil.previewDecalEnd_mat;
            ChangeDecalTexture(currentCloneSpellProfil.TagList.element);
            gsm.CanalisationParameterLaunch(0.5f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);
            m_canEndShot = false;
        }

        public void DeactivateCanalisation()
        {
            m_canalisationTimer = 0.0f;
            //gsm.CanalisationParameterStop();
            m_uiPlayerInfos.DeactiveSpellCanalisation();
            isCasting = false;
        }

        public void ActivateCanalisation()
        {
            m_canalisationTimer = m_totalCanalisationDuration;
            SpellSystem.SpellProfil spellProfil = spellProfils[m_currentRotationIndex];
            int maxStack = GetMaxStack(spellProfil);
            UpdateCanalisationBar(m_totalCanalisationDuration);
            m_activeSpellLoad = true;
            hasCanalise = false;
        }

        private bool ShotCanalisation()
        {
            if (!m_activeSpellLoad || hasCanalise) return false;


            UpdateCanalisationBar(m_totalCanalisationDuration);


            bool isLightCanalisation = currentCloneSpellProfil.TagList.canalisationType == CanalisationType.LIGHT_CANALISATION;
            bool highCanalisationTest = currentCloneSpellProfil.TagList.canalisationType == CanalisationType.HEAVY_CANALISATION && m_shootInputActive;

            if(isLightCanalisation)
            {
                // Decal changes 
                currentPreviewDecalTexture = currentCloneSpellProfil.previewDecal_mat;
                currentPreviewDecalEndTexture = currentCloneSpellProfil.previewDecalEnd_mat;
                ChangeDecalTexture(currentCloneSpellProfil.TagList.element);

                m_activeSpellLoad = false;
                m_isShooting = false;
                m_canalisationTimer = m_totalCanalisationDuration;

                hasCanalise = true;
                return true;
            }


            if (highCanalisationTest)
            {
                m_CharacterMouvement.m_SpeedReduce = currentCloneSpellProfil.GetFloatStat(StatType.SpeedReduce);
                //gsm.CanalisationParameterLaunch(0.01f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);
                currentPreviewDecalTexture = currentCloneSpellProfil.previewDecal_mat;
                currentPreviewDecalEndTexture = currentCloneSpellProfil.previewDecalEnd_mat;
                ChangeDecalTexture(currentCloneSpellProfil.TagList.element);
                hasCanalise = false;
                float canalisationDuration = currentCloneSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime;
                if (m_canalisationTimer >= canalisationDuration)
                {
                    //gsm.CanalisationParameterLaunch(0.01f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);
                    m_activeSpellLoad = false;
                    m_isShooting = false;
                    m_canalisationTimer = m_totalCanalisationDuration;
                    UpdateCanalisationBar(m_totalCanalisationDuration);

                    hasCanalise = true;
                    return true;

                }
                else
                {
                    m_canalisationTimer += Time.deltaTime;

                    return false;
                }

            }
            else
            {
                m_CharacterMouvement.m_SpeedReduce = 1;
                m_canalisationTimer = 0.0f;
                return false;
            }

           

        }

        private void UpdateCanalisationBar(float maxValue)
        {
            float ratio = m_canalisationTimer / maxValue;
            m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
            m_characterAim.vfxCast.SetFloat("Progress", ratio);
            m_characterAim.vfxCastEnd.SetFloat("Progress", ratio);
            if (lastElementToUse != null)
            {
                lastElementToUse.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2, ratio);
            }
        }

        private bool CancelShoot()
        {
            if (!m_shootInputActive && hasCanalise)
            {

                EndShoot();
                return true;
            }
            return false;
        }

        private void Shoot()
        {
            if (!m_canShoot) return;

            //GlobalSoundManager.PlayOneShot(27, transform.position);

            m_BookAnimator.SetBool("Shooting", true);
            m_lastTimeShot = Time.time;
            m_CharacterMouvement.m_SpeedReduce = 0.25f;
            if (currentShotNumber == 0 && !m_hasBeenLoad)
            {
                StartShoot();

                m_CharacterAnimator.SetTrigger("Shot" + m_currentIndexCapsule);
                return;
            }

            if (m_currentIndexCapsule == -1 || m_canEndShot)
            {
                EndShoot();
                return;
            }

            if (m_currentType == GuerhoubaGames.GameEnum.BuffType.DAMAGE_SPELL)
            {
                ShootAttack(m_currentIndexCapsule, ref currentShotNumber, ref m_canEndShot);
                m_stackingClock[m_currentRotationIndex].RemoveStack();
                gsm.CanalisationParameterLaunch(0.5f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);
            }


        }

        public int GetCapsuleIndex(int offset)
        {
            offset = Mathf.Clamp(offset, -spellEquip.Length - 1, spellEquip.Length - 1);
            int index = m_currentIndexCapsule + offset;
            if (index >= spellEquip.Length)
            {
                index -= spellEquip.Length;
            }
            if (index < 0)
            {
                index += spellEquip.Length;
            }

            return m_currentIndexCapsule;
        }

        public int GetCurrentCapsuleIndex()
        {
            return m_currentIndexCapsule;
        }

        public void LaunchShootUniqueSpell(int index)
        {
            //Debug.Log("Launch ");
            for (int i = 0; i < m_spellCouroutine.Length; i++)
            {
                if (m_spellCouroutine[i] == null)
                {
                    m_spellCouroutine[i] = StartCoroutine(ShootUniqueSpell(index, i, EndCouroutine));
                    return;
                }
            }


        }

        public void EndCouroutine(int index)
        {
            m_spellCouroutine[index] = null;
        }

        private IEnumerator ShootUniqueSpell(int indexSpell, int indexCouroutine, System.Action<int> test)
        {
            SpellSystem.SpellProfil stats = GetCurrentWeaponStat(indexSpell);
            bool isFinish = false;
            int currentSpellCount = 0;
            while (!isFinish)
            {
                ShootSpell(indexSpell, ref currentSpellCount, ref isFinish);
                yield return new WaitForSeconds(stats.GetFloatStat(StatType.TimeBetweenShot));
            }
            yield return null;
            test(indexCouroutine);
        }

        private void ShootSpell(int index, ref int currentShootCount, ref bool endShoot)
        {
            ShootAttack(index, ref currentShootCount, ref endShoot);
        }

        private void ShootAttack(int index, ref int currentShootCount, ref bool endShoot)
        {

            SpellSystem.SpellProfil stats = currentCloneSpellProfil;
            // The first spell nature indicate the spell launching interpretation
            if (stats.TagList.spellNatureType == SpellNature.PROJECTILE)
            {
                endShoot = ShootAttackProjectile(index, ref currentShootCount);
                return;
            }

            if (stats.TagList.spellNatureType == SpellNature.AREA)
                endShoot = ShootAttackArea(index);


            if (stats.TagList.spellNatureType == SpellNature.MULTI_HIT_AREA)
                endShoot = ShootAttackDot(index);

            if (stats.TagList.spellNatureType == SpellNature.SUMMON)
                endShoot = ShootSummon(index);


        }

        private SpellSystem.SpellProfil GetCurrentWeaponStat(int index) { return spellProfils[m_currentIndexCapsule]; }

        public bool ShootAttackDot(int capsuleIndex)
        {
            SpellSystem.SpellProfil spellProfil = currentCloneSpellProfil;
            BehaviorLevel[] behaviorLevels;
            {
                BehaviorLevel[] behaviorLevels1 = spellProfil.GetBehaviorsLevels();
                    BehaviorLevel[] behaviorLevels2 = m_characterChainEffect.GetAllBehaviorLevel();
                behaviorLevels = new BehaviorLevel[behaviorLevels1.Length + behaviorLevels2.Length];
                behaviorLevels1.CopyTo(behaviorLevels, 0);
                behaviorLevels2.CopyTo(behaviorLevels, behaviorLevels1.Length);
            }

            if (areaInstance)
            {

                SpellSystem.AreaMeta areaMetaComponent = areaInstance.GetComponent<SpellSystem.AreaMeta>();
                areaMetaComponent.RelaunchArea();
                return false;

            }

            Transform transformUsed = transform;
            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            Vector3 directoinInstance =  m_characterAim.GetAimFinalPoint() - transformUsed.position;
            //Vector3 instancePosition = transformUsed.position + directoinInstance.normalized * spellProfil.GetFloatStat(StatType.Range);
            Vector3 instancePosition = m_characterAim.GetAimFinalPoint();
            areaInstance = GameObject.Instantiate(spellProfil.objectToSpawn, instancePosition, rot);

            DamageCalculComponent damageCalculComponent = areaInstance.GetComponent<DamageCalculComponent>();
            damageCalculComponent.Init(m_characterDamageComponent, this,spellProfil);

            if (spellProfil.TagList.EqualsSpellNature(SpellNature.MULTI_HIT_AREA))
            {
                SpellSystem.MultiHitAreaData dataDot = new SpellSystem.MultiHitAreaData();
                dataDot.spellProfil = spellProfil;
                dataDot.characterShoot = this;
                dataDot.currentMaxHitCount = m_currentStack[m_currentRotationIndex];
                SpellSystem.MultiHitAreaMeta dOTMeta = areaInstance.GetComponent<SpellSystem.MultiHitAreaMeta>();
                dOTMeta.dotData = dataDot;
                dOTMeta.ResetOnSpawn();
            }

            SpellSystem.AreaData data = FillAreaData(spellProfil, m_characterAim.lastRawPosition);
            SpellSystem.AreaMeta areaMeta = areaInstance.GetComponent<SpellSystem.AreaMeta>();
            areaMeta.areaData = data;
            areaMeta.ResetOnSpawn();

            foreach (BehaviorLevel behaviorLevel in behaviorLevels)
            {

                behaviorLevel.ActiveInstanceBehavior( areaInstance,  spellProfil);
            }

            return false;
        }

        private bool ShootAttackProjectile(int capsuleIndex, ref int currentShotCount)
        {
            if (m_currentStack[m_currentRotationIndex] <= 0)
            {

                m_stackingClock[m_currentRotationIndex].RemoveAllStack();
                return false;
            }


                SpellSystem.SpellProfil spellProfil = currentCloneSpellProfil;
            PlayerEffectStats<StatData> stats = spellProfil.gameEffectStats;
            BehaviorLevel[] behaviorLevels;
            {
                BehaviorLevel[] behaviorLevels1 = spellProfil.GetBehaviorsLevels();
                BehaviorLevel[] behaviorLevels2 = m_characterChainEffect.GetAllBehaviorLevel();
                behaviorLevels = new BehaviorLevel[behaviorLevels1.Length + behaviorLevels2.Length];
                behaviorLevels1.CopyTo(behaviorLevels, 0);
                behaviorLevels2.CopyTo(behaviorLevels, behaviorLevels1.Length);
            }

           
            float angle = GetShootAngle(spellProfil);
            int mod = GetStartIndexProjectile(spellProfil);

            for (int i = 0; i < spellProfil.GetIntStat(StatType.Projectile); i++)
            {
                Transform transformUsed = transform;


                Vector3 position = transformUsed.position + Quaternion.AngleAxis(transform.rotation.eulerAngles.y ,transform.up) * m_spawnPointSpell;
                Quaternion rot = m_characterAim.GetTransformHead().rotation * Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up);

                if (stats.tagData.spellMovementBehavior == SpellMovementBehavior.Fix)
                {
                    position = m_characterAim.lastRawPosition + Mathf.Clamp(i, 0, 1) * (Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up) * m_characterAim.GetTransformHead().forward * spellProfil.GetFloatStat(StatType.OffsetDistance));
                    rot = m_characterAim.GetTransformHead().rotation; ;
                }

                if (stats.tagData.spellProjectileTrajectory == SpellProjectileTrajectory.RANDOM)
                {
                    rot = m_characterAim.GetTransformHead().rotation * Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), transformUsed.up);

                }

                GameObject projectileCreate = GamePullingSystem.SpawnObject(spellProfil.objectToSpawn, position, rot);
                projectileCreate.transform.localScale = projectileCreate.transform.localScale;

                DamageCalculComponent damageCalculComponent = projectileCreate.GetComponent<DamageCalculComponent>();

                damageCalculComponent.Init(m_characterDamageComponent, this, spellProfil);
                if (projectileCreate.GetComponent<Projectile>())
                {
                    ProjectileData data = FillProjectileData(spellProfil, 0, angle, transformUsed);
                    projectileCreate.GetComponent<Projectile>().SetProjectile(data, this.m_chracterProfil);
                    data.characterShoot = this;
                    foreach (BehaviorLevel behaviorLevel in behaviorLevels)
                    {
                        ProjectileShootData projectileShootData = new ProjectileShootData();
                        projectileShootData.position = position;
                        projectileShootData.rotation = rot;
                        projectileShootData.profil = spellProfil;
                        projectileShootData.projectileData = data;

                        behaviorLevel.OnProjectileShoot(projectileShootData,projectileCreate);
                        behaviorLevel.ActiveInstanceBehavior(projectileCreate, spellProfil);
                    }
                }
                else
                {
                    foreach (BehaviorLevel behaviorLevel in behaviorLevels)
                    {
                        behaviorLevel.ActiveInstanceBehavior(projectileCreate, spellProfil);
                    }
                }

                if (stats.tagData.EqualsSpellNature(SpellNature.AREA))
                {
                    SpellSystem.AreaData data = FillAreaData(spellProfil, m_characterAim.lastRawPosition);
                    SpellSystem.AreaMeta areaMeta = projectileCreate.GetComponent<SpellSystem.AreaMeta>();
                    if(areaMeta)
                    {
                        areaMeta.areaData = data;
                        areaMeta.ResetOnSpawn();
                    }
                  
                }


                angle = -angle;
            }


            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            m_currentStack[m_currentRotationIndex]--;
            //m_uiPlayerInfos.UpdateStackingObjects(m_currentRotationIndex, m_currentStack[m_currentRotationIndex]);
            float ratio = (float)(m_currentStack[m_currentRotationIndex] / spellProfil.GetIntStat(StatType.ShootNumber));
            if (m_canalisationType == CanalisationBarType.ByPart)
            {
                m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, spellProfil.GetIntStat(StatType.ShootNumber));
                //m_characterAim.vfxCast.SetFloat("Progress", ratio);
                //m_characterAim.vfxCastEnd.SetFloat("Progress", ratio);
            }
            if (m_currentStack[m_currentRotationIndex] <= 0)
            {
                return false;

            }

            else
            {
                return false;
            }


        }

        private bool ShootAttackArea(int capsuleIndex)
        {
            SpellSystem.SpellProfil spellProfil = currentCloneSpellProfil;

            BehaviorLevel[] behaviorLevels;
            {
                BehaviorLevel[] behaviorLevels1 = spellProfil.GetBehaviorsLevels();
                BehaviorLevel[] behaviorLevels2 = m_characterChainEffect.GetAllBehaviorLevel();
                behaviorLevels = new BehaviorLevel[behaviorLevels1.Length + behaviorLevels2.Length];
                behaviorLevels1.CopyTo(behaviorLevels, 0);
                behaviorLevels2.CopyTo(behaviorLevels, behaviorLevels1.Length);
            }

            Transform transformUsed = transform;
            Vector3 directoinInstance = m_characterAim.lastRawPosition - transformUsed.position;
            //Vector3 instancePosition = transformUsed.position + directoinInstance * spellProfil.GetFloatStat(StatType.Range);
            Vector3 instancePosition = m_characterAim.GetAimFinalPoint(); ;
            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            GameObject areaInstance = GamePullingSystem.SpawnObject(spellProfil.objectToSpawn, instancePosition, rot);


            DamageCalculComponent damageCalculComponent = areaInstance.GetComponent<DamageCalculComponent>();
            damageCalculComponent.Init(m_characterDamageComponent, this, spellProfil);

            SpellSystem.AreaData data = FillAreaData(spellProfil, instancePosition);
            SpellSystem.AreaMeta areaMeta = areaInstance.GetComponent<SpellSystem.AreaMeta>();
            areaMeta.areaData = data;
            areaMeta.ResetOnSpawn();

            m_currentStack[m_currentRotationIndex]--;

            foreach (BehaviorLevel behaviorLevel in behaviorLevels)
            {
                behaviorLevel.ActiveInstanceBehavior(areaInstance, spellProfil);
            }

            if (m_currentStack[m_currentRotationIndex] <= 0)
                return false;
            else
                return false;
        }

        private bool ShootSummon(int capsuleIndex)
        {
            SpellSystem.SpellProfil spellProfil = currentCloneSpellProfil;
            PlayerEffectStats<StatData> stats = spellProfil.gameEffectStats;

            BehaviorLevel[] behaviorLevels;
            {
                BehaviorLevel[] behaviorLevels1 = spellProfil.GetBehaviorsLevels();
                BehaviorLevel[] behaviorLevels2 = m_characterChainEffect.GetAllBehaviorLevel();
                behaviorLevels = new BehaviorLevel[behaviorLevels1.Length + behaviorLevels2.Length];
                behaviorLevels1.CopyTo(behaviorLevels, 0);
                behaviorLevels2.CopyTo(behaviorLevels, behaviorLevels1.Length);
            }


            //Check if we have the max summon
            if (m_characterSummmonManager.m_summonSpellDictionnary.ContainsKey(spellProfil.id))
            {
                int summonCount = m_characterSummmonManager.GetSummonCount(spellProfil.id);
                if (summonCount == spellProfil.GetIntStat(StatType.MaxSummon))
                {
                    m_characterSummmonManager.CallSpecialAbility(spellProfil.id);
                    return false;
                }
            }
            //Spawn a new summon
            Vector3 positionToSpawn = transform.position;
            if (stats.tagData.spellMovementBehavior == SpellMovementBehavior.Fix)
            {
                positionToSpawn = m_characterAim.lastRawPosition;
            }

            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            GameObject summonInstance = GamePullingSystem.SpawnObject(spellProfil.objectToSpawn, positionToSpawn, rot);

            SpellSystem.SummonData data = new SpellSystem.SummonData();
            data.spellProfil = spellProfil;
            data.characterSummonManager = m_characterSummmonManager;
            summonInstance.GetComponent<SpellSystem.SummonsMeta>().summonData = data;


            // When the summon at the area tag
            if (stats.tagData.EqualsSpellNature(SpellNature.AREA))
            {
                SpellSystem.AreaData dataArea = FillAreaData(spellProfil, m_characterAim.lastRawPosition);
                SpellSystem.AreaMeta areaMeta = summonInstance.GetComponent<SpellSystem.AreaMeta>();
                areaMeta.areaData = dataArea;
                areaMeta.ResetOnSpawn();

            }

            if (stats.tagData.EqualsSpellNature(SpellNature.MULTI_HIT_AREA))
            {
                SpellSystem.MultiHitAreaData dataDot = new SpellSystem.MultiHitAreaData();
                dataDot.spellProfil = spellProfil;
                dataDot.characterShoot = this;
                dataDot.currentMaxHitCount = m_currentStack[m_currentRotationIndex];
                SpellSystem.MultiHitAreaMeta dOTMeta = summonInstance.GetComponent<SpellSystem.MultiHitAreaMeta>();
                dOTMeta.dotData = dataDot;
                dOTMeta.ResetOnSpawn();
            }

            if (!m_characterSummmonManager.m_summonSpellDictionnary.ContainsKey(spellProfil.id))
            {
                List<GameObject> summonList = new List<GameObject>();
                summonList.Add(summonInstance);
                m_characterSummmonManager.m_summonSpellDictionnary.Add(spellProfil.id, summonList);

            }
            else
            {
                List<GameObject> summonList = m_characterSummmonManager.m_summonSpellDictionnary[spellProfil.id];
                summonList.Add(summonInstance);
                m_characterSummmonManager.m_summonSpellDictionnary[spellProfil.id] = summonList;
            }

            foreach (BehaviorLevel behaviorLevel in behaviorLevels)
            {
                behaviorLevel.ActiveInstanceBehavior(areaInstance, spellProfil);
            }


            return true;
        }

        public void ActiveOnHit(Vector3 position, EntitiesTrigger tag, GameObject agent, GameElement element)
        {
            onHit(position, tag, agent, element);
        }

        private void StartShoot()
        {
            m_hasBeenLoad = true;
            m_currentType = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).gameEffectStats.tagData.type;
            m_canEndShot = false;

            ChainEffect[] chainEffectArray = currentCloneSpellProfil.GetChainEffects();
            for (int i = 0; i < chainEffectArray.Length; i++)
            {
                m_characterChainEffect.AddChainEffect(chainEffectArray[i],currentCloneSpellProfil);
            }

            m_characterChainEffect.ApplyChainEffect(currentCloneSpellProfil,false);



            m_uiPlayerInfos.ActiveSpellCanalisationUIv2(GetMaxStack(currentCloneSpellProfil), spellAttribution[m_currentRotationIndex].imageSpell);

            //if (m_CharacterMouvement.combatState) m_cameraBehavior.BlockZoom(true);
        }

        private void EndShoot()
        {

            if (!m_canShoot) return;

            currentShotNumber = 0;
            //gsm.CanalisationParameterStop();
            ChangeDecalTexture(GameElement.NONE);
            m_characterAim.vfxCast.SendEvent("EndShoot");
            m_characterAim.vfxCastEnd.SendEvent("EndShoot");
            m_characterAim.vfxCast.SetFloat("Progress", 0);
            m_characterAim.vfxCastEnd.SetFloat("Progress", 0);
            m_canalisationTimer = 0.0f;
            m_spellLaunchTime = 0.0f;
            m_CharacterMouvement.m_SpeedReduce = 1;
            m_uiPlayerInfos.DeactiveSpellCanalisation();
            areaInstance = null;
            m_characterDamageComponent.ResetDamage();
            if (!m_activeSpellLoad)
            {
                m_currentIndexCapsule = ChangeProjecileIndex();

                int maxStack = GetMaxStack(spellProfils[m_currentRotationIndex]);
                m_uiPlayerInfos.ActiveSpellCanalisationUIv2(maxStack, spellAttribution[m_currentRotationIndex].imageSpell);
                ChangeVfxOnUI(m_currentIndexCapsule);
            }

            // Chain Effect
            m_characterChainEffect.ApplyChainEffect(currentCloneSpellProfil, true);

            m_CharacterAnimator.ResetTrigger("Shot" + m_currentIndexCapsule);
            //castingVFXAnimator.ResetTrigger("Shot");
            m_currentType = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.type;

            lastElement = currentCloneSpellProfil.TagList.element;
            currentCloneSpellProfil = spellProfils[m_currentIndexCapsule].Clone();
            Color colToUse = capColorByElement[(int)GeneralTools.GetElementalArrayIndex(currentCloneSpellProfil.TagList.element)];
            Color colFlameToUse = capFalmeColorByElement[(int)GeneralTools.GetElementalArrayIndex(currentCloneSpellProfil.TagList.element)];

            m_characterAim.vfxStartShot.SetVector4("ColorBeam", colToUse);
            m_Mat_capeSkinedMesh.SetColor("_SelfLitColor", colToUse);        //Feedback Cape Color Element
            m_Mat_capeSkinedMesh.SetFloat("_SelfLitPower", (float)m_currentStack[m_currentRotationIndex]);                                                               //Feedback Cape Color Element
            m_Mat_Cape_Flamme[0].SetColor("_Color01", colFlameToUse);
            m_Mat_Cape_Flamme[0].SetColor("_Color02", colFlameToUse);
            m_Mat_Cape_Flamme[0].SetColor("_Color03", colFlameToUse);
            m_Mat_Cape_Flamme[1].SetColor("_Color01", colFlameToUse);
            m_Mat_Cape_Flamme[1].SetColor("_Color02", colFlameToUse);
            m_Mat_Cape_Flamme[1].SetColor("_Color03", colFlameToUse);
            m_Mat_capucheSkinedMesh.SetColor("_SelfLitColor", capColorByElement[(int)GeneralTools.GetElementalArrayIndex(currentCloneSpellProfil.TagList.element)]);
            m_Mat_capucheSkinedMesh.SetFloat("_SelfLitPower", (float)m_currentStack[m_currentRotationIndex]);
            ChangeVfxElement(((int)lastElement));
            if (!m_shootInput)
            {
                m_shootInputActive = false;
            }


            m_canShoot = false;
            m_isShooting = true;
            m_hasBeenLoad = false;



            float ratio = (float)(m_currentStack[m_currentRotationIndex] / GetMaxStack(currentCloneSpellProfil));
            m_uiPlayerInfos.DeactiveSpellCanalisation();

            m_deltaTimeFrame = UnityEngine.Time.deltaTime;

            float canalisationTime = 0;
            if (currentCloneSpellProfil.gameEffectStats.HasStats(StatType.SpellCanalisation))
                 canalisationTime = currentCloneSpellProfil.GetFloatStat(StatType.SpellCanalisation);
         
            m_totalCanalisationDuration = canalisationTime + baseCanalisationTime + m_deltaTimeFrame;
            SpellSystem.SpellProfil spellProfil = spellProfils[m_currentRotationIndex];

            if (m_canalisationType == CanalisationBarType.ByPart)
                m_totalLaunchingDuration = (m_currentStack[m_currentRotationIndex]);

            m_canalisationTimer = 0.0f;
            m_activeSpellLoad = true;
            hasCanalise = false;
            hasStartShoot = false;
        }

        public void ChangeVfxElement(int elementIndex)
        {
            for (int i = 0; i < (GeneralTools.GetElementalArrayIndex(GameElement.EARTH)); i++)
            {
                if (i == elementIndex)
                {
                    vfxElementSign[i].SetActive(true);
                    lastElementToUse = vfxElementSign[i];
                }
                else
                {
                    vfxElementSign[i].SetActive(false);
                }
            }
        }

        private ProjectileData FillProjectileData(SpellSystem.SpellProfil spellProfil, float index, float angle, Transform transformUsed)
        {
            ProjectileData data = new ProjectileData();

            data.characterShoot = this;
            Vector3 baseDirection = transform.forward;
            if (m_characterAim.HasCloseTarget() || m_aimModeState != AimMode.AimControls) baseDirection = m_characterAim.GetAimDirection();
            data.direction = Quaternion.AngleAxis(angle * ((index + 1) / 2), transformUsed.up) * baseDirection;

            data.spellProfil = spellProfil;

            //if (spellProfil.spellNatureType == SpellNature.PROJECTILE)
            //{
            //    Vector3 dest = Quaternion.AngleAxis(angle * ((index + 1) / 2), transformUsed.up) * m_characterAim.GetAimFinalPoint();
            //    if ((dest - transformUsed.position).magnitude > spellProfil.GetFloatStat(StatType.Range))
            //        dest = transformUsed.position - (Vector3.up * 0.5f) + (dest - transformUsed.position).normalized * spellProfil.GetFloatStat(StatType.Range);
            //}

            data.destination = m_characterAim.GetAimFinalPoint();
            data.objectType = CharacterObjectType.SPELL;
            return data;
        }

        private int GetMaxStack(SpellSystem.SpellProfil spellProfil)
        {
            int maxStack = 1;
            if (spellProfil.TagList.spellNatureType == SpellNature.PROJECTILE) maxStack = spellProfil.GetIntStat(StatType.ShootNumber);
            else if (spellProfil.TagList.spellNatureType == SpellNature.MULTI_HIT_AREA) maxStack = spellProfil.GetIntStat(StatType.HitNumber);
            return maxStack;
        }

        private SpellSystem.AreaData FillAreaData(SpellSystem.SpellProfil spell, Vector3 spawnPosition)
        {
            SpellSystem.AreaData areaData = new SpellSystem.AreaData();

            areaData.characterShoot = this;
            areaData.spellProfil = spell;
            areaData.destination = spawnPosition;
            areaData.direction = transform.forward;
            areaData.objectType = CharacterObjectType.SPELL;
            return areaData;
        }

        private float GetShootAngle(SpellSystem.SpellProfil spellProfil)
        {
            return spellProfil.GetFloatStat(StatType.ShootAngle) / spellProfil.GetIntStat(StatType.Projectile);
        }

        /// <summary>
        /// Help to find the start position for create a circular arc effect
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetStartIndexProjectile(SpellSystem.SpellProfil spellProfil)
        {
            return spellProfil.GetIntStat(StatType.Projectile) % 2 == 1 ? 0 : 1;
        }

        public void UpdateMultipleShoot(StatType statType)
        {
            if (m_timerBetweenShoot >= currentCloneSpellProfil.GetFloatStat(statType))
            {

                if (m_canalisationType == CanalisationBarType.ByPart) m_spellLaunchTime -= 1;


                m_timerBetweenShoot = 0.0f;
                if (m_canEndShot)
                {
                    EndShoot();
                }
                Shoot();
            }
            else
            {

                m_timerBetweenShoot += Time.deltaTime;
                if (m_canalisationType == CanalisationBarType.Continious) m_spellLaunchTime += Time.deltaTime;

            }
            float ratio = 0;
            if (m_canalisationType == CanalisationBarType.Continious)
            {
                ratio = (m_totalLaunchingDuration - m_spellLaunchTime) / m_totalLaunchingDuration;
            }
            if (m_canalisationType == CanalisationBarType.ByPart)
            {
                ratio = m_spellLaunchTime / m_totalLaunchingDuration;

            }

            m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
            //m_characterAim.vfxCast.SetFloat("Progress", ratio);
            //m_characterAim.vfxCastEnd.SetFloat("Progress", ratio);
        }

        #endregion

        #region Stacking Functions
        public void UpdateStackingSystem()
        {
            for (int i = 0; i < maxSpellIndex; i++)
            {
                bool inputTest = !m_shootInputActive || i != m_currentRotationIndex;
                int index = spellEquip[i];
                SpellSystem.SpellProfil spellProfil = spellProfils[index];
                m_stackingClock[i].UpdateStackByCurrent(m_currentStack[i]);
                int maxStack = GetMaxStack(spellProfil);


                if (m_currentStack[i] != maxStack) m_stackingClock[i].ActiaveClock();

                inputTest = true;
                if (i == m_currentRotationIndex)
                {
                    m_uiPlayerInfos.CalculateRatio(m_currentStack[i], maxStack, m_stackingClock[i].GetRatio());
                }

                if (inputTest && m_stackingClock[i].UpdateTimer())
                {
                    m_currentStack[i] += spellProfils[index].GetIntStat(StatType.GainPerStack);

                    m_currentStack[i] = Mathf.Clamp(m_currentStack[i], 0, maxStack);
                    spellProfils[index] = spellProfil;
                    //m_uiPlayerInfos.UpdateStackingObjects(i, m_currentStack[i]);
                    if (m_currentStack[i] == maxStack) m_stackingClock[i].DeactivateClock();
                }

                ////else
                //{
                //    // Setup UI Display
                //}
            }
            m_Mat_capeSkinedMesh.SetFloat("_SelfLitPower", (float)m_currentStack[m_currentRotationIndex] + m_stackingClock[m_currentRotationIndex].GetRatio()); //Feedback Cape Color Element
            m_Mat_capucheSkinedMesh.SetFloat("_SelfLitPower", (float)m_currentStack[m_currentRotationIndex] + m_stackingClock[m_currentRotationIndex].GetRatio());
        }

        public void AddSpellStack()
        {
            int[] value = new int[maxSpellIndex];
            for (int i = 0; i < maxSpellIndex; i++)
            {
                int index = spellEquip[i];
                SpellSystem.SpellProfil spellProfil = spellProfils[index];
                m_currentStack[i] += spellProfils[index].GetIntStat(StatType.GainPerStack);
                m_currentStack[i] = Mathf.Clamp(m_currentStack[i], 0, GetMaxStack(spellProfil));
                value[i] = m_currentStack[i];
                spellProfils[index] = spellProfil;
            }
            //m_uiPlayerInfos.UpdateStackingObjects(value);
        }
        #endregion

        private int ChangeProjecileIndex()
        {
            if (m_currentRotationIndex == maxSpellIndex - 1)
            {
                m_isReloading = true;
                m_currentRotationIndex = 0;
                EndCasting();
                return spellEquip[0];
            }
            else
            {
                m_currentRotationIndex++;
                return spellEquip[m_currentRotationIndex];
            }
        }

        /// <summary>
        /// This function is counting the timing between each spell
        /// </summary>
        private void CountingCooldownBetweenSpell()
        {
            if (m_canShoot || m_isReloading) return;

            m_CharacterAnimator.SetBool("Shooting", false);
            m_BookAnimator.SetBool("Shooting", false);
            m_CharacterMouvement.m_SpeedReduce = 1;

            if (m_timerBetweenSpell > baseTimeBetweenSpell)
            {

                // Update Spell bar
                ResetReloadEffectSpellBar();

                m_canShoot = true;
                m_timerBetweenSpell = 0;

                return;
            }
            else
            {
                m_timerBetweenSpell += Time.deltaTime;
                // Update Spell bar 
                UpdatingSpellBarUI(baseTimeBetweenSpell);
            }
        }

        /// <summary>
        /// This function is counting the timing between a end of rotation and the next rotation
        /// </summary>
        private void ReloadWeapon(float time)
        {
            if (m_canShoot || !m_isReloading) return;


            if (!m_CharacterMouvement.activeCombatModeConstant)
                SetCombatMode(CombatPlayerState.NONE);
            //m_cameraBehavior.BlockZoom(false);
            SpellSystem.SpellProfil spellProfil = spellProfils[m_currentRotationIndex];
            int maxStack = GetMaxStack(spellProfil);

            float totalShootTime = time;
            if (m_reloadTimer > totalShootTime)
            {
                m_reloadTimer = 0;
                m_isReloading = false;
                m_canShoot = true;
                IsRealoadingSpellRotation = false;
                //m_uiPlayerInfos.ActiveSpellCanalisationUIv2(maxStack, icon_Sprite[m_currentRotationIndex]);
                ActiveIcon();
                AddSpellStack();

                ResetReloadEffectSpellBar();
                return;
            }
            else
            {

                m_reloadTimer += Time.deltaTime;
                IsRealoadingSpellRotation = true;

                UpdatingSpellBarUI(totalShootTime);


            }
        }

        #region UI Spell Bar Functions
        private void UpdatingSpellBarUI(float totalTime)
        {
            //---for (int i = 0; i < m_spellGlobalCooldown.Count; i++)
            //---{
            //---    m_spellGlobalCooldown[i].fillAmount = (totalTime - m_reloadTimer) / totalTime;
            //---    m_TextSpellGlobalCooldown[i].text = (totalTime - m_reloadTimer).ToString(".#");
            //---}
        }

        private void ResetReloadEffectSpellBar()
        {
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                //---icon_Sprite[i].color = Color.white;
                //---m_TextSpellGlobalCooldown[i].text = "";
            }
        }

        #endregion



        /// <summary>
        /// Start casting is the function launch when shoot input is done
        /// </summary>
        public void StartCasting()
        {
            if (isCasting) return;
            //currentManaValue -= 2;
            if (m_CharacterMouvement.isSliding)
            {
                m_CharacterMouvement.isSliding = false;
                m_CharacterAnimator.SetBool("sliding", false);
                m_BookAnimator.SetBool("sliding", false);
            }
            m_CharacterMouvement.CancelSlide();
            isCasting = true;
            m_CharacterAnimator.SetBool("Casting", true);
            m_BookAnimator.SetBool("Casting", true);
            SetCombatMode(CombatPlayerState.COMBAT);
            m_uiPlayerInfos.ActiveSpellCanalisationUIv2(GetMaxStack(currentCloneSpellProfil), spellAttribution[m_currentRotationIndex].imageSpell);

            //if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(true); bookSmoothFollow.JumpRandomly(); }
            return;
        }

        public void EndCasting()
        {

            if (!isCasting) return;

            // m_isCasting = false;
            if (!m_shootInput) m_shootInputActive = false;
            //m_cameraBehavior.BlockZoom(false);

            m_lastTimeShot = Mathf.Infinity;
            avatarTransform.localRotation = Quaternion.identity;
            //bookTransform.localRotation = Quaternion.identity;
            if (!m_CharacterMouvement.activeCombatModeConstant) 
                SetCombatMode(CombatPlayerState.NONE);
            //if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(false); bookSmoothFollow.Snap(); }

        }

        public void GetCircleInfos()
        {
            ActiveIcon();
        }

        private void ActiveIcon()
        {
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                //---icon_Sprite[i].color = Color.white;
            }
            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
        }

        public void RefreshActiveIcon(SpellSystem.SpellProfil[] spellProfilsIcon)
        {
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;

                int index = spellEquip[i];
                //---if (!icon_Sprite[i].IsActive()) icon_Sprite[i].enabled = true;
                //---icon_Sprite[i].sprite = spellProfilsIcon[index].spell_Icon;
                //---icon_Sprite[i].material = spellProfilsIcon[index].matToUse;

                //SignPosition[i].GetComponent<SpriteRenderer>().sprite = icon_Sprite[i].sprite;
            }

        }

        public Sprite[] GetSpellSprite()
        {
            Sprite[] spriteArray = new Sprite[maxSpellIndex];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;
                int index = spellEquip[i];
                spriteArray[i] = m_characterSpellBook.GetAllSpells()[index].spell_Icon;
            }

            return spriteArray;
        }
        public Material[]  GetSpellMaterial()
        {
            Material[] materialArray = new Material[maxSpellIndex];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;
                int index = spellEquip[i];
                materialArray[i] = m_characterSpellBook.GetAllSpells()[index].matToUse;
            }
            return materialArray;
        }
        public int[] GetSpellLevel()
        {
            int[] levelArray = new int[maxSpellIndex];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;
                int index = spellEquip[i];
                levelArray[i] = m_characterSpellBook.GetAllSpells()[index].currentSpellTier;
            }

            return levelArray;
        }

        public void InputChangeAimLayout(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                int indexAim = (int)m_aimModeState;
                //gsm.CanalisationParameterStop();
                indexAim++;

                if (indexAim == 3) indexAim = 0;

                m_aimModeState = AimMode.FullControl;
                Debug.Log("Change Aim mode : " + m_aimModeState.ToString());
                UpdateFeedbackAimLayout();
            }
        }

        public void UpdateSpellRarityCadre(SpellSystem.SpellProfil[] spellProfilsIcon)
        {
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;

                int index = spellEquip[i];
                //---spell_rarity[i].material = m_gameResources.raritySprite[(int)spellProfilsIcon[index].currentSpellTier];
                int spellNumberUpgrade = spellProfilsIcon[i].spellExp;
                for (int j = 0; j < 12; j++)
                {
                    //---if(j > spellNumberUpgrade)
                    //---{
                    //---    rankPoint[i].GetChild(j).gameObject.SetActive(false);
                    //---}
                    //---else
                    //---{
                    //---    rankPoint[i].GetChild(j).gameObject.SetActive(true);
                    //---}
                }
                //SignPosition[i].GetComponent<SpriteRenderer>().sprite = icon_Sprite[i].sprite;
            }
            for(int j = 0; j < spellEquip.Length; j++)
            {
                if (spellEquip[j] != -1)
                {
                    spellAttribution[j].AcquireSpellData(spellProfilsIcon[j]);
                }

            }
        }

        public void UpdateFeedbackAimLayout()
        {
            //m_textCurrentLayout.text = "Current layout : \n" + m_aimModeState.ToString();
        }

        #region Spell Functions


        public void AddSpell(int index)
        {
            int prevIndex = m_characterSpellBook.GetSpellCount();
            SpellSystem.SpellProfil clone = m_spellManger.spellProfils[index].Clone(true);
            if (spellIndexGeneral.Count + 1 <= spellEquip.Length)
            {
                Debug.Log("Add Normaly");
                m_characterSpellBook.AddSpell(clone);
                spellIndexGeneral.Add(index);


                m_dropInventory.AddNewItem(index);

                CreatePullObject(clone);
                if (spellIndexGeneral.Count <= spellEquip.Length)
                {
                    //---icon_Sprite[spellIndexGeneral.Count - 1].transform.parent.gameObject.SetActive(true);
                    spellProfils.Add(clone);
                    spellAttribution[spellIndexGeneral.Count - 1].AcquireSpellData(clone);
                    spellEquip[spellIndexGeneral.Count - 1] = m_characterSpellBook.GetSpellCount() - 1;
                    m_characterSpellBook.m_spellsRotationArray[spellIndexGeneral.Count - 1] = clone;
                    spellAttribution[spellIndexGeneral.Count - 1].AcquireSpellData(clone);
                    m_characterSpellBook.m_currentSpellInRotationCount++;
                    //m_clockImage[spellIndexGeneral.Count - 1].gameObject.SetActive(true);
                    //m_textStack[spellIndexGeneral.Count - 1].gameObject.SetActive(true);
                    m_stackingClock[maxSpellIndex] = new ClockTimer();
                    m_stackingClock[maxSpellIndex].ActiaveClock();
                    m_stackingClock[maxSpellIndex].SetTimerDuration(spellProfils[maxSpellIndex].GetFloatStat(StatType.StackDuration), m_clockImage[maxSpellIndex], m_textStack[maxSpellIndex]);
                    maxSpellIndex = Mathf.Clamp(spellIndexGeneral.Count, 0, 4);
                    RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
                    m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();
                }
            }else
            {
                Debug.Log("Add Temp Spell");
                m_characterSpellBook.AddTempSpell(clone);
                m_characterSpellBook.OpenUIExchange();
            }
        }

        public void RemoveSpell(int index)
        {
            int spellIndex = spellEquip[index];
            this.spellIndexGeneral.RemoveAt(spellIndex);
            spellProfils.RemoveAt(spellIndex);
            m_characterSpellBook.RemoveSpell(spellIndex);
            m_stackingClock[index].DeactivateClock();
            m_characterSpellBook.m_currentSpellInRotationCount--;

            for (int i = index; i < 2; i++)
            {
                if (spellIndex < spellEquip[i + 1])
                    spellEquip[i + 1] = spellEquip[i + 1] - 1;

                spellEquip[i] = spellEquip[i + 1];
                m_characterSpellBook.m_spellsRotationArray[i] = m_characterSpellBook.GetSpecificSpell(spellEquip[i]);
            }

            maxSpellIndex = Mathf.Clamp(this.spellIndexGeneral.Count, 0, 4);
            spellEquip[maxSpellIndex] = -1;
            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
            m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();
        }

        public bool IsSpellAlreadyUse(int indexSpell)
        {
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (indexSpell == spellEquip[i]) return true;
            }
            return false;
        }

        public void ChangeSpell(int spellSlot, int indexSpell)
        {
            spellEquip[spellSlot] = indexSpell;

            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
            m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();
        }

        public void ExchangeSpell(int indexSpell1, int indexSpell2)
        {

            if (spellEquip[indexSpell1] == -1 || spellEquip[indexSpell2] == -1) return;

            int tempIndex = spellEquip[indexSpell1];
            spellEquip[indexSpell1] = spellEquip[indexSpell2];
            spellEquip[indexSpell2] = tempIndex;
            SpellProfil spellProfilTemp = m_characterSpellBook.m_spellsRotationArray[tempIndex];
            m_characterSpellBook.m_spellsRotationArray[indexSpell1] = m_characterSpellBook.m_spellsRotationArray[indexSpell2];
            m_characterSpellBook.m_spellsRotationArray[indexSpell2] = spellProfilTemp;


            m_stackingClock[indexSpell1].SetTimerDuration(spellProfils[indexSpell1].GetFloatStat(StatType.StackDuration), m_clockImage[indexSpell1], m_textStack[indexSpell1]);
            m_stackingClock[indexSpell2].SetTimerDuration(spellProfils[indexSpell2].GetFloatStat(StatType.StackDuration), m_clockImage[indexSpell2], m_textStack[indexSpell2]);

            int tempStack = m_currentStack[indexSpell1];
            m_currentStack[indexSpell1] = m_currentStack[indexSpell2];
            m_currentStack[indexSpell2] = tempStack;
            m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();
            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
            m_characterSpellBook.ActualizeUI();


        }

        public void ExchangeRotationSpellWithNew(int indexSpell)
        {

            SpellManager.ReAddSpell((m_characterSpellBook.m_spellsRotationArray[indexSpell]));
            CreatePullObject(m_characterSpellBook.tempSpell);
            spellProfils[indexSpell] = m_characterSpellBook.tempSpell;
            m_characterSpellBook.ReplaceSpell(m_characterSpellBook.m_spellsRotationArray[indexSpell], m_characterSpellBook.tempSpell);
            m_characterSpellBook.m_spellsRotationArray[indexSpell] = m_characterSpellBook.tempSpell;
             m_stackingClock[indexSpell] = new ClockTimer();
            m_stackingClock[indexSpell].ActiaveClock();
            m_stackingClock[indexSpell].SetTimerDuration(spellProfils[indexSpell].GetFloatStat(StatType.StackDuration), m_clockImage[indexSpell], m_textStack[indexSpell]);
            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());

            m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();


            //----------  UpdateUI ----------------
            currentCloneSpellProfil = spellProfils[0].Clone();
            m_currentIndexCapsule = 0;
            currentPreviewDecalTexture = spellProfils[0].previewDecal_mat;
            currentPreviewDecalEndTexture = spellProfils[0].previewDecalEnd_mat;
            ChangeDecalTexture(spellProfils[0].TagList.element);

            UpdateCanalisationBar(m_totalCanalisationDuration);
            gsm.CanalisationParameterLaunch(0.5f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).TagList.element - 0.01f);

        }


        public int GetIndexFromSpellBar(int indexSpellBar) { return spellEquip[indexSpellBar]; }

        #endregion

        public void ChangeVfxOnUI(int currentVfxToActive)
        {
            if (currentVfxToActive > 0)
            {
                vfxUISign[currentVfxToActive].SendEvent("OnPlay");
                vfxUISign[currentVfxToActive - 1].SendEvent("OnStop");
            }
            else
            {
                vfxUISign[currentVfxToActive].SendEvent("OnPlay");
                vfxUISign[spellProfils.Count - 1].SendEvent("OnStop");
            }
        }

        public Gradient SetDecalColor(GameElement gameElement)
        {
            Gradient color = m_gameResources.gradientDecalElement[0];
            switch (gameElement)
            {
                case GameElement.NONE:
                    color = m_gameResources.gradientDecalElement[0];
                    break;
                case GameElement.AIR:
                    color = m_gameResources.gradientDecalElement[2];
                    break;
                case GameElement.EARTH:
                    color = m_gameResources.gradientDecalElement[4];
                    break;
                case GameElement.FIRE:
                    color = m_gameResources.gradientDecalElement[3];
                    break;
                case GameElement.WATER:
                    color = m_gameResources.gradientDecalElement[1];
                    break;
            }
            return color;
        }

        public void ChangeDecalTexture(GameElement element)
        {
            //currentPreviewDecalTexture = m_initialPreviewDecal;
            m_characterAim.vfxCast.SendEvent("InitShot");
            m_characterAim.vfxCast.SetTexture("Symbol", currentPreviewDecalTexture);
            m_characterAim.vfxCast.SetGradient("Gradient 1", SetDecalColor(element));
            m_characterAim.vfxCastEnd.SendEvent("InitShot");
            m_characterAim.vfxCastEnd.SetTexture("Symbol", currentPreviewDecalEndTexture);
            m_characterAim.vfxCastEnd.SetGradient("Gradient 1", SetDecalColor(element));
        }


        public int CountSpellInstanceToSpawn(SpellProfil spellProfil)
        {
            int projectileCount = 1;
            int shootCount = 1;

            if (spellProfil.gameEffectStats.HasStats(StatType.Projectile)) projectileCount = spellProfil.GetIntStat(StatType.Projectile);
            if (spellProfil.gameEffectStats.HasStats(StatType.ShootNumber)) shootCount = spellProfil.GetIntStat(StatType.ShootNumber);

            int finalNumber = shootCount * projectileCount;

            // Spell can be reshoot before they are all destroy. So we boost the quantity;
            finalNumber += (int)(finalNumber * .7f);

            return finalNumber;
        }

        public void CreatePullObject(SpellProfil spellProfil)
        {
            int quantity = CountSpellInstanceToSpawn(spellProfil);
            PullConstructionData pullConstrutionData = new PullConstructionData(spellProfil.objectToSpawn, quantity);
            GamePullingSystem.instance.CreatePull(pullConstrutionData);
        }

        public void UpdatePullObject(SpellProfil spellProfil)
        {
            int quantity = CountSpellInstanceToSpawn(spellProfil);
            int id = GamePullingSystem.GetDeterministicHashCode(spellProfil.objectToSpawn.name);
            GamePullingSystem.instance.UpdatePullQuantity(quantity, id);
        }


        #region Combat State Functions
        public void SetCombatMode(CombatPlayerState newState)
        {
            if (newState == combatPlayerState) return;
            combatPlayerState = newState;
            return;
            if (combatPlayerState == CombatPlayerState.COMBAT)
            {
               
                OnCombatStarting?.Invoke();

                bookSmoothFollow.ChangeForBook(true);
            }
            if(combatPlayerState == CombatPlayerState.NONE)
            {
                
                OnCombatEnding?.Invoke();

                bookSmoothFollow.ChangeForBook(false);
            }

            
        }

        internal bool IsCombatMode()
        {
            return combatPlayerState == CombatPlayerState.COMBAT;
        }
        #endregion

        public void OnDrawGizmosSelected()
        {

            Vector3 gizmoPosition = transform.position + Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up) * m_spawnPointSpell;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gizmoPosition, .3f);
        }

    }




}