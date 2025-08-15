using GuerhoubaGames.Character;
using GuerhoubaGames.UI;
using UnityEngine;

public class SpellBarInfoUI : MonoBehaviour
{
    public TooltipTrigger[] m_spellTooltipArray = new TooltipTrigger[4];
    private CharacterShoot m_characterShoot;

    private string[] spellText = { "Damage :", "Size :", "Speed", "Projectile:", "ShootProjectile", "PiercingMax" };
    // Start is called before the first frame update
    void Start()
    {
        m_characterShoot = FindAnyObjectByType<CharacterShoot>();
        if (m_spellTooltipArray[0] == null)
        {
            m_spellTooltipArray = GetComponentsInChildren<TooltipTrigger>();
        }
        for (int i = 0; i < m_spellTooltipArray.Length; i++)
        { 
            m_spellTooltipArray[i].IsActive = true;
            m_spellTooltipArray[i].TooltipEventData.index = i;
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
        int capsuleIndex = m_characterShoot.spellEquip[eventData.index];
        string headerText = m_characterShoot.spellProfils[capsuleIndex].name;
        string contextText = "Level : " + m_characterShoot.spellProfils[capsuleIndex].spellExp + "\n" + m_characterShoot.spellProfils[capsuleIndex].description + "\n"+ m_characterShoot.spellProfils[capsuleIndex].gameEffectStats.DebugStat();


        int countSpellTier = m_characterShoot.spellProfils[capsuleIndex].levelSpellsProfiles.Length;

        m_spellTooltipArray[eventData.index].additionnalDatas = new TooltipAdditionnalData[countSpellTier];


        for (int i = 0; i < countSpellTier; i++)
        {
            m_spellTooltipArray[eventData.index].additionnalDatas[i].additionnalTooltipDisplay.header = m_characterShoot.spellProfils[capsuleIndex].levelSpellsProfiles[i].nameLevelUprade;
            m_spellTooltipArray[eventData.index].additionnalDatas[i].additionnalTooltipDisplay.content = m_characterShoot.spellProfils[capsuleIndex].levelSpellsProfiles[i].description;
        }

        m_spellTooltipArray[eventData.index].content = contextText;
        m_spellTooltipArray[eventData.index].header = headerText;
    }

}
