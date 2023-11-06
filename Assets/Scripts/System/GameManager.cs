using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string profileName;
    // Start is called before the first  frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject); 
    }


}
