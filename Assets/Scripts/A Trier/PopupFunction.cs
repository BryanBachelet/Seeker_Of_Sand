using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupFunction : MonoBehaviour, IPointerEnterHandler
{
    static public ChooseSpellManager spellManager;
    public ChooseSpellManager spellManagerPublic;
    public bool overring = false;
    public int popupIndex;
    // Start is called before the first frame update
    void Start()
    {
        if (spellManager != spellManagerPublic)
        {
            spellManager = spellManagerPublic;
        }
    }

    public void Update()
    {
        if (!GameState.instance.IsGamepad()) return;

        bool isSelected = EventSystem.current.currentSelectedGameObject == this.gameObject;

        if (isSelected)
        {
            overring = true;
            spellManager.SpellOverring(popupIndex, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        overring = true;
        spellManagerPublic.SpellOverring(popupIndex, this);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        overring = false;
    }
}
