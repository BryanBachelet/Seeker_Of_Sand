using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;

public class SpellBarInfoUI : MonoBehaviour
{
    public TooltipTrigger[] m_spellTooltipArray = new TooltipTrigger[4];
    private Character.CharacterShoot m_characterShoot;

    private string[] spellText = { "Damage :", "Size :", "Speed", "Projectile:", "ShootProjectile", "PiercingMax" };
    // Start is called before the first frame update
    void Start()
    {
        m_characterShoot = FindAnyObjectByType<Character.CharacterShoot>();
        m_spellTooltipArray = GetComponentsInChildren<TooltipTrigger>();
        for (int i = 0; i < m_spellTooltipArray.Length; i++)
        {
            m_spellTooltipArray[i].OnEnterData += GetSpellInfo;
        }
    }


    public void GetSpellInfo(TooltipEventData eventData)
    {
        if (m_characterShoot.maxSpellIndex <= eventData.index)
        {
            m_spellTooltipArray[eventData.index].content = "";
            m_spellTooltipArray[eventData.index].header = "";
            return;
        }
        int capsuleIndex = m_characterShoot.spellIndex[eventData.index];
        string headerText = m_characterShoot.m_capsuleManager.spellProfils[capsuleIndex].name;
        string contextText = m_characterShoot.m_capsuleManager.spellProfils[capsuleIndex].description + "\n"+ m_characterShoot.spellProfils[eventData.index].DebugStat();

        m_spellTooltipArray[eventData.index].content = contextText;
        m_spellTooltipArray[eventData.index].header = headerText;
    }

}
