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
using Unity.VisualScripting;

namespace Character
{
    public class CharacterShoot : MonoBehaviour
    {

        private float m_timeBetweenRotation = 0.5f;
        public GlobalSoundManager gsm;
        [HideInInspector] public int projectileNumber;

        [Header("Shoot Parameters")]
        public float baseTimeBetweenSpell;
        public float reloadTime;
        public float baseCanalisationTime = 0.5f;
        public bool autoAimActive;
        public bool activeRandom = false;

        [Header("Spell Composition Parameters")]
        public int[] spellEquip;
        [HideInInspector] public int maxSpellIndex;
        public List<int> spellIndexGeneral;
        public List<int> spellIndexSpecific;

        [HideInInspector] public int m_currentIndexCapsule = 0;
        private int m_currentRotationIndex = 0;

        [Header("Shoot Component Setup")]
        public SpellManager m_spellManger;
        [SerializeField] private Transform m_OuterCircleHolder;
        [SerializeField] private Transform avatarTransform;
        [SerializeField] private Transform bookTransform;

        private SpellSystem.SpellProfil currentCloneSpellProfil;
        [HideInInspector] public List<SpellSystem.SpellProfil> spellProfils = new List<SpellSystem.SpellProfil>();
        public SpellSystem.SpellProfil weaponStat { get { return currentCloneSpellProfil; } private set { } }
        private int currentShotNumber;

        [SerializeField] private Render.Camera.CameraShake m_cameraShake;
        [SerializeField] private Render.Camera.CameraBehavior m_cameraBehavior;

        private CharacterAim m_characterAim;
        [HideInInspector] public CharacterMouvement m_CharacterMouvement; // Add reference to move script
        private Buff.BuffsManager m_buffManager;
        private CharacterProfile m_chracterProfil;
        private Character.CharacterUpgrade m_characterUpgrade;
        private Rigidbody m_rigidbody;

        private Animator m_CharacterAnimator;
        private Animator m_BookAnimator;


        [Header("Shoot Infos")]
        public GameElement lastElement;
        [SerializeField] public AimMode m_aimModeState;
        public bool isCasting;
        [SerializeField] private bool IsRealoadingSpellRotation;


        [Header("Shoot Feedback")]
        [SerializeField] private float m_shakeDuration = 0.1f;
        public GameObject[] vfxElementSign = new GameObject[4];
        [SerializeField] public List<UnityEngine.VFX.VisualEffect> m_SpellReadyVFX = new List<UnityEngine.VFX.VisualEffect>();
        private GameObject lastElementToUse;


        public bool isDirectSpellLaunchActivate = true;

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

        [SerializeField] private TMPro.TMP_Text m_textCurrentLayout;


        public float m_lastTimeShot = 0;
        [SerializeField] private float m_TimeAutoWalk = 2;

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
        [SerializeField] public List<Image> m_spellGlobalCooldown;
        [SerializeField] public List<TextMeshProUGUI> m_TextSpellGlobalCooldown;

        [SerializeField] private Sprite[] raritySprite;


        [Header("Spell Unique ")]
        public int numberOfUniqueSpell = 10;
        public Coroutine[] m_spellCouroutine;

        private DropInventory m_dropInventory;
        public Animator castingVFXAnimator;

        private bool m_hasCancel;
        private bool m_activeSpellLoad;
        private bool m_hasBeenLoad;

        public int specifiqueSpellStart = 0;
        public bool chooseBuild = false;

        public Texture currentPreviewDecalTexture; // Inspector, set texture named "box"
        public Texture currentPreviewDecalEndTexture; // Inspector, set texture named "box"

        private Texture m_initialPreviewDecal;

        public GameObject areaInstance;

        [GradientUsage(true)]
        public Gradient[] gradientDecalElement;
        public enum CanalisationBarType
        {
            ByPart,
            Continious,
        }

        [Header("CanalisationBar")]
        [SerializeField] private CanalisationBarType m_canalisationType;
        private float m_totalCanalisationDuration;
        private float m_totalLaunchingDuration;
        private float m_spellTimer;
        private float m_spellLaunchTime;
        private float m_deltaTimeFrame;
        private bool m_test;

        // Stacking Variable
        private ClockTimer[] m_stackingClock;
        private int[] m_currentStack = new int[4];
        private Image[] m_clockImage;
        private TMPro.TMP_Text[] m_textStack = new TMP_Text[4];

