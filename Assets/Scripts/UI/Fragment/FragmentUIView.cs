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
        [SerializeField] private Image m_nameText;

        public void UpdateInteface(ArtefactsInfos artefactsInfos)
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;
          
            int indexElement = (int)artefactsInfos.gameElement;
            Debug.Assert(indexElement == 0, "Artefact "+ artefactsInfos.nameArtefact +  " doesn't have element");
           
            // Adapt the element index for sprite array size;
            indexElement--;

            m_backgroundColorImg.sprite = instanceResources.backgroundSprite[(int)artefactsInfos.gameElement];
            m_elementImg.sprite = instanceResources.elementSprite[(int)artefactsInfos.gameElement];
            m_borderColorImg.sprite = instanceResources.raretySprite[(int)artefactsInfos.levelTierFragment];
            m_spriteImg.sprite = artefactsInfos.icon;
        }
       
    }
}
