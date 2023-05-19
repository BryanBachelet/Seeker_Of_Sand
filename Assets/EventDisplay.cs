using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventDisplay : MonoBehaviour
{
    private bool currentDisplayState;
    private Animator myAnimator;

    public Text m_textDescription1;
    public Text m_textDescription2;
    // Start is called before the first frame update
    void Start()
    {
        InitComponent();
    }

    private void InitComponent()
    {
        myAnimator = this.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Change the display animator statue [0 --> Invert current status; 1 --> Set status to true (open); 2 --> Set status to false (close)]
    /// </summary>
    public void InvertDisplayStatus(int SetStatus)
    {
        if(SetStatus == 0)
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
        myAnimator.SetBool("DisplayEvent", currentDisplayState);

    }
}