        #region Mana Variable
        [Header("Mana Parameters")]
        public Image manaSlider;
        public float manaMax;
        public float currentManaValue;
        public float regenPerSec;
        public float tempsAvantActiveRegenMana = 2;
        private bool activeDebug;

        #endregion
        public bool hasCanalise = false;
        public bool hasStartShoot = true;
        public bool hasShootBlock = false;

        public VisualEffect[] vfxUISign = new VisualEffect[4];

        private CharacterSummonManager m_characterSummmonManager;

        private SmoothFollow bookSmoothFollow;

        #region Unity Functions

        private void Awake()
        {
            m_spellCouroutine = new Coroutine[100];
            currentManaValue = manaMax;
            m_initialPreviewDecal = currentPreviewDecalTexture;
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
            int maxStack = GetMaxStack(spellProfil);
            //m_uiPlayerInfos.ActiveSpellCanalisationUIv2(maxStack, icon_Sprite[m_currentRotationIndex]);
            m_deltaTimeFrame = Time.deltaTime;
            m_totalCanalisationDuration = currentCloneSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime + m_deltaTimeFrame;

            m_aimModeState = AimMode.FullControl;
            if (m_canalisationType == CanalisationBarType.ByPart)
                m_totalLaunchingDuration = (m_currentStack[m_currentRotationIndex]);
        }

        private void Update()
        {
            if (!state.isPlaying) { return; } // Block Update during pause state
            if (m_lastTimeShot + tempsAvantActiveRegenMana < Time.time)
            {
                //float manaRegen = (Time.deltaTime * regenPerSec);
                //if (currentManaValue + manaRegen <= manaMax)
                //{
                //    currentManaValue += manaRegen;
                //}
                //else
                //{
                //    currentManaValue = manaMax;
                //}
            }
            //manaSlider.fillAmount = currentManaValue / manaMax;
            if (m_CharacterMouvement.combatState)
            {
                UpdateAvatarModels();
                AutomaticAimMode();
            }
            UpdateFullControlAimMode();
            UpdateStackingSystem();
            CountingCooldownBetweenSpell();
            ReloadWeapon(m_timeBetweenRotation);
        }

        #endregion

        private void UpdateAvatarModels()
        {
            if (m_aimModeState == AimMode.Automatic && !m_characterAim.HasCloseTarget()) return;

            m_characterAim.FeedbackHeadRotation();
            Quaternion rotationFromHead = m_characterAim.GetTransformHead().rotation;
            avatarTransform.rotation = rotationFromHead;
            bookTransform.rotation = rotationFromHead;
        }

        #region Aim Mode Functions
        private void AutomaticAimMode()
        {
            if (m_aimModeState == AimMode.FullControl) return;

            if (!m_isShooting)
                Shoot();
            if (m_isShooting)
            {
                if (m_timerBetweenShoot > currentCloneSpellProfil.GetFloatStat(StatType.TimeBetweenShot))
                {
                    Shoot();
                    m_CharacterMouvement.m_lastTimeShot = Time.time;
                    m_timerBetweenShoot = 0.0f;
                }
                else
                {
                    m_timerBetweenShoot += Time.deltaTime;
                }
            }
        }

