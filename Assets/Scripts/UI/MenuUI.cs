using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GuerhoubaGames.SaveData;

public class MenuUI : MonoBehaviour
{
    public void ChangeScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }
    public void Quit()
    {
        Debug.Log("Quit app");
        GameData.UpdateGameDataAtQuit();
        Application.Quit();
    }
}
