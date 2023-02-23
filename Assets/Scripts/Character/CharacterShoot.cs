using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterShoot : MonoBehaviour, CharacterComponent
    {
        [HideInInspector] public int projectileNumber;
        public float shootTime;
        public float reloadTime;
        public LauncherProfil launcherProfil;
        public ChainEffect[] chainEffects;


        private CapsuleSystem.Capsule[] capsulesPosses;
        private int m_currentIndexCapsule = 0;

        public int[] capsuleIndex;

        public CapsuleManager m_capsuleManager;

        private float m_shootTimer;
        private float m_reloadTimer;
        private float m_timeBetweenShoot;

        private bool m_canShoot;
        private bool m_isShooting;
        private bool m_shootInput;
        private bool m_isReloading;

        private CapsuleStats currentWeaponStats;
        [HideInInspector] public LauncherStats launcherStats;
        [HideInInspector] public CapsuleStats[] capsuleStatsAlone;
        public CapsuleStats weaponStat { get { return currentWeaponStats; } private set { } }
        private int currentShotNumber;
        //    private int currentIndexWeapon;

        private CharacterAim m_characterAim;
        private CharacterMouvement m_CharacterMouvement; // Add reference to move script
        [SerializeField] private CameraShake m_cameraShake;
        [SerializeField] private float m_shakeDuration = 0.1f;
        [SerializeField] private Buff.BuffsManager m_buffManager;
        [SerializeField] private CharacterProfile m_chracterProfil;
        private Loader_Behavior m_LoaderInUI;


        private CapsuleSystem.CapsuleType m_currentType;

        private void Awake()
        {
            launcherStats = launcherProfil.stats;
        }

        private void Start()
        {
            InitCapsule();
            InitComponent();
        }
        public void InitComponentStat(CharacterStat stat)
        {
            reloadTime = launcherStats.reloadTime;
            shootTime = launcherStats.timeBetweenCapsule;
        }

        private void InitComponent()
        {
            capsuleStatsAlone = new CapsuleStats[capsulesPosses.Length];
            for (int i = 0; i < capsulesPosses.Length; i++)
            {
                if (capsulesPosses[i].type == CapsuleSystem.CapsuleType.ATTACK)
                {
                    CapsuleSystem.CapsuleAttack currentCap = (CapsuleSystem.CapsuleAttack)capsulesPosses[i];
                    capsuleStatsAlone[i] = currentCap.stats.stats;
                }
                else
                    capsuleStatsAlone[i] = new CapsuleStats();
            }
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>(); // Assignation du move script
            m_LoaderInUI = GameObject.Find("LoaderDisplay").GetComponent<Loader_Behavior>();
            m_LoaderInUI.SetCapsuleOrder(capsuleIndex);

            m_buffManager = GetComponent<Buff.BuffsManager>();
            m_chracterProfil = GetComponent<CharacterProfile>();
        }

        private void InitCapsule()
        {
            capsulesPosses = new CapsuleSystem.Capsule[capsuleIndex.Length];
            for (int i = 0; i < capsuleIndex.Length; i++)
            {
                capsulesPosses[i] = m_capsuleManager.capsules[capsuleIndex[i]];
            }
        }

        private void Update()
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
            ReloadShot();
            ReloadWeapon();
        }



        public void ShootInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_shootInput = true;
                m_CharacterMouvement.speed = m_CharacterMouvement.initialSpeed / 3; // Reduce speed while shooting 
            }
            if (ctx.canceled)
            {
                m_shootInput = false;
                m_CharacterMouvement.speed = m_CharacterMouvement.initialSpeed;
            }
        }

        private void Shoot()
        {
            if (!m_canShoot) return;

            if (currentShotNumber == 0) StartShoot();  

            if(m_currentType == CapsuleSystem.CapsuleType.ATTACK)
            {
                ShootAttack();
            }
            if(m_currentType == CapsuleSystem.CapsuleType.BUFF)
            {
                ShootBuff(((CapsuleSystem.CapsuleBuff)capsulesPosses[m_currentIndexCapsule]));
                EndShoot();
            }
         
        }

        private void ShootAttack()
        {
            float angle = GetShootAngle(currentWeaponStats);
            int mod = GetStartIndexProjectile(currentWeaponStats);

            for (int i = mod; i < currentWeaponStats.projectileNumber + mod; i++)
            {

                GameObject projectileCreate = GameObject.Instantiate(((CapsuleSystem.CapsuleAttack)capsulesPosses[m_currentIndexCapsule]).projectile
                    , transform.position, Quaternion.AngleAxis(90.0f, m_characterAim.GetTransformHead().right));
                ProjectileData data = new ProjectileData();
                data.direction = Quaternion.AngleAxis(angle * ((i + 1) / 2), transform.up) * m_characterAim.GetAim();
                data.speed = currentWeaponStats.speed;
                data.life = currentWeaponStats.range / currentWeaponStats.speed;
                data.damage = currentWeaponStats.damage;

                projectileCreate.GetComponent<Projectile>().SetProjectile(data);
                angle = -angle;
            }

            GlobalSoundManager.PlayOneShot(1, Vector3.zero);

            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            currentShotNumber++;
            if (currentShotNumber == currentWeaponStats.shootNumber) EndShoot();
        }

        private void ShootBuff(CapsuleSystem.CapsuleBuff capsuleBuff)
        {
            Buff.BuffCharacter buff = new Buff.BuffCharacter(capsuleBuff.profil, capsuleBuff.duration);
            m_buffManager.AddBuff(buff);
            GlobalSoundManager.PlayOneShot(1, Vector3.zero);
            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
        }

        private void StartShoot()
        {
            m_currentType = capsulesPosses[m_currentIndexCapsule].type;
            if (!m_LoaderInUI.GetReloadingstate()) m_LoaderInUI.RemoveCapsule();
            if (m_currentType == CapsuleSystem.CapsuleType.ATTACK)
            {
                currentWeaponStats = ((CapsuleSystem.CapsuleAttack)capsulesPosses[m_currentIndexCapsule]).stats.stats;
            }
                m_isShooting = true;
        }

        private void EndShoot()
        {
            currentShotNumber = 0;
            m_currentIndexCapsule = ChangeProjecileIndex();
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
            if (m_currentIndexCapsule == capsulesPosses.Length - 1)
            {
                m_isReloading = true;
                return 0;
            }
            else
            {
                return m_currentIndexCapsule + 1;
            }
        }

        private void ReloadShot()
        {
            if (m_canShoot || m_isReloading) return;

            if (m_shootTimer > shootTime)
            {

                m_canShoot = true;
                m_shootTimer = 0;
                return;
            }
            else
            {

                m_shootTimer += Time.deltaTime;
            }
        }


        private void ReloadWeapon()
        {
            if (m_canShoot || !m_isReloading) return;

            if (m_reloadTimer > reloadTime)
            {
                m_canShoot = true;
                m_isReloading = false;
                m_reloadTimer = 0;
                return;
            }
            else
            {
                m_reloadTimer += Time.deltaTime;
            }
        }
    }

}