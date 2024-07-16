using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum StatType
{
    HP = 1001,
    DAMAGE = 1002,
    NAME = 3001,
    Description = 3002,


}

public enum ValueType
{
    BOOL = 0,
    INT = 1000,
    FLOAT = 2000,
    STRING = 3000,
}

public enum BuffType
{
    DAMAGE_SPELL,
    BUFF_SPELL,
}

public enum SpellObjectType
{
    AURA= 0,
    PROJECTILE=1,
    AREA=2,
    INVOCATION =3,
}



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

[CreateAssetMenu(fileName = "spellProfil", menuName = "Spell")]
public class SpellProfil : ScriptableObject
{
    public BuffType type;
    public SpellObjectType spellObjectType;
    public List<StatData> statDatas = new List<StatData>();


    public void Clone()
    {

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
        bool testResult = type == BuffType.DAMAGE_SPELL;
        ManageStat(StatType.DAMAGE, testResult);
        ManageStat(StatType.HP, testResult);

        testResult = spellObjectType == SpellObjectType.AURA;
        ManageStat(StatType.Description, testResult);
    }

}
