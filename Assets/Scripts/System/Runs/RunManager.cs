using BorsalinoTools;
using GuerhoubaGames.GameEnum;
using JetBrains.Annotations;
using SeekerOfSand.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames
{
    public enum DayStep
    {
        DAY = 0,
        NIGHT = 1,
        BOSS = 2,
    }

    public class RunManager : MonoBehaviour
    {
        public static RunManager instance;

        [Header("Day Variables")]
        public int maxHoursPointPerDay = 0;
        public int currentHoursPoint { get; private set; }
        public Action OnRemoveHoursPoint;
        public Action<int> OnChangeHoursPoint;

        [Header("Night Variables")]
        public float nightDurationSeconds = 0;
        public float currentNightTimeCountdown { get; private set; }

        [Header("Dissonance Heart")]
        public int dissonanceHeartBroken = 0;
        public Action<int> OnDissonanceHeartBroken;

        [Header("Day General Variable")]
        // Variable Day step
        public DayStep dayStep;
        public int dayCount = 0;
        public int maxRoomPerDay = 5;
        [HideInInspector] public int countRoomDone = 0;
        public DayTimeController m_daytimecontroller;



        [HideInInspector] public bool isNightCountdownStarted = false;
        public Action OnDayStart;
        public Action OnNightStart;
        public Action OnNightCountdownStart;
        public Action OnBossStart;

        private TerrainGenerator m_terrainGeneratorComponent;

        [Header("Reward Variables")]
        public GameObject rewardLootPrefab;

        [Header("Merchant Variables")]
        public GameObject merchandCaravanaPrefab;
        [SerializeField] private int m_hoursSpendToSpawnMerchant = 0;


        [Header("Important Object")]
        public GameObject playerInstance;


        [Header("Tracking Stats")]
        public int eventDone;


        [Header("Debug variables")]
        [SerializeField] private bool m_isRunManagerDebugActive = false;
        [SerializeField] private bool m_fastDay = false;


        #region Unity Functions
        public void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Debug.LogError("Double " + this.name.ToString() + " present in  the scene");
        }

        public void Start()
        {
            m_terrainGeneratorComponent = TerrainGenerator.instance;
            if (m_fastDay)
            {
                maxHoursPointPerDay = 3;
            }
            if (dayStep == DayStep.DAY) StartDay();

        }

        public void Update()
        {
            UpdateNight();
        }
        #endregion

        public void StartDay()
        {
            currentHoursPoint = maxHoursPointPerDay;
            OnDayStart?.Invoke();
            OnChangeHoursPoint?.Invoke(currentHoursPoint);
            dissonanceHeartBroken = 0;
            dayCount++;
            countRoomDone = 0;
            m_daytimecontroller.StartDay();
            m_daytimecontroller.StartDay(); 
            if (m_isRunManagerDebugActive)
                ScreenDebuggerTool.AddMessage("Day Start");
        }

        public void RemoveHourPoint()
        {
            currentHoursPoint--;
            OnRemoveHoursPoint?.Invoke();
            OnChangeHoursPoint?.Invoke(currentHoursPoint);
            if (currentHoursPoint == 0)
            {
                dayStep = DayStep.NIGHT;
                StartNight();
            }

            if (m_isRunManagerDebugActive)
                ScreenDebuggerTool.AddMessage("Remove point. Current point " + currentHoursPoint);
        }



        public void StartNight()
        {
            OnNightStart?.Invoke();
            currentNightTimeCountdown = nightDurationSeconds;

            if (m_isRunManagerDebugActive)
                ScreenDebuggerTool.AddMessage("Night Start");
        }

        public void UpdateNight()
        {
            if (dayStep != DayStep.NIGHT || !isNightCountdownStarted || !GameState.IsPlaying()) return;

            ScreenDebuggerTool.AddMessage("Night Time " + currentNightTimeCountdown.ToString(), 10);
            currentNightTimeCountdown -= Time.deltaTime;
            if (currentNightTimeCountdown <= 0)
            {
                dayStep = DayStep.BOSS;
                
                StartBossPhase(true);
                m_daytimecontroller.ActiveBoss();
            }
        }

        public void LaunchNightCountdown()
        {
            if (dayStep != DayStep.NIGHT) return;

            OnNightCountdownStart?.Invoke();
            isNightCountdownStarted = true;
            m_daytimecontroller.StartNight();
            currentNightTimeCountdown = nightDurationSeconds;
        }

        public void StartBossPhase(bool isNightEnd)
        {
            OnBossStart?.Invoke();
            isNightCountdownStarted = false;

            if (isNightEnd)
            {
                m_terrainGeneratorComponent.GenerateBossMap(true);
                m_terrainGeneratorComponent.SelectTerrain(0);
                m_daytimecontroller.currentHour = 3;
                m_daytimecontroller.currentPhase = 0;
            }

            if (m_isRunManagerDebugActive)
                ScreenDebuggerTool.AddMessage("Boss Start");
        }

        public static bool IsNight()
        {
            return instance._IsNight();
        }
        private bool _IsNight()
        {
            return dayStep == DayStep.NIGHT;
        }

        public void BrokeDissonanceHeart()
        {
            dissonanceHeartBroken++;
            OnDissonanceHeartBroken?.Invoke(dissonanceHeartBroken);
        }


        #region Player Functions

        public static Vector3 GetPlayerPosition()
        {
            return instance.playerInstance.transform.position;
        }

        public static Quaternion GetPlayerRotation()
        {
            return instance.playerInstance.transform.rotation;
        }

        public static GameObject GetPlayerObject()
        {
            return instance.playerInstance;
        }

        public static Transform GetPlayerTransform()
        {
            return instance.playerInstance.transform;
        }

        #endregion


        #region Reward Functions
        public static GameObject SpawnReward(RewardType rewardType, Vector3 position, GameElement elementReward)
        {
            return instance._SpawnReward(rewardType, position, elementReward);
        }

        public static GameObject SpawnRandomReward(Vector3 position)
        {
            return instance._SpawnRandomReward(position);
        }

        private GameObject _SpawnReward(RewardType rewardType, Vector3 position, GameElement elementReward)
        {
            GameObject instance = GameObject.Instantiate(rewardLootPrefab, position, Quaternion.identity);

            RewardInteraction rewardInteraction = instance.GetComponent<RewardInteraction>();
            rewardInteraction.rewardElement = elementReward;
            rewardInteraction.rewardType = rewardType;

            return instance;
        }

        private GameObject _SpawnRandomReward(Vector3 position, bool includeSpell = false)
        {
            GameObject instance = GameObject.Instantiate(rewardLootPrefab, position, Quaternion.identity);

            RewardType rewardType = RewardType.MERCHANT;
            if (includeSpell)
            {
                rewardType = (RewardType)UnityEngine.Random.Range(0, 3);
            }
            else
            {
                int indexReward = UnityEngine.Random.Range(0, 1);
                rewardType = indexReward == 1 ? RewardType.ARTEFACT : RewardType.UPGRADE;
            }
            GameElement elementReward = GeneralTools.GetRandomBaseElement();

            RewardInteraction rewardInteraction = instance.GetComponent<RewardInteraction>();
            rewardInteraction.rewardElement = elementReward;
            rewardInteraction.rewardType = rewardType;

            return instance;
        }

        #endregion

        #region Merchand Functions

        public static bool IsSpawningMerchantValid()
        {
            return instance._IsSpawingMerchandValid();
        }

        private bool _IsSpawingMerchandValid()
        {
            return maxHoursPointPerDay - currentHoursPoint == m_hoursSpendToSpawnMerchant;
        }

        public static GameObject SpawnMerchand(Vector3 position, Quaternion rotation)
        {
            return instance._SpawnMerchant(position, rotation);
        }

        private GameObject _SpawnMerchant(Vector3 position, Quaternion rotation)
        {
            GameObject instance = GameObject.Instantiate(merchandCaravanaPrefab, position, rotation);
            return instance;
        }

        #endregion

        #region AstresMoving

        #endregion
    }
}
