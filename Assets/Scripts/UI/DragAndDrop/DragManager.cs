using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GuerhoubaGames.GameEnum;

namespace GuerhoubaGames.UI
{
    public struct DragData
    {
        public CharacterObjectType currentType;
        public int  indexObj;
    }


    public class DragManager : MonoBehaviour
    {
       public static DragManager instance;

        public bool isDragging;
        public Image imageToDrag;
        public DragData dragData;

        private RectTransform m_imageRectTransform;
        private GameObject m_playerGO;
        private UI_Fragment_Tooltip m_fragmentToolTip;


        public void StartDragManager()
        {
            instance = this;
            isDragging = false;
            m_imageRectTransform = imageToDrag.GetComponent<RectTransform>();
            m_playerGO = GameState.s_playerGo;
            imageToDrag.gameObject.SetActive(false);
            m_fragmentToolTip = GameState.m_uiManager.GetComponent<UI_Fragment_Tooltip>();
        }

        public void ActiveDragDrop(CharacterObjectType type, int index, Vector2 mousePosition)
        {
           
            switch (type)
            {
                case CharacterObjectType.SPELL:
                    Character.CharacterShoot characterShoot = m_playerGO.GetComponent<Character.CharacterShoot>();
                    int spellindex = characterShoot.spellEquip[index];
                    if(spellindex == -1)
                    {
                        return;
                    }
                    imageToDrag.sprite = characterShoot.GetSpellSprite()[index];
                    break;
                case CharacterObjectType.FRAGMENT:
                    CharacterArtefact characterArtefact = m_playerGO.GetComponent<CharacterArtefact>();
                    imageToDrag.sprite = characterArtefact.artefactsList[index].icon;
                    break;
                default:
                    break;
            }
            isDragging = true;
            dragData.currentType = type;
            dragData.indexObj = index;
            imageToDrag.gameObject.SetActive(true);
            m_imageRectTransform.anchoredPosition = mousePosition - new Vector2(Screen.width/2,Screen.height/2);
        }

        public void DeactiveDragDrop()
        {
            isDragging = false;
            imageToDrag.gameObject.SetActive(false);
        }


        public void UpdateImage(Vector2 position)
        {
            if (!isDragging) return;
                m_imageRectTransform.anchoredPosition = position - new Vector2(Screen.width / 2, Screen.height / 2);
        }

        public void ChangeSpellPosition(int index)
        {
            if (!isDragging || dragData.currentType == CharacterObjectType.FRAGMENT) return;

            Character.CharacterShoot characterShoot =  m_playerGO.GetComponent<Character.CharacterShoot>();
            characterShoot.ExchangeSpell(index,dragData.indexObj);
        }


    }

}