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




    public void ActiveUIEndScreen(EndInfoStats stats)
    {
        m_parentEndMenu.SetActive(true);

        m_durationGameText.text = ConvertGameTimeToString((int)stats.durationGame);
        m_killCountText.text = stats.enemyKill.ToString();
        m_nightValidateText.text = stats.nightValidate.ToString();
        m_altarLaunchText.text = stats.altarRepeated.ToString();
        m_altarSuccessedText.text = stats.altarSuccessed.ToString();
        m_biggestComboText.text = stats.bigestCombo.ToString("F1");

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


}
