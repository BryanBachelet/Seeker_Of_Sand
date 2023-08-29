using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_MainMenu_ButtonOver : MonoBehaviour
{
    private bool buttonActive = false;
    private static Main_UI_Controle mainControlScript;
    private Image buttonImage;

    [SerializeField] private int IndexNumber;
    // Start is called before the first frame update
    void Start()
    {
        if(mainControlScript == null)
        {
            mainControlScript = GameObject.Find("MainMenu").GetComponent<Main_UI_Controle>();
        }
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
            mainControlScript.ButtonOver(IndexNumber);
            buttonActive = true;
        }

    }

    public void OnMouseExit()
    {
        if (buttonActive)
        {
            buttonActive = false;
            //mainControlScript.ButtonOver(-1);
        }
    }
}
