using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class MiniMapActualisation : MonoBehaviour
{
    private bool currentDisplayState;
    public Animator minimap_Animator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ui_DisplayMiniMap.set = txtMiniMap;
    }

    /// <summary>
    /// Change the display animator statue [0 --> Invert current status; 1 --> Set status to true (open); 2 --> Set status to false (close)]
    /// </summary>
    public void InvertDisplayStatus(int SetStatus)
    {
        if (SetStatus == 0)
        {
            currentDisplayState = !currentDisplayState;
        }
        else if (SetStatus == 1)
        {
            currentDisplayState = true;
        }
        else if (SetStatus == 2)
        {
            currentDisplayState = false;
        }
        minimap_Animator.SetBool("DisplayEvent", currentDisplayState);

    }

    public void InvertDisplayStatusInput(InputAction.CallbackContext ctx)
    {
        currentDisplayState = !currentDisplayState;
        minimap_Animator.SetBool("DisplayEvent", currentDisplayState);
    }
}
