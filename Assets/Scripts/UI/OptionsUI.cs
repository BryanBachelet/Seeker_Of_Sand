using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMOD;

public class OptionsUI : MonoBehaviour
{
    public Text resolutionDisplayText;
    public Image fullScreenImage;
    public TMP_Dropdown m_resolutionsDropDown;

    private Resolution[] m_resolutions;
    private Resolution m_currentResolution;
    private int m_indexResolution;
    private int[] m_refreshRate;
    private Vector2[] m_screenResolution;
    private bool m_isFullScreen = true;
    private bool m_isVerticalSynchroEnable = false;


    [Header("Audio Settings")]
    [SerializeField] private bool m_isActive = false; //  Temp because I don't no the name and number of bus in our FMOD Project
    [SerializeField] private string m_masterBusName;
    [SerializeField] private string m_musicsBusName;
    private FMOD.Studio.Bus m_masterBus;
    private FMOD.Studio.Bus m_musicsBus;

    private void Awake()
    {
        if (m_isActive)
        {
            m_masterBus = FMODUnity.RuntimeManager.GetBus(m_masterBusName);
            m_musicsBus = FMODUnity.RuntimeManager.GetBus(m_musicsBusName);
        }


        GetAllAvailableResolutionsData();
    }
    private void Start()
    {
        UpdateVisualOptions();
        fullScreenImage.color = Color.green;
    }
    void GetAllAvailableResolutionsData()
    {
        m_resolutions = Screen.resolutions;
        m_resolutionsDropDown.ClearOptions();

        List<int> m_index = new List<int>();

        m_index.Add(0);

        for (int i = 1; i < m_resolutions.Length; i++)
        {
            if (IsSameResolution(m_resolutions[i - 1], m_resolutions[i]))
            {
                m_index[m_index.Count - 1] = i;
            }
            else
            {
                m_index.Add(i);
            }

        }

        List<string> m_resolutionString = new List<string>();
        m_screenResolution = new Vector2[m_index.Count];
        m_refreshRate = new int[m_index.Count];

        for (int i = 0; i < m_index.Count; i++)
        {
            int index = m_index[i];
            m_screenResolution[i].x = m_resolutions[index].width;
            m_screenResolution[i].y = m_resolutions[index].height;
            m_refreshRate[i] = m_resolutions[index].refreshRate;

            m_resolutionString.Add(m_screenResolution[i].x.ToString("F0") + " x " + m_screenResolution[i].y.ToString("F0"));


            if (IsSameResolution(Screen.currentResolution, m_resolutions[index]))
                m_indexResolution = i;

        }

        m_resolutionsDropDown.AddOptions(m_resolutionString);
        m_resolutionsDropDown.value = m_indexResolution;
        m_currentResolution = Screen.currentResolution;
    }

    private bool IsSameResolution(Resolution a, Resolution b)
    {
        return a.width == b.width && b.height == a.height;
    }
    public void ChangeResolution(int increase)
    {
        m_indexResolution = increase;
    }
    public void EnableVerticalSynchro()
    {
        m_isVerticalSynchroEnable = !m_isVerticalSynchroEnable;
    }

    public void ChangeFullScreen()
    {
        m_isFullScreen = !m_isFullScreen;
        if (m_isFullScreen) fullScreenImage.color = Color.green;
        else fullScreenImage.color = Color.red;

    }

    public void ChangeMasterSoundVolume(float value)
    {
    
            if (m_isActive) m_masterBus.setVolume(value);
    }
    public void ChangeMusicsSoundVolume(float value)
    {
        if (m_isActive) m_musicsBus.setVolume(value);
    }


    public void UpdateOptions()
    {
        UpdateVisualOptions();
        Screen.fullScreen = m_isFullScreen;
        if (m_isVerticalSynchroEnable)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    private void UpdateVisualOptions()
    {
        Screen.SetResolution((int)m_screenResolution[m_indexResolution].x, (int)m_screenResolution[m_indexResolution].y, m_isFullScreen);

    }
}
