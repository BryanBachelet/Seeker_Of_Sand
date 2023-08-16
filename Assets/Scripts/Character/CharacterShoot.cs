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

        [HideInInspector]
        public List<CapsuleSystem.Capsule> bookOfSpell = new List<CapsuleSystem.Capsule>();
        public int[] spellEquip;
        private int m_currentIndexCapsule = 0;
        private int m_currentRotationIndex = 0;

        public List<int> capsuleIndex;
        [SerializeField] private ChooseSkillManualy[] debugCapsule = new ChooseSkillManualy[4];

        public CapsuleManager m_capsuleManager;

        private float m_shootTimer;
        private float m_reloadTimer;
        private float m_timeBetweenShoot;

        private bool m_canShoot;
        private bool m_isShooting;
        private bool m_shootInput;
        private bool m_isReloading;
        public bool m_isCasting;

        private CapsuleStats currentWeaponStats;
        [HideInInspector] public LauncherStats launcherStats;
        [HideInInspector] public CapsuleStats[] capsuleStatsAlone;
        public CapsuleStats weaponStat { get { return currentWeaponStats; } private set { } }
        private int currentShotNumber;
        //    private int currentIndexWeapon;

        private CharacterAim m_characterAim;
        private CharacterMouvement m_CharacterMouvement; // Add reference to move script
        [SerializeField] private Render.Camera.CameraShake m_cameraShake;
        [SerializeField] private float m_shakeDuration = 0.1f;
        [SerializeField] private Buff.BuffsManager m_buffManager;
        [SerializeField] private CharacterProfile m_chracterProfil;
        [SerializeField] private Animator m_CharacterAnimator;
        [SerializeField] private Transform m_OuterCircleHolder;
        [SerializeField] private GameObject m_SkillBarHolder;
        [SerializeField] private Transform avatarTransform;

        private Animator m_AnimatorSkillBar;

        private Loader_Behavior m_LoaderInUI;
        [SerializeField] private List<Image> icon_Sprite;
        [SerializeField] private List<Image> m_spellGlobalCooldown;
        [SerializeField] private List<TextMeshProUGUI> m_TextSpellGlobalCooldown;


        private CapsuleSystem.CapsuleType m_currentType;
        private PauseMenu pauseScript;
        private ObjectState state;

        // Temp 
        private Vector3 pos;

        [SerializeField] public bool autoAimActive;
        private void Awake()
        {
            launcherStats = launcherProfil.stats;
        }

        private void Start()
        {
            state = new ObjectState();
            GameState.AddObject(state);
            if (activeRandom)
            {
                GenerateNewBuild();
            }
            InitCapsule();
            InitComponent();
        }
        public void InitComponentStat(CharacterStat stat)
        {
            reloadTime = launcherStats.reloadTime;
            shootTime = launcherStats.timeBetweenCapsule;
        }

        // ================= TEMP =====================
        public void IncreaseCapsuleIndex(InputAction.CallbackContext ctx)
        {
            if (ctx.started && state.isPlaying) SwitchCapsuleChange(true);
        }

        public void DecreaseCapsuleIndex(InputAction.CallbackContext ctx)
        {
            if (ctx.started && state.isPlaying) SwitchCapsuleChange(false);
        }

        private void SwitchCapsuleChange(bool increase)
        {
            if (m_isCasting) return;
            for (int i = 0; i < capsuleIndex.Count; i++)
            {
                if (increase)
                {
                    capsuleIndex[i]++;
                    if (capsuleIndex[i] != m_capsuleManager.capsules.Length) continue;

                    capsuleIndex[i] = 0;
                }
                else
                {
                    capsuleIndex[i]--;

                    if (capsuleIndex[i] >= 0) continue;

                    capsuleIndex[i] = m_capsuleManager.capsules.Length - 1;
                }
            }
            InitCapsule();
        }

        // ==============================================================

        public float GetPodRange() { return currentWeaponStats.range; }
        public CapsuleStats GetPod() { return currentWeaponStats; }
        private void InitComponent()
        {
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>(); // Assignation du move script
            pauseScript = GetComponent<PauseMenu>();
            m_AnimatorSkillBar = m_SkillBarHolder.GetComponent<Animator>();

            m_buffManager = GetComponent<Buff.BuffsManager>();
            m_chracterProfil = GetComponent<CharacterProfile>();

            if (m_currentType == CapsuleSystem.CapsuleType.ATTACK)
            {
                currentWeaponStats = ((CapsuleSystem.CapsuleAttack)bookOfSpell[m_currentIndexCapsule]).stats.stats;
            }
        }

        public void InitCapsule()
        {

            for (int i = 0; i < capsuleIndex.Count; i++)
            {
                bookOfSpell.Add(m_capsuleManager.capsules[capsuleIndex[i]]);
            }
            capsuleStatsAlone = new CapsuleStats[bookOfSpell.Count];
            for (int i = 0; i < bookOfSpell.Count; i++)
            {
                if (bookOfSpell[i].type == CapsuleSystem.CapsuleType.ATTACK)
                {
                    CapsuleSystem.CapsuleAttack currentCap = (CapsuleSystem.CapsuleAttack)bookOfSpell[i];
                    capsuleStatsAlone[i] = currentCap.stats.stats;
                }
                else
                    capsuleStatsAlone[i] = new CapsuleStats();
            }
            spellEquip = new int[4];
            for (int i = 0; i < spellEquip.Length; i++)
            {
                spellEquip[i] = i;
            }
            m_currentIndexCapsule = spellEquip[0];
            GetCircleInfo();
            if (m_LoaderInUI == null) return;
            m_LoaderInUI.CleanCapsule();
            m_LoaderInUI.SetCapsuleOrder(capsuleIndex.ToArray());
        }

        public void GenerateNewBuild()
        {

            for (int i = 0; i < 5; i++)
            {
                int RndCapsule = UnityEngine.Random.Range(0, 8);
                capsuleIndex.Add(RndCapsule);
            }
        }

        private void Update()
        {

            if (PauseMenu.gameState && !state.isPlaying) { return; }
            if (m_isCasting)
            {
                avatarTransform.rotation = m_characterAim.GetTransformHead().rotation;
                if (autoAimActive)
                {

                    if (!m_isShooting) Shoot();
                    if (m_isShooting)
                    {
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
                }
            }
            if(!autoAimActive)
            {
                if (m_shootInput)
                {
                    if (!m_isShooting) Shoot();
                }
                if (m_isShooting)
                {
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
            }
            ReloadShot();
            ReloadWeapon();
        }

        public void ShootInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && state.isPlaying)
            {
                if (ManagedCastCapacity(true))
                {
                    m_shootInput = true;
                }
            }
            if (ctx.canceled && state.isPlaying)
            {
                m_shootInput = false;
            }
        }

        private void Shoot()
        {
            if (!m_canShoot) return;
            m_CharacterMouvement.m_SpeedReduce = 0.25f;
            //Debug.Log("[" + m_CharacterMouvement.runSpeed + "] Run speed ");
            if (currentShotNumber == 0)
            {
                StartShoot();
            }

            if (m_currentType == CapsuleSystem.CapsuleType.ATTACK)
            {
                ShootAttack();
            }
            if (m_currentType == CapsuleSystem.CapsuleType.BUFF)
            {
                ShootBuff(((CapsuleSystem.CapsuleBuff)bookOfSpell[m_currentIndexCapsule]));
                EndShoot();
            }
            m_CharacterAnimator.SetBool("Shooting", true);


        }

        private void ShootAttack()
        {
            float angle = GetShootAngle(currentWeaponStats);
            int mod = GetStartIndexProjectile(currentWeaponStats);

            for (int i = mod; i < currentWeaponStats.projectileNumber + mod; i++)
            {
                Transform transformUsed = transform;
                Quaternion rot = m_characterAim.GetTransformHead().rotation;
                GameObject projectileCreate = GameObject.Instantiate(((CapsuleSystem.CapsuleAttack)bookOfSpell[m_currentIndexCapsule]).projectile
                    , transformUsed.position + new Vector3(0, 1, 0), rot);
                ProjectileData data = new ProjectileData();
                data.direction = Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up) * m_characterAim.GetAimDirection();
                data.speed = currentWeaponStats.speed;
                data.life = currentWeaponStats.lifetime;
                data.damage = currentWeaponStats.damage;
                Vector3 dest = Quaternion.AngleAxis(angle * ((i + 1) / 2), transformUsed.up) * m_characterAim.GetAimFinalPoint();
                if ((dest - transformUsed.position).magnitude > currentWeaponStats.range)
                    dest = transformUsed.position - (Vector3.up * 0.5f) + (dest - transformUsed.position).normalized * currentWeaponStats.range;

                data.destination = m_characterAim.GetAimFinalPoint();
                pos = dest;

                projectileCreate.GetComponent<Projectile>().SetProjectile(data);
                angle = -angle;
            }


            //GlobalSoundManager.PlayOneShot(1, Vector3.zero);

            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            currentShotNumber++;
            if (currentShotNumber == currentWeaponStats.shootNumber) EndShoot();
        }

        private void ShootBuff(CapsuleSystem.CapsuleBuff capsuleBuff)
        {
            Buff.BuffCharacter buff = new Buff.BuffCharacter(capsuleBuff.profil, capsuleBuff.duration);
            if (capsuleBuff.vfx) { GameObject vfxObject = Instantiate(capsuleBuff.vfx, transform.position, Quaternion.identity); }
            m_buffManager.AddBuff(buff);
            GlobalSoundManager.PlayOneShot(8, Vector3.zero);
            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
        }

        private void StartShoot()
        {
            m_currentType = bookOfSpell[m_currentIndexCapsule].type;
            icon_Sprite[m_currentRotationIndex].color = Color.gray;
           // SignPosition[m_currentRotationIndex].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0);

            if (m_currentType == CapsuleSystem.CapsuleType.ATTACK)
            {
                currentWeaponStats = (capsuleStatsAlone[m_currentIndexCapsule]);
                Instantiate(((CapsuleSystem.CapsuleAttack)bookOfSpell[m_currentIndexCapsule]).vfx, transform.position, m_characterAim.GetTransformHead().rotation);

            }
            m_isShooting = true;
        }

        private void EndShoot()
        {
            currentShotNumber = 0;
            m_currentIndexCapsule = ChangeProjecileIndex();
            m_currentType = bookOfSpell[m_currentIndexCapsule].type;
            if (m_currentType == CapsuleSystem.CapsuleType.ATTACK)
            {
                currentWeaponStats = ((CapsuleSystem.CapsuleAttack)bookOfSpell[m_currentIndexCapsule]).stats.stats;
            }
            m_canShoot = false;
            m_isShooting = false;



        }

        #region Shoot Function

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
            if (m_currentRotationIndex == spellEquip.Length - 1)
            {
                m_isReloading = true;
                m_currentRotationIndex = 0;
                ManagedCastCapacity(false);
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
            m_CharacterMouvement.m_SpeedReduce = 1;
            float totalShootTime = shootTime + currentWeaponStats.timeInterval;
            if (m_shootTimer > totalShootTime)
            {
                for (int i = 0; i < icon_Sprite.Count; i++)
                {
                    icon_Sprite[m_currentRotationIndex].color = Color.white;
                    m_TextSpellGlobalCooldown[i].text = "";
                }

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


        private void ReloadWeapon()
        {
            if (m_canShoot || !m_isReloading) return;

            if (m_reloadTimer > reloadTime)
            {
                m_isReloading = false;
                m_reloadTimer = 0;
                GetCircleInfo();
                return;
            }
            else
            {
                m_reloadTimer += Time.deltaTime;
            }
        }

        public void Debug_NewLoadout(int[] NewCapsule)
        {
            // capsuleIndex = NewCapsule;
            InitCapsule();
        }

        public bool ManagedCastCapacity(bool stateCall)
        {
            if (stateCall)
            {
                if (!m_isCasting)
                {

                    m_isCasting = true;
                    m_CharacterAnimator.SetBool("Casting", true);
                    //m_AnimatorSkillBar.SetBool("IsCasting", true);
                    m_canShoot = true;
                    m_CharacterMouvement.combatState = true;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (autoAimActive) return true;
                if (m_isCasting)
                {
                    m_isCasting = false;
                    m_shootInput = false;
                    m_CharacterAnimator.SetBool("Casting", false);
                    avatarTransform.localRotation = Quaternion.identity;
                    //m_AnimatorSkillBar.SetBool("IsCasting", false);
                    m_CharacterMouvement.combatState = false;
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public void GetCircleInfo()
        {
            ActiveIcon();
        }

        private void ActiveIcon()
        {
            for (int i = 0; i < icon_Sprite.Count; i++)
            {
                icon_Sprite[i].color = Color.white;
                //SignPosition[i].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
            }
            RefreshActiveIcon(bookOfSpell.ToArray());
        }

        public void RefreshActiveIcon(CapsuleSystem.Capsule[] capsuleState)
        {
            for (int i = 0; i < spellEquip.Length; i++)
            {
                int index = spellEquip[i];
                if (capsuleState[index].type == CapsuleSystem.CapsuleType.ATTACK)
                {
                    icon_Sprite[i].sprite = ((CapsuleSystem.CapsuleAttack)capsuleState[index]).sprite;
                }
                else if (capsuleState[index].type == CapsuleSystem.CapsuleType.BUFF)
                {
                    icon_Sprite[i].sprite = ((CapsuleSystem.CapsuleBuff)capsuleState[index]).sprite;
                }
                //SignPosition[i].GetComponent<SpriteRenderer>().sprite = icon_Sprite[i].sprite;
            }
        }


        public void StopCasting()
        {
            m_isCasting = false;
            m_shootInput = false;
            m_CharacterAnimator.SetBool("Casting", false);
            avatarTransform.localRotation = Quaternion.identity;
            //m_AnimatorSkillBar.SetBool("IsCasting", false);
            m_CharacterMouvement.combatState = false;
        }
        #region Spell Functions
        public void AddSpell(int index)
        {
            capsuleIndex.Add(index);
            bookOfSpell.Add(m_capsuleManager.capsules[index]);
        }

        public void OpenGrimoire(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                if (!spellGrimoire.isOpen)
                {
                    Sprite[] iconArray = new Sprite[bookOfSpell.Count];
                    for (int i = 0; i < iconArray.Length; i++)
                    {
                        iconArray[i] = bookOfSpell[i].sprite;
                    }

                    spellGrimoire.OpenUI(iconArray,spellEquip);
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
            RefreshActiveIcon(bookOfSpell.ToArray());
        }
        public CapsuleSystem.Capsule GetCapsuleInfo(int index) { return bookOfSpell[index]; }

        public int GetIndexFromSpellBar(int indexSpellBar ) { return spellEquip[indexSpellBar]; }

        #endregion


    }



}