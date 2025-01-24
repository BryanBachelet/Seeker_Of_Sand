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
        startButton.SetActive(true);
    }    

    public void EndTutoriel()
    {
        cameraFadeFunction.LaunchGame();
        endButton.SetActive(false);
    }

}
