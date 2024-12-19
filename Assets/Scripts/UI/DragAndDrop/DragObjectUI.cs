using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GuerhoubaGames.GameEnum;

namespace GuerhoubaGames.UI
{
    public class DragObjectUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
    {
        public CharacterObjectType objectType;
        public int index;
        public bool isLock;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isLock) return;
            DragManager.instance.ActiveDragDrop(objectType, index, eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragManager.instance.UpdateImage(eventData.position);
        }

        public void OnDrop(PointerEventData eventData)
        {
            DragManager.instance.ChangeSpellPosition(index);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragManager.instance.DeactiveDragDrop();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("On Pointer Down " + gameObject.name);
        }

        public void OnDisable()
        {
            DragManager.instance.DeactiveDragDrop();
        }

    }
}