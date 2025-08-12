using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.GameEnum;
using System.Globalization;
using Unity.VisualScripting;
using NUnit.Framework.Internal;
using SpellSystem;
using static UnityEngine.Rendering.DebugUI;
using System;
using UnityEngine.Assertions.Must;
using BorsalinoTools;
using OpenCover.Framework.Model;
using static MagicaCloth2.TeamManager;




[System.Serializable]
public struct TagData
{
    public GameElement element;
    public BuffType type;
    public SpellNature spellNatureType;
    public SpellNature spellNatureType1;
    public SpellNature spellNatureType2;
    public SpellProjectileTrajectory spellProjectileTrajectory;
    public CanalisationType canalisationType;
    public SpellMovementBehavior spellMovementBehavior;
    public DamageTrigger damageTriggerType;
    public SpellParticualarity spellParticualarity;
    public SpellParticualarity spellParticualarity1;
    public SpellParticualarity spellParticualarity2;
    public MouvementBehavior mouvementBehaviorType;
    public UpgradeSensitivity upgradeSensitivityType;
    public List<AfflictionType> afflictionTypes;


    public string GetValueTag(SpellTagOrder spellTagOrder, int index = 0)
    {
        switch (spellTagOrder)
        {
            case SpellTagOrder.GameElement:
                return element.ToString();
                break;
            case SpellTagOrder.BuffType:
                return type.ToString();
                break;
            case SpellTagOrder.SpellNature:
                return spellNatureType.ToString();
                break;
            case SpellTagOrder.SpellNature1:
                return spellNatureType1.ToString();
                break;
            case SpellTagOrder.SpellProjectileTrajectory:
                return spellProjectileTrajectory.ToString();
                break;
            case SpellTagOrder.CanalisationType:
                return canalisationType.ToString();
                break;
            case SpellTagOrder.SpellMovementBehavior:
                return spellMovementBehavior.ToString();
                break;
            case SpellTagOrder.DamageTrigger:
                return spellProjectileTrajectory.ToString();
                break;
            case SpellTagOrder.SpellParticualarity:
                return spellParticualarity.ToString();
                break;
            case SpellTagOrder.MouvementBehavior:
                return mouvementBehaviorType.ToString();
                break;
            case SpellTagOrder.UpgradeSensitivity:
                return upgradeSensitivityType.ToString();
                break;

            case SpellTagOrder.AfflictionType:
                if (afflictionTypes == null || afflictionTypes.Count >= index)
                    return "";

                return afflictionTypes[index].ToString();
                break;
            default:
                return "";
                break;
        }
    }

    public int GetIndexTagValue(SpellTagOrder spellTagOrder, int index = 0)
    {
        switch (spellTagOrder)
        {
            case SpellTagOrder.GameElement:
                return (int)element;
                break;
            case SpellTagOrder.BuffType:
                return (int)type;
                break;
            case SpellTagOrder.SpellNature:
                return (int)spellNatureType;
                break;
            case SpellTagOrder.SpellNature1:
                return (int)spellNatureType1;
                break;
            case SpellTagOrder.SpellNature2:
                return (int)spellNatureType2;
                break;
            case SpellTagOrder.SpellProjectileTrajectory:
                return (int)spellProjectileTrajectory;
                break;
            case SpellTagOrder.CanalisationType:
                return (int)canalisationType;
                break;
            case SpellTagOrder.SpellMovementBehavior:
                return (int)spellMovementBehavior;
                break;
            case SpellTagOrder.DamageTrigger:
                return (int)damageTriggerType;
                break;
            case SpellTagOrder.SpellParticualarity:
                return (int)spellParticualarity;
                break;
            case SpellTagOrder.SpellParticualarity1:
                return (int)spellParticualarity1;
                break;
            case SpellTagOrder.SpellParticualarity2:
                return (int)spellParticualarity2;
                break;
            case SpellTagOrder.MouvementBehavior:
                return (int)mouvementBehaviorType;
                break;
            case SpellTagOrder.UpgradeSensitivity:
                return (int)upgradeSensitivityType;
                break;

            case SpellTagOrder.AfflictionType:
                if (afflictionTypes == null || afflictionTypes.Count <= index) return 0;
                return (int)afflictionTypes[index];
                break;
            default:
                return -1;
                break;
        }
    }

    public bool HasAlreadyTheValue(SpellTagOrder order, int value, int index = 0)
    {
        switch (order)
        {
            case SpellTagOrder.AfflictionType:

                for (int i = 0; i < afflictionTypes.Count; i++)
                {
                    bool val = GetIndexTagValue(order, i) == value;
                    if (val) return true;
                }
                return false;

            default:
                return GetIndexTagValue(order, index) == value;
        }
    }

