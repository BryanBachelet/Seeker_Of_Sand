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

        public void UpdateTimeValue()
        {
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

    }
}