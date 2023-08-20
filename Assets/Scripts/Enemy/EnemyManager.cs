using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Enemies
{

    public class EnemyManager : MonoBehaviour
    {
        private ObjectState state;
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
        [SerializeField] private GameObject m_ExperiencePrefab;
        private EnemyKillRatio m_enemyKillRatio;
        private float m_spawnCooldown;

        public List<NpcHealthComponent> m_enemiesArray = new List<NpcHealthComponent>();

        static public bool EnemyTargetPlayer = true;

        public Transform altarObject;
        private AlatarHealthSysteme alatarRefScript;
        public List<Transform> altarTransformList = new List<Transform>();
        private List<AlatarHealthSysteme> altarScriptList = new List<AlatarHealthSysteme>();

        [SerializeField] private float m_tempsEntrePause;
        [SerializeField] private float m_tempsPause;
        private float tempsEcoulePause = 0;
        public bool spawningPhase = true;

        public GlobalSoundManager gsm;
        private void Start()
        {
            state = new ObjectState();
            GameState.AddObject(state);
            m_enemyKillRatio = GetComponent<EnemyKillRatio>();
            gsm = Camera.main.transform.GetComponentInChildren<GlobalSoundManager>();
           
            //if(altarObject != null) { alatarRefScript = altarObject.GetComponent<AlatarHealthSysteme>(); }
        }

        public void Update()
        {
            if (!state.isPlaying) return; 

            if (spawningPhase)
            {
                SpawnCooldown();
            }

        }

        public void ChangePauseState(bool state)
        {
            for (int i = 0; i < m_enemiesArray.Count; i++)
            {
                if (!state) m_enemiesArray[i].SetPauseState();
                else m_enemiesArray[i].RemovePauseState();
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

        public void ReplaceFarEnemy(GameObject enemy)
        {
            enemy.transform.position = FindPosition();
            Debug.Log("Repositioned at [" + enemy.transform.position + "]");
        }
        private float GetTimeSpawn()
        {
            return (m_spawnTime * ((Mathf.Sin(Time.time / 2.0f)) + 1.3f) / 2.0f);
        }

        private int GetNumberToSpawn()
        {
            int currentMaxUnit = (int)Mathf.Lerp(m_minUnitPerGroup, (m_maxUnitPerGroup + Time.time / 10), m_enemyKillRatio.GetRatioValue()); 
            Debug.Log("[" + currentMaxUnit + "]" + " Max unit spawn at [" + Time.time + "]" );
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
            int rnd = Random.Range(0, 520);
            GameObject enemySpawn;
            if (!EnemyTargetPlayer)
            {
                if (altarTransformList.Count <= 0) { return; }
                AlatarHealthSysteme nearestAltar = CheckDistanceAltar(positionSpawn);
                altarObject = nearestAltar.transform;
                altarScriptList.Add(nearestAltar);

            }

            if (rnd < 450)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation);
            }
            else if (rnd < 495 && rnd >= 450)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[1], positionSpawn, transform.rotation);
            }
            else if(rnd >= 496 && rnd < 501)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[2], positionSpawn, transform.rotation);
            }
            else if(rnd > 500 && rnd <= 510)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[3], positionSpawn, transform.rotation);
            }
            else if (rnd > 510)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[4], positionSpawn, transform.rotation);
            }
            else
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation);
            }

            NpcHealthComponent npcHealth = enemySpawn.GetComponent<NpcHealthComponent>();
            NpcMouvementComponent npcMove = enemySpawn.GetComponent<NpcMouvementComponent>();
            npcMove.enemymanage = this;
            npcHealth.SetInitialData( m_healthManager, this);

            if (EnemyTargetPlayer)
            {
                npcHealth.target  = m_playerTranform;
            }
            else
            {
                npcHealth.target =  altarObject;
            }
            m_enemiesArray.Add(npcHealth);
        }


        public void SpawnExp(Vector3 position ,int count )
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(m_ExperiencePrefab, position, Quaternion.identity);
            }
        }
        
        public void DestroyEnemy(NpcHealthComponent npcHealth)
        {

            if (!m_enemiesArray.Contains( npcHealth)) return;
            

            m_enemyKillRatio.AddEnemiKill();
            if (!EnemyTargetPlayer)
            {
                AlatarHealthSysteme nearestAltar = CheckDistanceAltar(npcHealth.transform.position);
                if (nearestAltar != null && Vector3.Distance(npcHealth.transform.position, nearestAltar.transform.position) < nearestAltar.rangeEvent)
                {
                    nearestAltar.IncreaseKillCount();
                }
                //alatarRefScript.IncreaseKillCount(); 

            }
            m_enemiesArray.Remove(npcHealth);
            Destroy(npcHealth.gameObject);
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

        public void ChangeSpawningPhase(bool spawning)
        {
            spawningPhase = spawning;
            if(spawning) { gsm.globalMusicInstance.setParameterByName("Repos", 0); }
            else { gsm.globalMusicInstance.setParameterByName("Repos", 1); }
        }
    }
}
