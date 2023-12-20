using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public string profileName;
    public TMP_InputField FieldNameProfile;
    public Character.AimMode m_aimModeChoose;
    // Start is called before the first  frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        FieldNameProfile.onEndEdit.AddListener(GetName);
    }

    public void ChangeAimMode(int aimModeIndex)
    {
        m_aimModeChoose = (Character.AimMode)aimModeIndex;
    }

    public void GetName(string name)
    {
        profileName = name;
    }
}
