using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SeekerOfSand.UI;
using UnityEngine.InputSystem;
using System.Runtime.Serialization.Formatters.Binary;
using Render.Camera;
using Unity.VisualScripting;
using GuerhoubaGames.UI;
namespace Enemies
{

    [System.Serializable]
    public enum EnemyType
    {
        BODYLESS,
        BODYFULL,
        CHAMAN,
        TANK,
        RUNNER,
        TWILIGHT_SISTER,
        SHROOM,
    }

    [System.Serializable]
    public struct EnemyTypeStats
    {
        public EnemyType type;
        public int instanceCount;
        [HideInInspector] public int instanceSpawnPerRoom;
        public AnimationCurve animationCurve;
    }

    public enum EnemySpawnCause
    {
        EVENT,
        NIGHT,
        DEBUG,
    }


    public class EnemyManager : MonoBehaviour
    {
        [HideInInspector] private ObjectState state;
        [HideInInspector] public Transform m_playerTranform;
        [HideInInspector] private Transform m_cameraTransform;

        [HideInInspector] private Vector3 position = new Vector3(0, 0, 0);
        [HideInInspector] private float m_spawnTime = 10;

        [SerializeField] private int m_groupEnemySize = 5;
        [HideInInspector] private HealthManager m_healthManager;

        [Header("Enemy Spawn Parameters")]
        [HideInInspector] private float m_minimumRadiusOfSpawn = 150;
        [HideInInspector] private float m_maximumRadiusOfSpawn = 300;
        [HideInInspector] private float m_offsetToSpawnCenter = 20.0f;
        [HideInInspector] private float m_upperStartPositionMagnitude = 50.0f;
        [SerializeField] private AnimationCurve enemyGenerateDissonanceProba;

        [SerializeField] public ui_DisplayText m_mainInformationDisplay;

        [SerializeField] public int countEnemySpawnMaximum;

        [SerializeField] public Transform AstrePositionReference;

        [HideInInspector] private CameraBehavior m_cameraBehavior;
        public void ResetSpawnStat()
        {
            for (int i = 0; i < enemyTypeStats.Length; i++)
            {
                enemyTypeStats[i].instanceSpawnPerRoom = 0;
            }
        }

        #region EnemyParameter

        [SerializeField] private EnemyTypeStats[] enemyTypeStats = new EnemyTypeStats[6];
        #endregion

        [HideInInspector] private EnemyKillRatio m_enemyKillRatio;

        [HideInInspector] private List<NpcMetaInfos> m_enemiesArray = new List<NpcMetaInfos>();
        [HideInInspector] private List<NpcMetaInfos> m_enemiesFocusAltar = new List<NpcMetaInfos>();

        [HideInInspector] private List<Transform> m_targetTransformLists = new List<Transform>();
        [HideInInspector] private List<ObjectHealthSystem> m_targetList = new List<ObjectHealthSystem>();

        [HideInInspector] private List<Transform> m_altarTransform = new List<Transform>();
        [HideInInspector] private List<AltarBehaviorComponent> m_altarList = new List<AltarBehaviorComponent>();

        [HideInInspector] private bool spawningPhase = true;

        [HideInInspector] public GlobalSoundManager gsm;
        [HideInInspector] private List<Vector3> posspawn = new List<Vector3>();

        [HideInInspector] public Character.CharacterUpgrade m_characterUpgrade;

        [HideInInspector] private int repositionningLimit = 2;
        [HideInInspector] private int repositionningCount;

        [HideInInspector] public DayCyclecontroller m_dayController;
        [HideInInspector] private float m_timeOfGame;

        [HideInInspector] private SerieController m_serieController;
        [HideInInspector] private TerrainGenerator m_terrainGenerator;

        [SerializeField] private GameObject m_spawningVFX;

        // Stats Variables
        [HideInInspector] private int altarLaunch;
        [HideInInspector] public int altarSuccessed;
        [HideInInspector] private int killCount;
        [HideInInspector] private const string fileStatsName = "\\Stats_data";
        [HideInInspector] private const int m_tryCountToSpawnEnemy = 10;

        // Test Variable
#if UNITY_EDITOR
        [HideInInspector] public bool activeTestPhase;
        [HideInInspector] public bool activeSpawnConstantDebug = false;
        // Allow the enemis to spawn only a enemy type
        [HideInInspector] public bool activeSpecialSquad = false;

#endif
        [HideInInspector] private int[] specialSquadSelect;
        [HideInInspector] private PlayerInput playerInput;
        [HideInInspector] private int m_squadCount;
        // --------------------

        public delegate void OnDeath(Vector3 position, EntitiesTrigger tag, GameObject objectHit, float distance);
        public event OnDeath OnDeathEvent = delegate { };
        public delegate void OnDeathSimple();
        public event OnDeathSimple OnDeathSimpleEvent = delegate { };

        [HideInInspector] private EnemiesPullingSystem m_pullingSystem;

