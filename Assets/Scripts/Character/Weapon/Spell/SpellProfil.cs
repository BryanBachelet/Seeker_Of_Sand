using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.GameEnum;
using System.Globalization;
using Unity.VisualScripting;

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


    public string GetValueTag(SpellTagOrder spellTagOrder)
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
            default:
                return "";
                break;
        }
    }

    public int GetIndexTagValue(SpellTagOrder spellTagOrder)
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
            default:
                return -1;
                break;
        }
    }

    public void SetIndexTagValue(SpellTagOrder spellTagOrder, int value)
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
            int tagIndex = GetIndexTagValue((SpellTagOrder)i);
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
                SetIndexTagValue((SpellTagOrder)i, indexValueArray[i]);
            }
        }
    }

}


namespace SpellSystem
{



    [System.Serializable]
    public class StatData
    {
        public StatType stat;
        public ValueType valueType;
        public int val_int;
        public float val_float;
        public string val_string;
        public bool val_bool;
        public bool isVisible;
    }

    [System.Serializable]
    public class StatDataLevel : StatData
    {
        public float multiply;
    }


    [CreateAssetMenu(fileName = "Spell Profil", menuName = "Spell/Spell Profil")]
    public class SpellProfil : ScriptableObject
    {
        public string name;
        public int id;
        [TextArea]
        public string description;
        public Sprite spell_Icon;
        public GameObject objectToSpawn;
        public Vector3 angleRotation;
        public GameObject VFX;
        public Material matToUse;
        public Texture previewDecal_mat;
        public Texture previewDecalEnd_mat;

        public int spellLevel;
        [HideInInspector] public int spellExp;
        private int spellExpNextLevel = 4;
        private int spellExpCountPerLevel = 4;

        private bool hasBeenSetup;
        [Header("Level Spell ")]
        public LevelSpell[] levelSpells;


        [Header("Effect Proc")]
        public bool OnContact = false;
        public bool OnDeath = false;
        public bool OnHit = true;

        [Header("Tag Parameters")]
        public TagData tagData;



        [Space]
        public List<StatData> statDatas = new List<StatData>();
        private StatType[] statTypes = new StatType[0];




        public SpellProfil Clone(bool isSetup = false)
        {
            SpellProfil spellProfil = Instantiate(this);


            spellProfil.statTypes = new StatType[spellProfil.statDatas.Count];
            for (int i = 0; i < spellProfil.statTypes.Length; i++)
            {
                spellProfil.statTypes[i] = spellProfil.statDatas[i].stat;
            }

            if (isSetup)
            {

                spellProfil.SetupSpell();
            }
            return spellProfil;
        }

        public void SetupSpell()
        {
            if (hasBeenSetup) return;
            LevelSpell.SetupLevelEffect(levelSpells);

            for (int i = 0; i < spellLevel; i++)
            {
                GainLevel(i);
            }

            hasBeenSetup = true;
        }

        #region Levels Functions

        public ChainEffect[] GetChainEffects()
        {
            List<ChainEffect> chainEffectsList = new List<ChainEffect>();
            if (levelSpells == null) return chainEffectsList.ToArray();

            for (int i = 0; i < spellLevel && i < levelSpells.Length; i++)
            {
                if (levelSpells[i] == null) continue;

                if (levelSpells[i].LevelType == SpellLevelType.CHAIN_EFFECT)
                {
                    chainEffectsList.Add((ChainEffect)levelSpells[i]);
                }
            }
            return chainEffectsList.ToArray();
        }

        public BehaviorLevel[] GetBehaviorsLevels()
        {
            List<BehaviorLevel> behaviorList = new List<BehaviorLevel>();
            if (levelSpells == null) return behaviorList.ToArray();

            for (int i = 0; i < spellLevel && i < levelSpells.Length; i++)
            {
                if (levelSpells[i] == null) continue;

                if (levelSpells[i].LevelType == SpellLevelType.BEHAVIOR)
                {
                    behaviorList.Add((BehaviorLevel)levelSpells[i]);
                }
            }
            return behaviorList.ToArray();
        }

        public bool CanGainLevel()
        {
            if (spellLevel == 3) return false;

            return true;
        }