        private void UpdateFullControlAimMode()
        {
            if (m_aimModeState != AimMode.FullControl) return;

            if (!m_CharacterMouvement.combatState || hasShootBlock) return;

            if (m_CharacterMouvement.mouvementState == CharacterMouvement.MouvementState.SpecialSpell) return;


            ShotCanalisation();
            if (!m_shootInputActive) m_CharacterMouvement.m_SpeedReduce = 1;
            if (IsRealoadingSpellRotation || !m_shootInputActive) return;

            InitShot();




            if (m_activeSpellLoad)
                return;

            if (!m_isShooting)
            {

                if (currentCloneSpellProfil.tagData.mouvementBehaviorType == MouvementBehavior.Dash)
                {
                    m_characterDash.SpellDash(currentCloneSpellProfil.GetFloatStat(StatType.MouvementTravelTime), currentCloneSpellProfil.GetFloatStat(StatType.DistanceDash));
                }
                m_isShooting = true;
                if (m_canalisationType == CanalisationBarType.ByPart) m_spellLaunchTime = m_totalLaunchingDuration;


                if (currentCloneSpellProfil.tagData.spellNatureType == SpellNature.PROJECTILE)
                {
                    if (!isDirectSpellLaunchActivate) m_timerBetweenShoot = currentCloneSpellProfil.GetFloatStat(StatType.TimeBetweenShot);

                    if (m_canalisationType == CanalisationBarType.Continious) m_totalLaunchingDuration = m_currentStack[m_currentRotationIndex] * currentCloneSpellProfil.GetFloatStat(StatType.TimeBetweenShot);

                }

                if (currentCloneSpellProfil.tagData.spellNatureType == SpellNature.AREA)
                {
                    if (!isDirectSpellLaunchActivate) m_timerBetweenShoot = currentCloneSpellProfil.GetFloatStat(StatType.SpellFrequency);

                    if (m_canalisationType == CanalisationBarType.Continious) m_totalLaunchingDuration = m_currentStack[m_currentRotationIndex] * currentCloneSpellProfil.GetFloatStat(StatType.SpellFrequency);

                }
                if (!isDirectSpellLaunchActivate) Shoot();
                return;
            }


            if (currentCloneSpellProfil.tagData.spellNatureType == SpellNature.PROJECTILE) UpdateMultipleShoot(StatType.TimeBetweenShot);
            if (currentCloneSpellProfil.tagData.spellNatureType == SpellNature.AREA) UpdateMultipleShoot(StatType.SpellFrequency);

            if (currentCloneSpellProfil.tagData.spellNatureType == SpellNature.DOT) Shoot();
            if (currentCloneSpellProfil.tagData.spellNatureType == SpellNature.SUMMON) Shoot();

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
            m_clockImage = m_uiPlayerInfos.ReturnClock();
            m_textStack = m_uiPlayerInfos.ReturnStack();
            if (m_BookAnimator.GetComponent<SmoothFollow>()) m_BookAnimator.GetComponent<SmoothFollow>();
        }

        private void InitSpriteSpell()
        {
            ActiveIcon();
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                m_spellGlobalCooldown[i].sprite = icon_Sprite[i].sprite;

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
                    m_characterSpellBook.m_currentSpellInRotationCount++;
                    icon_Sprite[i].transform.parent.gameObject.SetActive(true);
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
                //gsm.CanalisationParameterLaunch(0.01f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);
                m_shootInput = true;
                m_shootInputActive = true;
                m_lastTimeShot = Time.time;
                m_hasCancel = false;
                m_CharacterMouvement.combatState = true;
            }
            if (ctx.canceled && state.isPlaying)
            {
                m_shootInput = false;
                m_shootInputActive = false;
                CancelShoot();
                //gsm.CanalisationParameterLaunch(1, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);
                m_CharacterMouvement.m_SpeedReduce = 1;
            }
        }
        #endregion

        #region Shoot Function

        private void InitShot()
        {
            if (m_isShooting && m_activeSpellLoad || hasStartShoot) return;

            //m_activeSpellLoad = true;
            m_deltaTimeFrame = Time.deltaTime;
            hasStartShoot = true;
            currentPreviewDecalTexture = currentCloneSpellProfil.previewDecal_mat;
            currentPreviewDecalEndTexture = currentCloneSpellProfil.previewDecalEnd_mat;
            ChangeDecalTexture(currentCloneSpellProfil.tagData.element);
            gsm.CanalisationParameterLaunch(0.5f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);
            //m_totalCanalisationDuration = currentSpellProfil.spellCanalisation + baseCanalisationTime + m_deltaTimeFrame;

            //if (m_canalisationType == CanalisationBarType.ByPart)
            //    m_totalLaunchingDuration = (m_currentStack[m_currentRotationIndex]);

            //if (m_canalisationType == CanalisationBarType.Continious)
            //    m_totalLaunchingDuration = ((currentSpellProfil.timeBetweenShot) * (m_currentStack[m_currentRotationIndex] + 1));

            //Debug.Log(m_currentRotationIndex + "||" + m_spellGlobalCooldown[m_currentRotationIndex]);
            //m_uiPlayerInfos.ActiveSpellCanalisationUI(m_currentStack[m_currentRotationIndex], icon_Sprite[m_currentRotationIndex]);
            m_canEndShot = false;

            // m_isShooting = true;
            //  m_spellTimer = 0.0f;
        }