        [HideInInspector] public UIDispatcher uiDispatcher;

        [HideInInspector] private AltarBehaviorComponent lastAltarActivated;
        [HideInInspector] private int remainEnemy = 0;

        [HideInInspector] public bool isStopSpawn;
        // Spawn Cause Variable
        [HideInInspector] private bool[] m_spawnCauseState = new bool[4];

        [HideInInspector] private int comboCount;
        [HideInInspector] private int maxComboValue;
        [HideInInspector] private GameLayer m_gameLayer;

        [HideInInspector] private Vector3[] spawnPositionAvailable;
        [HideInInspector] private float[] spawnPositionTimer;
        [HideInInspector] private bool[] spawningStateOfSpawner;
        [HideInInspector] private int spawnMaxCD = 10;
        [SerializeField] private float waveSpawnPositionTimer;
        [SerializeField] private int waveSpawnMaxCD = 60;

        [SerializeField] private TMP_Text waveTimer;
        [SerializeField] private Image waveTimerFill;
        [SerializeField] private Color colorBeforeSpawnWave;
        public bool debugSpawningPerPosition = false;

        [SerializeField] private GameObject prefabSpawnTrail;
        private int enemyTypeNumber;

        static public AltarBehaviorComponent currentAltarBehavior = null;

        public GameObject tips2_CustomPass_Object = null;
        public void Awake()
        {
            m_gameLayer = this.GetComponent<GameLayer>();
            m_healthManager = this.GetComponent<HealthManager>();
            NavMesh.pathfindingIterationsPerFrame = 400;
            m_playerTranform = GameObject.Find("Player").transform;
#if UNITY_EDITOR
            playerInput = m_playerTranform.GetComponent<PlayerInput>();
            InputAction action = playerInput.actions.FindAction("SpawnEnemy");
            action.performed += InputSpawnSquad;
#endif

            TestReadDataSheet();
            state = new ObjectState();
            GameState.AddObject(state);
            m_enemyKillRatio = GetComponent<EnemyKillRatio>();
            m_cameraTransform = Camera.main.transform;
            gsm = m_cameraTransform.GetComponentInChildren<GlobalSoundManager>();
            m_cameraBehavior = m_cameraTransform.GetComponent<CameraBehavior>();
            m_characterUpgrade = m_playerTranform.GetComponent<Character.CharacterUpgrade>();
            m_dayController = GameObject.Find("DayController").gameObject.GetComponent<DayCyclecontroller>();
            m_serieController = m_playerTranform.GetComponent<SerieController>();

            m_terrainGenerator = FindAnyObjectByType<TerrainGenerator>();

            m_timeOfGame = 0;
            m_pullingSystem = GetComponent<EnemiesPullingSystem>();

            uiDispatcher = transform.parent.GetComponentInChildren<UIDispatcher>();
            waveSpawnPositionTimer = waveSpawnMaxCD - 10;
            enemyTypeNumber = System.Enum.GetValues(typeof(EnemyType)).Length;
            //if (m_uiManagerGameObject) m_UiEventManager = m_uiManagerGameObject.GetComponent<UI_EventManager>();
            //if(altarObject != null) { alatarRefScript = altarObject.GetComponent<AlatarHealthSysteme>(); }
        }

        public void Start()
        {
            playerInput.GetComponent<HealthPlayerComponent>().OnDamage += ResetCombot;
        }


        public void DebugInit()
        {
#if UNITY_EDITOR
            if (activeTestPhase)
                ActiveSpawnPhase(activeSpawnConstantDebug, EnemySpawnCause.DEBUG);
#endif

        }

        public void OnApplicationQuit()
        {
            GuerhoubaTools.LogSystem.Close();
        }

        public void Update()
        {

#if UNITY_EDITOR
            if (!activeTestPhase && DayCyclecontroller.choosingArtefactStart) return;
#else
            if (DayCyclecontroller.choosingArtefactStart) return;
#endif
            if (!GameState.IsPlaying()) return;
            repositionningCount = 0;

            m_timeOfGame += Time.deltaTime;
            remainEnemy = m_enemiesArray.Count;
            if (remainEnemy > 0)
            {
                //<size=130%>999 <voffset=0.2em> \n<size=100%>Remain
                //m_tmpTextEnemyRemain.text = "<size=130%>" + remainEnemy + "<voffset=0.2em> \n<size=100%>Remain";
            }
            else
            {
                //m_tmpTextEnemyRemain.text = "<size=130%>" + 0 + "<voffset=0.2em> \n<size=100%>Remain";
            }
            if (spawningPhase)
            {

                //m_maxUnittotal = (int)m_MaxUnitControl.Evaluate(TerrainGenerator.roomGeneration_Static + (3 - TerrainGenerator.staticRoomManager.eventNumber));
                //SpawnCooldown();
            }

            if (debugSpawningPerPosition)
            {
                //SpawnByUsingCorrupted();
                SpawnWaveByUsingCorrupted();
            }



        }

#if UNITY_EDITOR

