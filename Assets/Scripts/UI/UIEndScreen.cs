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
    [SerializeField] private Image m_nightCompletionFill;
    [SerializeField] private GameObject[] m_nightCompleted;
    [SerializeField] private GameObject fixeElement;
    [SerializeField] private GameObject[] m_spellDetail = new GameObject[4];
    [SerializeField] private TMP_Text[] m_spellDetailUpgrades = new TMP_Text[4];
    [SerializeField] private TMP_Text[] m_spellDetailTier = new TMP_Text[4];
    [SerializeField] private TMP_Text[] m_spellDetailDamages = new TMP_Text[4];
    [SerializeField] private TMP_Text[] m_spellDetailName = new TMP_Text[4];
    [SerializeField] private Image[] m_spellDetailImage = new Image[4];

    [SerializeField] private TMP_Text[] m_fragmentDetailDamages = new TMP_Text[2];
    [SerializeField] private TMP_Text[] m_fragmentDetailName = new TMP_Text[2];
    [SerializeField] private Image[] m_fragmentDetailImage = new Image[2];

    private bool m_finishDisplayStat = false;
    private bool m_isUpdatingStat = false;

    private EndInfoStats stat;
    private float lastXpBuffered = 0;

    public float timeToDisplay;


    [Range(1, 10)] public float tempsDisplay = 3;

    public Character.CharacterShoot characterShoot;

    private int[] spellDamaged = new int[4];
    int spellCount = 0;
    public void Update()
    {
        if (m_isUpdatingStat)
        {
            if (m_finishDisplayStat)
            {

                m_isUpdatingStat = false;
            }
            else
            {
                BufferXpDisplay(lastXpBuffered);
                Debug.Log(lastXpBuffered + " value Time");
            }
        }

    }

    public void ActiveUIEndScreen(EndInfoStats stats)
    {
        fixeElement.SetActive(false);
        m_parentEndMenu.SetActive(true);
        stat = stats;
        m_durationGameText.text = ConvertGameTimeToString((int)stats.durationGame);
        //StartDisplayStat();
        SpellLink(characterShoot.spellProfils);

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
        gameDurationText += minutes.ToString() + ":" + seconds.ToString() + " ";

        return gameDurationText;
    }

    public void StartDisplayStat()
    {
        lastXpBuffered = Time.time;
        m_finishDisplayStat = false;
        m_isUpdatingStat = true;

    }
    private void BufferXpDisplay(float time)
    {
        float tempsEcoule = (Mathf.Lerp(0, time, Time.time - time + 1) / 60);
        m_nightCompletionFill.fillAmount = tempsEcoule / 27;
        if (tempsEcoule > 9)
        {
            m_nightCompleted[0].SetActive(true);
        }
        if (tempsEcoule > 18.5f)
        {
            m_nightCompleted[1].SetActive(true);
        }
        if (tempsEcoule > 28)
        {
            m_nightCompleted[2].SetActive(true);
        }

        float progress = (Time.time - time) / tempsDisplay;
        m_killCountText.text = Mathf.Lerp(0, stat.enemyKill, progress).ToString("F0");
        m_nightValidateText.text = Mathf.Lerp(0, stat.nightValidate, progress).ToString("F0");
        m_altarLaunchText.text = Mathf.Lerp(0, stat.altarRepeated, progress).ToString("F0");
        m_altarSuccessedText.text = Mathf.Lerp(0, stat.altarSuccessed, progress).ToString("F0");
        m_biggestComboText.text = Mathf.Lerp(0, stat.roomCount, progress).ToString("F0");
        for (int i = 0; i < spellCount; i++)
        {

            m_spellDetailDamages[i].text = "" + Mathf.Lerp(0, spellDamaged[i], progress).ToString("F0");

        }
        Debug.Log(progress + " : Progress display");




    }

    private void SpellLink(List<SpellSystem.SpellProfil> spellProfils)
    {
        int[] ennemyKilledbySpell = new int[spellProfils.Count];
        int[] upgradeAddedBySpell = new int[spellProfils.Count];
        spellCount = 0;
        if (spellProfils.Count < 4)
        {
            spellCount = spellProfils.Count;
        }
        else
        {
            spellCount = 4;
        }

        GameStats.instance.ShowDamageLog();
        for (int i = 0; i < 4; i++)
        {
            if (i < spellCount)
            {
                m_spellDetail[i].SetActive(true);
                m_spellDetailImage[i].sprite = spellProfils[i].spell_Icon;
                m_spellDetailUpgrades[i].text = "" + spellProfils[i].level;
                m_spellDetailName[i].text = "" + spellProfils[i].name;


                spellDamaged[i] = GameStats.instance.GetDamage(spellProfils[i].name);
                int tier = Mathf.FloorToInt(spellProfils[i].level / 4);
                m_spellDetailTier[i].text = "" + tier;
            }
            else
            {
                m_spellDetail[i].SetActive(false);
            }

        }

    }

}