        public void DeactivateCanalisation()
        {
            m_spellTimer = 0.0f;
            //gsm.CanalisationParameterStop();
            m_uiPlayerInfos.DeactiveSpellCanalisation();
            isCasting = false;
        }

        public void ActivateCanalisation()
        {
            m_spellTimer = 0.0f;
            SpellSystem.SpellProfil spellProfil = spellProfils[m_currentRotationIndex];
            int maxStack = GetMaxStack(spellProfil);
            //m_uiPlayerInfos.ActiveSpellCanalisationUIv2(maxStack, icon_Sprite[m_currentRotationIndex]);
            UpdateCanalisationBar(m_totalCanalisationDuration);
            m_activeSpellLoad = true;
            hasCanalise = false;
        }

        private bool ShotCanalisation()
        {
            if (!m_activeSpellLoad || hasCanalise) return false;


            UpdateCanalisationBar(m_totalCanalisationDuration);


            bool highCanalisationTest = currentCloneSpellProfil.tagData.canalisationType == CanalisationType.HEAVY_CANALISATION && m_shootInputActive;

            if (highCanalisationTest)
            {
                m_CharacterMouvement.m_SpeedReduce = currentCloneSpellProfil.GetFloatStat(StatType.SpeedReduce);
                //gsm.CanalisationParameterLaunch(0.01f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);
                currentPreviewDecalTexture = currentCloneSpellProfil.previewDecal_mat;
                currentPreviewDecalEndTexture = currentCloneSpellProfil.previewDecalEnd_mat;
                ChangeDecalTexture(currentCloneSpellProfil.tagData.element);
            }
            else
            {
                m_CharacterMouvement.m_SpeedReduce = 1;
            }

            if (highCanalisationTest || currentCloneSpellProfil.tagData.canalisationType == CanalisationType.LIGHT_CANALISATION)
            {

                currentPreviewDecalTexture = currentCloneSpellProfil.previewDecal_mat;
                currentPreviewDecalEndTexture = currentCloneSpellProfil.previewDecalEnd_mat;
                ChangeDecalTexture(currentCloneSpellProfil.tagData.element);
                if (m_spellTimer >= currentCloneSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime)
                {
                    //gsm.CanalisationParameterLaunch(0.01f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);
                    m_activeSpellLoad = false;
                    m_isShooting = false;
                    m_spellTimer = m_totalCanalisationDuration;
                    UpdateCanalisationBar(m_totalCanalisationDuration);

                    hasCanalise = true;
                    return true;

                }
                else
                {
                    m_spellTimer += Time.deltaTime;

                    return false;
                }

            }
            else
            {
                m_spellTimer = 0.0f;
                return false;
            }

        }

        private void UpdateCanalisationBar(float maxValue)
        {
            float ratio = m_spellTimer / maxValue;
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
                gsm.CanalisationParameterLaunch(0.5f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);
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
            if (stats.tagData.spellNatureType == SpellNature.PROJECTILE)
            {
                endShoot = ShootAttackProjectile(index, ref currentShootCount);
                return;
            }

            if (stats.tagData.spellNatureType == SpellNature.AREA)
                endShoot = ShootAttackArea(index);


            if (stats.tagData.spellNatureType == SpellNature.DOT)
                endShoot = ShootAttackDot(index);

            if (stats.tagData.spellNatureType == SpellNature.SUMMON)
                endShoot = ShootSummon(index);


        }

        private SpellSystem.SpellProfil GetCurrentWeaponStat(int index) { return spellProfils[m_currentIndexCapsule]; }

