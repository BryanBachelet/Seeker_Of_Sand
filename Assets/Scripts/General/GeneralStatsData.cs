using GuerhoubaGames.GameEnum;
using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "GeneralStatData", menuName = "ScriptableObjects/Artefacts Data", order = 2)]
public class GeneralStatData : ScriptableObject
{
    public CharacterStat CharacterStat;

    public List<StatData> statDatas = new List<StatData>();
    public List<StatType> statTypes = new List<StatType>();
    private List<StatType> statTypesTemp = new List<StatType>();
#if UNITY_EDITOR

#endif


    public void OnValidate()
    {
        UpdateStats();
    }
    public void UpdateStats()
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            ManageStat(statTypes[i], statTypesTemp.Contains(statTypes[i]), false);
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


}
