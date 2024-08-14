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
        if(spellManager != spellManagerPublic)
        {
            spellManager = spellManagerPublic;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        overring = true;
        spellManager.SpellOverring(popupIndex, this);
    }

}
