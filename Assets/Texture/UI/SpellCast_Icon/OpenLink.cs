using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string urlButton;
    public void OpenChannel()
    {
        Application.OpenURL(urlButton);
        GUIUtility.systemCopyBuffer = urlButton;
    }
}
