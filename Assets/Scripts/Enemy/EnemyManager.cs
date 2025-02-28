


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
        private ObjectState state;
        [SerializeField] public Transform m_playerTranform;
        [SerializeField] public Transform m_basePlayerTransform;
        [SerializeField] private Transform m_cameraTransform;

        [SerializeField] private Vector3 m_offsetSpawnPos;
        [SerializeField] private Vector3 position;
        [SerializeField] private float m_spawnTime = 3.0f;

        [SerializeField] public int m_maxUnittotal = 400;
        [SerializeField] private int m_groupEnemySize = 5;
        [SerializeField] private AnimationCurve m_MaxUnitControl;
        static float currentMaxUnitValue;
        [SerializeField] private HealthManager m_healthManager;
        [SerializeField] private float m_radiusspawn;
        [SerializeField] private GameObject[] m_ExperiencePrefab = new GameObject[5];

        [Header("Enemy Spawn Parameters")]
        [SerializeField] private float m_minimumRadiusOfSpawn = 100;
        [SerializeField] private float m_maximumRadiusOfSpawn = 300;
        [SerializeField] private float m_offsetToSpawnCenter = 20.0f;
        [SerializeField] private float m_minimumSpeedToRepositing = 30.0f;
        private float m_upperStartPositionMagnitude = 50.0f;
        [SerializeField] private Transform m_enemyHolder;
        [SerializeField] private AnimationCurve enemyGenerateDissonanceProba;

        public ui_DisplayText m_mainInformationDisplay;

        public int countEnemySpawnMaximum;

        public Transform AstrePositionReference;

        [SerializeField] private CameraBehavior m_cameraBehavior;
        public void ResetSpawnStat()
        {
            for (int i = 0; i < enemyTypeStats.Length; i++)
            {
                enemyTypeStats[i].instanceSpawnPerRoom = 0;
            }
        }

        #region EnemyParameter

        [SerializeField] private EnemyTypeStats[] enemyTypeStats = new EnemyTypeStats[6];


        [Header("Enemy Bonus")]
        [SerializeField] private GameObject m_expBonus;
        [Range(0, 1.0f)][SerializeField] private float m_spawnRateExpBonus = 0.01f;
        #endregion

        private Experience_System m_experienceSystemComponent;

        private EnemyKillRatio m_enemyKillRatio;
        private float m_spawnCooldown;


        public List<NpcMetaInfos> m_enemiesArray = new List<NpcMetaInfos>();
        public List<NpcMetaInfos> m_enemiesFocusAltar = new List<NpcMetaInfos>();

        static public bool EnemyTargetPlayer = true;

        public Transform m_targetTranform;
        private ObjectHealthSystem m_targetScript;
        public List<Transform> m_targetTransformLists = new List<Transform>();
        private List<ObjectHealthSystem> m_targetList = new List<ObjectHealthSystem>();

        private List<Transform> m_altarTransform = new List<Transform>();
        private List<AltarBehaviorComponent> m_altarList = new List<AltarBehaviorComponent>();

        public bool spawningPhase = true;

        public GlobalSoundManager gsm;
        private List<Vector3> posspawn = new List<Vector3>();

        private Character.CharacterMouvement m_characterMouvement;
        [HideInInspector] public Character.CharacterUpgrade m_characterUpgrade;

        private int repositionningLimit = 2;
        private int repositionningCount;


        public DayCyclecontroller m_dayController;
        private float m_timeOfGame;

        private SerieController m_serieController;
        private TerrainGenerator m_terrainGenerator;

        [SerializeField] private GameObject m_spawningVFX;

        // Stats Variables
        [HideInInspector] public int altarLaunch;
        [HideInInspector] public int altarSuccessed;
        [HideInInspector] public int killCount;
        private const string fileStatsName = "\\Stats_data";
        private const int m_tryCountToSpawnEnemy = 10;

        [Header("Instruction UI")]
        [SerializeField] public TMP_Text m_Instruction;
        [SerializeField] public Image m_ImageInstruction;
        [SerializeField] public Sprite[] instructionSprite;
        [SerializeField] public Animator m_instructionAnimator;

        // Test Variable
#if UNITY_EDITOR
        [HideInInspector] public bool activeTestPhase;
        [HideInInspector] public bool activeSpawnConstantDebug = false;
        // Allow the enemis to spawn only a enemy type
        [HideInInspector] public bool activeSpecialSquad = false;

