using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public string profileName;
    public TMP_InputField FieldNameProfile;
    public Character.AimMode m_aimModeChoose;
    [SerializeField] public TMP_Text m_textName;
    [SerializeField] public TMP_Text m_placeHolderName;
    public static GameManager instance;

    [SerializeField] public static int aimModeNumber;
    [SerializeField] private Sprite[] m_layoutSelectedFeedback = new Sprite[2];
    private Image m_lastButtonSelected;
    // Start is called before the first  frame update
    void Awake()
    {

        if (instance!=null)
        {
            if(FieldNameProfile) instance.FieldNameProfile = this.FieldNameProfile;
            instance.m_textName = this.m_textName;
            instance.m_textName.text = instance.profileName;
            instance.m_placeHolderName = this.m_placeHolderName;
            instance.m_placeHolderName.text = instance.profileName;
            if (FieldNameProfile) instance.FieldNameProfile.onEndEdit.AddListener(instance.GetName); 
            this.gameObject.SetActive(false);
            this.enabled = false;
            Destroy(this.gameObject);

        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (FieldNameProfile) FieldNameProfile.onEndEdit.AddListener(GetName);
        }

        //else
        //{
        //    FieldNameProfile = GameObject.Find("InputField_PlayerName").GetComponent<TMP_InputField>();
        //    m_textName = GameObject.Find("Text_PlayerName").GetComponent<TMP_Text>();
        //    if (FieldNameProfile) FieldNameProfile.onEndEdit.AddListener(GetName);
        //}
    }

    private void Start()
    {
        //if(m_textName ==  null)
        //{
        //m_textName = GameObject.Find("ProfilName").GetComponent<TMP_Text>();
        //if (profileName != "") { m_textName.text = instance.profileName; }
        //}

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
        instance.profileName = name;
    }
}
