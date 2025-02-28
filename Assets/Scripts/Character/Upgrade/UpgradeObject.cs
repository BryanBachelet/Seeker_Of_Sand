using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using SpellSystem;



[CreateAssetMenu(fileName = "Upgrade Profil", menuName = "Upgrades/Upgrades Profil")]
public class UpgradeObject : ScriptableObject
{

    public string name;
    public int id;
    [TextArea]
    public string description;
    public Sprite spell_Icon;
    public int indexSpellLink;
    public bool IsMultiTagUpgrade;

    public TagData tagData;
    [Space]
    public List<StatData> statDatas = new List<StatData>();
    private StatType[] statTypes = new StatType[0];

    public UpgradeObject Clone()
    {
        UpgradeObject spellProfil = ScriptableObject.CreateInstance<UpgradeObject>();
        spellProfil = this;

        statTypes = new StatType[statDatas.Count];
        for (int i = 0; i < statTypes.Length; i++)
        {
            statTypes[i] = statDatas[i].stat;
        }

        return spellProfil;
    }

    private bool IsStatInt(StatType statsType)
    {
        return ((int)statsType) - ((1 * 1000)) < 1000;
    }
    private bool IsStatFloat(StatType statsType)
    {
        return ((int)statsType) - ((2 * 1000)) < 1000;
    }

    private bool IsStatBool(StatType statsType)
    {
        return ((int)statsType) - ((0 * 1000)) < 1000;
    }
    private bool IsStatString(StatType statsType)
    {
        return ((int)statsType) - ((3 * 1000)) < 1000;
    }

    public void Apply(SpellSystem.SpellProfil spellProfil)
    {
        for (int i = 0; i < statTypes.Length; i++)
        {
            if (IsStatBool(statTypes[i]))
            {
                spellProfil.ChangBoolValue(statTypes[i], statDatas[i].val_bool);
                continue;
            }


            if (IsStatInt(statTypes[i]))
            {
                spellProfil.AddToIntStats(statTypes[i], statDatas[i].val_int);
                continue;
            }
            if (IsStatFloat(statTypes[i]))
            {
                spellProfil.AddToFloatStats(statTypes[i], statDatas[i].val_float);
                continue;
            }

            if (IsStatString(statTypes[i]))
            {
                spellProfil.ChangeStringStats(statTypes[i], statDatas[i].val_string);
                continue;
            }
        }


    }

    public bool IsValidUpgrade(SpellSystem.SpellProfil spellProfil)
    {

        if (spellProfil.tagData.element == tagData.element) return true;
        if (spellProfil.tagData.type == tagData.type) return true;
        if (spellProfil.tagData.EqualsSpellNature(tagData.spellNatureType)) return true;
        if (spellProfil.tagData.EqualsSpellNature(tagData.spellNatureType1)) return true;
        if (spellProfil.tagData.spellProjectileTrajectory == tagData.spellProjectileTrajectory) return true;
        if (spellProfil.tagData.canalisationType == tagData.canalisationType) return true;
        if (spellProfil.tagData.spellMovementBehavior == tagData.spellMovementBehavior) return true;
        if (spellProfil.tagData.damageTriggerType == tagData.damageTriggerType) return true;
        if (spellProfil.tagData.EqualsSpellParticularity(tagData.spellParticualarity)) return true;
        if (spellProfil.tagData.EqualsSpellParticularity(tagData.spellParticualarity1)) return true;
        if (spellProfil.tagData.EqualsSpellParticularity(tagData.spellParticualarity2)) return true;
        if (spellProfil.tagData.mouvementBehaviorType == tagData.mouvementBehaviorType) return true;


        return false;
    }

    public bool IsAllTagMatching(SpellSystem.SpellProfil spellProfil)
    {
        if ((int)tagData.element != 0 && spellProfil.tagData.element != tagData.element) return false;
        if ((int)tagData.type != 0 && spellProfil.tagData.type != tagData.type) return false;
        if ((int)tagData.spellNatureType != 0 && !spellProfil.tagData.EqualsSpellNature(tagData.spellNatureType)) return false;
        if ((int)tagData.spellNatureType1 != 0 && !spellProfil.tagData.EqualsSpellNature(tagData.spellNatureType1)) return false;
        if ((int)tagData.spellProjectileTrajectory != 0 && spellProfil.tagData.spellProjectileTrajectory != tagData.spellProjectileTrajectory) return false;
        if ((int)tagData.canalisationType != 0 && spellProfil.tagData.canalisationType != tagData.canalisationType) return false;
        if ((int)tagData.spellMovementBehavior != 0 && spellProfil.tagData.spellMovementBehavior != tagData.spellMovementBehavior) return false;
        if ((int)tagData.damageTriggerType != 0 && spellProfil.tagData.damageTriggerType != tagData.damageTriggerType) return false;
        if ((int)tagData.spellParticualarity != 0 && !spellProfil.tagData.EqualsSpellParticularity(tagData.spellParticualarity)) return false;
        if ((int)tagData.spellParticualarity1 != 0 && !spellProfil.tagData.EqualsSpellParticularity(tagData.spellParticualarity1)) return false;
        if ((int)tagData.spellParticualarity2 != 0 && !(spellProfil.tagData.EqualsSpellParticularity(tagData.spellParticualarity2))) return false;
        if ((int)tagData.mouvementBehaviorType != 0 && spellProfil.tagData.mouvementBehaviorType != tagData.mouvementBehaviorType) return false;

        return true;
    }

    public bool IsAllTagCorresponding(int[] spellValidTag)
    {
        int[] upgradeValidTag = tagData.GetValidTag();

        for (int i = 0; i < upgradeValidTag.Length; i++)
        {
            if (upgradeValidTag[i] == 0) continue;

            if (upgradeValidTag[i] != spellValidTag[i])
            {
                return false;
            }
        }

        return true;
    }

    public bool HasThisStat(StatType statType)
    {
        for (int i = 0; i < statTypes.Length; i++)
        {
            if (statTypes[i] == statType) return true;
        }
        return false;
    }

    public string PrewiewApplyValue(StatData statData)
    {
        for (int i = 0; i < statDatas.Count; i++)
        {
            if (statData.stat == statDatas[i].stat)
            {

                if (IsStatBool(statTypes[i]))
                {
                    return ( statDatas[i].val_bool).ToString();
                }

                if (IsStatInt(statTypes[i]))
                {
                    return (statData.val_int + "-->"+ (statData.val_int + statDatas[i].val_int)).ToString();
                }


                if (IsStatFloat(statTypes[i]))
                {
                    return (statData.val_float + "-->" +(statData.val_float + statDatas[i].val_float)).ToString();
                }
            }
        }

        return "";
    }

}
