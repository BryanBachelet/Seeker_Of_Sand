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
        [SerializeField] private int m_maxUnittotal = 400;
        [SerializeField] private HealthManager m_healthManager;
        [SerializeField] private float m_radiusspawn;

        private EnemyKillRatio m_enemyKillRatio;
        private float m_spawnCooldown;

        public List<Enemy> m_enemiesArray = new List<Enemy>();

        static public bool EnemyTargetPlayer = true;

        public Transform altarObject;
        private AlatarHealthSysteme alatarRefScript;
        public List<Transform> altarTransformList = new List<Transform>();
        private List<AlatarHealthSysteme> altarScriptList = new List<AlatarHealthSysteme>();

        public UnityEngine.UI.Text ui_DangerLevelObject;
        [SerializeField] private float m_tempsEntrePause;
        [SerializeField] private float m_tempsPause;
        private float tempsEcoulePause = 0;
        private bool spawningPhase = true;
        private void Start()
        {
            m_enemyKillRatio = GetComponent<EnemyKillRatio>();
            ui_DangerLevelObject.text = "Medium";
            ui_DangerLevelObject.color = Color.red;
            //if(altarObject != null) { alatarRefScript = altarObject.GetComponent<AlatarHealthSysteme>(); }
        }

        public void Update()
        {
            if(spawningPhase)
            {
                SpawnCooldown();
                if(tempsEcoulePause < m_tempsEntrePause)
                {
                    tempsEcoulePause += Time.deltaTime;
                }
                else
                {
                    StartCoroutine(ChangeSpawningPhase(m_tempsPause));
                }
            }
            
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
                v3Pos = Random.onUnitSphere * m_radiusspawn + m_playerTranform.position;
                v3Pos.y = 50;
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
                if (m_enemiesArray.Count < m_maxUnittotal)
                {
                    SpawEnemiesGroup();
                }

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
            if (!EnemyTargetPlayer)
            {
                if (altarTransformList.Count <= 0) { return; }
                AlatarHealthSysteme nearestAltar = CheckDistanceAltar(positionSpawn);
                altarObject = nearestAltar.transform;
                altarScriptList.Add(nearestAltar);

            }

            if (rnd < 90)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation);
            }
            else if( rnd < 97)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[1], positionSpawn, transform.rotation);
            }
            else
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[2], positionSpawn, transform.rotation);
            }
            Enemy enemy = enemySpawn.GetComponent<Enemy>();
            enemy.SetManager(this, m_healthManager);
            if (EnemyTargetPlayer)
            {
                enemy.SetTarget(m_playerTranform);
            }
            else
            {
                enemy.SetTarget(altarObject);
            }
            m_enemiesArray.Add(enemy);
        }



        public void DestroyEnemy(Enemy enemy)
        {
            if (!m_enemiesArray.Contains(enemy)) return;

            m_enemyKillRatio.AddEnemiKill();
            if (!EnemyTargetPlayer)
            {
                AlatarHealthSysteme nearestAltar = CheckDistanceAltar(enemy.transform.position);
                if (nearestAltar != null && Vector3.Distance(enemy.transform.position, nearestAltar.transform.position) < nearestAltar.rangeEvent)
                {
                    nearestAltar.IncreaseKillCount();
                }
                //alatarRefScript.IncreaseKillCount(); 

            }
            m_enemiesArray.Remove(enemy);
            Destroy(enemy.gameObject);
        }

        public AlatarHealthSysteme CheckDistanceAltar(Vector3 position)
        {
            float distancePlusProche = 10000;
            if (altarTransformList.Count <= 0) { return null; }
            AlatarHealthSysteme altarSript = altarTransformList[0].GetComponent<AlatarHealthSysteme>();
            for (int i = 0; i < altarTransformList.Count; i++)
            {
                float distanceAltar = Vector3.Distance(position, altarTransformList[i].position);
                if (distanceAltar < distancePlusProche)
                {
                    altarSript = altarTransformList[i].GetComponent<AlatarHealthSysteme>();
                    distancePlusProche = distanceAltar;
                }
            }
            return altarSript;
        }

        public void AddAltarEvent(Transform Altar)
        {
            altarTransformList.Add(Altar);
            altarScriptList.Add(Altar.GetComponent<AlatarHealthSysteme>());
            EnemyTargetPlayer = false;
        }

        public void RemoveAltarEvent(Transform Altar)
        {
            altarTransformList.Remove(Altar);
            altarScriptList.Remove(Altar.GetComponent<AlatarHealthSysteme>());

            if (altarTransformList.Count <= 0)
            {
                EnemyTargetPlayer = true;
            }
            else
            {
                EnemyTargetPlayer = false;
            }
        }

        public IEnumerator ChangeSpawningPhase(float tempsPause)
        {
            spawningPhase = false;
            ui_DangerLevelObject.text = "Peaceful";
            ui_DangerLevelObject.color = Color.white;
            yield return new WaitForSeconds(tempsPause);
            tempsEcoulePause = 0;
            spawningPhase = true;
            ui_DangerLevelObject.text = "Medium";
            ui_DangerLevelObject.color = Color.red;
        }
    }
}
