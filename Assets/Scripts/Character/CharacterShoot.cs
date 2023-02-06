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
        public GameObject[] projectileGO;
        public LauncherProfil launcherProfil;
        public CapsuleProfil[] capsuleStats;
        public ChainEffect[] chainEffects;
        public int[] weaponOrder;

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
        private int currentIndexWeapon;
        private int currentProjectileIndex;

        private CharacterAim m_characterAim;
        private CharacterMouvement m_CharacterMouvement; // Add reference to move script
        [SerializeField] private CameraShake m_cameraShake;
        [SerializeField] private float m_shakeDuration = 0.1f;
        private Loader_Behavior m_LoaderInUI;

        private void Start()
        {
            InitComponent();
            launcherStats = launcherProfil.stats;

        }
        public void InitComponentStat(CharacterStat stat)
        {
          
            reloadTime = launcherStats.reloadTime;
            shootTime = launcherStats.timeBetweenCapsule;

        }

        private void InitComponent()
        {
            capsuleStatsAlone = new CapsuleStats[capsuleStats.Length];
            for (int i = 0; i < capsuleStats.Length; i++)
            {
                capsuleStatsAlone[i] = capsuleStats[i].stats;
            }
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>(); // Assignation du move script
            m_LoaderInUI = GameObject.Find("LoaderDisplay").GetComponent<Loader_Behavior>();
            m_LoaderInUI.SetCapsuleOrder(weaponOrder);
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
            float angle = GetShootAngle(currentWeaponStats);
            int mod = GetStartIndexProjectile(currentWeaponStats);

            for (int i = mod; i < currentWeaponStats.projectileNumber + mod; i++)
            {
                GameObject projectileCreate = GameObject.Instantiate(projectileGO[currentIndexWeapon], transform.position, transform.rotation);
                ProjectileData data = new ProjectileData();
                data.direction = Quaternion.AngleAxis(angle * ((i + 1) / 2), transform.up) * m_characterAim.GetAim();
                data.speed = currentWeaponStats.speed;
                data.life = currentWeaponStats.range / currentWeaponStats.speed;

                projectileCreate.GetComponent<Projectile>().SetProjectile(data);
                angle = -angle;
            }


            GlobalSoundManager.PlayOneShot(1, Vector3.zero);
            if (!m_LoaderInUI.GetReloadingstate()) m_LoaderInUI.RemoveCapsule();
            StartCoroutine(m_cameraShake.ShakeEffect(m_shakeDuration));
            currentShotNumber++;

            if (currentShotNumber == currentWeaponStats.shootNumber) EndShoot();

        }

        private void StartShoot()
        {
            currentIndexWeapon = weaponOrder[currentProjectileIndex];
            currentWeaponStats = capsuleStats[currentIndexWeapon].stats;

            if (currentProjectileIndex != 0)
            {
                int prevWeapon = weaponOrder[currentProjectileIndex - 1];
                //currentWeaponStats = chainEffects[prevWeapon].Active(currentWeaponStats,launcherProfil.stats) ;
            }

            m_isShooting = true;
        }

        private void EndShoot()
        {
            currentShotNumber = 0;
            currentProjectileIndex = ChangeProjecileIndex();
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
            if (currentProjectileIndex == weaponOrder.Length - 1)
            {
                m_isReloading = true;
                return 0;
            }
            else
            {
                return currentProjectileIndex + 1;
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