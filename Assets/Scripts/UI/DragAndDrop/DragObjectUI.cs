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

        public void OnBeginDrag(PointerEventData eventData)
        {
            DragManager.instance.ActiveDragDrop(objectType, index, eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragManager.instance.UpdateImage(eventData.delta);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("On Drop  " + gameObject.name);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragManager.instance.DeactiveDragDrop();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("On Pointer Down " + gameObject.name);
        }

    }
}