    public void SetIndexTagValue(SpellTagOrder spellTagOrder, int value, bool toAdd = false, int index = 0)
    {

        switch (spellTagOrder)
        {
            case SpellTagOrder.GameElement:
                element = (GameElement)value;
                break;
            case SpellTagOrder.BuffType:
                type = (BuffType)value;
                break;
            case SpellTagOrder.SpellNature:
                spellNatureType = (SpellNature)value;
                break;
            case SpellTagOrder.SpellNature1:
                spellNatureType1 = (SpellNature)value;
                break;
            case SpellTagOrder.SpellNature2:
                spellNatureType2 = (SpellNature)value;
                break;
            case SpellTagOrder.SpellProjectileTrajectory:
                spellProjectileTrajectory = (SpellProjectileTrajectory)value;
                break;
            case SpellTagOrder.CanalisationType:
                canalisationType = (CanalisationType)value;
                break;
            case SpellTagOrder.SpellMovementBehavior:
                spellMovementBehavior = (SpellMovementBehavior)value;
                break;
            case SpellTagOrder.DamageTrigger:
                damageTriggerType = (DamageTrigger)value;
                break;
            case SpellTagOrder.SpellParticualarity:
                spellParticualarity = (SpellParticualarity)value;
                break;
            case SpellTagOrder.SpellParticualarity1:
                spellParticualarity1 = (SpellParticualarity)value;
                break;
            case SpellTagOrder.SpellParticualarity2:
                spellParticualarity2 = (SpellParticualarity)value;
                break;
            case SpellTagOrder.MouvementBehavior:
                mouvementBehaviorType = (MouvementBehavior)value;
                break;
            case SpellTagOrder.UpgradeSensitivity:
                upgradeSensitivityType = (UpgradeSensitivity)value;
                break;
            case SpellTagOrder.AfflictionType:
                if (toAdd)
                {
                    afflictionTypes.Add((AfflictionType)value);
                    
                }
                else
                {
                    afflictionTypes[index] = (AfflictionType)value;
                }
                break;
            default:

                break;
        }
    }

    public int[] GetValidTag()
    {
        int tagCount = System.Enum.GetNames(typeof(SpellTagOrder)).Length;
        int[] indexTagValue = new int[tagCount];

        for (int i = 0; i < tagCount; i++)
        {
            int tagIndex = GetIndexTagValue((SpellTagOrder)(Math.Pow(2, i)));
            indexTagValue[i] = tagIndex;
        }
        return indexTagValue;
    }

    public bool EqualsSpellParticularity(SpellParticualarity value)
    {
        return (value == spellParticualarity) || (value == spellParticualarity1) || (value == spellParticualarity2);
    }

    public bool EqualsSpellNature(SpellNature value)
    {
        return (value == spellNatureType) || (value == spellNatureType1) || (value == spellNatureType2);
    }

    public string[] GetUIInfosValue()
    {
        string[] tagString = new string[5];

        SpellTagOrder[] spellTagOrdersArray = { SpellTagOrder.GameElement, SpellTagOrder.SpellNature, SpellTagOrder.SpellNature1, SpellTagOrder.SpellParticualarity, SpellTagOrder.SpellMovementBehavior };
        int diffIndex = 0;
        for (int i = 0; i < spellTagOrdersArray.Length; i++)
        {
            string value = GetValueTag(spellTagOrdersArray[i]);
            if (value == "NONE")
            {
                diffIndex++;
                continue;
            }
            value = CultureInfo.CurrentCulture.TextInfo.ToLower(value);
            value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value); ;
            tagString[i] = "";
            tagString[i - diffIndex] = value;
        }

        return tagString;
    }

    public void ChangeTag(TagData tagData)
    {
        int[] indexValueArray = tagData.GetValidTag();

        for (int i = 0; i < indexValueArray.Length; i++)
        {
            if (indexValueArray[i] != 0)
            {
                SetIndexTagValue((SpellTagOrder)(Math.Pow(2, i)), indexValueArray[i]);
            }
        }
    }


    public bool HasOneTagSimilar(TagData tagDataCompare)
    {
        if (tagDataCompare.element == element) return true;
        if (tagDataCompare.type == type) return true;
        if (tagDataCompare.EqualsSpellNature(spellNatureType)) return true;
        if (tagDataCompare.EqualsSpellNature(spellNatureType1)) return true;
        if (tagDataCompare.spellProjectileTrajectory == spellProjectileTrajectory) return true;
        if (tagDataCompare.canalisationType == canalisationType) return true;
        if (tagDataCompare.spellMovementBehavior == spellMovementBehavior) return true;
        if (tagDataCompare.damageTriggerType == damageTriggerType) return true;
        if (tagDataCompare.EqualsSpellParticularity(spellParticualarity)) return true;
        if (tagDataCompare.EqualsSpellParticularity(spellParticualarity1)) return true;
        if (tagDataCompare.EqualsSpellParticularity(spellParticualarity2)) return true;
        if (tagDataCompare.mouvementBehaviorType == mouvementBehaviorType) return true;

        for (int i = 0; i < afflictionTypes.Count; i++)
        {
            if (afflictionTypes[i].Equals(tagDataCompare.afflictionTypes[i]))
                return true;
        }

        return false;

    }

    public bool HasAllTagSimilar(TagData tagDataCompare)
    {
        if ((int)element != 0 && tagDataCompare.element != element) return false;
        if ((int)type != 0 && tagDataCompare.type != type) return false;
        if ((int)spellNatureType != 0 && !EqualsSpellNature(spellNatureType)) return false;
        if ((int)spellNatureType1 != 0 && !tagDataCompare.EqualsSpellNature(spellNatureType1)) return false;
        if ((int)spellProjectileTrajectory != 0 && tagDataCompare.spellProjectileTrajectory != spellProjectileTrajectory) return false;
        if ((int)canalisationType != 0 && tagDataCompare.canalisationType != canalisationType) return false;
        if ((int)spellMovementBehavior != 0 && tagDataCompare.spellMovementBehavior != spellMovementBehavior) return false;
        if ((int)damageTriggerType != 0 && tagDataCompare.damageTriggerType != damageTriggerType) return false;
        if ((int)spellParticualarity != 0 && !tagDataCompare.EqualsSpellParticularity(spellParticualarity)) return false;
        if ((int)spellParticualarity1 != 0 && !tagDataCompare.EqualsSpellParticularity(spellParticualarity1)) return false;
        if ((int)spellParticualarity2 != 0 && !(tagDataCompare.EqualsSpellParticularity(spellParticualarity2))) return false;
        if ((int)mouvementBehaviorType != 0 && tagDataCompare.mouvementBehaviorType != mouvementBehaviorType) return false;

        for (int i = 0; i < afflictionTypes.Count; i++)
        {
            if (!afflictionTypes[i].Equals(tagDataCompare.afflictionTypes[i]))
                return false;
        }

        return true;
    }
}


