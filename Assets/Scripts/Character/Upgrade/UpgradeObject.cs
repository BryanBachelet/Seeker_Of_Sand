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
        bool test = false;

        if (spellProfil.tagData.element == tagData.element) return true;
        if (spellProfil.tagData.type == tagData.type) return true;
        if (spellProfil.tagData.spellNatureType == tagData.spellNatureType) return true;
        if (spellProfil.tagData.spellProjectileTrajectory == tagData.spellProjectileTrajectory) return true;
        if (spellProfil.tagData.canalisationType == tagData.canalisationType) return true;
        if (spellProfil.tagData.spellMovementBehavior == tagData.spellMovementBehavior) return true;
        if (spellProfil.tagData.damageTriggerType == tagData.damageTriggerType) return true;
        if (spellProfil.tagData.spellParticualarity == tagData.spellParticualarity) return true;
        if (spellProfil.tagData.mouvementBehaviorType == tagData.mouvementBehaviorType) return true;


        return false;
    }

}
