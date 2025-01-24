using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialView : MonoBehaviour
{
    public GameObject startButton;
    public GameObject endButton;
    public CameraFadeFunction cameraFadeFunction;
    public void StartTutoriel()
    {
        
        if(GameState.IsPlaying())
            GameState.ChangeState();
        startButton.SetActive(true);
    }    

    public void EndTutoriel()
    {

        endButton.SetActive(false);
        GameState.ChangeState();
    }

}
