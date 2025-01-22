using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using Character;

public class FragmentUIInfo : MonoBehaviour
{

    public TooltipTrigger[] m_spellTooltipArray = new TooltipTrigger[4];
    private CharacterArtefact m_characterArtefact;


    // Start is called before the first frame update
    void Start()
    {
        m_characterArtefact = FindAnyObjectByType<CharacterArtefact>();
        //m_spellTooltipArray = GetComponentsInChildren<TooltipTrigger>();
        for (int i = 0; i < m_spellTooltipArray.Length; i++)
        {
            m_spellTooltipArray[i].TooltipEventData.index = i;
            m_spellTooltipArray[i].OnEnterData += GetArtefactInfo;
        }
    }



    public void GetArtefactInfo(TooltipEventData eventData)
    {
        if (m_characterArtefact.artefactsList.Count <= eventData.index)
        {
            m_spellTooltipArray[eventData.index].content = "";
            m_spellTooltipArray[eventData.index].header = "";
            return;
        }

        string headerText = m_characterArtefact.artefactsList[eventData.index].nameArtefact;
        string contextText = m_characterArtefact.artefactsList[eventData.index].descriptionResult;

        m_spellTooltipArray[eventData.index].content = contextText;
        m_spellTooltipArray[eventData.index].header = headerText;
    }

}
