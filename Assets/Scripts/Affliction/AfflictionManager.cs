using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AfflictionManager : MonoBehaviour
{

    /// <summary>
    ///  Nested struct of affliction data
    /// </summary>
    public struct AfflictionDrawData
    {
        public AfflictionType typeArray;
        public int stackCount;
    }



    [SerializeField] private AfflictionProfil m_afflictionProfil;
    public Affliction[] afflictionArray;



    [Header("Debug Infos")]
    public bool m_activeAfflictionInfo;

    private Affliction_UI_Manager m_afflictionUI;
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

        m_afflictionUI = GameState.m_uiManager.GetComponent<UIDispatcher>().afflictionUIManager;

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

    public void AddAfflictions(AfflictionDrawData[] afflictionTypes)
    {
        for (int i = 0; i < afflictionTypes.Length; i++)
        {

            Affliction affliction = new Affliction();
            affliction.type = afflictionTypes[i].typeArray  ;
            affliction.duration = GetAfflictionData(affliction.type).duration;
            affliction.stackCount = afflictionTypes[i].stackCount;
            AddAffliction(affliction);
            if (m_activeAfflictionInfo)
            {
                Debug.Log("Affliction : Add " + affliction.type);
            }
        }


    }




    public void AddAffliction(Affliction affliction)
    {
        Affliction currentAffliction = afflictionArray[(int)affliction.type - 1];

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
        afflictionArray[(int)affliction.type - 1] = currentAffliction;
        if (m_activeAfflictionInfo)
        {
            Debug.Log("Affliction : Set " + affliction.type);
        }
        UpdateEntityModiferAdd(currentAffliction);
        m_afflictionUI.DisplayAffliction(currentAffliction, m_entityModifier.IsObjectifTarget());

    }

    private Affliction TranformAffliction(Affliction currentAffliction)
    {
        if (currentAffliction.stackCount < GetAfflictionData((currentAffliction.type)).stackToTranform)
            return currentAffliction;


        switch (currentAffliction.type)
        {

            case AfflictionType.LACERATION:

                if (IsCarryAffliction(AfflictionType.BLEEDING)) return currentAffliction;
                Affliction bleedingAffliction = new Affliction();
                bleedingAffliction.type = AfflictionType.BLEEDING;
                bleedingAffliction.duration = GetAfflictionData(AfflictionType.BLEEDING).duration;
                bleedingAffliction.stackCount = 1;
                AddAffliction(bleedingAffliction);
                return currentAffliction;
                break;

            case AfflictionType.BURN:

                if (IsCarryAffliction(AfflictionType.BLAZE)) return currentAffliction;
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

            case AfflictionType.CHILL:
                Affliction freezeAffliction = new Affliction();
                freezeAffliction.type = AfflictionType.FREEZE;
                freezeAffliction.duration = GetAfflictionData(AfflictionType.FREEZE).duration;
                freezeAffliction.stackCount++;
                RemoveStack(AfflictionType.CHILL, GetAfflictionData((currentAffliction.type)).stackToTranform);
                AddAffliction(freezeAffliction);

                break;
            case AfflictionType.ELECTRIFIED:
                Affliction electrocuteAffliction = new Affliction();
                electrocuteAffliction.type = AfflictionType.ELECTROCUTE;
                electrocuteAffliction.stackCount++;
                RemoveStack(AfflictionType.ELECTRIFIED, GetAfflictionData((currentAffliction.type)).stackToTranform);
                AddAffliction(electrocuteAffliction);

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
                m_entityModifier.effectRemovePerHit = m_afflictionProfil.blazeData.effectRemovePerDamage;
                m_entityModifier.multiplierPercentBurningDamage = m_afflictionProfil.blazeData.damageIncreasePercentage;
                m_entityModifier.tickDurationBlazingDamage = m_afflictionProfil.blazeData.timeBetweenTick;
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
            case AfflictionType.CHILL:

                m_entityModifier.slownessPercent = m_afflictionProfil.chillData.slowPerStack * affliction.stackCount;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Chill affliction stats");
                break;

            case AfflictionType.FREEZE:

                m_entityModifier.ApplyFreeze();

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Freeze affliction stats");
                break;
            case AfflictionType.ELECTRIFIED:

                m_entityModifier.electrifyPercent = m_afflictionProfil.electrifyData.increaseDamagePercentPerStack * affliction.stackCount;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Electrified affliction stats");
                break;
            case AfflictionType.ELECTROCUTE:

                SpawnElectrocuteEffect(this.transform.position, m_afflictionProfil.electrocuteData.eletrocuteStrikeSpawner);

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Electrocute affliction stats");
                break;
            case AfflictionType.SCARE:

                m_entityModifier.demoralizedPercent = m_afflictionProfil.scareData.reduceDamagePerStack * affliction.stackCount;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Scare affliction stats");
                break;
            case AfflictionType.TERRIFY:

                m_entityModifier.increaseSpeedMovement = m_afflictionProfil.terrifyData.mouvementSpeedIncreasePercent;
                m_entityModifier.ApplyTerrify();

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Apply Scare affliction stats");
                break;

        }

    }

    public void RemoveAffliction(AfflictionType type)
    {
        afflictionArray[(int)type - 1].stackCount = 0;
        afflictionArray[(int)type - 1].duration = 0;
        UpdateEntityModiferRemove(afflictionArray[(int)type - 1]);
        afflictionArray[(int)type - 1].type = AfflictionType.NONE;


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
            case AfflictionType.CHILL:

                m_entityModifier.slownessPercent = 0;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Removes Chill affliction stats");
                break;

            case AfflictionType.FREEZE:

                m_entityModifier.FinishFreeze();

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Removes Freeze affliction stats");
                break;
            case AfflictionType.ELECTRIFIED:
                m_entityModifier.electrifyPercent = 0;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Removes Electrified affliction stats");
                break;
            case AfflictionType.SCARE:

                m_entityModifier.demoralizedPercent = 0;

                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Removes Scare affliction stats");
                break;
            case AfflictionType.TERRIFY:

                m_entityModifier.increaseSpeedMovement = 0;
                m_entityModifier.FinishTerrifyState();
                if (m_activeAfflictionInfo)
                    Debug.Log("Affliction : Removes Scare affliction stats");
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
        afflictionArray[(int)type].stackCount = Mathf.Clamp(afflictionArray[(int)type].stackCount, 0, int.MaxValue);
        UpdateEntityModiferAdd(afflictionArray[(int)type]);
        return afflictionArray[(int)type].stackCount;
    }


    public void SpawnElectrocuteEffect(Vector3 position, GameObject objectToSpawn)
    {
        GameObject instance = GamePullingSystem.SpawnObject(objectToSpawn, position, Quaternion.identity);
        AttackSpellSpawnerBehavior attackSpellSpawnerBehavior = instance.GetComponent<AttackSpellSpawnerBehavior>();
        if (attackSpellSpawnerBehavior != null)
        {
            attackSpellSpawnerBehavior.InitializeObject(
                m_afflictionProfil.electrocuteData.duration,
                m_afflictionProfil.electrocuteData.frequenceLightningStrike,
                m_afflictionProfil.electrocuteData.radiusElectrocute,
                m_afflictionProfil.electrocuteData.electrocuteStrike
                );

            attackSpellSpawnerBehavior.InitializeAreaData(
                GuerhoubaGames.GameEnum.GameElement.AIR,
                m_afflictionProfil.electrocuteData.radiusLightning,
                m_afflictionProfil.electrocuteData.dammagePerLightning
                );
        }

        if (m_activeAfflictionInfo)
        {
            Debug.Log("Affliction : Electrocute object spawn");
        }
    }


    #region Static Functions 



    public static AfflictionDrawData[] DrawAfflictionApplication(SpellProfil spell)
    {
        List<AfflictionDrawData> afflictionTypesSend = new List<AfflictionDrawData>();
       
        if (spell.tagData.afflictionTypes == null)
            return afflictionTypesSend.ToArray();

        for (int i = 0; i < spell.tagData.afflictionTypes.Count; i++)
        {
            float draw = UnityEngine.Random.Range(0.0f, 1.0f);
            float valueToProcAffliction = spell.GetFloatStat(StatType.AfflictionProbility, spell.GetAfflictionName(spell.tagData.afflictionTypes[i]));
            if (draw < valueToProcAffliction)
            {
                AfflictionDrawData data = new AfflictionDrawData();
                data.typeArray = spell.tagData.afflictionTypes[i];
                data.stackCount = spell.GetIntStat(StatType.AfflictionStack, spell.GetAfflictionName(spell.tagData.afflictionTypes[i]));
                afflictionTypesSend.Add(data);
            }
        }


        return afflictionTypesSend.ToArray();
    }

    #endregion


    #region UI Functions

    public void ShowAfflictionUI()
    {
        m_afflictionUI.CleanTargetAfflictionDisplay();
        for (int i = 0; i < afflictionArray.Length; i++)
        {
            if (afflictionArray[i] == null || afflictionArray[i].type == AfflictionType.NONE) continue;

            m_afflictionUI.DisplayAffliction(afflictionArray[i], m_entityModifier.IsObjectifTarget());

        }
    }

    #endregion
}
