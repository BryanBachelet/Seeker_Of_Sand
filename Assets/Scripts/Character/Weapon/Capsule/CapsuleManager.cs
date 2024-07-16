using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleManager : MonoBehaviour
{
    public SpellSystem.Capsule[] capsules;
    public SpellSystem.CapsuleAttackInfo[] attackInfo;
    public SpellSystem.CapsuleBuffInfo[] buffInfos;
    public SpellSystem.SpellProfil[] spellProfils;

    public static CapsuleManager instance;
    public static int capsuleCount;

    public static List<int> m_capsulePool =  new List<int>();

    private int indexAttackInfo;
    private int indexBuffInfo;
    
    public static int GetRandomCapsuleIndex(bool activeRemove =false)
    {
        int index = -1;

        if (m_capsulePool.Count == 0)
        {
            Debug.LogWarning("Altar don't have spell");
            return index;
        }

        int listIndex = Random.Range(0, m_capsulePool.Count);
        index = m_capsulePool[listIndex];
      if(activeRemove)  m_capsulePool.RemoveAt(listIndex);

        return index;
    }

    public static void RemoveSpecificSpellFromSpellPool(int index)
    {
        m_capsulePool.Remove(index);
    }

    public int GetCapsuleIndex(SpellSystem.Capsule capsule)
    {
        int index = -1;
        for (int i = 0; i < capsules.Length; i++)
        {
            if(capsule ==capsules[i])
            {
                return i;
            }
        }
        return index;
    }

    public void Awake()
    {
        instance = this; // Singleton Variable

        for (int i = 0; i < capsules.Length; i++)
        {
            m_capsulePool.Add(i);
        }

        CreateInfo();

        capsuleCount = capsules.Length;
    }

    public void CreateInfo()
    {
        for (int i = 0; i < capsules.Length; i++)
        {
            if(capsules[i].type == SpellSystem.CapsuleType.ATTACK)
            {
                capsules[i] = new SpellSystem.CapsuleAttack(attackInfo[indexAttackInfo]);
                indexAttackInfo++;
            }
            if (capsules[i].type == SpellSystem.CapsuleType.BUFF)
            {
                capsules[i] = new SpellSystem.CapsuleBuff(buffInfos[indexBuffInfo]);
                indexBuffInfo++;
            }
        }
    }

    public void OnValidate()
    {
        List<SpellSystem.SpellProfil> spellProfilsList = new List<SpellSystem.SpellProfil>(spellProfils.Length);

        for (int i = 0; i < spellProfils.Length; i++)
        {
            if (spellProfils[i].id >= spellProfilsList.Count)
            {
                spellProfilsList.Add(spellProfils[i]);
                continue;
            }
            spellProfilsList.Insert(spellProfils[i].id, spellProfils[i]);
        }

        spellProfils = spellProfilsList.ToArray();
    }

}
