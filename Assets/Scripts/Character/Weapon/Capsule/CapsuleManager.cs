using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleManager : MonoBehaviour
{
    public CapsuleSystem.Capsule[] capsules;

    public CapsuleSystem.CapsuleAttackInfo[] attackInfo;
    public CapsuleSystem.CapsuleBuffInfo[] buffInfos;


    public static CapsuleManager instance;
    public static int capsuleCount;

    public static List<int> m_capsulePool =  new List<int>();

    private int indexAttackInfo;
    private int indexBuffInfo;
    
    public static int GetRandomCapsuleIndex()
    {
        int index = -1;

        if (m_capsulePool.Count == 0)
        {
            Debug.LogWarning("Altar don't have spell");
            return index;
        }

        int listIndex = Random.Range(0, m_capsulePool.Count);
        index = m_capsulePool[listIndex];
        m_capsulePool.RemoveAt(listIndex);

        return index;
    }

    public static void RemoveSpecificSpellFromSpellPool(int index)
    {
        m_capsulePool.Remove(index);
    }

    public int GetCapsuleIndex(CapsuleSystem.Capsule capsule)
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
            if(capsules[i].type == CapsuleSystem.CapsuleType.ATTACK)
            {
                capsules[i] = new CapsuleSystem.CapsuleAttack(attackInfo[indexAttackInfo]);
                indexAttackInfo++;
            }
            if (capsules[i].type == CapsuleSystem.CapsuleType.BUFF)
            {
                capsules[i] = new CapsuleSystem.CapsuleBuff(buffInfos[indexBuffInfo]);
                indexBuffInfo++;
            }
        }
    }

}
