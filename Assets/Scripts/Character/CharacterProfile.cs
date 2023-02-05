using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface CharacterComponent
{
   public void InitComponentStat(CharacterStat stat);
}

public class CharacterProfile : MonoBehaviour
{
    private HealthSystem m_healthSystem;
    [SerializeField] public CharacterStat stats;
    [HideInInspector] public CharacterStat m_baseStat;
    private CharacterComponent[] m_characterComponent = new CharacterComponent[0];

    private void Start()
    {
        m_characterComponent = GetComponents<CharacterComponent>();
        m_baseStat = stats;
        ApplyStat(stats);
    }

    public void ApplyStat(CharacterStat newStat)
    {
        stats = newStat;
        for (int i = 0; i < m_characterComponent.Length; i++)
        {
            m_characterComponent[i].InitComponentStat(stats);
        }
    }



}
