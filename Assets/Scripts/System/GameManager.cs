using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public string profileName;
    public TMP_InputField FieldNameProfile;
    public Character.AimMode m_aimModeChoose;

    [SerializeField] public static int aimModeNumber;
    [SerializeField] private Sprite[] m_layoutSelectedFeedback = new Sprite[2];
    private Image m_lastButtonSelected;
    // Start is called before the first  frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        FieldNameProfile.onEndEdit.AddListener(GetName);
    }

    public void ChangeAimMode(int aimModeIndex)
    {
        aimModeNumber = aimModeIndex;
        m_aimModeChoose = (Character.AimMode)aimModeIndex;
    }
    public void ChangeAimFeedback(Image newLayout)
    {
        if (m_lastButtonSelected != null)
        {
            m_lastButtonSelected.sprite = m_layoutSelectedFeedback[0];

            newLayout.sprite = m_layoutSelectedFeedback[1];
            m_lastButtonSelected = newLayout;

        }
        else
        {
            newLayout.sprite = m_layoutSelectedFeedback[1];
            m_lastButtonSelected = newLayout;
        }


    }
    public void GetName(string name)
    {
        profileName = name;
    }
}
