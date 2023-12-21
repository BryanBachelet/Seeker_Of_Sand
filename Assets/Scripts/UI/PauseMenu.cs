using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{


    public void CallPauseMenu()
    {
        GameState.ChangeState();
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    
    public void QuitFunction()
    {
        Application.Quit();
    }
}
