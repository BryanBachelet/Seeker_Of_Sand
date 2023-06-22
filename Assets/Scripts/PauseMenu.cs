using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    static public bool gameState;
    public GameObject menuObject;
    public GameObject buttonHolder;
    private Animator animatorMenu;
    private Animator buttonAnimator;
    [SerializeField] Animator m_DebugSkillAnimator;
    // Start is called before the first frame update
    void Start()
    {
        animatorMenu = menuObject.GetComponent<Animator>();
        buttonAnimator = buttonHolder.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivePauseMenu(int State)
    {
        if(State == 0)
        {
            gameState = !gameState;
        }
        else if(State == 1)
        {
            gameState = false;
        }
        else if(State == 2)
        {
            gameState = true;
        }
        animatorMenu.SetBool("MenuOpen", gameState);
        m_DebugSkillAnimator.SetBool("OpenDebugMenu", gameState);
        //menuObject.SetActive(gameState);
        if (gameState)
        {
            Time.timeScale = 0.02f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    IEnumerator LoadYourAsyncScene(int SceneNumber)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneNumber);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void SelectionScene(int SceneNumber)
    {
        Time.timeScale = 1;
        StartCoroutine(LoadYourAsyncScene(SceneNumber));
    }
    public void ChangeMenuPlayLayout(int State)
    {
        bool newState = false;
        if (State == 0)
        {
            newState = !newState;
        }
        else if (State == 1)
        {
            newState = true;
        }
        else if (State == 2)
        {
            newState = false;
        }
        buttonAnimator.SetBool("PlayMenuOn", newState);
    }

    public void ChangeMenuOptionLayout(int State)
    {
        bool newState = false;
        if (State == 0)
        {
            newState = !newState;
        }
        else if (State == 1)
        {
            newState = true;
        }
        else if (State == 2)
        {
            newState = false;
        }
        buttonAnimator.SetBool("OptionMenu", newState);
    }

    public void ChangeMenuCodexLayout(int State)
    {
        bool newState = false;
        if (State == 0)
        {
            newState = !newState;
        }
        else if (State == 1)
        {
            newState = true;
        }
        else if (State == 2)
        {
            newState = false;
        }
        buttonAnimator.SetBool("CodexMenu", newState);
    }

    public void ReturnMain()
    {
        buttonAnimator.SetBool("PlayMenuOn", false);
        buttonAnimator.SetBool("OptionMenu", false);
        buttonAnimator.SetBool("CodexMenu", false);
    }

    public void QuitFunction()
    {

        Application.Quit();
    }
}
