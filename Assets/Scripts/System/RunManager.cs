using BorsalinoTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames
{
    public enum DayStep
    {
        DAY = 0,
        NIGHT =1,
        BOSS =2,
    }

    public class RunManager : MonoBehaviour
    {
        public static RunManager instance;

        [Header("Day Variables")]
        public int maxHoursPointPerDay =0 ;
        public int currentHoursPoint { get; private set; }
        public Action OnRemoveHoursPoint;
        public Action<int> OnChangeHoursPoint;

        [Header("Night Variables")]
        public float nightDurationSeconds = 0;
        public float currentNightTimeCountdown { get { return currentNightTimeCountdown; } private set { } }

        [Header("Dissonance Heart")]
        public int dissonanceHeartBroken = 0;
        public Action<int> OnDissonanceHeartBroken;

        [Header("Day General Variable")]
        // Variable Day step
        public DayStep dayStep;
        public int dayCount = 0;
        public Action OnDayStart;
        public Action OnNightStart;
        public Action OnBossStart;

        [Header("Debug variables")]
        [SerializeField] private bool m_isRunManagerDebugActive = false;

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
            StartDay();
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

          if(m_isRunManagerDebugActive)
                ScreenDebuggerTool.AddMessage("Remove point. Current point " +  currentHoursPoint);
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
            if (dayStep != DayStep.NIGHT) return;

            currentNightTimeCountdown -= Time.deltaTime;
            if (currentNightTimeCountdown <= 0)
            {
                dayStep = DayStep.BOSS;
                StartBoss();
            }
        }


        public void StartBoss()
        {
            OnBossStart?.Invoke();

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
