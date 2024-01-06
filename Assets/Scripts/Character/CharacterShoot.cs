using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


namespace Character
{
    public class CharacterShoot : MonoBehaviour, CharacterComponent
    {
        public bool activeRandom = false;
        [HideInInspector] public int projectileNumber;
        public float shootTime;
        public float reloadTime;
        public LauncherProfil launcherProfil;
        public UiSpellGrimoire spellGrimoire;


        public int[] spellEquip;
        public int maxSpellIndex;
        private int m_currentIndexCapsule = 0;
        private int m_currentRotationIndex = 0;

        public List<int> capsuleIndex;
        [SerializeField] private ChooseSkillManualy[] debugCapsule = new ChooseSkillManualy[4];

        public CapsuleManager m_capsuleManager;

        private float m_shootTimer;
        private float m_reloadTimer;
        private float m_timeBetweenShoot;

        private bool m_canShoot;
        private bool m_canEndShot;
        private bool m_isShooting;
        private bool m_shootInput;
        private bool m_isReloading;
        public bool m_isCasting;

        private CapsuleStats currentWeaponStats;
        [HideInInspector] public LauncherStats launcherStats;
        [HideInInspector] public List<CapsuleStats> capsuleStatsAlone = new List<CapsuleStats>();
        public CapsuleStats weaponStat { get { return currentWeaponStats; } private set { } }
        private int currentShotNumber;
        //    private int currentIndexWeapon;

        private CharacterAim m_characterAim;
        [HideInInspector] public CharacterMouvement m_CharacterMouvement; // Add reference to move script
        [SerializeField] private Render.Camera.CameraShake m_cameraShake;
        [SerializeField] private Render.Camera.CameraBehavior m_cameraBehavior;
        [SerializeField] private float m_shakeDuration = 0.1f;
        [SerializeField] private Buff.BuffsManager m_buffManager;
        [SerializeField] private CharacterProfile m_chracterProfil;
        [SerializeField] private Animator m_CharacterAnimator;
        [SerializeField] private Animator m_BookAnimator;
        [SerializeField] private Transform m_OuterCircleHolder;
        [SerializeField] public GameObject m_SkillBarHolder;
        [SerializeField] private Transform avatarTransform;
        [SerializeField] private Transform bookTransform;
        [SerializeField] public List<UnityEngine.VFX.VisualEffect> m_SpellReadyVFX = new List<UnityEngine.VFX.VisualEffect>();
        private Rigidbody m_rigidbody;

        private Animator m_AnimatorSkillBar;

        [SerializeField] public List<Image> icon_Sprite;
        [SerializeField] public List<Image> m_spellGlobalCooldown;
        [SerializeField] public List<TextMeshProUGUI> m_TextSpellGlobalCooldown;


        private SpellSystem.CapsuleType m_currentType;
        private PauseMenu pauseScript;
        private ObjectState state;

        // Temp 
        private Vector3 pos;

        [SerializeField] public bool autoAimActive;
        [SerializeField] public AimMode m_aimModeState;
        [SerializeField] private bool globalCD;
        [SerializeField] private TMPro.TMP_Text m_textCurrentLayout;


        public float m_lastTimeShot = 0;
        [SerializeField] private float m_TimeAutoWalk = 2;

        private CharacterSpellBook m_characterInventory;

        #region Unity Functions
        private void Awake()
        {
            launcherStats = launcherProfil.stats;
        }

        private void Start()
        {
            state = new ObjectState();
            GameState.AddObject(state);

            if (activeRandom) GenerateNewBuild();

            InitComponents();
            InitCapsule();
            InitSpriteSpell();

            // Init Variables
            m_currentRotationIndex = 0;
            m_currentIndexCapsule = spellEquip[0];
            m_canShoot = true;
        }

