using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialView : MonoBehaviour
{
    public GameObject startButton;
    public GameObject endButton;
    public bool isEndPauseState = true;
    public CameraFadeFunction cameraFadeFunction;
    public void StartTutoriel()
    {
        return;
        if(GameState.IsPlaying())
            GameState.ChangeState();
        startButton.SetActive(true);
    }    

    public void EndTutoriel()
    {
        return;
        endButton.SetActive(false);
        if (isEndPauseState && !GameState.IsPlaying())
            GameState.ChangeState();
    }

}
