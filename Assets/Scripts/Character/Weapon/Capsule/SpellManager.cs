using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;


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

    public static List<int> idFamilyBan = new List<int>();

    public static int GetElementRandomSpellIndex(GameElement element)
    {
        int index = -1;


        if (m_capsulePool.Count == 0)
        {
            Debug.LogWarning("No spell left");
            return index;
        }

        List<int> m_capsuleElementQuantity = new List<int>();
        for (int i = 0; i < m_capsulePool.Count; i++)
        {
            SpellSystem.SpellProfil spellProfil = instance.spellProfils[m_capsulePool[i]];
            if (spellProfil.tagData.element == element)
            {
                m_capsuleElementQuantity.Add(m_capsulePool[i]);
            }
        }

        for (int i = 0; i < m_capsuleElementQuantity.Count; i++)
        {
            for (int j = 0; j < idFamilyBan.Count; j++)
            {
                SpellSystem.SpellProfil spellProfil = instance.spellProfils[m_capsuleElementQuantity[i]];
                if (spellProfil.idFamily == 0) 
                    continue;
                if (spellProfil.idFamily == idFamilyBan[j])
                {
                    m_capsuleElementQuantity.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }
       

        if (m_capsuleElementQuantity.Count == 0)
        {
            Debug.LogWarning("No " + element.ToString() + " spell left");
            return GetRandomSpellIndex();
        }

        int listIndex = Random.Range(0, m_capsuleElementQuantity.Count);
        index = m_capsuleElementQuantity[listIndex];

        return index;

    }

    public static int GetRandomSpellIndexWithoutOneElememt(GameElement element)
    {
        int index = -1;


        if (m_capsulePool.Count == 0)
        {
            Debug.LogWarning("No spell left");
            return index;
        }

        List<int> m_capsuleElementQuantity = new List<int>();
        for (int i = 0; i < m_capsulePool.Count; i++)
        {
            SpellSystem.SpellProfil spellProfil = instance.spellProfils[m_capsulePool[i]];
            if (spellProfil.tagData.element != element)
            {
                m_capsuleElementQuantity.Add(m_capsulePool[i]);
            }
        }

        for (int i = 0; i < m_capsuleElementQuantity.Count; i++)
        {
            for (int j = 0; j < idFamilyBan.Count; j++)
            {
                SpellSystem.SpellProfil spellProfil = instance.spellProfils[m_capsuleElementQuantity[i]];
                if (spellProfil.idFamily == 0)
                    continue;
                if (spellProfil.idFamily == idFamilyBan[j])
                {
                    m_capsuleElementQuantity.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }

        if (m_capsuleElementQuantity.Count == 0)
        {
            Debug.LogWarning("No " + element.ToString() + " spell left");
            return GetRandomSpellIndex();
        }

        int listIndex = Random.Range(0, m_capsuleElementQuantity.Count);
        index = m_capsuleElementQuantity[listIndex];

        return index;
    }


    public static int GetRandomSpellIndex(bool activeRemove = false)
    {
        int index = -1;

        if (m_capsulePool.Count == 0)
        {
            Debug.LogWarning("No spell left");
            return index;
        }

        List<int> m_spellQuantity = new List<int>();
        for (int i = 0; i < m_capsulePool.Count; i++)
        {
            bool isValid = true;
            for (int j = 0; j < idFamilyBan.Count; j++)
            {
                SpellSystem.SpellProfil spellProfil = instance.spellProfils[m_capsulePool[i]];

                if (spellProfil.idFamily == idFamilyBan[j])
                {
                    isValid =false;
                    break;
                }
            }

            if(isValid) m_spellQuantity.Add(m_capsulePool[i]);
        }

        int listIndex = Random.Range(0, m_spellQuantity.Count);
        index = m_spellQuantity[listIndex];
        if (activeRemove) m_spellQuantity.RemoveAt(listIndex);

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
