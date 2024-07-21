using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.GameEnum;


[System.Serializable]
public struct TagData
{
    public GameElement element;
    public BuffType type;
    public SpellNature spellNatureType;
    public SpellNature spellNatureType1;
    public SpellProjectileTrajectory spellProjectileTrajectory;
    public CanalisationType canalisationType;
    public SpellMovementBehavior spellMovementBehavior;
    public DamageTrigger damageTriggerType;
    public SpellParticualarity spellParticualarity;
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
                return (int)spellProjectileTrajectory;
                break;
            case SpellTagOrder.SpellParticualarity:
                return (int)spellParticualarity;
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

}


namespace SpellSystem
{



    [System.Serializable]
    public struct StatData
    {
        public StatType stat;
        public ValueType valueType;
        public int val_int;
        public float val_float;
        public string val_string;
        public bool val_bool;

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
        public GameObject VFX;
        public Material matToUse;

        [HideInInspector] public int level;
        [Header("Tag Parameters")]
        public TagData tagData;

        [Space]
        public List<StatData> statDatas = new List<StatData>();
        private StatType[] statTypes = new StatType[0];

        public SpellProfil Clone()
        {
            SpellProfil spellProfil = Instantiate(this);


            spellProfil.statTypes = new StatType[spellProfil.statDatas.Count];
            for (int i = 0; i < statTypes.Length; i++)
            {
                spellProfil.statTypes[i] = spellProfil.statDatas[i].stat;
            }

            return spellProfil;
        }

        private bool IsStatInt(StatType statsType)
        {
            return (int)statsType - ((1 * 1000)) < 1000;
        }
        private bool IsStatFloat(StatType statsType)
        {
            return (int)statsType - ((2 * 1000)) < 1000;
        }
        private bool IsStatBool(StatType statsType)
        {
            return (int)statsType - ((0 * 1000)) < 1000;
        }
        private bool IsStatString(StatType statsType)
        {
            return (int)statsType - ((3 * 1000)) < 1000;
        }

        public int GetIntStat(StatType statsType)
        {
            if(!IsStatInt(statsType))
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
            if (!IsStatFloat(statsType))
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

        public void AddToIntStats(StatType statsType ,int val)
        {
            if (!IsStatInt(statsType))
            {
                Debug.LogError("This stats isn't an integer");
                return ;
            }

            for (int i = 0; i < statTypes.Length; i++)
            {
                if (statsType == statTypes[i])
                {
                    StatData statData = statDatas[i];
                    statData.val_int += val;
                    statDatas[i] = statData;
                    return;
                }
            }
        }
        public void AddToFloatStats(StatType statsType, float val)
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



        //TODO : Debug function
        public string DebugStat()
        {
            return "";
        }


        #region Stats setup Functions
#if UNITY_EDITOR
        public void UpdateStatistics()
        {

            bool testResult = tagData.type == BuffType.DAMAGE_SPELL;
            ManageStat(StatType.Damage, testResult);


            SetupSpellNatureStats(tagData.spellNatureType, tagData.spellNatureType1);
            SetupUpgradeSensitivity();
            SetupSpellParaticularity();
            SetupCanalisationType();
            SetupMovementType();
            SetupSpellMouvement();

            ManageStat(StatType.StackDuration, true);
            ManageStat(StatType.GainPerStack, true);

        }

        private void ManageStat(StatType statToCheck, bool isAdd)
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
        }

        private void SetupSpellParaticularity()
        {
            bool testResult = tagData.spellParticualarity == SpellParticualarity.Explosion;
            ManageStat(StatType.SizeExplosion, testResult);
            ManageStat(StatType.DamageAdditionel, testResult);

            testResult = tagData.spellParticualarity == SpellParticualarity.Delayed;
            ManageStat(StatType.TimeDelay, testResult);

            testResult = tagData.spellParticualarity == SpellParticualarity.Piercing;
            ManageStat(StatType.Piercing, testResult);

            testResult = tagData.spellParticualarity == SpellParticualarity.Bouncing;
            ManageStat(StatType.BounceNumber, testResult);


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
            testResult = ( type == SpellNature.PROJECTILE || type2 == SpellNature.PROJECTILE);
            ManageStat(StatType.LifeTime, testResult);
            ManageStat(StatType.Range, testResult);
            ManageStat(StatType.Projectile, testResult);
            ManageStat(StatType.ShootNumber, testResult);
            ManageStat(StatType.ShootAngle, testResult);
            ManageStat(StatType.TimeBetweenShot, testResult);
            ManageStat(StatType.CompleteShotTime, testResult);


            bool testResult2 = tagData.spellProjectileTrajectory == SpellProjectileTrajectory.CURVE;
            ManageStat(StatType.AngleTrajectory, testResult2 && testResult);
            ManageStat(StatType.TrajectoryTimer, testResult2 && testResult);

            testResult = (type == SpellNature.AREA || type2 == SpellNature.AREA);
            ManageStat(StatType.Size, testResult);
            ManageStat(StatType.SizeMuplitiplicator, testResult);

            testResult = (type == SpellNature.AURA || type2 == SpellNature.AURA);
            ManageStat(StatType.Size, testResult);
            ManageStat(StatType.SizeMuplitiplicator, testResult);

            testResult = (type == SpellNature.SUMMON || type2 == SpellNature.SUMMON);
            ManageStat(StatType.MaxSummon, testResult);
            ManageStat(StatType.SummonSimultanely, testResult);

            testResult = (type == SpellNature.DOT || type2 == SpellNature.DOT);
            ManageStat(StatType.HitFrequency, testResult);
            ManageStat(StatType.HitNumber, testResult);

        }

#endif
        #endregion
    }
}