        public bool ShootAttackDot(int capsuleIndex)
        {
            SpellSystem.SpellProfil spellProfil = currentCloneSpellProfil;


            if (areaInstance)
            {

                SpellSystem.AreaMeta areaMetaComponent = areaInstance.GetComponent<SpellSystem.AreaMeta>();
                areaMetaComponent.RelaunchArea();
                return false;

            }


            if (areaInstance)
            {

                SpellSystem.AreaMeta areaMetaComponent = areaInstance.GetComponent<SpellSystem.AreaMeta>();
                areaMetaComponent.RelaunchArea();
                return false;
                    
            }

            Transform transformUsed = transform;
            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            areaInstance = GameObject.Instantiate(spellProfil.objectToSpawn, m_characterAim.lastRawPosition, rot);
            if (spellProfil.tagData.EqualsSpellNature(SpellNature.DOT))
            {
                SpellSystem.DOTData dataDot = new SpellSystem.DOTData();
                dataDot.spellProfil = spellProfil;
                dataDot.characterShoot = this;
                dataDot.currentHitCount = m_currentStack[m_currentRotationIndex];
                SpellSystem.DOTMeta dOTMeta = areaInstance.GetComponent<SpellSystem.DOTMeta>();
                dOTMeta.dotData = dataDot;
                dOTMeta.ResetOnSpawn();
            }

            SpellSystem.AreaData data = FillAreaData(spellProfil, m_characterAim.lastRawPosition);
            SpellSystem.AreaMeta areaMeta = areaInstance.GetComponent<SpellSystem.AreaMeta>();
            areaMeta.areaData = data;
            areaMeta.ResetOnSpawn();

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
            BehaviorLevel[] behaviorLevels = spellProfil.GetBehaviorsLevels();
            float angle = GetShootAngle(spellProfil);
            int mod = GetStartIndexProjectile(spellProfil);

            for (int i = 0; i < spellProfil.GetIntStat(StatType.Projectile); i++)
            {
                Transform transformUsed = transform;


                Vector3 position = transformUsed.position + m_characterAim.GetTransformHead().forward * 10 + new Vector3(0, 4, 0);
                Quaternion rot = m_characterAim.GetTransformHead().rotation * Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up);
               
                if (spellProfil.tagData.spellMovementBehavior == SpellMovementBehavior.Fix)
                {
                    position = m_characterAim.lastRawPosition + Mathf.Clamp(i, 0, 1) * (Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up) * m_characterAim.GetTransformHead().forward * spellProfil.GetFloatStat(StatType.OffsetDistance));
                    rot = m_characterAim.GetTransformHead().rotation; ;
                }

                if (spellProfil.tagData.spellProjectileTrajectory == SpellProjectileTrajectory.RANDOM)
                {
                    rot = m_characterAim.GetTransformHead().rotation * Quaternion.AngleAxis(Random.Range(0,360), transformUsed.up);

                }

                    GameObject projectileCreate = GamePullingSystem.SpawnObject(spellProfil.objectToSpawn, position, rot);
                projectileCreate.transform.localScale = projectileCreate.transform.localScale;

                if (projectileCreate.GetComponent<Projectile>())
                {
                    ProjectileData data = FillProjectileData(spellProfil, 0, angle, transformUsed);
                    projectileCreate.GetComponent<Projectile>().SetProjectile(data, this.m_chracterProfil);
                    data.characterShoot =this;
                    foreach (BehaviorLevel behaviorLevel in behaviorLevels)
                    {
                        ProjectileShootData projectileShootData = new ProjectileShootData();
                        projectileShootData.position = position;
                        projectileShootData.rotation = rot;
                        projectileShootData.profil = spellProfil;
                        projectileShootData.projectileData = data;

                        behaviorLevel.OnProjectileShoot(projectileShootData);
                    }
                }

                if (spellProfil.tagData.EqualsSpellNature(SpellNature.AREA))
                {
                    SpellSystem.AreaData data = FillAreaData(spellProfil, m_characterAim.lastRawPosition);
                    SpellSystem.AreaMeta areaMeta = projectileCreate.GetComponent<SpellSystem.AreaMeta>();
                    areaMeta.areaData = data;
                    areaMeta.ResetOnSpawn();
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

            Transform transformUsed = transform;
            Vector3 position = transformUsed.position + m_characterAim.GetTransformHead().forward * 10 + new Vector3(0, 4, 0);
            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            GameObject areaInstance = GamePullingSystem.SpawnObject(spellProfil.objectToSpawn, position, rot);

            SpellSystem.AreaData data = FillAreaData(spellProfil, position);
            SpellSystem.AreaMeta areaMeta = areaInstance.GetComponent<SpellSystem.AreaMeta>();
            areaMeta.areaData = data;
            areaMeta.ResetOnSpawn();

            m_currentStack[m_currentRotationIndex]--;

            if (m_currentStack[m_currentRotationIndex] <= 0)
                return false;
            else
                return false;
        }

