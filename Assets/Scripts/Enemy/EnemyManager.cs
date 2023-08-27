using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;


namespace Enemies
{

    public class EnemyManager : MonoBehaviour
    {
        private ObjectState state;
        [SerializeField] public Transform m_playerTranform;
        [SerializeField] private GameObject[] m_enemyGO = new GameObject[2];
        [SerializeField] private Vector3 m_offsetSpawnPos;
        [SerializeField] private Vector3 position;
        [SerializeField] private float m_spawnTime = 3.0f;
        [SerializeField] private int m_maxUnitPerGroup = 3;
        [SerializeField] private int m_minUnitPerGroup = 2;
        [SerializeField] private int m_maxUnittotal = 400;
        [SerializeField] private AnimationCurve m_MaxUnitControl;
        [SerializeField] private HealthManager m_healthManager;
        [SerializeField] private float m_radiusspawn;
        [SerializeField] private GameObject m_ExperiencePrefab;
        private EnemyKillRatio m_enemyKillRatio;
        private float m_spawnCooldown;

        public List<NpcHealthComponent> m_enemiesArray = new List<NpcHealthComponent>();
        public List<NpcHealthComponent> m_enemiesFocusAltar = new List<NpcHealthComponent>();

        static public bool EnemyTargetPlayer = true;

        public Transform m_targetTranform;
        private ObjectHealthSystem m_targetScript;
        public List<Transform> m_targetTransformLists = new List<Transform>();
        private List<ObjectHealthSystem> m_targetList = new List<ObjectHealthSystem>();

        private List<Transform> m_altarTransform = new List<Transform>();
        private List<AltarBehaviorComponent> m_altarList = new List<AltarBehaviorComponent>();

        [SerializeField] private float m_tempsEntrePause;
        [SerializeField] private float m_tempsPause;
        private float tempsEcoulePause = 0;
        public bool spawningPhase = true;

        public GlobalSoundManager gsm;
        private List<Vector3> posspawn = new List<Vector3>();

        public int[] debugSpawnValue;
        public void Awake()
        {
            TestReadDataSheet();
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
                m_maxUnittotal = (int)m_MaxUnitControl.Evaluate(Time.time / 60);
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
            return (m_spawnTime + (m_spawnTime * ((Mathf.Sin(Time.time / 2.0f)) + 1.3f) / 2.0f));
        }

        private int GetNumberToSpawn()
        {
            int currentMaxUnit = (int)Mathf.Lerp(m_minUnitPerGroup, (m_maxUnitPerGroup), m_enemyKillRatio.GetRatioValue());
            int number = Mathf.FloorToInt((currentMaxUnit * ((Mathf.Sin(Time.time / 2.0f + 7.5f)) + 1.3f) / 2.0f));
            number = number <= 0 ? 1 : number;
            return number;
        }

        private void SpawEnemiesGroup()
        {
            position = FindPosition();
            posspawn.Add(position);
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
            for (int i = 0; i < posspawn.Count; i++)
            {
                Gizmos.DrawSphere(posspawn[i], 1);
            }
        }

