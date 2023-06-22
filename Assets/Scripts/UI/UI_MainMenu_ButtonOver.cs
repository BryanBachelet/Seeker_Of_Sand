using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_MainMenu_ButtonOver : MonoBehaviour
{
    private bool buttonActive = false;
    private Image buttonImage;
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = this.GetComponent<Image>();
        //buttonImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseOver()
    {
        Debug.Log("TestUI");
        if(!buttonActive)
        {
            Debug.Log("TestUI 2!");
            buttonImage.enabled = true;
            buttonActive = true;
        }
    }

    public void OnMouseExit()
    {
        if (buttonActive)
        {
            buttonImage.enabled = false;
            buttonActive = false;
        }
    }
}