        private void Update()
        {
            if (!state.isPlaying) { return; } // Block Update during pause state

            if (m_CharacterMouvement.combatState)
            {
                UpdateAvatarModels();
                AutomaticAimMode();
            }
            UpdateFullControlAimMode();
            ReloadShot();
            ReloadWeapon(0.5f);
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
                if (m_timeBetweenShoot > currentWeaponStats.timeBetweenShot)
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

            if (!m_shootInput || globalCD) return;

            if (!m_isShooting)
            {
                Shoot();
                return;
            }
            if (m_timeBetweenShoot > currentWeaponStats.timeBetweenShot)
            {
                Shoot();
                m_timeBetweenShoot = 0.0f;
            }
            else
            {
                m_timeBetweenShoot += Time.deltaTime;
            }
        }
        #endregion


        #region Start Functions
        public void InitComponents()
        {
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>();
            pauseScript = GetComponent<PauseMenu>();
            m_AnimatorSkillBar = m_SkillBarHolder.GetComponent<Animator>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_buffManager = GetComponent<Buff.BuffsManager>();
            m_chracterProfil = GetComponent<CharacterProfile>();
            m_characterInventory = GetComponent<CharacterSpellBook>();
        }

        private void InitSpriteSpell()
        {
            ActiveIcon();
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                m_spellGlobalCooldown[i].sprite = icon_Sprite[i].sprite;

            }
            if (m_currentType == SpellSystem.CapsuleType.ATTACK)
            {
                currentWeaponStats = capsuleStatsAlone[m_currentIndexCapsule];
            }
        }

        public void InitComponentStat(CharacterStat stat) // To verify this system
        {
            reloadTime = launcherStats.reloadTime;
            shootTime = launcherStats.timeBetweenCapsule;
        }

        public void InitCapsule()
        {
            // Add Spell in Spell List
            // Get the spell stats for each spell
            for (int i = 0; i < capsuleIndex.Count; i++)
            {
                if (capsuleIndex[i] == -1) continue;

                m_characterInventory.AddSpell(m_capsuleManager.capsules[capsuleIndex[i]]);
                CapsuleManager.RemoveSpecificSpellFromSpellPool(capsuleIndex[i]);

                if (m_characterInventory.GetSpecificSpell(i).type != SpellSystem.CapsuleType.ATTACK)
                    capsuleStatsAlone.Add(new CapsuleStats());
                else
                {
                    SpellSystem.CapsuleAttack currentCap = (SpellSystem.CapsuleAttack)m_characterInventory.GetSpecificSpell(i);
                    capsuleStatsAlone.Add(currentCap.profil.stats);
                }

            }

            //  Set to Spell Equip
            spellEquip = new int[4];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (i >= capsuleIndex.Count)
                    spellEquip[i] = -1;
                else
                    spellEquip[i] = i;
            }
            m_currentIndexCapsule = spellEquip[0];

