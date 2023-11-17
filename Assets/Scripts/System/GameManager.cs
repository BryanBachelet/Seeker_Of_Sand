using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public string profileName;
    public TMP_InputField FieldNameProfile;
    // Start is called before the first  frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        FieldNameProfile.onEndEdit.AddListener(GetName);
    }



    public void GetName(string name)
    {
        profileName = name;
    }
}