        public void GainLevel()
        {
            if (spellLevel == 3) return;



            if (levelSpells == null || levelSpells.Length <= spellLevel || levelSpells[spellLevel] == null)
            {
                spellLevel++;
                spellExpNextLevel = spellExp + spellExpCountPerLevel;
                return;
            }
            if (levelSpells[spellLevel].isPermanent)
            {
                if (levelSpells[spellLevel].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpells[spellLevel]);
                    statsLevel.Apply(this);
                }
                if (levelSpells[spellLevel].LevelType == SpellLevelType.BEHAVIOR)
                {
                    BehaviorLevel statsLevel = (BehaviorLevel)(levelSpells[spellLevel]);
                    statsLevel.OnUpgradeGain();
                }

            }

            spellLevel++;
            spellExpNextLevel = spellExp + spellExpCountPerLevel;

        }

        public void GainLevel(int index)
        {
            if (index == 3) return;



            if (levelSpells == null || levelSpells.Length <= index || levelSpells[index] == null)
            {
                // spellLevel++;
                spellExpNextLevel = spellExp + spellExpCountPerLevel;
                return;
            }
            if (levelSpells[index].isPermanent)
            {
                if (levelSpells[index].LevelType == SpellLevelType.STATS)
                {
                    StatsLevel statsLevel = (StatsLevel)(levelSpells[index]);
                    statsLevel.Apply(this);
                }
                if (levelSpells[index].LevelType == SpellLevelType.BEHAVIOR)
                {
                    BehaviorLevel statsLevel = (BehaviorLevel)(levelSpells[index]);
                    statsLevel.OnUpgradeGain();
                }

            }

            // spellLevel++;
            spellExpNextLevel = spellExp + spellExpCountPerLevel;

        }

        public bool AddSpellExpPoint(int points)
        {
            spellExp += points;

            bool isLevelUp = spellExp >= spellExpNextLevel;


            return isLevelUp;
        }

        #endregion

        private bool IsStatBool(StatType statsType)
        {
            return (int)statsType - ((0 * 1000)) < 1000;
        }
        private bool IsStatInt(StatType statsType)
        {
            return (int)statsType - ((1 * 1000)) < 1000;
        }
        private bool IsStatFloat(StatType statsType)
        {
            return (int)statsType - ((2 * 1000)) < 1000;
        }
        private bool IsStatString(StatType statsType)
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

        public void AddToIntStats(StatType statsType, int val, float multiplier = 1)
        {
            if (!IsStatInt(statsType))
            {
                Debug.LogError("This stats isn't an integer");
                return;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    StatData statData = statDatas[i];
                    statData.val_int += val;
                    if (multiplier != 0) statData.val_int = (int)(statData.val_int * multiplier);
                    statDatas[i] = statData;
                    return;
                }
            }
        }
        public void AddToFloatStats(StatType statsType, float val, float multiplier = 1)
        {

            if (!IsStatFloat(statsType))
            {
                Debug.LogError("This stats isn't an float");
                return;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    StatData statData = statDatas[i];
                    statData.val_float += val;
                    if (multiplier != 0) statData.val_float *= multiplier;
                    statDatas[i] = statData;
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
                    statDatas[i] = statData;
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
                    statDatas[i] = statData;
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

            ManageStat(StatType.StackDuration, true);
            ManageStat(StatType.GainPerStack, true);
            ManageStat(StatType.Range, true);

            statTypes = new StatType[statDatas.Count];
            for (int i = 0; i < statTypes.Length; i++)
            {
                statTypes[i] = statDatas[i].stat;
            }


        }

        private void ManageStat(StatType statToCheck, bool isAdd, bool isVisible = false)
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
                statDatas.Add(statData);
            }

            return;
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
            ManageStat(StatType.SpellCanalisation, true);


            testResult = tagData.canalisationType == CanalisationType.HEAVY_CANALISATION;
            ManageStat(StatType.SpellCanalisation, true);
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

            testResult = tagData.EqualsSpellNature(SpellNature.DOT);
            ManageStat(StatType.HitFrequency, testResult);
            ManageStat(StatType.HitNumber, testResult, true);

            testResult = tagData.EqualsSpellNature(SpellNature.AREA) || tagData.EqualsSpellNature(SpellNature.AURA) || tagData.EqualsSpellNature(SpellNature.DOT);
            ManageStat(StatType.AreaTargetSimulately, testResult, true);

            testResult = tagData.spellNatureType == SpellNature.SUMMON && tagData.EqualsSpellNature(SpellNature.PROJECTILE);
            ManageStat(StatType.AttackReload, testResult, true);

        }


        #endregion
    }
}