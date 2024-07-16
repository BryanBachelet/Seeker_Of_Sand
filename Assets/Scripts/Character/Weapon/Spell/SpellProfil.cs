using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GuerhoubaGames.GameEnum;



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
        public GameElement element;
        public BuffType type;
        public SpellObjectType spellObjectType;
        public SpellProjectileTrajectory spellProjectileTrajectory;

        [Space]
        public List<StatData> statDatas = new List<StatData>();


        public SpellProfil Clone()
        {
            SpellProfil spellProfil = ScriptableObject.CreateInstance<SpellProfil>();
            spellProfil = this;
            return spellProfil;
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


        public void UpdateStatistics()
        {
            ManageStat(StatType.SpellCanalisation, true);
            
            bool testResult = type == BuffType.DAMAGE_SPELL;
            ManageStat(StatType.Damage, testResult);

            testResult = spellObjectType == SpellObjectType.PROJECTILE;
            ManageStat(StatType.LifeTime, testResult);
            ManageStat(StatType.Speed, testResult);
            ManageStat(StatType.Range, testResult);
            ManageStat(StatType.Projectile, testResult);
            ManageStat(StatType.ShootNumber, testResult);
            ManageStat(StatType.ShootAngle, testResult);
            ManageStat(StatType.CompleteShotTime, testResult);
            ManageStat(StatType.Size, testResult);
            ManageStat(StatType.SizeMuplitiplicator, testResult);
            if(testResult)
            {
                bool testResult2 = spellProjectileTrajectory == SpellProjectileTrajectory.CURVE;
                ManageStat(StatType.AngleTrajectory, testResult2);
                ManageStat(StatType.TrajectoryTimer, testResult2);
            }

            testResult = spellObjectType == SpellObjectType.AREA;
            ManageStat(StatType.LifeTime, testResult);
            ManageStat(StatType.Size, testResult);
            ManageStat(StatType.SizeMuplitiplicator, testResult);
            ManageStat(StatType.HitTick, testResult);

            testResult = spellObjectType == SpellObjectType.AURA;
            ManageStat(StatType.LifeTime, testResult);
            ManageStat(StatType.Size, testResult);
            ManageStat(StatType.SizeMuplitiplicator, testResult);
            ManageStat(StatType.HitTick, testResult);

            ManageStat(StatType.StackDuration, true);
            ManageStat(StatType.GainPerStack, true);

        }

    }
}