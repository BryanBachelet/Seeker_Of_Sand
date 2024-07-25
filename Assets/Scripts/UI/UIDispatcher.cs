using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDispatcher : MonoBehaviour
{
    public MarchandUiView marchandUiView;
    public GameObject fixeGameplayUI;
    public UI_Inventory uiInventory;
    public GuerhoubaGames.UI.DamageRecapUI damageRecap; 
    public GuerhoubaGames.UI.DragManager dragManager;

    public GameObject bandenoir;
    public void ActiveUIElement()
    {
        uiInventory.InitComponent();
        dragManager.StartDragManager();
    }

}