#endif
        [HideInInspector] public int[] specialSquadSelect;
        [HideInInspector] public PlayerInput playerInput;
        private int m_squadCount;
        // --------------------

        public delegate void OnDeath(Vector3 position, EntitiesTrigger tag, GameObject objectHit, float distance);
        public event OnDeath OnDeathEvent = delegate { };
        public delegate void OnDeathSimple();
        public event OnDeathSimple OnDeathSimpleEvent = delegate { };

        [SerializeField] private Animator detectionAnimator;
        [SerializeField] private Image m_enemyIcon;
        [SerializeField] private TMP_Text m_tmpTextEnemyRemain;
        [SerializeField] private Color[] colorSignUI = new Color[2];

        private EnemiesPullingSystem m_pullingSystem;

        public GameObject m_uiManagerGameObject;
        public UIDispatcher uiDispatcher;
        private UI_EventManager m_UiEventManager;

        private AltarBehaviorComponent lastAltarActivated;
        private List<Punketone> punketoneInvoked = new List<Punketone>();
        private int lastSkeletonCount;
        public int remainEnemy = 0;


        [HideInInspector] public bool isStopSpawn;
        // Spawn Cause Variable
        private bool[] m_spawnCauseState = new bool[4];


        private int comboCount;
        private int maxComboValue;
        [SerializeField] private LayerMask layerMaskGround;
        public void Awake()
        {
            NavMesh.pathfindingIterationsPerFrame = 400;
#if UNITY_EDITOR
            playerInput = m_playerTranform.GetComponent<PlayerInput>();
            InputAction action = playerInput.actions.FindAction("SpawnEnemy");
            action.performed += InputSpawnSquad;
#endif

            TestReadDataSheet();
            state = new ObjectState();
            GameState.AddObject(state);
            m_enemyKillRatio = GetComponent<EnemyKillRatio>();
            gsm = m_cameraTransform.GetComponentInChildren<GlobalSoundManager>();

            m_characterMouvement = m_playerTranform.GetComponent<Character.CharacterMouvement>();
            m_characterUpgrade = m_playerTranform.GetComponent<Character.CharacterUpgrade>();
            m_experienceSystemComponent = m_playerTranform.GetComponent<Experience_System>();
            m_dayController = GameObject.Find("DayController").gameObject.GetComponent<DayCyclecontroller>();
            m_serieController = m_playerTranform.GetComponent<SerieController>();

            m_terrainGenerator = FindAnyObjectByType<TerrainGenerator>();

            m_timeOfGame = 0;
            m_pullingSystem = GetComponent<EnemiesPullingSystem>();

            uiDispatcher = m_uiManagerGameObject.GetComponent<UIDispatcher>();
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

                m_maxUnittotal = (int)m_MaxUnitControl.Evaluate(TerrainGenerator.roomGeneration_Static + (3 - TerrainGenerator.staticRoomManager.eventNumber));
                SpawnCooldown();
            }



            //if (m_uiManagerGameObject && lastAltarActivated != null)
            //{
            //    lastSkeletonCount = lastAltarActivated.skeletonCount;
            //    if (lastSkeletonCount != punketoneInvoked.Count)
            //    {
            //        punketoneInvoked.Clear();
            //        for (int i = 0; i < lastSkeletonCount; i++)
            //        {
            //            punketoneInvoked.Add(lastAltarActivated.punketonHP[i]);
            //            m_UiEventManager.SetupUIBoss(i);
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < lastSkeletonCount; i++)
            //        {
            //            m_UiEventManager.UpdateUIBossLifebar(i, punketoneInvoked[i].percentHP, punketoneInvoked[i].currentHP);
            //        }
            //    }
            //
            //}
            //else
            //{
            //    if (punketoneInvoked.Count > 0)
            //    {
            //        for (int i = 0; i < lastSkeletonCount; i++)
            //        {
            //            m_UiEventManager.RemoveUIBoss(i);
            //        }
            //        punketoneInvoked.Clear();
            //    }
            //}

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


        public IEnumerator DisplayInstruction(string instruction, float time, Color colorText, Sprite iconSprite)
        {
            if (!m_Instruction)
                yield break;

            m_Instruction.color = colorText;
            m_Instruction.text = instruction;
            m_ImageInstruction.sprite = iconSprite;
            m_instructionAnimator.SetTrigger("DisplayInstruction");
            yield return new WaitForSeconds(time);
            m_instructionAnimator.ResetTrigger("DisplayInstruction");
        }
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
            Vector3 position = FindPositionAroundTarget(enemy.GetComponent<NpcHealthComponent>().targetData.target);
            enemy.GetComponent<NavMeshAgent>().Warp(position);
            enemy.transform.position = position;
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
                m_spawnCooldown = 0;
                return;
            }
