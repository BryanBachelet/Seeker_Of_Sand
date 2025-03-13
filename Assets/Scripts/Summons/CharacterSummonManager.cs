using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSummonManager : MonoBehaviour
{

    public Dictionary<int, List<GameObject>> m_summonSpellDictionnary = new Dictionary<int, List<GameObject>>();


    public int GetSummonCount(int id)
    {
     
        return m_summonSpellDictionnary[id].Count;
    }



    public void RemoveSummon(int id, GameObject obj)
    {

        int count = m_summonSpellDictionnary[id].Count;
        if(count == 1)
        {
            m_summonSpellDictionnary.Remove(id);
        }
        else
        {
            m_summonSpellDictionnary[id].Remove(obj);
        }
     
    }


public void CallSpecialAbility(int id)
    {
        List<GameObject> summonList = m_summonSpellDictionnary[id];
        for (int i = 0; i < summonList.Count; i++)
        {
            summonList[i].GetComponent<SpellSystem.SummonsMeta>().OnSpecialSkill.Invoke();
        }
    }
}
