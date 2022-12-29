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
        private CharacterAim m_characterAim;

        private void Start()
        {
            m_characterAim = GetComponent<CharacterAim>();
        }

        public void ShootInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started) Shoot();
        }

        private void Shoot()
        {
            if (!m_canShoot) return;
            float angle = m_shootAngle / projectileNumber;
            int mod = projectileNumber % 2 == 1 ? 0 : 1;
            for (int i = mod; i < projectileNumber + mod; i++)
            {
                GameObject projectileCreate = GameObject.Instantiate(projectileGO, transform.position, transform.rotation);
                projectileCreate.GetComponent<Projectile>().SetDirection(Quaternion.AngleAxis(angle * ((i + 1) / 2), transform.up) * m_characterAim.GetAim());
                angle = -angle;
            }
            m_shootSounds.Play();
            m_canShoot = false;
        }

        private void Update()
        {
            ReloadWeapon();
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