        private void SpawnEnemy(Vector3 positionSpawn)
        {
            int rnd = Random.Range(0, 520);
            GameObject enemySpawn;
            if (!EnemyTargetPlayer)
            {
                if (m_targetTransformLists.Count <= 0) { return; }
                ObjectHealthSystem nearestAltar = CheckDistanceTarget(positionSpawn);
                m_targetTranform = nearestAltar.transform;
                m_targetList.Add(nearestAltar);

            }

            if (rnd < 450)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation);
            }
            else if (rnd < 495 && rnd >= 450)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[1], positionSpawn, transform.rotation);
            }
            else if (rnd >= 496 && rnd < 501)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[2], positionSpawn, transform.rotation);
            }
            else if (rnd > 500 && rnd <= 510)
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
            npcMove.enemiesManager = this;
            npcHealth.SetInitialData(m_healthManager, this);

            if (EnemyTargetPlayer)
            {
                npcHealth.targetData.target = m_playerTranform;
                npcHealth.targetData.isMoving = true;
            }
            else
            {
                npcHealth.targetData.target = m_targetTranform;
                npcHealth.targetData.isMoving = false;
                m_enemiesFocusAltar.Add(npcHealth);
            }
            m_enemiesArray.Add(npcHealth);
        }


        public void AddTarget(Transform target)
        {
            m_targetTransformLists.Add(target);
            m_targetList.Add(target.GetComponent<ObjectHealthSystem>());
            EnemyTargetPlayer = false;
        }


        public void RemoveTarget(Transform target)
        {
            m_targetTransformLists.Remove(target);
            m_targetList.Remove(target.GetComponent<ObjectHealthSystem>());
            for (int i = 0; i < m_enemiesFocusAltar.Count; i++)
            {
                NpcHealthComponent npcHealth = m_enemiesFocusAltar[i];
                if (npcHealth)
                {
                    npcHealth.targetData.target = m_playerTranform;
                    npcHealth.targetData.isMoving = true;
                    npcHealth.ResetTarget();
                }
            }

            if (m_targetTransformLists.Count <= 0)
            {
                EnemyTargetPlayer = true;
            }
            else
            {
                EnemyTargetPlayer = false;
            }
        }

        public void AddAltar(Transform altarTarget)
        {
            m_altarTransform.Add(altarTarget);
            m_altarList.Add(altarTarget.GetComponent<AltarBehaviorComponent>());
        }

        public void RemoveAltar(Transform altarTarget)
        {
            m_altarTransform.Remove(altarTarget);
            m_altarList.Remove(altarTarget.GetComponent<AltarBehaviorComponent>());
        }

        public void SpawnExp(Vector3 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(m_ExperiencePrefab, position, Quaternion.identity);
            }
        }

        public void IncreseAlterEnemyCount(NpcHealthComponent npcHealth)
        {
            if (!EnemyTargetPlayer)
            {
                AltarBehaviorComponent nearestAltar = FindClosestAltar(npcHealth.transform.position);
                if (nearestAltar != null && Vector3.Distance(npcHealth.transform.position, nearestAltar.transform.position) < nearestAltar.rangeEvent)
                {
                    nearestAltar.IncreaseKillCount();
                }

            }
        }

        public void DestroyEnemy(NpcHealthComponent npcHealth)
        {

            if (!m_enemiesArray.Contains(npcHealth)) return;


            m_enemyKillRatio.AddEnemiKill();

            m_enemiesArray.Remove(npcHealth);
            Destroy(npcHealth.gameObject);
        }

        public AltarBehaviorComponent FindClosestAltar(Vector3 position)
        {
            float closestDistance = 10000;
            if (m_altarList.Count <= 0) return null;
            AltarBehaviorComponent altarScript = m_altarList[0];
            for (int i = 0; i < m_altarTransform.Count; i++)
            {
                float distanceAltar = Vector3.Distance(position, m_altarTransform[i].position);
                if (distanceAltar < closestDistance)
                {
                    altarScript = m_altarList[i];
                    closestDistance = distanceAltar;
                }
            }
            return altarScript;
        }

        public ObjectHealthSystem CheckDistanceTarget(Vector3 position)
        {
            float distancePlusProche = 10000;
            if (m_targetTransformLists.Count <= 0) { return null; }
            ObjectHealthSystem altarSript = m_targetTransformLists[0].GetComponent<ObjectHealthSystem>();
            for (int i = 0; i < m_targetTransformLists.Count; i++)
            {
                float distanceAltar = Vector3.Distance(position, m_targetTransformLists[i].position);
                if (distanceAltar < distancePlusProche)
                {
                    altarSript = m_targetTransformLists[i].GetComponent<ObjectHealthSystem>();
                    distancePlusProche = distanceAltar;
                }
            }
            return altarSript;
        }

        public void ChangeSpawningPhase(bool spawning)
        {
            spawningPhase = spawning;
            if (spawning) { gsm.globalMusicInstance.setParameterByName("Repos", 0); }
            else { gsm.globalMusicInstance.setParameterByName("Repos", 1); }
        }

        public void CreateCurveSheet()
        {
            StreamReader strReader = new StreamReader("C:\\Projets\\Guerhouba\\K-TrainV1\\Assets\\Progression Demo - SpawnSheet (2).csv");
            bool endOfFile = false;
            while (!endOfFile)
            {
                string data_String = strReader.ReadLine();
                if (data_String == null)
                {
                    endOfFile = true;
                    Debug.Log("Read done");
                    break;
                }
                var data_values = data_String.Split(',');
                for (int i = 0; i < data_values.Length; i++)
                {
                    Debug.Log("value: " + i.ToString() + " " + data_values[i].ToString());

                }
                //Debug.Log(data_values[0].ToString() + " " + data_values[1].ToString() + " " + data_values[2].ToString() + " ");
            }
        }

        static string ReadSpecificLine(string filePath, int lineNumber)
        {
            string content = null;

            using (StreamReader file = new StreamReader(filePath))
            {
                for (int i = 1; i < lineNumber; i++)
                {
                    file.ReadLine();

                    if (file.EndOfStream)
                    {
                        //Console.WriteLine($"End of file.  The file only contains {i} lines.");
                        break;
                    }
                }
                content = file.ReadLine();
            }
            return content;

        }

        public void TestReadDataSheet()
        {
            AnimationCurve tempAnimationCurve = new AnimationCurve();
            string debugdata = "";
            string filePath = Application.dataPath + "\\Progression Demo - SpawnSheet (5).csv";
            int lineNumber = 5;

            string lineContents = ReadSpecificLine(filePath, lineNumber);
            string[] data_values = lineContents.Split(',');
            int[] dataTransformed = new int[data_values.Length - 1];
            for (int i = 0; i < dataTransformed.Length; i++)
            {
                dataTransformed[i] = int.Parse(data_values[i + 1]);
                tempAnimationCurve.AddKey(i, dataTransformed[i]);
                debugdata = debugdata + " , " + dataTransformed[i];

            }
            debugSpawnValue = dataTransformed;


            m_MaxUnitControl = tempAnimationCurve;
            //Debug.Log(debugdata);

        }
    }
}