        private bool ShootSummon(int capsuleIndex)
        {
            SpellSystem.SpellProfil spellProfil = currentCloneSpellProfil;

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
            if (spellProfil.tagData.spellMovementBehavior == SpellMovementBehavior.Fix)
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
            if (spellProfil.tagData.EqualsSpellNature(SpellNature.AREA))
            {
                SpellSystem.AreaData dataArea = FillAreaData(spellProfil, m_characterAim.lastRawPosition);
                SpellSystem.AreaMeta areaMeta = summonInstance.GetComponent<SpellSystem.AreaMeta>();
                areaMeta.areaData = dataArea;
                areaMeta.ResetOnSpawn();

            }

            if (spellProfil.tagData.EqualsSpellNature(SpellNature.DOT))
            {
                SpellSystem.DOTData dataDot = new SpellSystem.DOTData();
                dataDot.spellProfil = spellProfil;
                dataDot.characterShoot = this;
                dataDot.currentHitCount = m_currentStack[m_currentRotationIndex];
                SpellSystem.DOTMeta dOTMeta = summonInstance.GetComponent<SpellSystem.DOTMeta>();
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



            return true;
        }

        public void ActiveOnHit(Vector3 position, EntitiesTrigger tag, GameObject agent, GameElement element)
        {
            onHit(position, tag, agent, element);
        }

        private void StartShoot()
        {
            m_hasBeenLoad = true;
            m_currentType = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.type;
            m_canEndShot = false;

            ChainEffect[] chainEffectArray = currentCloneSpellProfil.GetChainEffects();
            for (int i = 0; i < chainEffectArray.Length; i++)
            {
                m_characterChainEffect.AddChainEffect(chainEffectArray[i]);
            }

            m_characterChainEffect.ApplyChainEffect(currentCloneSpellProfil);



            m_uiPlayerInfos.ActiveSpellCanalisationUIv2(GetMaxStack(currentCloneSpellProfil), icon_Sprite[m_currentRotationIndex]);

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
            m_spellTimer = 0.0f;
            m_spellLaunchTime = 0.0f;
            m_CharacterMouvement.m_SpeedReduce = 1;
            m_uiPlayerInfos.DeactiveSpellCanalisation();
            areaInstance = null;

            if (!m_activeSpellLoad)
            {
                m_currentIndexCapsule = ChangeProjecileIndex();

                int maxStack = GetMaxStack(spellProfils[m_currentRotationIndex]);
                m_uiPlayerInfos.ActiveSpellCanalisationUIv2(maxStack, icon_Sprite[m_currentRotationIndex]);
                ChangeVfxOnUI(m_currentIndexCapsule);
            }



            currentManaValue -= 2;
            m_CharacterAnimator.ResetTrigger("Shot" + m_currentIndexCapsule);
            //castingVFXAnimator.ResetTrigger("Shot");
            m_currentType = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.type;

            lastElement = currentCloneSpellProfil.tagData.element;
            currentCloneSpellProfil = spellProfils[m_currentIndexCapsule].Clone();
            ChangeVfxElement(((int)lastElement));
            if (!m_shootInput)
            {
                m_shootInputActive = false;
                //gsm.CanalisationParameterStop(); 
            }
            //else gsm.CanalisationParameterLaunch(0.1f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element - 0.01f);


            m_canShoot = false;
            m_isShooting = true;
            m_hasBeenLoad = false;



            float ratio = (float)(m_currentStack[m_currentRotationIndex] / GetMaxStack(currentCloneSpellProfil));
            m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
           //m_characterAim.vfxCast.SetFloat("Progress", ratio);
           //m_characterAim.vfxCastEnd.SetFloat("Progress", ratio);
            m_uiPlayerInfos.DeactiveSpellCanalisation();

            m_deltaTimeFrame = UnityEngine.Time.deltaTime;
            m_totalCanalisationDuration = currentCloneSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime + m_deltaTimeFrame;
            SpellSystem.SpellProfil spellProfil = spellProfils[m_currentRotationIndex];

            if (m_canalisationType == CanalisationBarType.ByPart)
                m_totalLaunchingDuration = (m_currentStack[m_currentRotationIndex]);

            m_spellTimer = 0.0f;
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
            if (spellProfil.tagData.spellNatureType == SpellNature.PROJECTILE) maxStack = spellProfil.GetIntStat(StatType.ShootNumber);
            else if (spellProfil.tagData.spellNatureType == SpellNature.DOT) maxStack = spellProfil.GetIntStat(StatType.HitNumber);
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
            float totalShootTime = baseTimeBetweenSpell;

            if (m_timerBetweenSpell > totalShootTime)
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
                UpdatingSpellBarUI(totalShootTime);
            }
        }