namespace SpellSystem
{

    public interface IStatInterface
    {
        public void ConvertStat(StatData data);
    }


    [System.Serializable]
    public class StatData : ICloneable, IStatInterface
    {
        public StatType stat;
        public GuerhoubaGames.GameEnum.ValueType valueType;
        public string nameStat;
        public int val_int;
        public float val_float;
        public string val_string;
        public bool val_bool;
        public bool isVisible;


        public StatData()
        {
            val_int = 0;
            val_float = 0.0f;
            val_string = null;
            val_bool = false;
            isVisible = false;
            nameStat = null;
            stat = 0;
            valueType = 0;
        }

        public StatData(StatData data)
        {
            val_int = data.val_int;
            val_float = data.val_float;
            val_string = data.val_string;
            val_bool = data.val_bool;
            isVisible = data.isVisible;
            nameStat = data.nameStat;
            stat = data.stat;
            valueType = data.valueType;
        }

        public virtual StatData Clone()
        {
            return (StatData)this.MemberwiseClone();
        }

        public virtual void ConvertStat(StatData data)
        {
            val_int = data.val_int;
            val_float = data.val_float;
            val_string = data.val_string;
            val_bool = data.val_bool;
            isVisible = data.isVisible;
            nameStat = data.nameStat;
            stat = data.stat;
            valueType = data.valueType;
        }


        public virtual StatData ConvertStat(StatData data, Type t)
        {
            if (t ==  typeof(StatDataUpgrade))
            {
                return new StatDataUpgrade(data);
            }
            if (t == typeof(StatDataLevel))
            {
                return new StatDataLevel(data);
            }

            return this;
            
        }

        object ICloneable.Clone() => this.Clone();

    }

    [System.Serializable]
    public class StatDataLevel : StatData , IStatInterface
    {
        public float multiply;

        public override StatData Clone()
        {
            return (StatDataLevel)this.MemberwiseClone();
        }

        public StatDataLevel(StatData data)
        {
            val_int = data.val_int;
            val_float = data.val_float;
            val_string = data.val_string;
            val_bool = data.val_bool;
            isVisible = data.isVisible;
            nameStat = data.nameStat;
            stat = data.stat;
            valueType = data.valueType;
            multiply = 0;
        }

        public override void ConvertStat(StatData data) 
        {
            val_int = data.val_int;
            val_float = data.val_float;
            val_string = data.val_string;
            val_bool = data.val_bool;
            isVisible = data.isVisible;
            nameStat = data.nameStat;
            stat = data.stat;
            valueType = data.valueType;

        }
    }

    [System.Serializable]
    public class StatDataUpgrade : StatData, IStatInterface
    {
        public bool isOnlyAddWithTag;

        public override StatData Clone()
        {
            return (StatDataUpgrade)this.MemberwiseClone();
        }

        public StatDataUpgrade(StatData data)
        {
            val_int = data.val_int;
            val_float = data.val_float;
            val_string = data.val_string;
            val_bool = data.val_bool;
            isVisible = data.isVisible;
            nameStat = data.nameStat;
            stat = data.stat;
            valueType = data.valueType;
            isOnlyAddWithTag = false;
        }

        public override void ConvertStat(StatData data)
        {
            val_int = data.val_int;
            val_float = data.val_float;
            val_string = data.val_string;
            val_bool = data.val_bool;
            isVisible = data.isVisible;
            nameStat = data.nameStat;
            stat = data.stat;
            valueType = data.valueType;
        }
    }



