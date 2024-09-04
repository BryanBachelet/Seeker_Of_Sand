using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GuerhoubaGames.UI;
public class PauseMenu : MonoBehaviour
{
    public GameObject fixeUIelement;
    public GameObject firstObjectToPick;
    public void CallPauseMenu()
    {
        UITools.instance.SetUIObjectSelect(firstObjectToPick);
        GameState.ChangeState();
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        fixeUIelement.gameObject.SetActive(!fixeUIelement.gameObject.activeSelf);
    }


    public void QuitFunction()
    {
        GuerhoubaTools.LogSystem.LogMsg("Quit Game", true);
        Application.Quit();
    }
}