            maxSpellIndex = Mathf.Clamp(capsuleIndex.Count, 0, 4);

        }

        private void GenerateNewBuild()
        {
            for (int i = 0; i < 4; i++)
            {
                int RndCapsule = UnityEngine.Random.Range(0, 8);
                capsuleIndex.Add(RndCapsule);
            }
        }

        #endregion


        public float GetPodRange() { return currentWeaponStats.range; }
        public CapsuleStats GetPod() { return currentWeaponStats; }

        #region Inputs Functions
        public void ShootInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && state.isPlaying)
            {
                StartCasting();
                m_shootInput = true;
                m_lastTimeShot = Time.time;
            }
            if (ctx.canceled && state.isPlaying)
            {

            }
        }
        #endregion

        #region Shoot Function

        private void Shoot()
        {
            if (!m_canShoot) return;

            GlobalSoundManager.PlayOneShot(27, transform.position);

            m_lastTimeShot = Time.time;
            m_CharacterMouvement.m_SpeedReduce = 0.25f;

            if (currentShotNumber == 0) StartShoot();

            if (m_currentIndexCapsule == -1)
            {
                EndShoot();
                return;
            }

            if (m_currentType == SpellSystem.CapsuleType.ATTACK) ShootAttack();

            if (m_currentType == SpellSystem.CapsuleType.BUFF) ShootBuff(((SpellSystem.CapsuleBuff)m_characterInventory.GetSpecificSpell(m_currentIndexCapsule)));

            if (m_canEndShot) EndShoot();

            m_CharacterAnimator.SetBool("Shooting", true);
            m_BookAnimator.SetBool("Shooting", true);


        }

        private void ShootAttack()
        {
            if (currentWeaponStats.formType == FormTypeSpell.PROJECTILE)
            {
                ShootAttackProjectile();
                return;
            }

            if (currentWeaponStats.formType == FormTypeSpell.AREA) ShootAtttackArea();
        }


        public void ShootAtttackArea()
        {
            float angle = GetShootAngle(currentWeaponStats);
            int mod = GetStartIndexProjectile(currentWeaponStats);

            Transform transformUsed = transform;
            Vector3 position = transformUsed.position + new Vector3(0, 5, 0);
            Quaternion rot = m_characterAim.GetTransformHead().rotation;
            GameObject projectileCreate = GameObject.Instantiate(((SpellSystem.CapsuleAttack)m_characterInventory.GetSpecificSpell(m_currentIndexCapsule)).projectile, position, rot);

            projectileCreate.transform.localScale = projectileCreate.transform.localScale;

            ProjectileData data = FillProjectileData(0, angle, transformUsed);
            projectileCreate.GetComponent<Projectile>().SetProjectile(data);

            m_canEndShot = true;
        }

        private void ShootAttackProjectile()
        {
            float angle = GetShootAngle(currentWeaponStats);
            int mod = GetStartIndexProjectile(currentWeaponStats);
            for (int i = mod; i < currentWeaponStats.projectileNumber + mod; i++)
            {
                Transform transformUsed = transform;

                Vector3 position = transformUsed.position + new Vector3(0, 5, 0);
                Quaternion rot = m_characterAim.GetTransformHead().rotation * Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up);

                GameObject projectileCreate = GameObject.Instantiate(((SpellSystem.CapsuleAttack)m_characterInventory.GetSpecificSpell(m_currentIndexCapsule)).projectile, position, rot);
                projectileCreate.transform.localScale = projectileCreate.transform.localScale * (currentWeaponStats.size * currentWeaponStats.sizeMultiplicatorFactor);

                ProjectileData data = FillProjectileData(0, angle, transformUsed);
                projectileCreate.GetComponent<Projectile>().SetProjectile(data);
                angle = -angle;
            }

            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            currentShotNumber++;
            if (currentShotNumber == currentWeaponStats.shootNumber) m_canEndShot = true;
        }

        private void ShootBuff(SpellSystem.CapsuleBuff capsuleBuff) // TODO : No Use. Need to be rethink
        {
            Buff.BuffCharacter buff = new Buff.BuffCharacter(capsuleBuff.profil, capsuleBuff.duration);
            if (capsuleBuff.vfx) { GameObject vfxObject = Instantiate(capsuleBuff.vfx, transform.position, Quaternion.identity); }
            m_buffManager.AddBuff(buff);
            GlobalSoundManager.PlayOneShot(8, Vector3.zero);
            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            m_canEndShot = true;
        }

        private void StartShoot()
        {
            m_currentType = m_characterInventory.GetSpecificSpell(m_currentIndexCapsule).type;
            m_isShooting = true;
            m_canEndShot = false;
            m_cameraBehavior.BlockZoom(true);
        }

        private void EndShoot()
        {
            currentShotNumber = 0;
            m_currentIndexCapsule = ChangeProjecileIndex();

            m_currentType = m_characterInventory.GetSpecificSpell(m_currentIndexCapsule).type;
            if (m_currentType == SpellSystem.CapsuleType.ATTACK)
            {
                currentWeaponStats = capsuleStatsAlone[m_currentIndexCapsule];
            }
            m_shootInput = false;
            m_canShoot = false;
            m_isShooting = false;
        }

        private ProjectileData FillProjectileData(float index, float angle, Transform transformUsed)
        {
            ProjectileData data = new ProjectileData();

            Vector3 baseDirection =transform.forward;
            if (m_characterAim.HasCloseTarget()) baseDirection = m_characterAim.GetAimDirection(); 
            data.direction = Quaternion.AngleAxis(angle * ((index + 1) / 2), transformUsed.up) * baseDirection;
            data.speed = currentWeaponStats.speed + m_rigidbody.velocity.magnitude;
            data.life = currentWeaponStats.lifetime;
            data.damage = currentWeaponStats.damage;
            data.travelTime = currentWeaponStats.trajectoryTimer;
            data.piercingMax = currentWeaponStats.piercingMax;
            data.salveNumber = (int)currentWeaponStats.projectileNumber;
            data.shootNumber = (int)currentWeaponStats.shootNumber;
            data.size = currentWeaponStats.size;
            data.sizeFactor = currentWeaponStats.sizeMultiplicatorFactor;
            Vector3 dest = Quaternion.AngleAxis(angle * ((index + 1) / 2), transformUsed.up) * m_characterAim.GetAimFinalPoint();
            if ((dest - transformUsed.position).magnitude > currentWeaponStats.range)
                dest = transformUsed.position - (Vector3.up * 0.5f) + (dest - transformUsed.position).normalized * currentWeaponStats.range;

            data.destination = m_characterAim.GetAimFinalPoint();
            pos = dest;
            return data;
        }


        private float GetShootAngle(CapsuleStats weaponStats)
        {
            return weaponStats.shootAngle / weaponStats.projectileNumber;
        }

        /// <summary>
        /// Help to find the start position for create a circular arc effect
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetStartIndexProjectile(CapsuleStats weaponStats)
        {
            return weaponStats.projectileNumber % 2 == 1 ? 0 : 1;
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

        private void ReloadShot()
        {
            if (m_canShoot || m_isReloading) return;

            m_CharacterAnimator.SetBool("Shooting", false);
            m_BookAnimator.SetBool("Shooting", false);

            m_CharacterMouvement.m_SpeedReduce = 1;

            float totalShootTime = shootTime + currentWeaponStats.timeInterval;

            if (m_shootTimer > totalShootTime)
            {
                for (int i = 0; i < icon_Sprite.Count; i++)
                {
                    icon_Sprite[m_currentRotationIndex].color = Color.white;
                    m_TextSpellGlobalCooldown[i].text = "";
                }

                //m_SpellReadyVFX[m_currentRotationIndex].Play();
                m_canShoot = true;
                m_shootTimer = 0;
                return;
            }
            else
            {
                m_shootTimer += Time.deltaTime;
                for (int i = m_currentRotationIndex; i < m_spellGlobalCooldown.Count; i++)
                {
                    m_spellGlobalCooldown[i].fillAmount = (totalShootTime - m_shootTimer) / totalShootTime;
                    m_TextSpellGlobalCooldown[i].text = (totalShootTime - m_shootTimer).ToString(".#");
                }


            }
        }

        private void ReloadWeapon(float time)
        {
            if (m_canShoot || !m_isReloading) return;

            m_shootInput = false;

            avatarTransform.localRotation = Quaternion.identity;
            bookTransform.localRotation = Quaternion.identity;


            if (!m_CharacterMouvement.activeCombatModeConstant) m_CharacterMouvement.SetCombatMode(false);

            m_cameraBehavior.BlockZoom(false);


            float totalShootTime = time + currentWeaponStats.timeInterval;
            if (m_reloadTimer > totalShootTime)
            {
                m_isReloading = false;
                globalCD = false;
                m_reloadTimer = 0;
                ActiveIcon();
                for (int i = 0; i < icon_Sprite.Count; i++)
                {
                    icon_Sprite[i].color = Color.white;
                    m_TextSpellGlobalCooldown[i].text = "";
                }
                return;
            }
            else
            {

                m_reloadTimer += Time.deltaTime;
                for (int i = 0; i < m_spellGlobalCooldown.Count; i++)
                {
                    m_spellGlobalCooldown[i].fillAmount = (totalShootTime - m_reloadTimer) / totalShootTime;
                    m_TextSpellGlobalCooldown[i].text = (totalShootTime - m_reloadTimer).ToString(".#");
                }
                globalCD = true;
            }
        }

        public void StartCasting()
        {
            if (m_isCasting) return;

            m_isCasting = true;
            m_CharacterAnimator.SetBool("Casting", true);
            m_BookAnimator.SetBool("Casting", true);
            m_CharacterMouvement.SetCombatMode(true);
            return;
        }

        public void EndCasting()
        {
            if (autoAimActive) return;

            if (!m_isCasting) return;

            m_isCasting = false;
            m_shootInput = false;
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
            RefreshActiveIcon(m_characterInventory.GetAllSpells());
        }

        public void RefreshActiveIcon(SpellSystem.Capsule[] capsuleState)
        {
            for (int i = 0; i < spellEquip.Length; i++)
            {
                if (spellEquip[i] == -1) continue;

                int index = spellEquip[i];
                if (capsuleState[index].type == SpellSystem.CapsuleType.ATTACK)
                {
                    icon_Sprite[i].sprite = ((SpellSystem.CapsuleAttack)capsuleState[index]).sprite;
                }
                else if (capsuleState[index].type == SpellSystem.CapsuleType.BUFF)
                {
                    icon_Sprite[i].sprite = ((SpellSystem.CapsuleBuff)capsuleState[index]).sprite;
                }
                //SignPosition[i].GetComponent<SpriteRenderer>().sprite = icon_Sprite[i].sprite;
            }
        }


        public void InputChangeAimLayout(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                int indexAim = (int)m_aimModeState;
                indexAim++;

                if (indexAim == 3) indexAim = 0;

                m_aimModeState = (AimMode)indexAim;
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
            int prevIndex = m_characterInventory.GetSpellCount();
            capsuleIndex.Add(index);
            m_characterInventory.AddSpell(m_capsuleManager.capsules[index]);

            // Update New Capsule
            if (m_characterInventory.GetSpecificSpell(prevIndex).type == SpellSystem.CapsuleType.ATTACK)
            {
                SpellSystem.CapsuleAttack currentCap = (SpellSystem.CapsuleAttack)m_characterInventory.GetSpecificSpell(prevIndex);
                capsuleStatsAlone.Add(currentCap.profil.stats);
            }
            else
                capsuleStatsAlone.Add(new CapsuleStats());

            if (capsuleIndex.Count <= spellEquip.Length)
            {
                spellEquip[capsuleIndex.Count - 1] = m_characterInventory.GetSpellCount() - 1;
                maxSpellIndex = Mathf.Clamp(capsuleIndex.Count, 0, 4);
                RefreshActiveIcon(m_characterInventory.GetAllSpells());
            }
        }

        public void OpenGrimoire(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                if (!spellGrimoire.isOpen)
                {
                    Sprite[] iconArray = new Sprite[m_characterInventory.GetSpellCount()];
                    for (int i = 0; i < iconArray.Length; i++)
                    {
                        iconArray[i] = m_characterInventory.GetSpecificSpell(i).sprite;
                    }

                    spellGrimoire.OpenUI(iconArray, spellEquip);
                    return;
                }
                spellGrimoire.CloseUI();

            }
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
            RefreshActiveIcon(m_characterInventory.GetAllSpells());
        }

        public int GetIndexFromSpellBar(int indexSpellBar) { return spellEquip[indexSpellBar]; }

        #endregion


    }



}