#endif

            if (remainEnemy < RoomManager.enemyMaxSpawnInRoon)
            {
                for (int i = 0; i < m_groupEnemySize; i++)
                {
                    SpawnEnemyByPool(position + Random.insideUnitSphere * 5f);

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
                SpawnEnemyByPool(positionCustom + Random.insideUnitSphere * 5f);

            }
            InstantiateSpawnFeedback();



        }

        private void SpawnCooldown()
        {
            if (remainEnemy + m_groupEnemySize >= m_maxUnittotal && m_spawnCooldown > GetTimeSpawn()/2.0f)
            {
                return;
            }
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

        private int FindValidTypeEnemyToSpawn()
        {
            int enemyIndex = -1;
            int countTentative = 0;
            while (countTentative < m_tryCountToSpawnEnemy)
            {
                countTentative++;
                enemyIndex = Random.Range(0, 5);
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
            if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, layerMaskGround))
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
            npcHealth.SetTarget(m_playerTranform, m_basePlayerTransform);

            npcMove = enemyObjectPull.GetComponent<NpcMouvementComponent>();
            npcMove.enabled = true;
            npcMove.enemiesManager = this;
            npcInfo.RestartEnemy();
            m_enemiesArray.Add(npcInfo);
            return enemyObjectPull;
        }

        private void SpawnEnemyByPool(Vector3 positionSpawn)
        {

            if (isStopSpawn) return;

            int enemyIndexChoose = FindValidTypeEnemyToSpawn();

            if (enemyIndexChoose == -1) return;

            GameObject enemyObjectPull = null;
            NpcHealthComponent npcHealth = null;
            NpcMouvementComponent npcMove = null;
            NpcMetaInfos npcInfo = null;

            enemyObjectPull = m_pullingSystem.GetEnemy((EnemyType)enemyIndexChoose);
            enemyTypeStats[enemyIndexChoose].instanceCount += 1;
            enemyTypeStats[enemyIndexChoose].instanceSpawnPerRoom += 1;
            enemyObjectPull.transform.position = positionSpawn;
            enemyObjectPull.GetComponent<NavMeshAgent>().updatePosition = true;
            enemyObjectPull.GetComponent<NavMeshAgent>().Warp(positionSpawn);
            enemyObjectPull.GetComponent<NavMeshAgent>().enabled = true;

            npcInfo = enemyObjectPull.GetComponent<NpcMetaInfos>();
            npcInfo.RestartEnemy();
            npcInfo.manager = this;



            npcHealth = enemyObjectPull.GetComponent<NpcHealthComponent>();
            npcHealth.SetInitialData(m_healthManager, this);
            npcHealth.spawnMinute = (int)(m_timeOfGame / 60);
            npcHealth.targetData.isMoving = true;
            npcHealth.RestartObject(m_characterUpgrade.avatarUpgradeList.Count);
            npcHealth.SetTarget(m_playerTranform, m_basePlayerTransform);
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
            npcHealth.SetTarget(m_playerTranform, m_basePlayerTransform);

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


        public void SendInstruction(string Instruction, Color colorText, Sprite iconAssociate)
        {
            StartCoroutine(DisplayInstruction(Instruction, 2, colorText, iconAssociate));
        }


        public void ActiveEvent(Transform target)
        {
            ActiveSpawnPhase(true, EnemySpawnCause.EVENT);

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
            }

            //m_UiEventManager.SetupEventUI(healthSystemReference, indexTargetList);
        }

        public void DeactiveEvent(Transform target)
        {
            m_targetList.Remove(target.GetComponent<ObjectHealthSystem>());
            ActiveSpawnPhase(false, EnemySpawnCause.EVENT);
            ObjectHealthSystem healthSystem = target.GetComponent<ObjectHealthSystem>();
            healthSystem.ResetUIHealthBar();
            int indexTargetList = healthSystem.indexUIEvent;

            m_altarList.Remove(target.GetComponent<AltarBehaviorComponent>());
            m_altarTransform.Remove(target);
            lastAltarActivated = null;

            //m_UiEventManager.RemoveEventUI(indexTargetList);
        }


        public void SpawnExp(Vector3 position, int count, int indexMob)
        {
            for (int i = 0; i < count; i++)
            {

            }
            GameObject expObj = Instantiate(m_ExperiencePrefab[indexMob], position, Quaternion.identity);
            ExperienceMouvement experienceMouvement = expObj.GetComponent<ExperienceMouvement>();
            m_experienceSystemComponent.AddExpParticule(experienceMouvement);

            float rate = Random.Range(0.0f, 1.0f);
            if (rate <= m_spawnRateExpBonus)
            {
                Instantiate(m_expBonus, position, Quaternion.identity);

            }
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
            //if (detectionAnimator) detectionAnimator.SetBool("ShadowDetection", spawningPhase);
            if (spawning)
            {
                //gsm.globalMusicInstance.setParameterByName("Repos", 0);
                //gsm.StartCoroutine(gsm.UpdateParameterWithDelay(3, "Intensity", 13, 1,"TransitionIntensity"));
                gsm.UpdateParameter(1.5f, "Intensity");
                StartCoroutine(DisplayInstruction("Corrupt spirit appears", 2, Color.white, instructionSprite[0]));
                //m_enemyIcon.color = colorSignUI[0];
                //m_tmpTextEnemyRemain.color = Color.Lerp(colorSignUI[0], Color.red, 0.5f); ;
            }
            else
            {
                //gsm.globalMusicInstance.setParameterByName("Repos", 1);
                //gsm.StartCoroutine(gsm.UpdateParameterWithDelay(0.1f, "Intensity", 13, 2, "TransitionIntensity"));
                gsm.UpdateParameter(0.1f, "Intensity");
                StartCoroutine(DisplayInstruction("Corrupt spirit stop appears", 2, Color.white, instructionSprite[1]));
                //m_enemyIcon.color = colorSignUI[1];
                //m_tmpTextEnemyRemain.color = colorSignUI[1];
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
    }


}




