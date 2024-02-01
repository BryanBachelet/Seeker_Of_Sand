using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Enemies
{

    public class EnemyManager : MonoBehaviour
    {
        private ObjectState state;
        [SerializeField] public Transform m_playerTranform;
        [SerializeField] private Transform m_cameraTransform;
        [SerializeField] private GameObject[] m_enemyGO = new GameObject[2];
        [SerializeField] private Vector3 m_offsetSpawnPos;
        [SerializeField] private Vector3 position;
        [SerializeField] private float m_spawnTime = 3.0f;
        [SerializeField] private int m_maxUnitPerGroup = 3;
        [SerializeField] private int m_minUnitPerGroup = 2;
        [SerializeField] public int m_maxUnittotal = 400;
        [SerializeField] private AnimationCurve m_MaxUnitControl;
        static float currentMaxUnitValue;
        [SerializeField] private HealthManager m_healthManager;
        [SerializeField] private float m_radiusspawn;
        [SerializeField] private GameObject m_ExperiencePrefab;

        [Header("Enemy Spawn Parameters")]
        [SerializeField] private float m_minimumRadiusOfSpawn = 100;
        [SerializeField] private float m_maximumRadiusOfSpawn = 300;
        [SerializeField] private float m_offsetToSpawnCenter = 20.0f;
        [SerializeField] private float m_minimumSpeedToRepositing = 30.0f;
        private float m_upperStartPositionMagnitude = 50.0f;
        [SerializeField] private Transform m_enemyHolder;

        [Header("Enemy Target Rate")]
        [Range(0, 1.0f)] [SerializeField] private float m_bodylessEventTargetRate = .5f;
        [Range(0, 1.0f)] [SerializeField] private float m_fullBodyEventTargetRate = .5f;
        [Range(0, 1.0f)] [SerializeField] private float m_shamanEventTargetRate = 1f;
        [Range(0, 1.0f)] [SerializeField] private float m_runnerEventTargetRate = 0.0f;
        [Range(0, 1.0f)] [SerializeField] private float m_tankEventTargetRate = 0.25f;

        [Header("Enemy Bonus")]
        [SerializeField] private GameObject m_expBonus;
        [Range(0, 1.0f)] [SerializeField] private float m_spawnRateExpBonus = 0.01f;


        private Experience_System m_experienceSystemComponent;

        private EnemyKillRatio m_enemyKillRatio;
        private float m_spawnCooldown;


        public List<NpcHealthComponent> m_enemiesArray = new List<NpcHealthComponent>();
        public List<NpcHealthComponent> m_enemiesFocusAltar = new List<NpcHealthComponent>();

        static public bool EnemyTargetPlayer = true;

        [Header("Events Parameters")]
        public Image[] m_imageLifeEvents = new Image[3];
        public GameObject[] m_imageLifeEventsObj = new GameObject[3];
        public TMP_Text[] m_textProgressEvent = new TMP_Text[3];
        public Image[] m_sliderProgressEvent = new Image[3];

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

        private Character.CharacterMouvement m_characterMouvement;


        private int repositionningLimit = 10;
        private int repositionningCount;


        private DayCyclecontroller m_dayController;
        private float m_timeOfGame;

        private SerieController m_serieController;

        [SerializeField] private GameObject m_spawningVFX;

        // Stats Variables
        [HideInInspector] public int altarLaunch;
        [HideInInspector] public int altarSuccessed;
        [HideInInspector] public int killCount;
        private const string fileStatsName="\\Stats_data";

        [SerializeField] public TMP_Text m_Instruction;
        [SerializeField] public Image m_ImageInstruction;
        [SerializeField] public Sprite[] instructionSprite;
        [SerializeField] public Animator m_instructionAnimator;

        public bool spawningConstant = false;


        public delegate void OnDeath(Vector3 position, EntitiesTrigger tag, GameObject objectHit, float distance);
        public event OnDeath OnDeathEvent = delegate { };

        [SerializeField] private Animator detectionAnimator;
        [SerializeField] private Image m_enemyIcon;
        [SerializeField] private TMP_Text m_tmpTextEnemyRemain;
        [SerializeField] private Color[] colorSignUI = new Color[2];

        public int remainEnemy = 0;
        public void Awake()
        {
           
            TestReadDataSheet();
            state = new ObjectState();
            GameState.AddObject(state);
            m_enemyKillRatio = GetComponent<EnemyKillRatio>();
            gsm = m_cameraTransform.GetComponentInChildren<GlobalSoundManager>();

            m_characterMouvement = m_playerTranform.GetComponent<Character.CharacterMouvement>();
            m_experienceSystemComponent = m_playerTranform.GetComponent<Experience_System>();
            m_dayController = GameObject.Find("DayController").gameObject.GetComponent<DayCyclecontroller>();
            m_serieController = m_playerTranform.GetComponent<SerieController>();
            m_timeOfGame = 0;
            //if(altarObject != null) { alatarRefScript = altarObject.GetComponent<AlatarHealthSysteme>(); }
        }

        public void Update()
        {

            if (DayCyclecontroller.choosingArtefactStart) return;
            if (!GameState.IsPlaying()) return;
            repositionningCount = 0;
            m_timeOfGame += Time.deltaTime;
            remainEnemy = m_enemiesArray.Count;
            if(remainEnemy > 0)
            {
                m_tmpTextEnemyRemain.text = "Remain : " + (remainEnemy -1);
            }
            if (spawningPhase || m_dayController.isNight || m_targetTransformLists.Count > 0)
            {
                if(m_dayController.isNight && spawningPhase == false)
                {
                    ChangeSpawningPhase(true);
                }
                m_maxUnittotal = (int)m_MaxUnitControl.Evaluate(m_timeOfGame / 60);
                SpawnCooldown();
            }

        }


        public IEnumerator DisplayInstruction(string instruction, float time, Color colorText, Sprite iconSprite)
        {
            m_Instruction.color = colorText;
            m_Instruction.text = instruction;
            m_ImageInstruction.sprite = iconSprite;
            m_instructionAnimator.SetTrigger("DisplayInstruction");
            yield return new WaitForSeconds(time);
            m_instructionAnimator.ResetTrigger("DisplayInstruction");
        }
        public void ChangePauseState(bool state)
        {
            for (int i = 0; i < remainEnemy; i++)
            {
                if (!state) m_enemiesArray[i].SetPauseState();
                else m_enemiesArray[i].RemovePauseState();
            }
        }

        // To be sure the enemy spwan is out of the screen 
        // We gonna use the ScreenToWorldPoint but we still want to the position be at minimun and max distance'
        // if the are not we search a new position again the 

        private float GetPositionRandom(float minNegatif, float maxNegatif, float minPositif, float maxPositif)
        {
            bool isPositf = Random.Range(-1.0f, 1.0f) > 0;
            if (isPositf)
            {
                return Random.Range(minPositif, maxPositif);
            }
            else
            {
                return Random.Range(minNegatif, maxNegatif);
            }
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 pos = Random.insideUnitCircle.normalized;
            pos.z = pos.y;
            pos.y = 0.0f;
            float radius = Random.Range(m_minimumRadiusOfSpawn, m_maximumRadiusOfSpawn);
            return pos * radius;
        }

        private Vector3 FindPosition()
        {
            float magnitude = (m_playerTranform.position - m_cameraTransform.position).magnitude;
            for (int i = 0; i < 25; i++)
            {
                Vector3 basePosition = m_playerTranform.transform.position + m_playerTranform.forward * m_offsetToSpawnCenter;
                basePosition += Vector3.up * m_upperStartPositionMagnitude;
                basePosition += GetRandomPosition();

                Vector3 v3Pos = basePosition;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(v3Pos, out hit, Mathf.Infinity, NavMesh.AllAreas))
                {

                    return hit.position;
                }

            }
            return Vector3.zero;
        }

        private Vector3 FindPositionAroundTarget(Transform targetTransform)
        {
            float magnitude = (targetTransform.position - m_cameraTransform.position).magnitude;
            for (int i = 0; i < 25; i++)
            {
                Vector3 basePosition = targetTransform.transform.position;
                basePosition += Vector3.up * m_upperStartPositionMagnitude;
                basePosition += GetRandomPosition();

                Vector3 v3Pos = basePosition;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(v3Pos, out hit, Mathf.Infinity, NavMesh.AllAreas))
                {

                    return hit.position;
                }

            }
            return Vector3.zero;
        }


        public bool ReplaceFarEnemy(GameObject enemy)
        {
            if (m_characterMouvement.GetCurrentSpeed() > m_minimumSpeedToRepositing || repositionningCount >= repositionningLimit)
                return false;

            repositionningCount++;
            enemy.transform.position = FindPositionAroundTarget(enemy.GetComponent<NpcHealthComponent>().targetData.target);
            return true;
        }
        private float GetTimeSpawn()
        {
            return (m_spawnTime + (m_spawnTime * ((Mathf.Sin(m_timeOfGame/ 2.0f)) + 1.3f) / 2.0f));
        }

        private int GetNumberToSpawn()
        {
            int currentMaxUnit = (int)Mathf.Lerp(m_minUnitPerGroup, (m_maxUnitPerGroup), m_enemyKillRatio.GetRatioValue());
            int number = Mathf.FloorToInt((currentMaxUnit * ((Mathf.Sin(m_timeOfGame / 2.0f + 7.5f)) + 1.3f) / 2.0f));
            number = number <= 0 ? 1 : number;
            return number;
        }

        private void SpawEnemiesGroup()
        {
            position = FindPosition();
            posspawn.Add(position);
            Instantiate(m_spawningVFX, position, transform.rotation);
            GlobalSoundManager.PlayOneShot(37, position);
            for (int i = 0; i < GetNumberToSpawn(); i++)
            {
                SpawnEnemy(position + Random.insideUnitSphere * 5f);
            }
        }

        private void SpawnCooldown()
        {
            if (m_spawnCooldown > GetTimeSpawn())
            {
                if (remainEnemy < m_maxUnittotal)
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
            Gizmos.DrawWireSphere(m_playerTranform.position, m_minimumRadiusOfSpawn);
            Gizmos.DrawWireSphere(m_playerTranform.position, m_maximumRadiusOfSpawn);
            for (int i = 0; i < posspawn.Count; i++)
            {
                Gizmos.DrawSphere(posspawn[i], 1);
            }
        }

        private void SpawnEnemy(Vector3 positionSpawn)
        {
            int rnd = Random.Range(0, 520);
            GameObject enemySpawn;
            float targetRate = 0.0f;
            bool focusPlayer = false;
            if (!EnemyTargetPlayer)
            {
                if (m_targetTransformLists.Count <= 0) { return; }
                ObjectHealthSystem nearestAltar = CheckDistanceTarget(positionSpawn);
                m_targetTranform = nearestAltar.transform;

                targetRate = Random.Range(0.0f, 1.0f);
            }

            if (rnd < 450)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation, m_enemyHolder);
                if (!EnemyTargetPlayer)
                {
                    if (targetRate > m_bodylessEventTargetRate)
                    {
                        focusPlayer = true;
                    }
                }
            }
            else if (rnd < 495 && rnd >= 450)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[1], positionSpawn, transform.rotation, m_enemyHolder);
                if (!EnemyTargetPlayer)
                {
                    if (targetRate > m_fullBodyEventTargetRate)
                    {
                        focusPlayer = true;
                    }
                }
            }
            else if (rnd >= 496 && rnd < 501)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[2], positionSpawn, transform.rotation, m_enemyHolder);
                if (!EnemyTargetPlayer)
                {
                    if (targetRate > m_tankEventTargetRate)
                    {
                        focusPlayer = true;
                    }
                }
            }
            else if (rnd > 500 && rnd <= 510)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[3], positionSpawn, transform.rotation, m_enemyHolder);
                if (!EnemyTargetPlayer)
                {
                    if (targetRate > m_shamanEventTargetRate)
                    {
                        focusPlayer = true;
                    }
                }
            }
            else if (rnd > 510)
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[4], positionSpawn, transform.rotation, m_enemyHolder);
                if (!EnemyTargetPlayer)
                {
                    if (targetRate > m_runnerEventTargetRate)
                    {
                        focusPlayer = true;
                    }
                }
            }
            else
            {
                enemySpawn = GameObject.Instantiate(m_enemyGO[0], positionSpawn, transform.rotation, m_enemyHolder);
                if (!EnemyTargetPlayer)
                {
                    if (targetRate > m_bodylessEventTargetRate)
                    {
                        focusPlayer = true;
                    }
                }
            }

            NpcHealthComponent npcHealth = enemySpawn.GetComponent<NpcHealthComponent>();
            NpcMouvementComponent npcMove = enemySpawn.GetComponent<NpcMouvementComponent>();
            npcMove.enemiesManager = this;
            npcHealth.SetInitialData(m_healthManager, this);

            if (EnemyTargetPlayer)
            {
                npcHealth.targetData.isMoving = true;
                npcHealth.SetTarget( m_playerTranform);
            }
            else
            {

                if (focusPlayer)
                {
                    npcHealth.targetData.isMoving = true;
                    npcHealth.SetTarget(m_playerTranform);
                   

                }
                else
                {
                     npcHealth.SetTarget( m_targetTranform);
                    npcHealth.targetData.isMoving = false;
                    m_enemiesFocusAltar.Add(npcHealth);
                }

            }
            m_enemiesArray.Add(npcHealth);
        }


        public void AddTarget(Transform target)
        {

            ObjectHealthSystem healthSystem = target.GetComponent<ObjectHealthSystem>();
            if (m_targetTransformLists.Contains(target) && m_targetList.Contains(healthSystem)) return;
            m_targetTransformLists.Add(target);
            ChangeSpawningPhase(true);
            m_targetList.Add(target.GetComponent<ObjectHealthSystem>());
            int indexTargetList = m_targetList.Count - 1;
            ObjectHealthSystem healthSystemReference = target.GetComponent<ObjectHealthSystem>();
            healthSystemReference.m_eventLifeUIFeedback = m_imageLifeEvents[indexTargetList];
            healthSystemReference.m_eventLifeUIFeedbackObj = m_imageLifeEventsObj[indexTargetList];
            healthSystemReference.m_eventProgressUIFeedback = m_textProgressEvent[indexTargetList];
            if(target.GetComponent<AltarBehaviorComponent>())
            {
                altarLaunch++;
                target.GetComponent<AltarBehaviorComponent>().m_eventProgressionSlider = m_sliderProgressEvent[indexTargetList];
                m_sliderProgressEvent[indexTargetList].gameObject.SetActive(true);
            }
            m_imageLifeEventsObj[indexTargetList].SetActive(true);
            m_imageLifeEvents[indexTargetList].gameObject.SetActive(true);
            m_textProgressEvent[indexTargetList].gameObject.SetActive(true);
            EnemyTargetPlayer = false;
        }


        public void RemoveTarget(Transform target)
        {

            if (!m_targetTransformLists.Contains(target)) return;
            m_targetTransformLists.Remove(target);
            int prevCount = m_targetList.Count;
            m_targetList.Remove(target.GetComponent<ObjectHealthSystem>());
            ChangeSpawningPhase(false);
            ObjectHealthSystem healthSystem = target.GetComponent<ObjectHealthSystem>();
            healthSystem.ResetUIHealthBar();
            m_imageLifeEventsObj[prevCount - 1].SetActive(false);
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


        public void RemoveAllTarget()
        {
            for (int i = 0; i < m_targetTransformLists.Count; i++)
            {
                RemoveTarget(m_targetTransformLists[i]);
            }   
        }

        public void RemoveAllAltar()
        {
            for (int i = 0; i < m_altarTransform.Count; i++)
            {
                RemoveAltar(m_altarTransform[i]);
            }
        }

        public void AddAltar(Transform altarTarget)
        {
            m_altarTransform.Add(altarTarget);
            m_altarList.Add(altarTarget.GetComponent<AltarBehaviorComponent>());
        }

       

        public void SendInstruction(string Instruction, Color colorText, Sprite iconAssociate)
        {
            StartCoroutine(DisplayInstruction(Instruction, 2, colorText, iconAssociate));
        }
        public void RemoveAltar(Transform altarTarget)
        {
            if (!m_altarTransform.Contains(altarTarget)) return;
            m_altarTransform.Remove(altarTarget);
            m_altarList.Remove(altarTarget.GetComponent<AltarBehaviorComponent>());
        }

        public void SpawnExp(Vector3 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject expObj = Instantiate(m_ExperiencePrefab, position, Quaternion.identity);
                m_experienceSystemComponent.AddExpParticule(expObj.GetComponent<ExperienceMouvement>());
            }

            float rate = Random.Range(0.0f, 1.0f);
            if (rate <= m_spawnRateExpBonus)
            {
                Instantiate(m_expBonus, position, Quaternion.identity);

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


        public void EnemyHasDied(NpcHealthComponent npcHealth, int xpCount)
        {

            Vector3 position = npcHealth.transform.position;
            SpawnExp(position, xpCount);
            IncreseAlterEnemyCount(npcHealth);
            float distance = Vector3.Distance(m_playerTranform.position, npcHealth.transform.position);
            OnDeathEvent(position,EntitiesTrigger.Enemies,npcHealth.gameObject, distance);
        }
        public void DestroyEnemy(NpcHealthComponent npcHealth)
        {

            if (!m_enemiesArray.Contains(npcHealth)) return;

            killCount++;
            m_enemyKillRatio.AddEnemiKill();
            if (m_enemiesFocusAltar.Contains(npcHealth)) m_enemiesFocusAltar.Remove(npcHealth);
            m_enemiesArray.Remove(npcHealth);
            Destroy(npcHealth.gameObject);
        }

        public void DeathEnemy()
        {
            m_serieController.RefreshSeries(false);
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
            detectionAnimator.SetBool("ShadowDetection", spawningPhase);
            if (spawning) 
            { 
                gsm.globalMusicInstance.setParameterByName("Repos", 0); 
                StartCoroutine(DisplayInstruction("Corrupt spirit appears", 2, Color.white, instructionSprite[0])); 
                m_enemyIcon.color = colorSignUI[0];
                m_tmpTextEnemyRemain.color = Color.Lerp(colorSignUI[0], Color.red, 0.5f); ; 
            }
            else 
            { 
                gsm.globalMusicInstance.setParameterByName("Repos", 1); 
                StartCoroutine(DisplayInstruction("Corrupt spirit stop appears", 2, Color.white, instructionSprite[1])); 
                m_enemyIcon.color = colorSignUI[1]; 
                m_tmpTextEnemyRemain.color = colorSignUI[1]; 
            }
        }

        public void CreateCurveSheet()
        {
            StreamReader strReader = new StreamReader("C:\\Projets\\Guerhouba\\K-TrainV1\\Assets\\Progression Demo - SpawnSheet.csv");
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
#if UNITY_EDITOR
            string filePath = Application.dataPath + "\\Game data use\\Progression Demo - SpawnSheet.csv";
#else

        string filePath = Application.dataPath + "\\Progression Demo - SpawnSheet.csv";
       

#endif 

            int lineNumber = 5;

            string lineContents = ReadSpecificLine(filePath, lineNumber);
            string[] data_values = lineContents.Split(',');
            long[] dataTransformed = new long[data_values.Length - 1];
            for (int i = 0; i < dataTransformed.Length; i++)
            {
                if (data_values[i] == "") continue;
                dataTransformed[i] = long.Parse(data_values[i] );
                tempAnimationCurve.AddKey(i, dataTransformed[i]);
                debugdata = debugdata + " , " + dataTransformed[i];

            }
            m_MaxUnitControl = tempAnimationCurve;
            //Debug.Log(debugdata);

        }


        #region EndStat

        public EndInfoStats FillEndStat()
        {
            EndInfoStats endInfoStats = new EndInfoStats();

            endInfoStats.durationGame = m_timeOfGame;
            endInfoStats.enemyKill = killCount;
            endInfoStats.altarSuccessed = altarSuccessed;
            endInfoStats.altarRepeated = altarLaunch;
            endInfoStats.bigestCombo = m_serieController.m_biggestMultiplicator;
            endInfoStats.nightValidate = m_dayController.m_nightCount ;

            CheckEndStat(endInfoStats);
            return endInfoStats;
        }


        public void CheckEndStat(EndInfoStats stats)
        {
#if UNITY_EDITOR
            string filePath = Application.dataPath +"\\Temp"+ fileStatsName + GameState.profileName + ".sost";
#else
            string filePath = Application.dataPath + fileStatsName + GameState.profileName + ".txt";
#endif
            EndInfoStats statsSave = Save.SaveManager.ReadEndStats(filePath);
            if(statsSave.HasSuperiorValue(stats))
            {
                Save.SaveManager.WriteEndStats(filePath, stats);
            }
        }
      


#endregion

    }

}