        public void InputSpawnSquad(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                SpawEnemiesGroup(true);
            }
        }
#endif


        public void ChangePauseState(bool state)
        {
            for (int i = 0; i < m_enemiesArray.Count; i++)
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
                float distance = 200;
                Vector3 basePosition = new Vector3(0, 0, 0);
                //while (Mathf.Abs(distance) > 40)
                //{
                basePosition = m_playerTranform.transform.position + m_playerTranform.forward * m_offsetToSpawnCenter;
                basePosition += Vector3.up * m_upperStartPositionMagnitude;
                basePosition += GetRandomPosition();

                //}
                Vector3 v3Pos = basePosition;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(v3Pos, out hit, Mathf.Infinity, NavMesh.AllAreas))
                {
                    distance = hit.position.y - m_playerTranform.position.y;
                    if (distance < 0) { distance = -distance; }
                    if (distance < 40)
                    {
                        i = 25;
                        return hit.position;
                    }

                }

            }
            return Vector3.zero;
        }

        private Vector3 FindPositionAroundTarget(Transform targetTransform)
        {
            float magnitude = (targetTransform.position - m_cameraTransform.position).magnitude;
            for (int i = 0; i < 10; i++)
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
            if (/*m_characterMouvement.GetCurrentSpeed() > m_minimumSpeedToRepositing ||*/ repositionningCount >= repositionningLimit)
                return false;

            repositionningCount++;
            NpcHealthComponent tempNPCHealth = enemy.GetComponent<NpcHealthComponent>();
            Vector3 position = FindPositionAroundTarget(tempNPCHealth.targetData.target);
            Debug.Log("Repositioning");
            enemy.GetComponent<NavMeshAgent>().Warp(position);
            enemy.transform.position = position;
            tempNPCHealth.ResetTrail();
            return true;
        }
        private float GetTimeSpawn()
        {
            return (m_spawnTime + (m_spawnTime * ((Mathf.Sin(m_timeOfGame / 2.0f)) + 1.3f) / 2.0f));
        }

        private int GetNumberToSpawn()
        {
            //int currentMaxUnit = (int)Mathf.Lerp(m_minUnitPerGroup, (m_maxUnitPerGroup), m_enemyKillRatio.GetRatioValue());
            //int number = Mathf.FloorToInt((currentMaxUnit * ((Mathf.Sin(m_timeOfGame / 2.0f + 7.5f)) + 1.3f) / 2.0f));
            int currentMaxUnit = 5;
            int number = currentMaxUnit;
            number = number <= 0 ? 1 : number;
            return number;
        }



        private void SpawEnemiesGroup(bool isDebug = false)
        {
            if (!isDebug && isStopSpawn) return;
            position = FindPosition();
            posspawn.Add(position);

#if UNITY_EDITOR
            if (activeSpecialSquad)
            {
                SpawnSpecificSquad(position);
                return;
            }
#endif

            if (remainEnemy < RoomManager.enemyMaxSpawnInRoon)
            {
                for (int i = 0; i < m_groupEnemySize; i++)
                {
                    SpawnEnemyByPool(position + Random.insideUnitSphere * 5f, position);

                }
                InstantiateSpawnFeedback();
            }



        }

        public void SpawEnemiesGroupCustom(Vector3 positionCustom, int groupSize, bool isDebug = false)
        {
            if (!isDebug && isStopSpawn) return;
            position = FindPosition();
            posspawn.Add(positionCustom);


            for (int i = 0; i < groupSize; i++)
            {
                SpawnEnemyByPool(positionCustom + Random.insideUnitSphere * 5f, positionCustom);

            }
            InstantiateSpawnFeedback();



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

        private int FindValidTypeEnemyToSpawn()
        {
            int enemyIndex = -1;
            int countTentative = 0;
            while (countTentative < m_tryCountToSpawnEnemy)
            {
                countTentative++; 
                enemyIndex = Random.Range(0, enemyTypeNumber -1);

                if (!CanEnemySpawn(enemyIndex))
                {
                    enemyIndex = -1;
                    continue;
                }
                if (!m_pullingSystem.IsStillInstanceOf((EnemyType)enemyIndex))
                {
                    enemyIndex = -1;
                    continue;
                }
                break;

            }

            return enemyIndex;
        }

        private void InstantiateSpawnFeedback()
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, m_gameLayer.groundLayerMask))
            {

            }
            Instantiate(m_spawningVFX, hit.point + new Vector3(0, -18, 0), transform.rotation);
            GlobalSoundManager.PlayOneShot(37, position);
        }

        public GameObject SpawnBoss(Vector3 pos, EnemyType enemyType)
        {
            GameObject enemyObjectPull = null;
            NpcHealthComponent npcHealth = null;
            NpcMouvementComponent npcMove = null;
            NpcMetaInfos npcInfo = null;

            enemyObjectPull = m_pullingSystem.GetEnemy(enemyType);
            enemyTypeStats[(int)enemyType].instanceCount += 1;
            enemyTypeStats[(int)enemyType].instanceSpawnPerRoom += 1;
            enemyObjectPull.GetComponent<NavMeshAgent>().updatePosition = true;
            enemyObjectPull.GetComponent<NavMeshAgent>().Warp(pos);
            enemyObjectPull.GetComponent<NavMeshAgent>().enabled = true;

            npcInfo = enemyObjectPull.GetComponent<NpcMetaInfos>();
            npcInfo.manager = this;



            npcHealth = enemyObjectPull.GetComponent<NpcHealthComponent>();
            npcHealth.SetInitialData(m_healthManager, this);
            npcHealth.spawnMinute = (int)(m_timeOfGame / 60);
            npcHealth.targetData.isMoving = true;
            npcHealth.RestartObject(m_characterUpgrade.avatarUpgradeList.Count);
            npcHealth.SetTarget(m_playerTranform, m_playerTranform);

            npcMove = enemyObjectPull.GetComponent<NpcMouvementComponent>();
            npcMove.enabled = true;
            npcMove.enemiesManager = this;
            npcInfo.RestartEnemy();
            m_enemiesArray.Add(npcInfo);
            return enemyObjectPull;
        }

        private void SpawnEnemyByPool(Vector3 positionSpawn, Vector3 positionFromSpawn)
        {

            if (isStopSpawn) return;

            int enemyIndexChoose = FindValidTypeEnemyToSpawn();

            if (enemyIndexChoose == -1) return;

            GameObject enemyObjectPull = null;
            NpcHealthComponent npcHealth = null;
            NpcMouvementComponent npcMove = null;
            NpcMetaInfos npcInfo = null;

            ExperienceMouvement signExperienceMouvement = Instantiate(prefabSpawnTrail, positionFromSpawn, Quaternion.identity).GetComponent<ExperienceMouvement>();
            signExperienceMouvement.ActiveParticuleByPosition(positionSpawn);
            float delaySpawn = 3;

            while (delaySpawn > 0)
            {
                delaySpawn -= Time.deltaTime;
                Debug.Log("Delay remain : " + delaySpawn);
            }
            Debug.Log("Active spawn Mob");
            enemyObjectPull = m_pullingSystem.GetEnemy((EnemyType)enemyIndexChoose);
            enemyTypeStats[enemyIndexChoose].instanceCount += 1;
            enemyTypeStats[enemyIndexChoose].instanceSpawnPerRoom += 1;
            enemyObjectPull.transform.position = positionSpawn;
            NavMeshAgent tempNavMesh = enemyObjectPull.GetComponent<NavMeshAgent>();
            tempNavMesh.updatePosition = true;
            tempNavMesh.Warp(positionSpawn);
            tempNavMesh.enabled = true;

            npcInfo = enemyObjectPull.GetComponent<NpcMetaInfos>();
            npcInfo.RestartEnemy();
            npcInfo.manager = this;



            npcHealth = enemyObjectPull.GetComponent<NpcHealthComponent>();
            npcHealth.SetInitialData(m_healthManager, this);
            npcHealth.spawnMinute = (int)(m_timeOfGame / 60);
            npcHealth.targetData.isMoving = true;
            npcHealth.RestartObject(m_characterUpgrade.avatarUpgradeList.Count);
            npcHealth.SetTarget(m_playerTranform, m_playerTranform);
            countEnemySpawnMaximum++;

            npcMove = enemyObjectPull.GetComponent<NpcMouvementComponent>();
            npcMove.enabled = true;
            npcMove.enemiesManager = this;

            m_enemiesArray.Add(npcInfo);
        }

        private IEnumerator SpawnEnemyByPoolWithDelay(Vector3 positionSpawn, Vector3 positionFromSpawn)
        {

            if (isStopSpawn) yield break;

            int enemyIndexChoose = FindValidTypeEnemyToSpawn();

            if (enemyIndexChoose == -1) yield break;

            GameObject enemyObjectPull = null;
            NpcHealthComponent npcHealth = null;
            NpcMouvementComponent npcMove = null;
            NpcMetaInfos npcInfo = null;

            ExperienceMouvement signExperienceMouvement = Instantiate(prefabSpawnTrail, positionFromSpawn, Quaternion.identity).GetComponent<ExperienceMouvement>();
            signExperienceMouvement.ActiveParticuleByPosition(positionSpawn);
            yield return new WaitForSeconds(5);
            Debug.Log("Active spawn Mob");
            enemyObjectPull = m_pullingSystem.GetEnemy((EnemyType)enemyIndexChoose);
            enemyTypeStats[enemyIndexChoose].instanceCount += 1;
            enemyTypeStats[enemyIndexChoose].instanceSpawnPerRoom += 1;
            enemyObjectPull.transform.position = positionSpawn;
            NavMeshAgent tempNavMesh = enemyObjectPull.GetComponent<NavMeshAgent>();
            tempNavMesh.updatePosition = true;
            tempNavMesh.Warp(positionSpawn);
            tempNavMesh.enabled = true;

            npcInfo = enemyObjectPull.GetComponent<NpcMetaInfos>();
            npcInfo.RestartEnemy();
            npcInfo.manager = this;



            npcHealth = enemyObjectPull.GetComponent<NpcHealthComponent>();
            npcHealth.SetInitialData(m_healthManager, this);
            npcHealth.spawnMinute = (int)(m_timeOfGame / 60);
            npcHealth.targetData.isMoving = true;
            npcHealth.RestartObject(m_characterUpgrade.avatarUpgradeList.Count);
            npcHealth.SetTarget(m_playerTranform, m_playerTranform);
            countEnemySpawnMaximum++;

            npcMove = enemyObjectPull.GetComponent<NpcMouvementComponent>();
            npcMove.enabled = true;
            npcMove.enemiesManager = this;

            m_enemiesArray.Add(npcInfo);
        }
        public void SetSpawnSquad(int[] mobCount)
        {
            specialSquadSelect = mobCount;
            m_squadCount = 0;
            for (int i = 0; i < specialSquadSelect.Length; i++)
            {
                m_squadCount += specialSquadSelect[i];
            }
        }
        private void SpawnSpecificSquad(Vector3 positionSpawn)
        {
            for (int i = 0; i < specialSquadSelect.Length; i++)
            {
                for (int j = 0; j < specialSquadSelect[i]; j++)
                {
                    if (!m_pullingSystem.IsStillInstanceOf((EnemyType)i)) continue;

                    SpawnDirectEnemy(positionSpawn, i);
                }
            }
        }

        private void SpawnDirectEnemy(Vector3 position, int enemyType)
        {
            GameObject enemyObjectPull = null;
            NpcHealthComponent npcHealth = null;
            NpcMouvementComponent npcMove = null;
            NpcMetaInfos npcInfo = null;

            enemyObjectPull = m_pullingSystem.GetEnemy((EnemyType)enemyType);
            enemyTypeStats[enemyType].instanceCount += 1;
            enemyTypeStats[enemyType].instanceSpawnPerRoom += 1;
            enemyObjectPull.transform.position = position;
            enemyObjectPull.GetComponent<NavMeshAgent>().updatePosition = true;
            enemyObjectPull.GetComponent<NavMeshAgent>().Warp(position);

            npcInfo = enemyObjectPull.GetComponent<NpcMetaInfos>();
            npcInfo.manager = this;

            npcMove = enemyObjectPull.GetComponent<NpcMouvementComponent>();
            npcMove.enabled = true;
            npcMove.enemiesManager = this;

            npcHealth = enemyObjectPull.GetComponent<NpcHealthComponent>();
            npcHealth.SetInitialData(m_healthManager, this);
            npcHealth.spawnMinute = (int)(m_timeOfGame / 60);
            npcHealth.targetData.isMoving = true;
            npcHealth.RestartObject(m_characterUpgrade.avatarUpgradeList.Count);
            npcHealth.SetTarget(m_playerTranform, m_playerTranform);

            m_enemiesArray.Add(npcInfo);
        }


        private bool CanEnemySpawn(int enemyType)
        {
            float value = enemyTypeStats[enemyType].animationCurve.Evaluate(TerrainGenerator.roomGeneration_Static);
            int maxInstance = Mathf.RoundToInt(value);
            if (DayCyclecontroller.m_nightCountGlobal == 0)
            {
                if (enemyType < 2)
                {
                    bool canSpawn = enemyTypeStats[enemyType].instanceCount < maxInstance;
                    return canSpawn;
                }
                else
                {
                    bool canSpawn = false;
                    if (countEnemySpawnMaximum < RoomManager.enemyMaxSpawnInRoon / 2)
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < (maxInstance / 2);
                    }
                    else
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < maxInstance;
                    }
                    return canSpawn;


                }
            }
            else if (DayCyclecontroller.m_nightCountGlobal == 1)
            {
                if (enemyType < 2)
                {
                    bool canSpawn = enemyTypeStats[enemyType].instanceCount < maxInstance;
                    return canSpawn;
                }
                else
                {
                    bool canSpawn = false;
                    if (countEnemySpawnMaximum < RoomManager.enemyMaxSpawnInRoon / 2 && RoomManager.progress < 0.5f)
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < (maxInstance / 2);
                    }
                    else
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < maxInstance;
                    }
                    return canSpawn;


                }
            }
            else if (DayCyclecontroller.m_nightCountGlobal == 2)
            {
                if (enemyType < 2)
                {
                    bool canSpawn = enemyTypeStats[enemyType].instanceCount < maxInstance;
                    return canSpawn;
                }
                else
                {
                    bool canSpawn = false;
                    if (countEnemySpawnMaximum < RoomManager.enemyMaxSpawnInRoon / 3 && RoomManager.progress < 0.66f)
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < (maxInstance / 3);
                    }
                    else if (countEnemySpawnMaximum < RoomManager.enemyMaxSpawnInRoon / 2 && RoomManager.progress < 0.33f)
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < (maxInstance / 2);
                    }
                    else
                    {
                        canSpawn = enemyTypeStats[enemyType].instanceSpawnPerRoom < maxInstance;
                    }
                    return canSpawn;


                }
            }
            return true;

        }


        public void ActiveEvent(Transform target)
        {
            ActiveSpawnPhase(true, EnemySpawnCause.EVENT);
            ActiveMobAggro();
            m_targetList.Add(target.GetComponent<ObjectHealthSystem>());
            int indexTargetList = m_targetList.Count - 1;
            ObjectHealthSystem healthSystemReference = target.GetComponent<ObjectHealthSystem>();
            isStopSpawn = false;
            if (target.GetComponent<AltarBehaviorComponent>())
            {
                altarLaunch++;
                m_altarList.Add(target.GetComponent<AltarBehaviorComponent>());
                m_altarTransform.Add(target);
                lastAltarActivated = target.GetComponent<AltarBehaviorComponent>();
                currentAltarBehavior = lastAltarActivated;
                tips2_CustomPass_Object.SetActive(true);
            }

            //m_UiEventManager.SetupEventUI(healthSystemReference, indexTargetList);
        }

        public void ActiveMobAggro()
        {
            for (int i = 0; i < m_enemiesArray.Count; i++)
            {
                if (m_enemiesArray[i].state == NpcState.IDLE)
                {
                    m_enemiesArray[i].state = NpcState.MOVE;
                }
            }
        }
        public void DeactiveEvent(Transform target)
        {
            m_targetList.Remove(target.GetComponent<ObjectHealthSystem>());
            ActiveSpawnPhase(false, EnemySpawnCause.EVENT);
            ObjectHealthSystem healthSystem = target.GetComponent<ObjectHealthSystem>();
            healthSystem.ResetUIHealthBar();
            int indexTargetList = healthSystem.indexUIEvent;
            tips2_CustomPass_Object.SetActive(false);
            m_altarList.Remove(target.GetComponent<AltarBehaviorComponent>());
            m_altarTransform.Remove(target);
            lastAltarActivated = null;
            currentAltarBehavior = null;


        }

        public void IncreseAlterEnemyCount(NpcHealthComponent npcHealth)
        {
            AltarBehaviorComponent nearestAltar = FindClosestAltar(npcHealth.transform.position);
            if (nearestAltar != null)
            {
                nearestAltar.IncreaseKillCount();
            }
        }


        public void EnemyHasDied(NpcHealthComponent npcHealth, int xpCount)
        {

            NpcMetaInfos npcMetaInfos = npcHealth.GetComponent<NpcMetaInfos>();
            if (npcMetaInfos.type == EnemyType.TWILIGHT_SISTER)
            {
                m_terrainGenerator.currentRoomManager.bossRoom.EndRoomBoss();
            }

            Vector3 position = npcHealth.transform.position;
            //SpawnExp(position, xpCount, npcHealth.indexEnemy);
            IncreseAlterEnemyCount(npcHealth);
            float distance = Vector3.Distance(m_playerTranform.position, npcHealth.transform.position);
            OnDeathEvent(position, EntitiesTrigger.Enemies, npcHealth.gameObject, distance);
            OnDeathSimpleEvent();
        }

        public void TeleportEnemyOut(NpcMetaInfos npcInfos)
        {
            if (!m_enemiesArray.Contains(npcInfos)) return;

            EnemyType type = npcInfos.type;
            killCount++;
            m_enemyKillRatio.AddEnemiKill();

            if (m_enemiesFocusAltar.Contains(npcInfos))
                m_enemiesFocusAltar.Remove(npcInfos);

            enemyTypeStats[(int)type].instanceCount -= 1;
            m_enemiesArray.Remove(npcInfos);
            m_pullingSystem.ResetEnemy(npcInfos.gameObject, type);
        }



        public void ResetCombot(AttackDamageInfo attackDamageInfo)
        {
            if (comboCount > maxComboValue)
            {
                maxComboValue = comboCount;
            }

            comboCount = 0;
        }

        public void DeathEnemy()
        {
            m_serieController.RefreshSeries(false);
            comboCount++;
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

        private bool CanActiveSpawnPhase()
        {
            for (int i = 0; i < m_spawnCauseState.Length; i++)
            {
                if (m_spawnCauseState[i] == true)
                    return true;
            }
            return false;
        }
        public void ActiveSpawnPhase(bool state, EnemySpawnCause spawnCause)
        {
            m_spawnCauseState[(int)spawnCause] = state;
            if (CanActiveSpawnPhase() != spawningPhase)
            {
                ChangeSpawningPhase(!spawningPhase);
                if (spawningPhase == true) { StartCoroutine(m_cameraBehavior.DeZoomCamera()); }
                else { m_cameraBehavior.isZoomActive = true; }
            }
        }

        public void ChangeSpawningPhase(bool spawning)
        {
            spawningPhase = spawning;

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

            string filePath = Application.dataPath + "\\Progression Demo -SpawnSheet.csv";
#endif

            int lineNumber = 5;

            string lineContents = ReadSpecificLine(filePath, lineNumber);
            string[] data_values = lineContents.Split(',');
            long[] dataTransformed = new long[data_values.Length - 1];
            for (int i = 0; i < dataTransformed.Length; i++)
            {
                if (data_values[i] == "") continue;
                dataTransformed[i] = long.Parse(data_values[i]);
                tempAnimationCurve.AddKey(i, dataTransformed[i]);
                debugdata = debugdata + " , " + dataTransformed[i];

            }
            //m_MaxUnitControl = tempAnimationCurve;
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
            endInfoStats.roomCount = TerrainGenerator.roomGeneration_Static;
            endInfoStats.nightValidate = m_dayController.m_nightCount;
            endInfoStats.maxCombo = maxComboValue;

            CheckEndStat(endInfoStats);
            return endInfoStats;
        }

        public void AddDataInPool(NpcHealthComponent npcHealth)
        {

        }
        public void CheckEndStat(EndInfoStats stats)
        {
#if UNITY_EDITOR
            string filePath = Application.dataPath + "\\Temp" + fileStatsName + GameState.profileName + ".sost";
#else
            string filePath = Application.dataPath + fileStatsName + GameState.profileName + ".txt";
#endif
            if (!Directory.Exists(Application.dataPath + "\\Temp")) return;

            EndInfoStats statsSave = Save.SaveManager.ReadEndStats(filePath);
            if (statsSave.HasSuperiorValue(stats))
            {
                Save.SaveManager.WriteEndStats(filePath, stats);
            }

        }

        public void TestSaveFunction() //--- Temp 
        {
#if UNITY_EDITOR
            string filePath = Application.dataPath + "\\Temp" + fileStatsName + GameState.profileName + ".sost";
#else
            string filePath = Application.dataPath + fileStatsName + GameState.profileName + ".txt";
#endif
            if (!Directory.Exists(Application.dataPath + "\\Temp")) return;

            TestStruct testStruct = new TestStruct();
            testStruct.floatTest = .05f;
            testStruct.stringTest = "Phrase test";
            Save.SaveManager.TestStructSave(filePath, testStruct);

            TestStruct data;
            using (var file = File.OpenRead(filePath))
            {
                var reader = new BinaryFormatter();
                data = (TestStruct)reader.Deserialize(file); // Reads the entire list.
            }

        }



        #endregion

        public void OnValidate()
        {
            if (enemyTypeStats.Length <= 0) return;
            for (int i = 0; i < enemyTypeStats.Length; i++)
            {
                enemyTypeStats[i].type = (EnemyType)i;
            }
        }

        public void ResetAllSpawingPhasse()
        {
            for (int i = 0; i < m_spawnCauseState.Length; i++)
            {
                m_spawnCauseState[i] = false;
            }

        }


        public void DestroyAllEnemy()
        {

            // ActiveSpawnPhase(false, EnemySpawnCause.SHADOW);
            for (int i = 0; i < m_enemiesArray.Count; i++)
            {
                NpcMetaInfos npcHealth = m_enemiesArray[i];

                EnemyType type = npcHealth.GetComponent<NpcMetaInfos>().type;
                NpcHealthComponent healthComponent = npcHealth.GetComponent<NpcHealthComponent>();
                if (m_enemiesFocusAltar.Contains(npcHealth))
                    m_enemiesFocusAltar.Remove(npcHealth);

                enemyTypeStats[(int)type].instanceCount -= 1;
                if (healthComponent != null) { healthComponent.GetDestroy(Vector3.zero, 0); }

                m_pullingSystem.ResetEnemyNavMesh(npcHealth.gameObject, type);
            }
            //m_enemiesArray.Clear();
        }

        public bool GenerateDissonance()
        {
            bool canSpawnDissonance = false;
            int randomNumber = Random.Range(0, 100);
            if (randomNumber < enemyGenerateDissonanceProba.Evaluate(m_enemiesArray.Count))
            {
                canSpawnDissonance = true;
            }
            else
            {
                canSpawnDissonance = false;
            }
            return canSpawnDissonance;
        }

        public void GetDataSpawner(GameObject[] spawnerObject)
        {
            spawnPositionAvailable = new Vector3[spawnerObject.Length];
            spawnPositionTimer = new float[spawnerObject.Length];
            spawningStateOfSpawner = new bool[spawnerObject.Length];
            for (int i = 0; i < spawnPositionAvailable.Length; i++)
            {
                spawnPositionAvailable[i] = spawnerObject[i].transform.position;
                spawnPositionTimer[i] = Random.Range(0, spawnMaxCD);
                spawningStateOfSpawner[i] = true;
            }
            if (spawnPositionAvailable.Length > 0) { debugSpawningPerPosition = true; }
        }

        public void DesactiveSpawner(GameObject spawner)
        {
            for (int i = 0; i < spawnPositionAvailable.Length; i++)
            {
                if (spawnPositionAvailable[i] == spawner.transform.position)
                {
                    spawningStateOfSpawner[i] = false;
                }

            }
            RoomInfoUI.instance.UpdateRoomInfoDisplay(null, CountSpawnerActive());
        }

        public void SpawnByUsingCorrupted()
        {
            float addTime = Time.deltaTime;
            for (int i = 0; i < spawningStateOfSpawner.Length; i++)
            {
                if (spawningStateOfSpawner[i] == true)
                {
                    spawnPositionTimer[i] += addTime;
                    if (spawnPositionTimer[i] >= spawnMaxCD)
                    {
                        //CALL groupe spawn function en dehors du spawn normal
                        for (int j = 0; j < m_groupEnemySize; j++)
                        {

                            SpawnEnemyByPool(FindPositionAtSpawner(spawnPositionAvailable[i]), spawnPositionAvailable[i]);

                        }
                        InstantiateSpawnFeedback();
                        spawnPositionTimer[i] = 0;
                    }
                }
            }
        }

        public void SpawnWaveByUsingCorrupted()
        {
            float addTime = Time.deltaTime;
            int[] spawnersData = CountSpawnerActive();
            waveSpawnPositionTimer += Time.deltaTime * (spawnersData[1] - spawnersData[0]);
            if (waveSpawnPositionTimer > waveSpawnMaxCD)
            {
                for (int i = 0; i < spawningStateOfSpawner.Length; i++)
                {
                    if (spawningStateOfSpawner[i] == true)
                    {
                        for (int j = 0; j < m_groupEnemySize; j++)
                        {
                            //SpawnEnemyByPool(FindPositionAtSpawner(spawnPositionAvailable[i]), spawnPositionAvailable[i]);
                            StartCoroutine(SpawnEnemyByPoolWithDelay(FindPositionAtSpawner(spawnPositionAvailable[i]), spawnPositionAvailable[i]));
                        }
                        InstantiateSpawnFeedback();
                    }
                }
                waveSpawnPositionTimer = 0;
            }
            float secondeBeforeWave = (waveSpawnMaxCD - waveSpawnPositionTimer);
            float ratio = waveSpawnPositionTimer / waveSpawnMaxCD;
            Color color = Color.Lerp(Color.white, colorBeforeSpawnWave, ratio);
            waveTimer.color = color;
            waveTimerFill.fillAmount = 1 - ratio;
            //waveTimer.text = "" + (int)(waveSpawnPositionTimer / 60) + ":" + (secondeBeforeWave).ToString(".");
        }
        public Vector3 FindPositionAtSpawner(Vector3 position)
        {
            NavMeshHit hit;
            position += FindPositionAroundCorruptionSpawner(50, 125);
            if (NavMesh.SamplePosition(position, out hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                return hit.position;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public int[] CountSpawnerActive()
        {
            if (spawningStateOfSpawner == null)
                return new int[] { 0, 0 };

            int spawnerTotal = spawningStateOfSpawner.Length;
            int spawnerRemain = 0;
            for (int i = 0; i < spawnerTotal; i++)
            {
                if (spawningStateOfSpawner[i] == false) { spawnerRemain++; }
            }
            int[] dataSpawner = new int[] { spawnerRemain, spawnerTotal };
            return dataSpawner;
        }

        public Vector3 FindPositionAroundCorruptionSpawner(float rangeMin, float rangeMax)
        {
            Vector3 position = Vector3.zero;
            int randomPosition = UnityEngine.Random.Range(0, 4);
            if (randomPosition == 0)
            {
                position = new Vector3(UnityEngine.Random.Range(rangeMin, rangeMax), 0, UnityEngine.Random.Range(rangeMin, rangeMax));
            }
            else if (randomPosition == 1)
            {
                position = new Vector3(-UnityEngine.Random.Range(rangeMin, rangeMax), 0, UnityEngine.Random.Range(rangeMin, rangeMax));
            }
            else if (randomPosition == 2)
            {
                position = new Vector3(UnityEngine.Random.Range(rangeMin, rangeMax), 0, -UnityEngine.Random.Range(rangeMin, rangeMax));
            }
            else if (randomPosition == 3)
            {
                position = new Vector3(-UnityEngine.Random.Range(rangeMin, rangeMax), 0, -UnityEngine.Random.Range(rangeMin, rangeMax));
            }
            return position;
        }
    }


}




