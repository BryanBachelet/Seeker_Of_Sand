using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GuerhoubaGames.UI;
using TMPro;
using UnityEngine.VFX;
using GuerhoubaGames.Character;

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
    [SerializeField] private TMP_Text m_roomCountText;
    [SerializeField] private Image[] m_nightCompletionFill = new Image[3];
    [SerializeField] private GameObject[] m_nightCompleted;
    [SerializeField] private VisualEffect[] m_vfxNightCompleted = new VisualEffect[3];
    [SerializeField] private GameObject fixeElement;
    [SerializeField] private int m_spelltDisplayCount = 4;
    [SerializeField] private GameObject[] m_spellDetail = new GameObject[4];
    [SerializeField] private SpellAttribution[] m_spellAttribution = new SpellAttribution[4];
    [SerializeField] private TMP_Text[] m_spellDetailUpgrades = new TMP_Text[4];
    [SerializeField] private TMP_Text[] m_spellDetailTier = new TMP_Text[4];
    [SerializeField] private TMP_Text[] m_spellDetailDamages = new TMP_Text[4];
    [SerializeField] private TMP_Text[] m_spellDetailName = new TMP_Text[4];

    [SerializeField] private int m_fragmentDisplayCount = 3;
    [SerializeField] private GameObject[] m_fragmentDetails = new GameObject[2];
    [SerializeField] private TMP_Text[] m_fragmentDetailDamages = new TMP_Text[2];
    [SerializeField] private TMP_Text[] m_fragmentDetailName = new TMP_Text[2];
    [SerializeField] private TMP_Text[] m_fragmentDetailActivationCount = new TMP_Text[2];
    [SerializeField] private Image[] m_fragmentDetailImage = new Image[2];

    private bool m_finishDisplayStat = false;
    private bool m_isUpdatingStat = false;

    private EndInfoStats stat;
    private float lastXpBuffered = 0;

    public float timeToDisplay;


    [Range(1, 10)] public float tempsDisplay = 3;

    public CharacterShoot characterShoot;

    private int[] spellDamaged = new int[4];
    int spellCount = 0;


    public GameObject FirstMenuButtonObject;
    public GameObject FirstObjectSelect;

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
        FragmentLink(characterShoot.GetComponent<CharacterArtefact>().GetMostDamageArtefactInfo(m_fragmentDisplayCount));
        if (GameState.instance.IsGamepad()) 
            UITools.instance.SetUIObjectSelect(FirstMenuButtonObject);
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

        if (GameState.instance.IsGamepad()) 
            UITools.instance.SetUIObjectSelect(FirstObjectSelect);

    }
    private void BufferXpDisplay(float time)
    {
        float tempsEcoule = (Mathf.Lerp(0, time, Time.time - time + 1) / 60);

        float progress = (Time.time - time) / tempsDisplay;
        if (progress < 0.33f)
        {
            m_nightCompletionFill[0].fillAmount = Mathf.Lerp(0, stat.roomCount / 7f, progress / 0.33f);
            //m_nightCompletionFill[1].fillAmount = 0;
            //m_nightCompletionFill[2].fillAmount = 0;
        }
        if(stat.nightValidate > 0 && progress > 0.33f)
        {
            m_nightCompleted[0].SetActive(true);
            m_vfxNightCompleted[0].Play();
            m_nightCompletionFill[0].fillAmount = 1;
            m_nightCompletionFill[1].fillAmount = Mathf.Lerp(0, (stat.roomCount - 7) / 7.0f, (progress - 0.33f) / 0.33f);
            //m_nightCompletionFill[2].fillAmount = 0;
        }
        if (stat.nightValidate > 1 && progress > 0.66f)
        {
            m_nightCompleted[1].SetActive(true);
            m_vfxNightCompleted[1].Play();
            m_nightCompletionFill[0].fillAmount = 1;
           m_nightCompletionFill[1].fillAmount = 1;
            m_nightCompletionFill[2].fillAmount = Mathf.Lerp(0, (stat.roomCount - 14) / 7f, (progress - 0.66f) / 0.33f);
        }
        if (stat.nightValidate > 2 && progress >= 0.99f)
        {
            m_nightCompleted[2].SetActive(true);
            m_vfxNightCompleted[2].Play();
            m_nightCompletionFill[0].fillAmount = 1;
            m_nightCompletionFill[1].fillAmount = 1;
            m_nightCompletionFill[2].fillAmount = 1;
            m_finishDisplayStat = true;
        }

        m_killCountText.text = Mathf.Lerp(0, stat.enemyKill, progress).ToString("F0");
        m_nightValidateText.text = Mathf.Lerp(0, stat.nightValidate, progress).ToString("F0");
        m_altarLaunchText.text = Mathf.Lerp(0, stat.altarRepeated, progress).ToString("F0");
        m_altarSuccessedText.text = Mathf.Lerp(0, stat.altarSuccessed, progress).ToString("F0");
        m_biggestComboText.text = Mathf.Lerp(0, stat.maxCombo, progress).ToString("F0");
        m_roomCountText.text = Mathf.Lerp(0, stat.roomCount, progress).ToString("F0");
        for (int i = 0; i < spellCount; i++)
        {

            m_spellDetailDamages[i].text = "" + Mathf.Lerp(0, spellDamaged[i], progress).ToString("F0");

        }
        Debug.Log(progress + " : Progress display");



        if (progress >= 1.0f)
        {
            m_finishDisplayStat = true;
        }

    }

    private void FragmentLink(ArtefactsInfos[] artefactsInfos)
    {
        float artefactCount = 0;

        if (artefactsInfos.Length < m_fragmentDisplayCount)
        {
            artefactCount = artefactsInfos.Length;
        }
        else
        {
            artefactCount = m_fragmentDisplayCount;
        }

        for (int i = 0; i < m_fragmentDisplayCount; i++)
        {
            if(i<artefactCount)
            {
                m_fragmentDetailName[i].text = artefactsInfos[i].nameArtefact;
                m_fragmentDetailImage[i].sprite = artefactsInfos[i].icon;
                m_fragmentDetailDamages[i].text = GameStats.instance.GetDamage(artefactsInfos[i].nameArtefact).ToString();
                m_fragmentDetailActivationCount[i].text = artefactsInfos[i].activationCount.ToString(); ;
                m_fragmentDetails[i].SetActive(true);
            }
            else
            {
                m_fragmentDetails[i].SetActive(false);
            }
           
        }
    }
    
    private void SpellLink(List<SpellSystem.SpellProfil> spellProfils)
    {
        int[] ennemyKilledbySpell = new int[spellProfils.Count];
        int[] upgradeAddedBySpell = new int[spellProfils.Count];
        spellCount = 0;
        if (spellProfils.Count < m_spelltDisplayCount)
        {
            spellCount = spellProfils.Count;
        }
        else
        {
            spellCount = m_spelltDisplayCount;
        }

        GameStats.instance.ShowDamageLog();
        for (int i = 0; i < m_spelltDisplayCount; i++)
        {
            if (i < spellCount)
            {
                m_spellDetail[i].SetActive(true);
                m_spellAttribution[i].AcquireSpellData(spellProfils[i]);
                //m_spellDetailImage[i].sprite = spellProfils[i].spell_Icon;
                m_spellDetailUpgrades[i].text = "" + spellProfils[i].spellExp;
                m_spellDetailName[i].text = "" + spellProfils[i].name;


                spellDamaged[i] = GameStats.instance.GetDamage(spellProfils[i].name);
                int tier = Mathf.FloorToInt(spellProfils[i].currentSpellTier);
                m_spellDetailTier[i].text = "" + m_spellAttribution[i].level;
            }
            else
            {
                m_spellDetail[i].SetActive(false);
            }

        }

    }

}
