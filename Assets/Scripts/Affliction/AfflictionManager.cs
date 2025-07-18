using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AfflictionManager : MonoBehaviour
{
    [SerializeField] private AfflictionProfil m_afflictionProfil;
    public Affliction[] afflictionArray;
    [Header("Debug Infos")]
    public bool m_activeAfflictionInfo;


    private EntityModifier m_entityModifier;
    [HideInInspector] public AfflictionData[] afflictionData = new AfflictionData[12];
    #region Unity Functions
    public void Start()
    {
        InitializeAfflictionArray();
    }

    public void Update()
    {
        UpdateAffliction();
    }

    #endregion

  
    private void OnInit()
    {
        afflictionData[0] = m_afflictionProfil.lacerateData;
        afflictionData[1] = m_afflictionProfil.bleedingData;
        afflictionData[2] = m_afflictionProfil.burnData;
        afflictionData[3] = m_afflictionProfil.blazeData;
        afflictionData[4] = m_afflictionProfil.chillData;
        afflictionData[5] = m_afflictionProfil.freezeData;
        afflictionData[6] = m_afflictionProfil.poisonData;
        afflictionData[7] = m_afflictionProfil.intoxicateData;
        afflictionData[8] = m_afflictionProfil.electrifyData;
        afflictionData[9] = m_afflictionProfil.electrocuteData;
        afflictionData[10] = m_afflictionProfil.scareData;
        afflictionData[11] = m_afflictionProfil.terrifyData;
    }
    public AfflictionData GetAfflictionData(AfflictionType type) { return afflictionData[(int)(type) - 1]; }
    public void InitializeAfflictionArray()
    {
        OnInit();



        m_entityModifier = GetComponent<EntityModifier>();

        int arrayLength = Enum.GetNames(typeof(AfflictionType)).Length;
        afflictionArray = new Affliction[arrayLength - 1];
    }

    public void UpdateAffliction()
    {
        for (int i = 0; i < afflictionArray.Length; i++)
        {
            if (afflictionArray[i] == null || afflictionArray[i].type == AfflictionType.NONE) continue;

            afflictionArray[i].duration -= Time.deltaTime;
            if (m_activeAfflictionInfo)
            {
                Debug.Log("Affliction  " + afflictionArray[i].type + " is update");
            }
            if (afflictionArray[i].duration <= 0)
            {
                RemoveAffliction(afflictionArray[i].type);
            }
        }

    }

    public void AddAfflictions(AfflictionType[] afflictionTypes)
    {
        for (int i = 0; i < afflictionTypes.Length; i++)
        {

            Affliction affliction = new Affliction();
            affliction.type = afflictionTypes[i];
            affliction.duration = GetAfflictionData(affliction.type).duration;
            affliction.stackCount = 1;
            AddAffliction(affliction);
            if (m_activeAfflictionInfo)
            {
                Debug.Log("Affliction : Add " + affliction.type);
            }
        }


    }


    public void AddAffliction(Affliction affliction)
    {
        Affliction currentAffliction = afflictionArray[(int)affliction.type];

        if (currentAffliction == null)
        {
            currentAffliction = new Affliction();
            currentAffliction.type = affliction.type;
            currentAffliction.duration = affliction.duration;
            currentAffliction.stackCount += affliction.stackCount;

        }
        else
        {
            currentAffliction.type = affliction.type;
            currentAffliction.duration = affliction.duration;
            currentAffliction.stackCount += affliction.stackCount;
        }

        currentAffliction = TranformAffliction(currentAffliction);
        afflictionArray[(int)affliction.type] = currentAffliction;
        if (m_activeAfflictionInfo)
        {
            Debug.Log("Affliction : Set " + affliction.type);
        }
        UpdateEntityModiferAdd(currentAffliction);
        // 4. Update UI and feedback

    }

    private Affliction TranformAffliction(Affliction currentAffliction)
    {
        if (currentAffliction.stackCount < GetAfflictionData((currentAffliction.type)).stackToTranform)
            return currentAffliction;


        switch (currentAffliction.type)
        {

            case AfflictionType.LACERATION:

                Affliction bleedingAffliction = new Affliction();
                bleedingAffliction.type = AfflictionType.BLEEDING;
                bleedingAffliction.duration = GetAfflictionData(AfflictionType.BLEEDING).duration;
                bleedingAffliction.stackCount = 1;
                AddAffliction(bleedingAffliction);
                return currentAffliction;
            break;

            case AfflictionType.BURN:

                Affliction blazeAffliction = new Affliction();
                blazeAffliction.type = AfflictionType.BLAZE;
                blazeAffliction.duration = GetAfflictionData(AfflictionType.BLAZE).duration;
                blazeAffliction.stackCount = 1;
                AddAffliction(blazeAffliction);
                return currentAffliction;

                break;
            case AfflictionType.POISON:

                Affliction intoxicateAffliction = new Affliction();
                intoxicateAffliction.type = AfflictionType.INTOXICATE;
                intoxicateAffliction.duration = GetAfflictionData(AfflictionType.INTOXICATE).duration;
                intoxicateAffliction.stackCount++;
                RemoveStack(AfflictionType.POISON, GetAfflictionData((currentAffliction.type)).stackToTranform);
                AddAffliction(intoxicateAffliction);
                return currentAffliction;

            break;

            default:
                break;
        }

        return currentAffliction;
    }

    private void UpdateEntityModiferAdd(Affliction affliction)
    {
        switch (affliction.type)
        {
            case (AfflictionType.LACERATION):
                m_entityModifier.bleedingDamage = m_afflictionProfil.lacerateData.damagerPerStack * affliction.stackCount;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply lacerate affliction stats");
                break;

            case AfflictionType.BLEEDING:
                
                m_entityModifier.multiplierPercentBleedingDamage = m_afflictionProfil.bleedingData.increaseDamageBleeding;
                m_entityModifier.distanceProcBleeding = m_afflictionProfil.bleedingData.movementQuantityToReduceHitCooldown;
                m_entityModifier.ActiveBleedingState();

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply bleeding affliction stats");
                break;

            case AfflictionType.BURN:
                m_entityModifier.burningDamage = m_afflictionProfil.burnData.damagePerStack * affliction.stackCount;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply burning affliction stats");
                break;

            case AfflictionType.BLAZE:
                m_entityModifier.effectRemovePerHit = m_afflictionProfil.blazeData.effectRemovePerDamage ;
                m_entityModifier.multiplierPercentBurningDamage = m_afflictionProfil.blazeData.damageIncreasePercentage ;
                m_entityModifier.tickDurationBlazingDamage = m_afflictionProfil.blazeData.timeBetweenTick ;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply blaze affliction stats");
                break;
            case AfflictionType.POISON:

                m_entityModifier.poisonDamage = m_afflictionProfil.poisonData.dammagePoisonPerStack * affliction.stackCount;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Poison affliction stats");

                break;

            case AfflictionType.INTOXICATE:

                m_entityModifier.multiplierPercentPoisonDamage = m_afflictionProfil.intoxicateData.multiplerPoisonDamage;
                m_entityModifier.intoxicateExecuteThreshold = m_afflictionProfil.intoxicateData.exectutionPercentPerStack * affliction.stackCount;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Intoxicate affliction stats");

                break;
        }

    }

    public void RemoveAffliction(AfflictionType type)
    {
        afflictionArray[(int)type].stackCount = 0;
        afflictionArray[(int)type].duration = 0;
        UpdateEntityModiferRemove(afflictionArray[(int)type]);
        afflictionArray[(int)type].type = AfflictionType.NONE;
        // 4. Update UI and feedback

    }



    private void UpdateEntityModiferRemove(Affliction affliction)
    {
        switch (affliction.type)
        {
            case (AfflictionType.LACERATION):
                m_entityModifier.bleedingDamage = 0;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove lacerate  affliction stats");
                break;

            case AfflictionType.BLEEDING:

                m_entityModifier.multiplierPercentBleedingDamage = 0.0f;
                m_entityModifier.distanceProcBleeding = 0.0f;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove bleeding  affliction stats");
                break;

            case AfflictionType.BURN:
                m_entityModifier.burningDamage = 0;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove burning affliction stats");
                break;

            case AfflictionType.BLAZE:
                m_entityModifier.effectRemovePerHit = 0;
                m_entityModifier.multiplierPercentBurningDamage = 0;
                m_entityModifier.tickDurationBlazingDamage = 0;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove blaze affliction stats");
                break;

            case AfflictionType.POISON:
                m_entityModifier.poisonDamage = 0;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove Poison affliction stats");

                break;

            case AfflictionType.INTOXICATE:

                m_entityModifier.multiplierPercentPoisonDamage = 0;
                m_entityModifier.intoxicateExecuteThreshold = 0;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove Intoxicate affliction stats");

                break;

        }

    }

    public bool IsCarryAffliction(AfflictionType afflictionType)
    {
        return afflictionArray[(int)afflictionType] != null && afflictionArray[(int)afflictionType].type != AfflictionType.NONE;
    }

    public int RemoveStack(AfflictionType type, int stackRemove)
    {
        afflictionArray[(int)type].stackCount -= stackRemove;
        afflictionArray[(int)type].stackCount = Mathf.Clamp(afflictionArray[(int)type].stackCount,0, int.MaxValue);
        UpdateEntityModiferAdd(afflictionArray[(int)type]);
        return afflictionArray[(int)type].stackCount;
    }
}
