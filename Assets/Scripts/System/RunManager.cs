using BorsalinoTools;
using JetBrains.Annotations;
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


        [HideInInspector] public bool isNightCountdownStarted = false;
        public Action OnDayStart;
        public Action OnNightStart;
        public Action OnBossStart;

        private TerrainGenerator m_terrainGeneratorComponent;

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
            if (dayStep != DayStep.NIGHT || !isNightCountdownStarted) return;

            ScreenDebuggerTool.AddMessage("Night Time " + currentNightTimeCountdown.ToString(), 10);
            currentNightTimeCountdown -= Time.deltaTime;
            if (currentNightTimeCountdown <= 0)
            {
                dayStep = DayStep.BOSS;
                StartBossPhase(true);
            }
        }

        public void LaunchNightCountdown()
        {
            if (dayStep != DayStep.NIGHT) return;

            isNightCountdownStarted = true;
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
            }

            if (m_isRunManagerDebugActive)
                ScreenDebuggerTool.AddMessage("Boss Start");
        }

        public void BrokeDissonanceHeart()
        {
            dissonanceHeartBroken++;
            OnDissonanceHeartBroken?.Invoke(dissonanceHeartBroken);
        }


    }
}
