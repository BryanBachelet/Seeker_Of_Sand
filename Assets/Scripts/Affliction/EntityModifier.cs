using GuerhoubaGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using Enemies;
using System;


public class EntityModifier : MonoBehaviour
{
    private float m_tickDurationBetweenDot = 0.5f;
    private float m_timerTickDot;

    [HideInInspector] public int bleedingDamage = 0;
    [HideInInspector] public float multiplierPercentBleedingDamage = 0.0f;

    [HideInInspector] public int burningDamage = 0;
    [HideInInspector] public float multiplierPercentBurningDamage = 0.0f;

    [HideInInspector] public int effectRemovePerHit;
    [HideInInspector] public float tickDurationBlazingDamage = 0;
    private float m_timerTickBlaze = 0;

    [HideInInspector] public int poisonDamage = 0;
    [HideInInspector] public float multiplierPercentPoisonDamage = 0.0f;

    [HideInInspector] public float intoxicateExecuteThreshold = 0.0f;

    [HideInInspector] public float slownessPercent = 0.0f;

    [HideInInspector] public float electrifyPercent = 0.0f;
    [HideInInspector] public float damageTakenIncrease = 1.0f;


    [HideInInspector] public bool isFreeze;


    //Check the entity movement 
    private Vector3 m_prevPosition;
    private float m_distanceCounter;
    [HideInInspector] public float distanceProcBleeding = 50.0f;


    [HideInInspector] private IDamageReceiver m_damageReceiver;
    [HideInInspector] private AfflictionManager m_afflictionManager;


    public Action OnStartFreeze;
    public Action EndFreeze;

    [Header("Debug Variables and Infos")]
    public bool m_activeEnityModifierDebug = false;


    #region Unity Functions
    public void Start()
    {
        InitComponent();
    }

    public void Update()
    {
        if (m_damageReceiver.IsDead()) return;

        UpdateDamageOverTime();
        CheckEntityMovement();
        ApplyBlazeEffect();
        ApplyIntoxicateEffect();
    }

    private void ApplyIntoxicateEffect()
    {
       if(m_damageReceiver.GetLifeRatio() < intoxicateExecuteThreshold)
        {
            DamageStatData damageStatData = new DamageStatData();
            damageStatData.characterObjectType = CharacterObjectType.AFFLICTION;
            damageStatData.element = GameElement.NONE;
            damageStatData.damage = (m_damageReceiver.GetLastingLife());

            m_damageReceiver.ReceiveDamage("Execute intoxicate affliction", damageStatData, Vector3.zero, 0, -1, 0);

        }
    }

    #endregion



    private void InitComponent()
    {
        m_damageReceiver = GetComponent<IDamageReceiver>();
        m_afflictionManager = GetComponent<AfflictionManager>();
    }

    public void ActiveBleedingState()
    {
        m_prevPosition = transform.position;
    }



    private void ApplyBlazeEffect()
    {
        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.BLAZE)) return;

        m_timerTickBlaze += Time.deltaTime;

        if (m_timerTickBlaze > tickDurationBlazingDamage)
        {
            m_timerTickBlaze = 0;
            ApplyBurningDamage();
            int stackRemaining = m_afflictionManager.RemoveStack(AfflictionType.BURN, effectRemovePerHit);
            if (stackRemaining == 0)
            {
                m_afflictionManager.RemoveAffliction(AfflictionType.BLAZE);
                m_afflictionManager.RemoveAffliction(AfflictionType.BURN);
            }

            if (m_activeEnityModifierDebug)
            {
                Debug.Log("Affliction : Blaze is effect apply ");
            }
        }
    }

    private void UpdateDamageOverTime()
    {
        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.LACERATION)
            && !m_afflictionManager.IsCarryAffliction(AfflictionType.BLEEDING)
            && !m_afflictionManager.IsCarryAffliction(AfflictionType.BURN))
            return;

        m_timerTickDot += Time.deltaTime;

        if (m_timerTickDot > m_tickDurationBetweenDot)
        {
            m_timerTickDot = 0;
            ApplyDamageOverTime();
            ApplyBurningDamage();
            ApplyPoisonDamage();
        }
    }

    private void CheckEntityMovement()
    {

        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.BLEEDING))
            return;

        float distance = Vector3.Distance(m_prevPosition, transform.position);
        m_prevPosition = transform.position;
        m_distanceCounter += distance;

        if (m_distanceCounter > distanceProcBleeding)
        {
            m_distanceCounter = 0;
            ApplyBleedingDamage();

            if (m_activeEnityModifierDebug)
            {
                Debug.Log("Affliction : Bleeding distance has been covered" + bleedingDamage);
            }
        }
    }

    private void ApplyDamageOverTime()
    {
        ApplyBleedingDamage();
    }

    private void ApplyBleedingDamage()
    {
        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.LACERATION)
            && !m_afflictionManager.IsCarryAffliction(AfflictionType.BLEEDING))
            return;

        DamageStatData damageStatData = new DamageStatData();
        damageStatData.characterObjectType = CharacterObjectType.AFFLICTION;
        damageStatData.element = GameElement.NONE;
        damageStatData.damage = Mathf.RoundToInt(bleedingDamage * (1.0f + multiplierPercentBleedingDamage));

        m_damageReceiver.ReceiveDamage("Lacerate affliction", damageStatData, Vector3.zero, 0, -1, 0);

        if (m_activeEnityModifierDebug)
        {
            Debug.Log("Affliction : Lacerate Damage apply " + bleedingDamage);
        }
    }

    private void ApplyBurningDamage()
    {
        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.BURN) && m_afflictionManager.IsCarryAffliction(AfflictionType.BLAZE))
            return;

        DamageStatData damageStatData = new DamageStatData();
        damageStatData.characterObjectType = CharacterObjectType.AFFLICTION;
        damageStatData.element = GameElement.FIRE;
        damageStatData.damage = Mathf.RoundToInt(burningDamage * (1.0f + multiplierPercentBurningDamage));

        m_damageReceiver.ReceiveDamage("Burning affliction", damageStatData, Vector3.zero, 0, (int)GameElement.FIRE, 0);

        if (m_activeEnityModifierDebug)
        {
            Debug.Log("Affliction : Burning Damage apply " + bleedingDamage);
        }
    }

    private void ApplyPoisonDamage()
    {
        if (!m_afflictionManager.IsCarryAffliction(AfflictionType.POISON))
            return;

        DamageStatData damageStatData = new DamageStatData();
        damageStatData.characterObjectType = CharacterObjectType.AFFLICTION;
        damageStatData.element = GameElement.NONE;
        damageStatData.damage = Mathf.RoundToInt(poisonDamage * (1.0f + multiplierPercentPoisonDamage));

        m_damageReceiver.ReceiveDamage("Poison affliction", damageStatData, Vector3.zero, 0, -1, 0);

        if (m_activeEnityModifierDebug)
        {
            Debug.Log("Affliction : Poison Damage apply " + bleedingDamage);
        }
    }

    public bool IsObjectifTarget() { return m_damageReceiver.IsObjectifTarget(); }

    
    public void ApplyFreeze()
    {
        isFreeze = true;
        OnStartFreeze?.Invoke();
    }

    public void FinishFreeze()
    {
        isFreeze = false;
        EndFreeze?.Invoke();
    }



    #region Get Functions
    public float GetDamageIncreasePercent()
    {
        return damageTakenIncrease + electrifyPercent;
    }
    #endregion
}
