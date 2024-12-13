using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GuerhoubaGames.VFX
{

    public class VFX_Spawner : MonoBehaviour
    {
        public GameObject vfxToSpawn;
        public float spawnTime;
        private float m_spawnTimer;

        public bool onDamageSpawn;

        public void Start()
        {
            if(onDamageSpawn)
            {
                SpellSystem.DOTMeta dotMeta = GetComponent<SpellSystem.DOTMeta>();
                dotMeta.OnDamage += SpawnVFx;
            }
          
        }

        public void SpawnVFx(Vector3 position)
        {
            Instantiate(vfxToSpawn, position, transform.rotation);
        }

        public void UpdateTimeValue()
        {
            if (onDamageSpawn) return;

            if (m_spawnTimer > spawnTime)
            {
                m_spawnTimer = 0.0f;
                Instantiate(vfxToSpawn, transform.position, transform.rotation);
            }
            else
            {
                m_spawnTimer += Time.deltaTime;
            }
        }

        public void ResetTimer()
        {
                
        }

    }
}