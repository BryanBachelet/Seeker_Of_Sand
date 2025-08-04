using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GuerhoubaGames.UI
{
    public class ShopDragObjectUI : MonoBehaviour, IDropHandler
    {
        public MarchandUiView marchandUiView;

        public void OnDrop(PointerEventData eventData)
        {
            marchandUiView.DropElementMysteryBag(DragManager.instance.dragData);
        }


    }
}