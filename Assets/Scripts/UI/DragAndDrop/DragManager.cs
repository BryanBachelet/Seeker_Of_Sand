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
            isDragging = true;
            switch (type)
            {
                case CharacterObjectType.SPELL:
                    Character.CharacterShoot characterShoot = m_playerGO.GetComponent<Character.CharacterShoot>();
                    imageToDrag.sprite = characterShoot.GetSpellSprite()[index];
                    break;
                case CharacterObjectType.FRAGMENT:
                    imageToDrag.sprite = m_fragmentToolTip.imageFragmentTooltip[index].sprite;
                    break;
                default:
                    break;
            }

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


        public void UpdateImage(Vector2 delta)
        {
            m_imageRectTransform.anchoredPosition += delta;
        }


    }

}