using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Enemies
{

    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private Transform m_playerTranform;
        [SerializeField] private GameObject[] m_enemyGO = new GameObject[2];
        [SerializeField] private Vector3 m_offsetSpawnPos;
        [SerializeField] private Vector3 position;
        [SerializeField] private float m_spawnTime = 3.0f;
        [SerializeField] private int m_maxUnitPerGroup = 3;
        [SerializeField] private int m_minUnitPerGroup = 2;
        [SerializeField] private HealthManager m_healthManager;
        
        private EnemyKillRatio m_enemyKillRatio;
        private float m_spawnCooldown;
        
        private List<Enemy> m_enemiesArray = new List<Enemy>();


        private void Start()
        {
            m_enemyKillRatio = GetComponent<EnemyKillRatio>();
        }

        public void Update()
        {
            SpawnCooldown();
        }

        private Vector3 FindPosition()
        {
            float magnitude = (m_playerTranform.position - Camera.main.transform.position).magnitude;
            for (int i = 0; i < 25; i++)
            {
                Vector2 pos;
                float sign = Mathf.Sign(Random.Range(-1.0f, 1.0f));
                pos.y = sign * Random.Range(1.1f, 1.6f);
                sign = Mathf.Sign(Random.Range(-1.0f, 1.0f));
                pos.x = sign * Random.Range(1.0f, 1.6f);
                Vector3 v3Pos = Camera.main.ViewportToWorldPoint(new Vector3(pos.x, pos.y, magnitude));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(v3Pos, out hit, Mathf.Infinity, NavMesh.AllAreas))
                {
                    return hit.position;
                }

            }
            return Vector3.zero;
        }

        private float GetTimeSpawn()
        {
           return (m_spawnTime * ((Mathf.Sin(Time.time / 2.0f)) + 1.3f) / 2.0f);
        }

        private int GetNumberToSpawn()
        {
            int currentMaxUnit = (int)Mathf.Lerp(m_minUnitPerGroup, m_maxUnitPerGroup, m_enemyKillRatio.GetRatioValue());
            int number = Mathf.FloorToInt((currentMaxUnit * ((Mathf.Sin(Time.time / 2.0f + 7.5f)) + 1.3f) / 2.0f));
            number = number <= 0 ? 1 : number;
            return number;
        }

        private void SpawEnemiesGroup()
        {
            position = FindPosition();
            for (int i = 0; i < GetNumberToSpawn(); i++)
            {
                SpawnEnemy(position + Random.insideUnitSphere * 5f);
            }
        }

        private void SpawnCooldown()
        {
            if (m_spawnCooldown > GetTimeSpawn())
            {
                SpawEnemiesGroup();
                m_spawnCooldown = 0;
            }
            else
            {
                m_spawnCooldown += Time.deltaTime;

            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.5f);
        }

        private void SpawnEnemy(Vector3 positionSpawn)
        {
            int rnd = Random.Range(0, 100);
            GameObject enemySpawn;
            if (rnd < 95)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation);
            }
            else
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[1], positionSpawn, transform.rotation);
            }
            Enemy enemy = enemySpawn.GetComponent<Enemy>();
            enemy.SetManager(this, m_healthManager);
            enemy.SetTarget(m_playerTranform);
            m_enemiesArray.Add(enemy);
        }



        public void DestroyEnemy(Enemy enemy)
        {
            if (!m_enemiesArray.Contains(enemy)) return;

            m_enemyKillRatio.AddEnemiKill();

            m_enemiesArray.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }
}
