using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;

using TMPro;

public class UIEndScreen : MonoBehaviour
{
    [Header("End Screen Objects")]
    [SerializeField] private GameObject m_parentEndMenu;
    [SerializeField] private TMP_Text m_killCountText;
    [SerializeField] private TMP_Text m_nightValidateText;
    [SerializeField] private TMP_Text m_biggestComboText;
    [SerializeField] private TMP_Text m_altarLaunchText;
    [SerializeField] private TMP_Text m_altarSuccessedText;
    [SerializeField] private TMP_Text m_durationGameText;

    private bool m_finishDisplayStat = false;
    private bool m_isUpdatingStat = false;

    private EndInfoStats stat;
    private float lastXpBuffered = 0;

    public float timeToDisplay;
    public void Update()
    {
        if(m_isUpdatingStat)
        {
            if (m_finishDisplayStat)
            {
                
                m_isUpdatingStat = false;
            }
            else
            {
                BufferXpDisplay(lastXpBuffered);
            }
        }
        
    }

    public void ActiveUIEndScreen(EndInfoStats stats)
    {
        m_parentEndMenu.SetActive(true);
        stat = stats;
        m_durationGameText.text = ConvertGameTimeToString((int)stats.durationGame);
        StartDisplayStat();

    }

    private string ConvertGameTimeToString(int duration)
    {

        string gameDurationText = "";
        if (duration > 3600)
        {
            int hours = duration / 3600;
            duration -= 3600 * hours;
        }
        int minutes = duration / 60;
        int seconds = duration % 60;
        gameDurationText += minutes.ToString() + " min: " + seconds.ToString() + " ";

        return gameDurationText;
    }

    private void StartDisplayStat()
    {
        lastXpBuffered = Time.time;
        m_isUpdatingStat = true;
    }
    private void BufferXpDisplay(float time)
    {
        m_durationGameText.text = (Mathf.Lerp(0, time, Time.time - time + 1) / 60).ToString("F1") + "  Minutes";
        m_killCountText.text = Mathf.Lerp(0, stat.enemyKill, Time.time - time).ToString("F0"); 
        m_nightValidateText.text = Mathf.Lerp(0, stat.nightValidate, Time.time - time).ToString("F0");
        m_altarLaunchText.text = Mathf.Lerp(0, stat.altarRepeated, Time.time - time).ToString("F0");
        m_altarSuccessedText.text = Mathf.Lerp(0, stat.altarSuccessed, Time.time - time).ToString("F0");
        m_biggestComboText.text = Mathf.Lerp(0, stat.bigestCombo, Time.time - time).ToString("F0");

    }
}
