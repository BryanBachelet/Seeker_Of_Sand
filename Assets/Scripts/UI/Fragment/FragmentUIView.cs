using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{
    public class FragmentUIView : MonoBehaviour
    {
        [SerializeField] private Image m_backgroundColorImg;
        [SerializeField] private Image m_borderColorImg;
        [SerializeField] private Image m_nameImg;
        [SerializeField] private Image m_spriteImg;
        [SerializeField] private Image m_elementImg;
        [SerializeField] private TMPro.TMP_Text m_nameText;
        public TooltipTrigger tooltipTrigger;

        public void UpdateInteface(ArtefactsInfos artefactsInfos)
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;

            // Index that point the base element (Water,Air,Fire,Earth)
            int indexBaseElement = GeneralTools.GetElementalArrayIndex(artefactsInfos.gameElement, true);
            int indexElement = (int)artefactsInfos.gameElement;
            if (indexBaseElement <= 0)
            {
                GameElement firstElement = GeneralTools.GetFirstBaseElement(artefactsInfos.gameElement);
                indexBaseElement = GeneralTools.GetElementalArrayIndex(firstElement, true);
            }


            //Debug.Assert(indexBaseElement != 0, "Artefact " + artefactsInfos.nameArtefact + " doesn't have element");

            m_backgroundColorImg.sprite = instanceResources.backgroundSprite[indexElement];
            m_elementImg.sprite = instanceResources.elementSprite[indexBaseElement];
            m_borderColorImg.sprite = instanceResources.raretySprite[(int)artefactsInfos.levelTierFragment];
            m_spriteImg.sprite = artefactsInfos.icon;
            m_nameText.text = artefactsInfos.nameArtefact;

            tooltipTrigger.header = artefactsInfos.nameArtefact;
            tooltipTrigger.content = artefactsInfos.description;
            tooltipTrigger.IsActive = true;
        }

        public void ResetFragmentUIView()
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;


            m_backgroundColorImg.sprite = instanceResources.backgroundSprite[0];
            m_elementImg.sprite = instanceResources.elementSprite[0];
            m_borderColorImg.sprite = instanceResources.raretySprite[0];
            m_spriteImg.sprite = null;
            m_nameText.text = "";

            tooltipTrigger.IsActive = false;
            tooltipTrigger.HideTooltip();
        }

    }
}
