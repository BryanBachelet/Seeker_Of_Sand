using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using SpellSystem;
using UnityEditor;
using BorsalinoTools;
using Unity.VisualScripting;



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
    public bool IsAddingTag;
    public SpellTagOrder TagToGive;

    public PlayerEffectStats<StatDataUpgrade> gameEffectStats;

    public UpgradeObject Clone()
    {
        UpgradeObject upgradeObject = Instantiate(this);

        upgradeObject.gameEffectStats = gameEffectStats.Clone();

        return upgradeObject;
    }

    public void Apply(SpellSystem.SpellProfil spellProfil)
    {
        bool isTagAdd = false;
        if (IsAddingTag)
        {
            isTagAdd = !spellProfil.gameEffectStats.tagData.HasAlreadyTheValue(TagToGive, gameEffectStats.tagData.GetIndexTagValue(TagToGive));
            
            if (isTagAdd)
            {
                spellProfil.gameEffectStats.tagData.SetIndexTagValue(TagToGive, gameEffectStats.tagData.GetIndexTagValue(TagToGive),true);
                spellProfil.gameEffectStats.UpdateStatistics();
            }
        }
        spellProfil.gameEffectStats.ChangeStats(gameEffectStats, isTagAdd);
    }

    public bool IsValidUpgrade(SpellSystem.SpellProfil spellProfil)
    {
        return gameEffectStats.tagData.HasOneTagSimilar(spellProfil.gameEffectStats.tagData);
    }

    public bool IsAllTagMatching(SpellSystem.SpellProfil spellProfil)
    {
        TagData tagData = gameEffectStats.tagData;
        tagData.SetIndexTagValue(TagToGive, 0);
        return gameEffectStats.tagData.HasAllTagSimilar(tagData);
    }

    public bool IsAllTagCorresponding(int[] spellValidTag)
    {
        int[] upgradeValidTag = gameEffectStats.tagData.GetValidTag();

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
        for (int i = 0; i < gameEffectStats.statTypes.Length; i++)
        {
            if (gameEffectStats.statTypes[i] == statType) return true;
        }
        return false;
    }

    public string PreviewApplyValue(StatData statData)
    {
        for (int i = 0; i < gameEffectStats.statDatas.Count; i++)
        {
            if (statData.stat == gameEffectStats.statDatas[i].stat)
            {

                if (gameEffectStats.IsStatBool(gameEffectStats.statTypes[i]))
                {
                    return (gameEffectStats.statDatas[i].val_bool).ToString();
                }

                if (gameEffectStats.IsStatInt(gameEffectStats.statTypes[i]))
                {
                    return (statData.val_int + "-->" + (statData.val_int + gameEffectStats.statDatas[i].val_int)).ToString();
                }



                if (gameEffectStats.IsStatFloat(gameEffectStats.statTypes[i]))
                {
                    return (statData.val_float + "-->" + (statData.val_float + gameEffectStats.statDatas[i].val_float)).ToString();
                }
            }




        }

        return "";
    }

    public string PreviewNewValue(int index)
    {

        if (gameEffectStats.IsStatBool(gameEffectStats.statTypes[index]))
        {
            return (gameEffectStats.statDatas[index].val_bool).ToString();
        }

        if (gameEffectStats.IsStatInt(gameEffectStats.statTypes[index]))
        {
            return (0 + "-->" + (gameEffectStats.statDatas[index].val_int)).ToString();
        }

        if (gameEffectStats.IsStatFloat(gameEffectStats.statTypes[index]))
        {
            return (0 + "-->" + (gameEffectStats.statDatas[index].val_float)).ToString();
        }
        return "";
    }

    public void OnValidate()
    {
        //gameEffectStats.UpdateOnlyTag();
        //EditorUtility.SetDirty(this);
    }

}
