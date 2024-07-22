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
        public List<int> spellIndex;

        private int m_currentIndexCapsule = 0;
        private int m_currentRotationIndex = 0;

        [Header("Shoot Component Setup")]
        public CapsuleManager m_capsuleManager;
        [SerializeField] private Transform m_OuterCircleHolder;
        [SerializeField] private Transform avatarTransform;
        [SerializeField] private Transform bookTransform;

        private SpellSystem.SpellProfil currentSpellProfil;
        [HideInInspector] public List<SpellSystem.SpellProfil> spellProfils = new List<SpellSystem.SpellProfil>();
        public SpellSystem.SpellProfil weaponStat { get { return currentSpellProfil; } private set { } }
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


        private float m_timerBetweenSpell;
        private float m_reloadTimer;
        private float m_timeBetweenShoot;

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

        private CharacterSpellBook m_characterSpellBook;

        public delegate void OnHit(Vector3 position, EntitiesTrigger tag, GameObject objectHit);
        public event OnHit onHit = delegate { };

        [Header("UI Object")]
        public GameObject uiManager;
        private UI_PlayerInfos m_uiPlayerInfos;
        [SerializeField] public List<Image> icon_Sprite;
        [SerializeField] public List<Image> m_spellGlobalCooldown;
        [SerializeField] public List<TextMeshProUGUI> m_TextSpellGlobalCooldown;


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

        #region Unity Functions

        private void Awake()
        {
            m_spellCouroutine = new Coroutine[100];
            currentManaValue = manaMax;
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
            for (int i = 0; i < spellIndex.Count; i++)
            {
                m_dropInventory.AddNewItem(spellIndex[i]);
            }

            // Init Variables
            m_currentRotationIndex = 0;
            m_currentIndexCapsule = spellEquip[0];
            currentSpellProfil = spellProfils[m_currentIndexCapsule];
            m_canShoot = true;
            m_activeSpellLoad = true;
            m_uiPlayerInfos.ActiveSpellCanalisationUI(m_currentStack[m_currentRotationIndex], icon_Sprite[m_currentRotationIndex]);
            m_deltaTimeFrame = Time.deltaTime;
            m_totalCanalisationDuration = currentSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime + m_deltaTimeFrame;

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
                if (m_timeBetweenShoot > currentSpellProfil.GetFloatStat(StatType.TimeBetweenShot))
                {
                    Shoot();
                    m_CharacterMouvement.m_lastTimeShot = Time.time;
                    m_timeBetweenShoot = 0.0f;
                }
                else
                {
                    m_timeBetweenShoot += Time.deltaTime;
                }
            }
        }

        private void UpdateFullControlAimMode()
        {
            if (m_aimModeState != AimMode.FullControl) return;

            if (!m_CharacterMouvement.combatState || hasShootBlock) return;

            ShotCanalisation();
            if (!m_shootInputActive) m_CharacterMouvement.m_SpeedReduce = 1;
            if (IsRealoadingSpellRotation || !m_shootInputActive) return;

            InitShot();




            if (m_activeSpellLoad)
                return;

            if (!m_isShooting)
            {
                m_isShooting = true;
                if (m_canalisationType == CanalisationBarType.ByPart) m_spellLaunchTime = m_totalLaunchingDuration;
                m_timeBetweenShoot = currentSpellProfil.GetFloatStat(StatType.TimeBetweenShot);
                Shoot();
                return;
            }

            if (m_timeBetweenShoot >= currentSpellProfil.GetFloatStat(StatType.TimeBetweenShot))
            {

                if (m_canalisationType == CanalisationBarType.ByPart) m_spellLaunchTime -= 1;
                gsm.CanalisationParameterLaunch(0.5f, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element + 0.5f);

                m_timeBetweenShoot = 0.0f;
                if (m_canEndShot)
                {
                    EndShoot();
                }
                Shoot();
            }
            else
            {

                m_timeBetweenShoot += Time.deltaTime;
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
            // m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
        }
        #endregion

        #region Start Functions
        public void InitComponents()
        {
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>();
            pauseScript = GetComponent<PauseMenu>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_buffManager = GetComponent<Buff.BuffsManager>();
            m_chracterProfil = GetComponent<CharacterProfile>();
            m_characterUpgrade = GetComponent<CharacterUpgrade>();
            m_characterSpellBook = GetComponent<CharacterSpellBook>();
            m_uiPlayerInfos = uiManager.GetComponent<UI_PlayerInfos>();
            m_CharacterAnimator = avatarTransform.GetComponent<Animator>();
            m_BookAnimator = bookTransform.GetComponent<Animator>();
            m_clockImage = m_uiPlayerInfos.ReturnClock();
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
                currentSpellProfil = spellProfils[m_currentIndexCapsule];
            }
        }



        public void InitCapsule()
        {
            // Add Spell in Spell List
            // Get the spell stats for each spell
            for (int i = 0; i < spellIndex.Count; i++)
            {
                if (spellIndex[i] == -1) continue;

                m_characterSpellBook.AddSpell(m_capsuleManager.spellProfils[spellIndex[i]].Clone()) ;
               // CapsuleManager.RemoveSpecificSpellFromSpellPool(spellIndex[i]);



            }

            //  Set to Spell Equip
            spellEquip = new int[4];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (i >= spellIndex.Count)
                    spellEquip[i] = -1;
                else

                {
                    spellEquip[i] = i;
                    spellProfils.Add(m_characterSpellBook.GetSpecificSpell(i));
                }
            }
            m_currentIndexCapsule = spellEquip[0];
            ChangeVfxOnUI(m_currentIndexCapsule);
            maxSpellIndex = Mathf.Clamp(spellIndex.Count, 0, 4);

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
                //spellIndex.Add(RndCapsule);
                spellIndex.Add(0);
            }
        }

        private void GenerateNewBuildSpecificStart(int index)
        {
            spellIndex.Add(index);
        }
        private void InitStacking()
        {
            m_stackingClock = new ClockTimer[4];
            for (int i = 0; i < maxSpellIndex; i++)
            {
                m_stackingClock[i] = new ClockTimer();
                m_stackingClock[i].ActiaveClock();
                m_stackingClock[i].SetTimerDuration(spellProfils[i].GetFloatStat(StatType.StackDuration), m_clockImage[i]);

            }
        }
        #endregion


        public float GetPodRange() { return currentSpellProfil.GetFloatStat(StatType.Range); }
        public SpellSystem.SpellProfil GetSpellProfil() { return currentSpellProfil; }

        #region Inputs Functions
        public void ShootInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && state.isPlaying)
            {
                StartCasting();
                gsm.CanalisationParameterLaunch(0, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element + 0.5f);
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
                gsm.CanalisationParameterLaunch(1, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element + 0.5f);
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
            m_uiPlayerInfos.DeactiveSpellCanalisation();
            isCasting = false;
        }


        public void ActivateCanalisation()
        {
            m_spellTimer = 0.0f;
            m_uiPlayerInfos.ActiveSpellCanalisationUI(m_currentStack[m_currentRotationIndex], icon_Sprite[m_currentRotationIndex]);
            UpdateCanalisationBar(m_totalCanalisationDuration);
            m_activeSpellLoad = true;
            hasCanalise = false;
        }

        private bool ShotCanalisation()
        {
            if (!m_activeSpellLoad || hasCanalise) return false;

            UpdateCanalisationBar(m_totalCanalisationDuration);
            //  if (!m_canShoot) return false;
            if (m_spellTimer >= currentSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime)
            {
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

        private void UpdateCanalisationBar(float maxValue)
        {
            float ratio = m_spellTimer / maxValue;
            m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
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
            m_CharacterAnimator.SetTrigger("Shot" + m_currentIndexCapsule);
            m_BookAnimator.SetBool("Shooting", true);
            m_lastTimeShot = Time.time;
            m_CharacterMouvement.m_SpeedReduce = 0.25f;

            if (currentShotNumber == 0 && !m_hasBeenLoad)
            {
                StartShoot();
                return;
            }

            if (m_currentIndexCapsule == -1 || m_canEndShot)
            {
                EndShoot();
                return;
            }

            if (m_currentType == GuerhoubaGames.GameEnum.BuffType.DAMAGE_SPELL) ShootAttack(m_currentIndexCapsule, ref currentShotNumber, ref m_canEndShot);


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

            SpellSystem.SpellProfil stats = GetCurrentWeaponStat(index);
            if (stats.tagData.spellNatureType == SpellNature.PROJECTILE)
            {
                endShoot = ShootAttackProjectile(index, ref currentShootCount);
                return;
            }

            if (stats.tagData.spellNatureType == SpellNature.AREA)
                endShoot = ShootAttackArea(index);
        }

        private SpellSystem.SpellProfil GetCurrentWeaponStat(int index) { return spellProfils[m_currentIndexCapsule]; }

        public bool ShootAttackArea(int capsuleIndex)
        {
            SpellSystem.SpellProfil spellProfil = GetCurrentWeaponStat(capsuleIndex);

            Transform transformUsed = transform;
            Vector3 position = transformUsed.position + new Vector3(0, 5, 0);
            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            GameObject areaInstance = GameObject.Instantiate(spellProfil.objectToSpawn, m_characterAim.projectorVisorObject.transform.position, rot);


            SpellSystem.AreaData data = FillAreaData(spellProfil, m_characterAim.projectorVisorObject.transform.position);
            areaInstance.GetComponent<SpellSystem.AreaMeta>().areaData = data;

            return true;
        }

        private bool ShootAttackProjectile(int capsuleIndex, ref int currentShotCount)
        {
            if (m_currentStack[m_currentRotationIndex] <= 0)
                return true;

            SpellSystem.SpellProfil spellProfil = GetCurrentWeaponStat(capsuleIndex);
            float angle = GetShootAngle(spellProfil);
            int mod = GetStartIndexProjectile(spellProfil);
            for (int i = 0; i < spellProfil.GetIntStat(StatType.Projectile); i++)
            {
                Transform transformUsed = transform;

                Vector3 position = transformUsed.position + m_characterAim.GetTransformHead().forward * 10 + new Vector3(0, 2, 0);
                Quaternion rot = m_characterAim.GetTransformHead().rotation * Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up);

                GameObject projectileCreate = GameObject.Instantiate(spellProfil.objectToSpawn, position, rot);
                projectileCreate.transform.localScale = projectileCreate.transform.localScale;

                ProjectileData data = FillProjectileData(spellProfil, 0, angle, transformUsed);
                projectileCreate.GetComponent<Projectile>().SetProjectile(data);
                angle = -angle;
            }


            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            m_currentStack[m_currentRotationIndex]--;
            //m_uiPlayerInfos.UpdateStackingObjects(m_currentRotationIndex, m_currentStack[m_currentRotationIndex]);
            float ratio = (float)(m_currentStack[m_currentRotationIndex] / spellProfil.GetIntStat(StatType.ShootNumber));
            m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
            if (m_currentStack[m_currentRotationIndex] <= 0)
                return true;
            else
                return false;
        }



        public void ActiveOnHit(Vector3 position, EntitiesTrigger tag, GameObject agent)
        {

            onHit(position, tag, agent);
        }
        private void StartShoot()
        {
            m_hasBeenLoad = true;
            m_currentType = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.type;
            m_canEndShot = false;
            if (m_CharacterMouvement.combatState) m_cameraBehavior.BlockZoom(true);
        }

        private void EndShoot()
        {
            if (!m_canShoot) return;

            currentShotNumber = 0;

            m_spellTimer = 0.0f;
            m_spellLaunchTime = 0.0f;
            m_CharacterMouvement.m_SpeedReduce = 1;
            m_uiPlayerInfos.DeactiveSpellCanalisation();
            gsm.CanalisationParameterLaunch(1, (float)m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element + 0.5f);

            if (!m_activeSpellLoad)
            {
                m_currentIndexCapsule = ChangeProjecileIndex();
                ChangeVfxOnUI(m_currentIndexCapsule);
            }



            currentManaValue -= 2;
            m_CharacterAnimator.ResetTrigger("Shot" + m_currentIndexCapsule);
            //castingVFXAnimator.ResetTrigger("Shot");
            m_currentType = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.type;

            currentSpellProfil = spellProfils[m_currentIndexCapsule];
            lastElement = m_characterSpellBook.GetSpecificSpell(m_currentIndexCapsule).tagData.element;
            ChangeVfxElement(((int)lastElement));

            if (!m_shootInput) m_shootInputActive = false;
            m_canShoot = false;
            m_isShooting = true;
            m_hasBeenLoad = false;
            float ratio = (float)(m_currentStack[m_currentRotationIndex] / currentSpellProfil.GetIntStat(StatType.ShootNumber));
            m_uiPlayerInfos.UpdateSpellCanalisationUI(ratio, (m_currentStack[m_currentRotationIndex]));
            m_uiPlayerInfos.DeactiveSpellCanalisation();

            m_deltaTimeFrame = UnityEngine.Time.deltaTime;
            m_totalCanalisationDuration = currentSpellProfil.GetFloatStat(StatType.SpellCanalisation) + baseCanalisationTime + m_deltaTimeFrame;
            m_uiPlayerInfos.ActiveSpellCanalisationUI(m_currentStack[m_currentRotationIndex], icon_Sprite[m_currentRotationIndex]);
            if (m_canalisationType == CanalisationBarType.ByPart)
                m_totalLaunchingDuration = (m_currentStack[m_currentRotationIndex]);

            m_spellTimer = 0.0f;
            m_activeSpellLoad = true;
            hasCanalise = false;
            hasStartShoot = false;
        }

        public void ChangeVfxElement(int elementIndex)
        {
            for (int i = 0; i < ((int)GameElement.EARTH) ; i++)
            {
                if (i == elementIndex-1)
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

            return data;
        }

        private SpellSystem.AreaData FillAreaData(SpellSystem.SpellProfil spell, Vector3 spawnPosition)
        {
            SpellSystem.AreaData areaData = new SpellSystem.AreaData();

            areaData.characterShoot = this;
            areaData.spellProfil = spell;
            areaData.destination = spawnPosition;
            areaData.direction = transform.forward;

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

        #endregion

        #region Stacking Functions
        public void UpdateStackingSystem()
        {
            for (int i = 0; i < maxSpellIndex; i++)
            {
                bool inputTest = !m_shootInputActive || i != m_currentRotationIndex;
                int index = spellEquip[i];
                SpellSystem.SpellProfil spellProfil = spellProfils[index];

                if (m_currentStack[i] != spellProfil.GetIntStat(StatType.ShootNumber)) m_stackingClock[i].ActiaveClock();

                if (inputTest && m_stackingClock[i].UpdateTimer())
                {
                    m_currentStack[i] += spellProfils[index].GetIntStat(StatType.GainPerStack);
                    if (i == m_currentRotationIndex) m_uiPlayerInfos.ActiveSpellCanalisationUI(m_currentStack[m_currentRotationIndex], icon_Sprite[m_currentRotationIndex]);
                    m_currentStack[i] = Mathf.Clamp(m_currentStack[i], 0, spellProfil.GetIntStat(StatType.ShootNumber));
                    spellProfils[index] = spellProfil;
                    //m_uiPlayerInfos.UpdateStackingObjects(i, m_currentStack[i]);
                    if (m_currentStack[i] == spellProfil.GetIntStat(StatType.ShootNumber)) m_stackingClock[i].DeactivateClock();
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
                m_currentStack[i] = Mathf.Clamp(m_currentStack[i], 0, spellProfil.GetIntStat(StatType.ShootNumber));
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

            float totalShootTime = baseTimeBetweenSpell ;

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

            m_cameraBehavior.BlockZoom(false);


            float totalShootTime = time;
            if (m_reloadTimer > totalShootTime)
            {
                m_reloadTimer = 0;
                m_isReloading = false;
                m_canShoot = true;
                IsRealoadingSpellRotation = false;
                m_uiPlayerInfos.ActiveSpellCanalisationUI(m_currentStack[m_currentRotationIndex], icon_Sprite[m_currentRotationIndex]);
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
            return;
        }

        public void EndCasting()
        {
            if (autoAimActive) return;

            if (!isCasting) return;

            // m_isCasting = false;
            if (!m_shootInput) m_shootInputActive = false;
            m_cameraBehavior.BlockZoom(false);

            m_lastTimeShot = Mathf.Infinity;
            avatarTransform.localRotation = Quaternion.identity;
            bookTransform.localRotation = Quaternion.identity;
            if (!m_CharacterMouvement.activeCombatModeConstant) m_CharacterMouvement.SetCombatMode(false);

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

        public void InputChangeAimLayout(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                int indexAim = (int)m_aimModeState;
                gsm.CanalisationParameterStop();
                indexAim++;

                if (indexAim == 3) indexAim = 0;

                m_aimModeState = AimMode.FullControl;
                Debug.Log("Change Aim mode : " + m_aimModeState.ToString());
                UpdateFeedbackAimLayout();
            }
        }


        public void UpdateFeedbackAimLayout()
        {
            m_textCurrentLayout.text = "Current layout : \n" + m_aimModeState.ToString();
        }

        #region Spell Functions


        public void AddSpell(int index)
        {
            int prevIndex = m_characterSpellBook.GetSpellCount();
            spellIndex.Add(index);
            m_characterSpellBook.AddSpell(m_capsuleManager.spellProfils[index].Clone()) ;
            m_dropInventory.AddNewItem(index);


            if (spellIndex.Count <= spellEquip.Length)
            {
                spellProfils.Add( m_capsuleManager.spellProfils[index]);
                spellEquip[spellIndex.Count - 1] = m_characterSpellBook.GetSpellCount() - 1;
                m_stackingClock[maxSpellIndex] = new ClockTimer();
                m_stackingClock[maxSpellIndex].ActiaveClock();
                m_stackingClock[maxSpellIndex].SetTimerDuration(spellProfils[maxSpellIndex].GetFloatStat(StatType.StackDuration), m_clockImage[maxSpellIndex]);
                maxSpellIndex = Mathf.Clamp(spellIndex.Count, 0, 4);
                RefreshActiveIcon(m_characterSpellBook.GetAllSpells());
                m_characterUpgrade.upgradeManager.UpdateCharacterUpgradePool();
            }
        }

        public void RemoveSpell(int index)
        {
            int spellIndex = spellEquip[index];
            this.spellIndex.RemoveAt(spellIndex);
            spellProfils.RemoveAt(spellIndex);
            m_characterSpellBook.RemoveSpell(spellIndex);
            m_stackingClock[index].DeactivateClock();


            for (int i = index; i < 2; i++)
            {
                if (spellIndex < spellEquip[i + 1])
                    spellEquip[i + 1] = spellEquip[i + 1] - 1;

                spellEquip[i] = spellEquip[i + 1];
            }

            maxSpellIndex = Mathf.Clamp(this.spellIndex.Count, 0, 4);
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

            m_stackingClock[indexSpell1].SetTimerDuration(spellProfils[indexSpell1].GetIntStat(StatType.StackDuration), m_clockImage[indexSpell1]);
            m_stackingClock[indexSpell2].SetTimerDuration(spellProfils[indexSpell2].GetIntStat(StatType.StackDuration), m_clockImage[indexSpell2]);

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

    }



}