    [System.Serializable]
    public class PlayerEffectStats<T> where T : StatData, IStatInterface
    {

        [Header("Tag Parameters")]
        public TagData tagData;

        [Space]
        public List<T> statDatas = new List<T>();
        [HideInInspector] public StatType[] statTypes = new StatType[0];



        public PlayerEffectStats<T> Clone()
        {
            PlayerEffectStats<T> stats = new PlayerEffectStats<T>();
            stats.statTypes = new StatType[statDatas.Count];
            stats.statDatas = new List<T>(statDatas.Count);
            for (int i = 0; i < stats.statTypes.Length; i++)
            {
                stats.statTypes[i] = statDatas[i].stat;
                T statDataInstance = (T)statDatas[i].Clone();
                stats.statDatas.Add(statDataInstance);
            }

            stats.tagData = new TagData();
            stats.tagData = tagData;
            stats.tagData.afflictionTypes = new List<AfflictionType>(tagData.afflictionTypes);
            return stats;
        }

        public bool IsStatBool(StatType statsType)
        {
            return (int)statsType - ((0 * 1000)) < 1000;
        }
        public bool IsStatInt(StatType statsType)
        {
            return (int)statsType - ((1 * 1000)) < 1000;
        }
        public bool IsStatFloat(StatType statsType)
        {
            return (int)statsType - ((2 * 1000)) < 1000;
        }
        public bool IsStatString(StatType statsType)
        {
            return (int)statsType - ((3 * 1000)) < 1000;
        }

