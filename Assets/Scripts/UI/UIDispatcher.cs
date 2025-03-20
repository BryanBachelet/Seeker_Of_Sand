using GuerhoubaGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDispatcher : MonoBehaviour
{
    public MarchandUiView marchandUiView;
    public GameObject fixeGameplayUI;
    public GameObject miniMap_UI;
    public UI_Inventory uiInventory;
    public GuerhoubaGames.UI.DamageRecapUI damageRecap;
    public GuerhoubaGames.UI.DragManager dragManager;
    public GuerhoubaGames.UI.CristalUI cristalUI;
    public GuerhoubaGames.UI.AnvilUIView anvilUIView;
    public PauseMenu pauseMenu;
     public GameObject bandenoir;

    public GameObject[] uiMouvementObject = new GameObject[2];
    public GameObject uiFragmentPrefab;
    public List<Tool_UiMovement> lastObjectCreated = new List<Tool_UiMovement>();
    public void ActiveUIElement()
    {
        uiInventory.InitComponent();
        dragManager.StartDragManager();
    }


    public bool IsAnythingIsOpen()
    {
     

        bool anvilResult = false;
        if(anvilUIView.anvilBehavior != null)
        {
            anvilResult = anvilUIView.anvilBehavior.isOpen;
        }
        return marchandUiView.isOpen || anvilResult;
    }


    public bool CloseInventory()
    {
        if (uiInventory.isOpen)
        {
            uiInventory.DeactivateInventoryInterface();
            return true;
        }
        return false;
    }

    public void CreateObject(GameObject initialPosition)
    {
        uiMouvementObject[0] = initialPosition;
        Tool_UiMovement objectCreated = Instantiate(initialPosition, uiMouvementObject[0].transform.position - new Vector3(0, 0, 20), uiMouvementObject[0].transform.rotation, anvilUIView.transform).GetComponent<Tool_UiMovement>();
        lastObjectCreated.Add(objectCreated);
        objectCreated.positionStart = initialPosition.transform.position;
        objectCreated.positionEnd = uiMouvementObject[1].transform.position;
        objectCreated.dispatcher = this;
        objectCreated.activeMovement = true;

    }

    public void HideOrShowFixeUi(bool active)
    {
        //fixeGameplayUI.SetActive(active);
        miniMap_UI.SetActive(active);
    }
}
