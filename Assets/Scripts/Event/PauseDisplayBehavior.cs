using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDisplayBehavior : MonoBehaviour
{
    private GameManager GM;
    public GameObject pauseDisplayState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pauseDisplayState.SetActive(!GameState.IsPlaying());    
    }
}