        public bool HasStats(StatType statsType)
        {
            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    return true;
                }
            }
            return false;
        }

        public int GetStats(StatType statsType, string nameAddional)
        {
            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i] && statDatas[i].nameStat == nameAddional)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetIntStat(StatType statsType)
        {
            if (!IsStatInt(statsType))
            {
                Debug.LogError("This stats isn't an integer");
                return -1;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    return statDatas[i].val_int;
                }
            }

            Debug.LogError("Didn't find this stat on the spell ");
            return -1;
        }

        public int GetIntStat(StatType statsType, string nameStatToChange)
        {
            if (!IsStatInt(statsType))
            {
                Debug.LogError("This stats isn't an integer");
                return -1;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i] && statDatas[i].nameStat == nameStatToChange)
                {
                    return statDatas[i].val_int;
                }
            }

            Debug.LogError("Didn't find this stat on the spell ");
            return -1;
        }

        public float GetFloatStat(StatType statsType)
        {
            if (!IsStatFloat(statsType))
            {
                Debug.LogError("This stats isn't an float");
                return -1;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    return statDatas[i].val_float;
                }
            }

            Debug.LogError("Didn't find this stat on the spell ");
            return -1;
        }
        public float GetFloatStat(StatType statsType, string nameStatToChange)
        {
            if (!IsStatFloat(statsType))
            {
                Debug.LogError("This stats isn't an float");
                return -1;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i] && statDatas[i].nameStat == nameStatToChange)
                {
                    return statDatas[i].val_float;
                }
            }

            Debug.LogError("Didn't find this stat on the spell ");
            return -1;
        }

        public bool GetBoolStat(StatType statsType)
        {
            if (!IsStatBool(statsType))
            {
                Debug.LogError("This stats isn't an bool");
                return false; ;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    return statDatas[i].val_bool;
                }
            }

            Debug.LogError("Didn't find this stat on the spell ");
            return false;
        }
        public string GetStringStat(StatType statsType)
        {
            if (!IsStatString(statsType))
            {
                Debug.LogError("This stats isn't an string");
                return "";
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    return statDatas[i].val_string;
                }
            }

            Debug.LogError("Didn't find this stat on the spell ");
            return "";
        }

        public void AddToIntStats(StatType statsType, int val, string nameAdditional = "", float multiplier = 1)
        {
            if (!IsStatInt(statsType))
            {
                Debug.LogError("This stats isn't an integer");
                return;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i] && statDatas[i].nameStat == nameAdditional)
                {
                    StatData statData = statDatas[i];
                    statData.val_int += val;
                    if (multiplier != 0) statData.val_int = (int)(statData.val_int * multiplier);
                    statDatas[i] = (T)statData;
                    return;
                }
            }
        }
        public void AddToFloatStats(StatType statsType, float val, string nameAdditional, float multiplier = 1)
        {

            if (!IsStatFloat(statsType))
            {
                Debug.LogError("This stats isn't an float");
                return;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i] && statDatas[i].nameStat == nameAdditional)
                {
                    StatData statData = statDatas[i];
                    statData.val_float += val;
                    if (multiplier != 0) statData.val_float *= multiplier;
                    statDatas[i] = (T)statData;
                    return;
                }
            }
        }
        public void ChangBoolValue(StatType statsType, bool val)
        {
            if (!IsStatBool(statsType))
            {
                Debug.LogError("This stats isn't an bool");
                return;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    StatData statData = statDatas[i];
                    statData.val_bool = val;
                    statDatas[i] = (T)statData;
                    return;
                }
            }
        }
        public void ChangeStringStats(StatType statsType, string val)
        {
            if (!IsStatString(statsType))
            {
                Debug.LogError("This stats isn't an string");
                return;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    StatData statData = statDatas[i];
                    statData.val_string = val;
                    statDatas[i] = (T)statData;
                    return;
                }
            }
        }
        public string GetStatValueToString(StatType statsType)
        {

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    if (IsStatBool(statsType)) return statDatas[i].val_bool.ToString();
                    if (IsStatInt(statsType)) return statDatas[i].val_int.ToString();
                    if (IsStatFloat(statsType)) return statDatas[i].val_float.ToString();
                    if (IsStatString(statsType)) return statDatas[i].val_string.ToString();
                }


            }

            return "";
        }
        public string DebugStat()
        {
            string debugStatString = "";
            for (int i = 0; i < statTypes.Length; i++)
            {

                if (!statDatas[i].isVisible) continue;
                if (IsStatBool(statTypes[i]))
                {
                    debugStatString += "<u>" + statTypes[i].ToString() + " </u> : <b>" + statDatas[i].val_bool.ToString() + " </b> \n";
                    continue;
                }

                if (IsStatInt(statTypes[i]))
                {
                    debugStatString += "<u>" + statTypes[i].ToString() + " </u> : <b>" + statDatas[i].val_int.ToString() + " </b> \n";
                    continue;
                }

                if (IsStatFloat(statTypes[i]))
                {
                    debugStatString += "<u>" + statTypes[i].ToString() + " </u> : <b>" + statDatas[i].val_float.ToString() + " </b> \n";
                    continue;
                }

                if (IsStatString(statTypes[i]))
                {
                    debugStatString += "<u>" + statTypes[i].ToString() + " </u> : <b>" + statDatas[i].val_string + " </b> \n";
                    continue;
                }
            }

            return debugStatString;
        }

        #region Stats setup Functions
        public void UpdateStatistics()
        {

            bool testResult = tagData.type == BuffType.DAMAGE_SPELL;
            ManageStat(StatType.Damage, testResult, true);


            SetupSpellNatureStats(tagData.spellNatureType, tagData.spellNatureType1);
            SetupUpgradeSensitivity();
            SetupSpellParaticularity();
            SetupCanalisationType();
            SetupMovementType();
            SetupSpellMouvement();
            SetupAfflictionStats(tagData.afflictionTypes.ToArray());

            ManageStat(StatType.StackDuration, true);
            ManageStat(StatType.GainPerStack, true);
            ManageStat(StatType.Range, true);

            statTypes = new StatType[statDatas.Count];
            for (int i = 0; i < statTypes.Length; i++)
            {
                statTypes[i] = statDatas[i].stat;
            }


        }

        public void UpdateOnlyTag()
        {
            SetupSpellNatureStats(tagData.spellNatureType, tagData.spellNatureType1);
            SetupUpgradeSensitivity();
            SetupSpellParaticularity();
            SetupCanalisationType();
            SetupMovementType();
            SetupSpellMouvement();
            SetupAfflictionStats(tagData.afflictionTypes.ToArray());

            statTypes = new StatType[statDatas.Count];
            for (int i = 0; i < statTypes.Length; i++)
            {
                statTypes[i] = statDatas[i].stat;
            }

        }

        public void ManageStat(StatType statToCheck, bool isAdd, bool isVisible = false)
        {
            for (int i = 0; i < statDatas.Count; i++)
            {
                if (statDatas[i].stat == statToCheck)
                {
                    if (!isAdd)
                    {
                        statDatas.RemoveAt(i);
                        i--;
                    }
                    return;
                }
            }

            if (isAdd)
            {
                StatData statData = new StatData();
                statData.stat = statToCheck;
                statData.isVisible = isVisible;
                statDatas.Add((T)statData);
            }

            return;
        }

        public void ManageAfflictionStat(StatType statToCheck, bool isAdd, AfflictionType type, bool isVisible = false) 
        {
            string nameAffliction = CultureInfo.CurrentCulture.TextInfo.ToLower(type.ToString());
            nameAffliction = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameAffliction);
            for (int i = 0; i < statDatas.Count; i++)
            {
                if (statDatas[i].stat == statToCheck && statDatas[i].nameStat == nameAffliction)
                {
                    if (!isAdd)
                    {
                        statDatas.RemoveAt(i);
                        i--;
                    }
                    return;
                }
            }

            if (isAdd)
            {
                StatData statData = new StatData();
                statData.stat = statToCheck;
                statData.nameStat = nameAffliction;
                statData.isVisible = isVisible;


                statDatas.Add((T)statData.ConvertStat(statData, typeof(T)));
             

            }

            return;
        }

        private void CheckAfflictionStat()
        {
            for (int i = 0; i < statDatas.Count; i++)
            {
                if (statDatas[i].stat == StatType.AfflictionProbility || statDatas[i].stat == StatType.AfflictionStack)
                {
                    bool hasToBeRemove = true;
                    for (int j = 0; j < tagData.afflictionTypes.Count; j++)
                    {
                        string nameStat = statDatas[i].nameStat.ToUpper();
                        if (nameStat == tagData.afflictionTypes[j].ToString())
                        {
                            hasToBeRemove = false;
                        }

                    }

                    if (hasToBeRemove)
                    {
                        statDatas.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void SetupSpellMouvement()
        {
            bool testResult = tagData.spellMovementBehavior == SpellMovementBehavior.FollowPlayer;
            ManageStat(StatType.Proximity, testResult);
            ManageStat(StatType.SpeedFollow, testResult);

            testResult = tagData.spellMovementBehavior == SpellMovementBehavior.Return;
            ManageStat(StatType.SpeedReturn, testResult);

            testResult = tagData.spellMovementBehavior == SpellMovementBehavior.Fix;
            ManageStat(StatType.OffsetDistance, testResult);


            testResult = tagData.spellMovementBehavior == SpellMovementBehavior.Direction;
            ManageStat(StatType.DirectionSpeed, testResult);

            testResult = tagData.spellMovementBehavior == SpellMovementBehavior.FollowMouse;
            ManageStat(StatType.DirectionSpeed, testResult);
        }

        private void SetupMovementType()
        {
            bool testResult = tagData.mouvementBehaviorType == MouvementBehavior.Dash;
            ManageStat(StatType.DistanceDash, testResult);
            ManageStat(StatType.MouvementTravelTime, testResult);
            ManageStat(StatType.Invunerability, testResult);
        }

        private void SetupCanalisationType()
        {
            bool testResult = tagData.canalisationType == CanalisationType.LIGHT_CANALISATION;
            ManageStat(StatType.SpellCanalisation, testResult);


            testResult = tagData.canalisationType == CanalisationType.HEAVY_CANALISATION;
            ManageStat(StatType.SpellCanalisation, testResult);
            ManageStat(StatType.SpeedReduce, testResult);

        }

        private void SetupSpellParaticularity()
        {
            bool testResult = tagData.EqualsSpellParticularity(SpellParticualarity.Explosion);
            ManageStat(StatType.SizeExplosion, testResult, true);
            ManageStat(StatType.DamageAdditionel, testResult, true);

            testResult = tagData.EqualsSpellParticularity(SpellParticualarity.Delayed);
            ManageStat(StatType.TimeDelay, testResult, true);

            testResult = tagData.EqualsSpellParticularity(SpellParticualarity.Piercing);
            ManageStat(StatType.Piercing, testResult, true);

            testResult = tagData.EqualsSpellParticularity(SpellParticualarity.Bouncing);
            ManageStat(StatType.BounceNumber, testResult, true);
            ManageStat(StatType.BounceRadius, testResult, true);


        }

        private void SetupUpgradeSensitivity()
        {
            bool testResult = tagData.upgradeSensitivityType == UpgradeSensitivity.HighScale;
            ManageStat(StatType.ScaleRate, testResult);
            if (testResult) return;

            testResult = tagData.upgradeSensitivityType == UpgradeSensitivity.LowScale;
            ManageStat(StatType.ScaleRate, testResult);
        }

        private void SetupSpellNatureStats(SpellNature type, SpellNature type2)
        {
            bool testResult;
            testResult = tagData.EqualsSpellNature(SpellNature.PROJECTILE);
            ManageStat(StatType.LifeTime, testResult);
            ManageStat(StatType.Projectile, testResult, true);
            ManageStat(StatType.ShootNumber, testResult, true);
            ManageStat(StatType.ShootAngle, testResult);
            ManageStat(StatType.TimeBetweenShot, testResult);


            bool testResult2 = tagData.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE;
            ManageStat(StatType.AngleTrajectory, testResult2 && testResult);
            ManageStat(StatType.TrajectoryTimer, testResult2 && testResult);

            testResult = tagData.spellNatureType == SpellNature.AREA;
            ManageStat(StatType.SpellCount, testResult, true);
            ManageStat(StatType.SpellFrequency, testResult);

            testResult = tagData.EqualsSpellNature(SpellNature.AREA) || tagData.EqualsSpellNature(SpellNature.AURA);
            ManageStat(StatType.Size, testResult, true);
            ManageStat(StatType.SizeMuplitiplicator, testResult);


            testResult = tagData.EqualsSpellNature(SpellNature.SUMMON);
            ManageStat(StatType.MaxSummon, testResult, true);
            ManageStat(StatType.SummonSimultanely, testResult, true);
            ManageStat(StatType.LifeTimeSummon, testResult, true);

            testResult = tagData.EqualsSpellNature(SpellNature.MULTI_HIT_AREA);
            ManageStat(StatType.HitFrequency, testResult);
            ManageStat(StatType.HitNumber, testResult, true);

            testResult = tagData.EqualsSpellNature(SpellNature.AREA) || tagData.EqualsSpellNature(SpellNature.AURA) || tagData.EqualsSpellNature(SpellNature.MULTI_HIT_AREA);
            ManageStat(StatType.AreaTargetSimulately, testResult, true);

            testResult = tagData.spellNatureType == SpellNature.SUMMON && tagData.EqualsSpellNature(SpellNature.PROJECTILE);
            ManageStat(StatType.AttackReload, testResult, true);

        }

        void SetupAfflictionStats(AfflictionType[] typeAffliction)
        {
            for (int i = 0; i < typeAffliction.Length; i++)
            {

                ManageAfflictionStat(StatType.AfflictionProbility, true, typeAffliction[i], true);
                ManageAfflictionStat(StatType.AfflictionStack, true, typeAffliction[i], true);
            }

            CheckAfflictionStat();
        }

        public string GetAfflictionName(AfflictionType type)
        {
            string nameAffliction = CultureInfo.CurrentCulture.TextInfo.ToLower(type.ToString());
            nameAffliction = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameAffliction);
            return nameAffliction;
        }




        #endregion

        public bool IsAllTagMatching(PlayerEffectStats<StatData> gameEffectStats)
        {
            return tagData.HasAllTagSimilar(gameEffectStats.tagData);
        }

        public bool IsValidUpgrade(PlayerEffectStats<StatData> gameEffectStats)
        {
            return tagData.HasOneTagSimilar(gameEffectStats.tagData);
        }

        public void ChangeStats<U>(PlayerEffectStats<U> gameEffectStats) where U : StatDataLevel
        {

            for (int i = 0; i < gameEffectStats.statTypes.Length; i++)
            {
                if (IsStatBool(gameEffectStats.statTypes[i]))
                {
                    ChangBoolValue(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_bool);
                    continue;
                }


                if (IsStatInt(gameEffectStats.statTypes[i]))
                {
                    AddToIntStats(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_int, gameEffectStats.statDatas[i].nameStat);
                    continue;
                }
                if (IsStatFloat(gameEffectStats.statTypes[i]))
                {
                    AddToFloatStats(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_float, gameEffectStats.statDatas[i].nameStat);
                    continue;
                }

                if (IsStatString(gameEffectStats.statTypes[i]))
                {
                    ChangeStringStats(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_string);
                    continue;
                }
            }
        }



        public void ChangeStats(PlayerEffectStats<StatDataUpgrade> gameEffectStats, bool isAdd = false)
        {
            for (int i = 0; i < gameEffectStats.statTypes.Length; i++)
            {
                bool IsAddingStat = (gameEffectStats.statDatas[i].isOnlyAddWithTag && isAdd) || !gameEffectStats.statDatas[i].isOnlyAddWithTag;

                if (IsStatBool(gameEffectStats.statTypes[i]) && IsAddingStat)
                {
                    ChangBoolValue(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_bool);
                    continue;
                }


                if (IsStatInt(gameEffectStats.statTypes[i]) && IsAddingStat)
                {
                    AddToIntStats(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_int, gameEffectStats.statDatas[i].nameStat);
                    continue;
                }
                if (IsStatFloat(gameEffectStats.statTypes[i]) && IsAddingStat)
                {
                    AddToFloatStats(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_float, gameEffectStats.statDatas[i].nameStat);
                    continue;
                }

                if (IsStatString(gameEffectStats.statTypes[i]) && IsAddingStat)
                {
                    ChangeStringStats(gameEffectStats.statTypes[i], gameEffectStats.statDatas[i].val_string);
                    continue;
                }
            }
        }


    }




    [CreateAssetMenu(fileName = "Spell Profil", menuName = "Spell/Spell Profil")]
    public class SpellProfil : ScriptableObject
    {
        public string name;
        public int id;
        public int idFamily = 0;
        [TextArea]
        public string description;
        public Sprite spell_Icon;
        public GameObject objectToSpawn;
        public Vector3 angleRotation;
        public GameObject VFX;
        public Material matToUse;
        public Texture previewDecal_mat;
        public Texture previewDecalEnd_mat;
        public GameObject hit_VFX;

        public int currentSpellTier;
        [HideInInspector] public int spellExp;
        private int spellExpNextLevel = 4;
        private int spellExpCountPerLevel = 4;

        private bool hasBeenSetup;
        [Header("Level Spell ")]
        public LevelSpell[] levelSpellsProfiles;


        [Header("Effect Proc")]
        public bool OnContact = false;
        public bool OnDeath = false;
        public bool OnHit = true;


        //[Header("Tag Parameters")]
        //public TagData tagData;

        //[Space]
        //public List<StatData> statDatas = new List<StatData>();
        //[HideInInspector] public StatType[] statTypes = new StatType[0];

        [HideInInspector] public SpellAttribution m_SpellAttributionAssociated;

        public PlayerEffectStats<StatData> gameEffectStats;

        public TagData TagList { get { return gameEffectStats.tagData; } set { } }


        public SpellProfil Clone(bool isSetup = false)
        {
            SpellProfil spellProfil = Instantiate(this);

            spellProfil.gameEffectStats = gameEffectStats.Clone();

            if (isSetup)
            {
                spellProfil.SetupSpell();
            }
            return spellProfil;
        }

        public void SetupSpell()
        {
            if (hasBeenSetup) return;
            LevelSpell.SetupLevelEffect(levelSpellsProfiles);

            for (int i = 0; i < currentSpellTier; i++)
            {
                GainLevel(i);
            }

            hasBeenSetup = true;
        }

        #region Levels Functions

        public ChainEffect[] GetChainEffects()
        {
            List<ChainEffect> chainEffectsList = new List<ChainEffect>();
            if (levelSpellsProfiles == null) return chainEffectsList.ToArray();

            for (int i = 0; i < currentSpellTier && i < levelSpellsProfiles.Length; i++)
            {
                if (levelSpellsProfiles[i] == null) continue;

                if (levelSpellsProfiles[i].LevelType == SpellLevelType.CHAIN_EFFECT)
                {
                    chainEffectsList.Add((ChainEffect)levelSpellsProfiles[i]);
                }
            }
            return chainEffectsList.ToArray();
        }

        public BehaviorLevel[] GetBehaviorsLevels()
        {
            List<BehaviorLevel> behaviorList = new List<BehaviorLevel>();
            if (levelSpellsProfiles == null) return behaviorList.ToArray();

            for (int i = 0; i < currentSpellTier && i < levelSpellsProfiles.Length; i++)
            {
                if (levelSpellsProfiles[i] == null) continue;

                if (levelSpellsProfiles[i].LevelType == SpellLevelType.BEHAVIOR)
                {
                    behaviorList.Add((BehaviorLevel)levelSpellsProfiles[i]);
                }
            }
            return behaviorList.ToArray();
        }

        public bool CanGainLevel()
        {
            if (currentSpellTier == 3) return false;

            return true;
        }

        public void GainLevel()
        {
            if (currentSpellTier == 3) return;


            if (levelSpellsProfiles == null || levelSpellsProfiles.Length <= currentSpellTier || levelSpellsProfiles[currentSpellTier] == null)
            {
                currentSpellTier++;
                spellExpNextLevel = spellExp + spellExpCountPerLevel;
                return;
            }
            if (levelSpellsProfiles[currentSpellTier].isPermanent)
            {
                if (levelSpellsProfiles[currentSpellTier].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpellsProfiles[currentSpellTier]);
                    statsLevel.Apply(this);
                }
                if (levelSpellsProfiles[currentSpellTier].LevelType == SpellLevelType.BEHAVIOR)
                {
                    BehaviorLevel statsLevel = (BehaviorLevel)(levelSpellsProfiles[currentSpellTier]);
                    statsLevel.OnGain(this);
                }

            }

            currentSpellTier++;
            spellExpNextLevel = spellExp + spellExpCountPerLevel;

        }

        public float GetSize()
        {
            if (!gameEffectStats.HasStats(StatType.Size))
                return 1;

            if (gameEffectStats.tagData.EqualsSpellParticularity(SpellParticualarity.Explosion))
            {
                return gameEffectStats.GetFloatStat(StatType.SizeExplosion);
            }
            return gameEffectStats.GetFloatStat(StatType.Size);
        }

        public void GainLevel(int index)
        {
            if (index == 3) return;



            if (levelSpellsProfiles == null || levelSpellsProfiles.Length <= index || levelSpellsProfiles[index] == null)
            {
                // spellLevel++;
                spellExpNextLevel = spellExp + spellExpCountPerLevel;
                return;
            }
            if (levelSpellsProfiles[index].isPermanent)
            {
                if (levelSpellsProfiles[index].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpellsProfiles[index]);
                    statsLevel.Apply(this);
                }
                if (levelSpellsProfiles[index].LevelType == SpellLevelType.BEHAVIOR)
                {
                    BehaviorLevel statsLevel = (BehaviorLevel)(levelSpellsProfiles[index]);
                    statsLevel.OnGain(this);
                }

            }

            // spellLevel++;
            spellExpNextLevel = spellExp + spellExpCountPerLevel;

        }

        public bool AddSpellExpPoint(int points)
        {
            spellExp += points;
            if (m_SpellAttributionAssociated != null) { m_SpellAttributionAssociated.StartCoroutine(m_SpellAttributionAssociated.UpdateSpellLevelDelay(this)); }
            bool isLevelUp = spellExp >= spellExpNextLevel;
            if (spellExp >= 13) isLevelUp = false;


            return isLevelUp;
        }

        #endregion

        public void UpdateStatistics()
        {

            gameEffectStats.UpdateStatistics();
        }

        internal float GetFloatStat(StatType stat)
        {
            return gameEffectStats.GetFloatStat(stat);
        }

        internal int GetIntStat(StatType stat)
        {
            return gameEffectStats.GetIntStat(stat);
        }
    }
}
