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
                RemoveAffliction(afflictionArray[i]);
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
        if (currentAffliction.stackCount < GetAfflictionData((currentAffliction.type)).stackMax)
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
                    Debug.Log("Affliction : Apply lacerate damage");
                break;
            case AfflictionType.BLEEDING:
                
                m_entityModifier.increaseBleedingDamage = m_afflictionProfil.bleedingData.increaseDamageBleeding;
                m_entityModifier.distanceProcBleeding = m_afflictionProfil.bleedingData.movementQuantityToReduceHitCooldown;
                m_entityModifier.ActiveBleedingState();

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply bleeding damage");
                break;
        }

    }

    public void RemoveAffliction(Affliction affliction)
    {
        affliction.stackCount = 0;
        affliction.duration = 0;
        affliction.type = AfflictionType.NONE;
        UpdateEntityModiferRemove(affliction);
        // 4. Update UI and feedback

    }



    private void UpdateEntityModiferRemove(Affliction affliction)
    {
        switch (affliction.type)
        {
            case (AfflictionType.LACERATION):
                m_entityModifier.bleedingDamage = 0;
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove lacerate damage");
                break;

            case AfflictionType.BLEEDING:

                m_entityModifier.increaseBleedingDamage = 0.0f;
                m_entityModifier.distanceProcBleeding = 0.0f;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Remove bleeding damage");
                break;

        }

    }

    public bool IsCarryAffliction(AfflictionType afflictionType)
    {
        return afflictionArray[(int)afflictionType] != null;
    }


}
