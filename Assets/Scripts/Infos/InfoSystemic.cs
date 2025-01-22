using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GameInfo
{
    public string id;
    public string header;
    [TextArea]
    public string content; 
}
public class InfoSystemic : MonoBehaviour
{
    public static InfoSystemic instance;
    public GameInfo[] gameInfoArray;
    public void Awake()
    {
        instance = this;
    }

    public string GetIdTitle(string id)
    {
        for (int i = 0; i < gameInfoArray.Length; i++)
        {
            if (id == gameInfoArray[i].id)
            {
                return gameInfoArray[i].header;
            }
        }

        Debug.LogWarning("This " + id + " doesn't exist");
        return string.Empty;
    }

    public string GetIdDescription(string id)
    {
        for (int i = 0; i < gameInfoArray.Length; i++)
        {
            if (id == gameInfoArray[i].id)
            {
                return gameInfoArray[i].content;
            }
        }

        Debug.LogWarning("This " + id + " doesn't exist");
        return string.Empty;
    }
    
}
