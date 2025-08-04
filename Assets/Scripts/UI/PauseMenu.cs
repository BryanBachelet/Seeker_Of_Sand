using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GuerhoubaGames.UI;
using TMPro;
public class PauseMenu : MonoBehaviour
{
    public GameObject fixeUIelement;
    public GameObject firstObjectToPick;
    [SerializeField] public TMP_Text m_textName;

    private UIDispatcher m_uiDispatcher;
    public void Awake()
    {
        
    }

    public void CallPauseMenu()
    {
        if(m_uiDispatcher ==null)
            m_uiDispatcher = GameState.m_uiManager.GetComponent<UIDispatcher>();

        if (m_uiDispatcher.IsAnythingIsOpen()) return;
        if (m_uiDispatcher.CloseInventory()) return;

        UITools.instance.SetUIObjectSelect(firstObjectToPick);
        GameState.ChangeState();
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        fixeUIelement.gameObject.SetActive(!fixeUIelement.gameObject.activeSelf);

        if(GameManager.instance.m_textName == null)
        {
            GameManager.instance.m_textName = m_textName;
            if (m_textName.text == "") { m_textName.text = GameManager.instance.profileName; }
        }

    }


    public void QuitFunction()
    {
        GuerhoubaTools.LogSystem.LogMsg("Quit Game", true);
        Application.Quit();
    }
}
