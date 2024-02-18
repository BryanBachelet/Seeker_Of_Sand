using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupFunction : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        overring = true;
        spellManager.SpellOverrring(popupIndex, this);
    }

    private void OnMouseExit()
    {
        overring = false;
        spellManager.SpellOverringExit();
    }
}