        /// <summary>
        /// This function is counting the timing between a end of rotation and the next rotation
        /// </summary>
        private void ReloadWeapon(float time)
        {
            if (m_canShoot || !m_isReloading) return;


            if (!m_CharacterMouvement.activeCombatModeConstant)
                m_CharacterMouvement.SetCombatMode(false);

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
            for (int i = 0; i < m_spellGlobalCooldown.Count; i++)
            {
                m_spellGlobalCooldown[i].fillAmount = (totalTime - m_reloadTimer) / totalTime;
                m_TextSpellGlobalCooldown[i].text = (totalTime - m_reloadTimer).ToString(".#");
            }
        }

        private void ResetReloadEffectSpellBar()
        {
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                icon_Sprite[i].color = Color.white;
                m_TextSpellGlobalCooldown[i].text = "";
            }
        }

        #endregion


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
            m_CharacterMouvement.SetCombatMode(true);
            //if (bookSmoothFollow) { bookSmoothFollow.ChangeForBook(true); bookSmoothFollow.JumpRandomly(); }
            return;
        }

        public void EndCasting()
        {
            if (autoAimActive) return;

            if (!isCasting) return;

            // m_isCasting = false;
            if (!m_shootInput) m_shootInputActive = false;
            //m_cameraBehavior.BlockZoom(false);

            m_lastTimeShot = Mathf.Infinity;
            avatarTransform.localRotation = Quaternion.identity;
            //bookTransform.localRotation = Quaternion.identity;
            if (!m_CharacterMouvement.activeCombatModeConstant) m_CharacterMouvement.SetCombatMode(false);
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
                icon_Sprite[i].color = Color.white;
            }
            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
        }

        public void RefreshActiveIcon(SpellSystem.SpellProfil[] spellProfilsIcon)
        {
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;

                int index = spellEquip[i];

                icon_Sprite[i].sprite = spellProfilsIcon[index].spell_Icon;
                icon_Sprite[i].material = spellProfilsIcon[index].matToUse;

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

        public int[] GetSpellLevel()
        {
            int[] levelArray = new int[maxSpellIndex];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;
                int index = spellEquip[i];
                levelArray[i] = m_characterSpellBook.GetAllSpells()[index].spellLevel;
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
                spell_rarity[i].sprite = raritySprite[(int)spellProfilsIcon[index].spellLevel];

                //SignPosition[i].GetComponent<SpriteRenderer>().sprite = icon_Sprite[i].sprite;
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
            spellIndexGeneral.Add(index);

            SpellSystem.SpellProfil clone = m_spellManger.spellProfils[index].Clone(true);
            m_characterSpellBook.AddSpell(clone);
            m_dropInventory.AddNewItem(index);

            CreatePullObject(clone);
            if (spellIndexGeneral.Count <= spellEquip.Length)
            {
                icon_Sprite[spellIndexGeneral.Count - 1].transform.parent.gameObject.SetActive(true);
                spellProfils.Add(clone);
                spellEquip[spellIndexGeneral.Count - 1] = m_characterSpellBook.GetSpellCount() - 1;
                m_characterSpellBook.m_spellsRotationArray[spellIndexGeneral.Count - 1] = clone;
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
            //m_uiPlayerInfos.UpdateStackingObjects(m_currentStack);
            m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();
            RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
            m_characterSpellBook.ActualizeUI();


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
            Gradient color = gradientDecalElement[0];
            switch (gameElement)
            {
                case GameElement.NONE:
                    color = gradientDecalElement[0];
                    break;
                case GameElement.AIR:
                    color = gradientDecalElement[2];
                    break;
                case GameElement.EARTH:
                    color = gradientDecalElement[4];
                    break;
                case GameElement.FIRE:
                    color = gradientDecalElement[3];
                    break;
                case GameElement.WATER:
                    color = gradientDecalElement[1];
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

            if (spellProfil.HasStats(StatType.Projectile)) projectileCount = spellProfil.GetIntStat(StatType.Projectile);
            if (spellProfil.HasStats(StatType.ShootNumber)) shootCount = spellProfil.GetIntStat(StatType.ShootNumber);

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


    }




}