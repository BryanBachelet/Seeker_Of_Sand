using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{

    public GameObject fixeUIelement;
    public void CallPauseMenu()
    {
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
