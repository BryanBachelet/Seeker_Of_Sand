using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void ChangeScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
