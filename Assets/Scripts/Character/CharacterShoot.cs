using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{

    public class CharacterShoot : MonoBehaviour
    {
        [SerializeField] [Range(0, 90.0f)] private float m_shootAngle = 90.0f;
        public int projectileNumber;
        public float shootTime;
        public GameObject projectileGO;
        public AudioSource m_shootSounds;

        private float m_shootTimer;
        private bool m_canShoot;
        private bool m_shootInput;
        private CharacterAim m_characterAim;
        private CharacterMouvement m_CharacterMouvement; // Add reference to move script
        [SerializeField] private CameraShake m_cameraShake;
        [SerializeField] private float m_shakeDuration = 0.1f;

        private void Start()
        {
            m_characterAim = GetComponent<CharacterAim>();
            m_CharacterMouvement = GetComponent<CharacterMouvement>(); // Assignation du move script
        }

        private void Update()
        {
            if (m_shootInput) Shoot();
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
            GlobalSoundManager.PlayOneShot(1, Vector3.zero);
            StartCoroutine( m_cameraShake.ShakeEffect(m_shakeDuration));
            float angle = m_shootAngle / projectileNumber;
            int mod = projectileNumber % 2 == 1 ? 0 : 1;
            for (int i = mod; i < projectileNumber + mod; i++)
            {
                GameObject projectileCreate = GameObject.Instantiate(projectileGO, transform.position, transform.rotation);
                if(projectileCreate.GetComponent<Projectile>())  projectileCreate.GetComponent<Projectile>().SetDirection(Quaternion.AngleAxis(angle * ((i + 1) / 2), transform.up) * m_characterAim.GetAim());
                if(projectileCreate.GetComponent<ProjectileExplosif>()) projectileCreate.GetComponent<ProjectileExplosif>().SetDirection(Quaternion.AngleAxis(angle * ((i + 1) / 2), transform.up) * m_characterAim.GetAim());
                angle = -angle;
            }
            m_shootSounds.Play();
            m_canShoot = false;
        }

        private void ReloadWeapon()
        {
            if (m_canShoot) return;

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
    }

}