using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public bool updateList;
    public SpellSystem.SpellProfil[] spellProfils;

    public static SpellManager instance;
    public static int capsuleCount;

    public static List<int> m_capsulePool = new List<int>();

    public bool isDebugActive;
    private int indexAttackInfo;
    private int indexBuffInfo;

    public static int GetRandomCapsuleIndex(bool activeRemove = false)
    {
        int index = -1;

        if (m_capsulePool.Count == 0)
        {
            Debug.LogWarning("Altar don't have spell");
            return index;
        }

        int listIndex = Random.Range(0, m_capsulePool.Count);
        index = m_capsulePool[listIndex];
        if (activeRemove) m_capsulePool.RemoveAt(listIndex);

        return index;
    }


    public static void AddSpecificSpellFromSpellPool(int index)
    {
        m_capsulePool.Add(index);

    }
    public static void RemoveSpecificSpellFromSpellPool(int index)
    {
        if (m_capsulePool.Contains(index))
        {
            if (instance.isDebugActive)
                Debug.Log(instance.spellProfils[index].name);

            m_capsulePool.Remove(index);
        }
    }

    public int GetCapsuleIndex(SpellSystem.SpellProfil spellProfile)
    {
        int index = -1;
        for (int i = 0; i < spellProfils.Length; i++)
        {
            if (spellProfile == spellProfils[i])
            {
                return i;
            }
        }
        return index;
    }

    public void Awake()
    {
        instance = this; // Singleton Variable

        for (int i = 0; i < spellProfils.Length; i++)
        {
            m_capsulePool.Add(i);
        }

        CreateInfo();

        capsuleCount = spellProfils.Length;
    }

    public void CreateInfo()
    {


        // Clone scriptable to avoid permanent modification
        for (int i = 0; i < spellProfils.Length; i++)
        {
            spellProfils[i] = spellProfils[i].Clone();
        }


    }

    public void OnValidate()
    {
        if (!updateList) return;
        List<SpellSystem.SpellProfil> spellProfilsList = new List<SpellSystem.SpellProfil>(spellProfils.Length);
        SpellComparer spellComparer = new SpellComparer();
        for (int i = 0; i < spellProfils.Length; i++)
        {
            if (spellProfils[i].id >= spellProfilsList.Count)
            {
                spellProfilsList.Add(spellProfils[i]);
                continue;
            }
            spellProfilsList.Insert(spellProfils[i].id, spellProfils[i]);
        }

        spellProfilsList.Sort(spellComparer);
        spellProfils = spellProfilsList.ToArray();
    }


}

public class SpellComparer : IComparer<SpellSystem.SpellProfil>
{
    public int Compare(SpellProfil x, SpellProfil y)
    {
        if (x.id == y.id) return 0;
        if (x.id < y.id) return -1;

        return 1;

    }
}
