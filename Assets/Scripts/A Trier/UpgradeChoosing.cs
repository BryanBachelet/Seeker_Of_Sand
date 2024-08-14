using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GuerhoubaGames.GameEnum;


public class UpgradeChoosing : MonoBehaviour
{
    public int indexSpellBar = 0;
    [Header("UI Object")]
    public Image[] spellInBar = new Image[4];
    public Image spellUpgradeFocus;
    public Image[] spellChoseUpgrade = new Image[3];
    public TMPro.TMP_Text upgradePointText;
    public TMPro.TMP_Text[] upgradeSelectable = new TMPro.TMP_Text[3];

    public EventSystem eventSystem;
    private UpgradeLevelingData m_upgradeLevelingData;
    [HideInInspector] public UpgradeManager m_upgradeManager;

    [SerializeField] private Image[] m_imageBandeau = new Image[3];
    [SerializeField] private Material[] m_materialBandeauDissolve = new Material[3];
    public GameObject[] decorationHolder = new GameObject[3];
    public float timeSolve;
    private SeekerOfSand.UI.UI_PlayerInfos m_UiPlayerInfo;

    // Stats Order 
    // - Damage
    // - Size
    // - Speed
    // - Projectile
    // - Shoot
    // - Pierce

    public TMPro.TMP_Text[] upgradeTextStatBase = new TMPro.TMP_Text[6];
    public TMPro.TMP_Text[] upgradeTextStatModify = new TMPro.TMP_Text[6];
    public UIOver[] uiUpgradeButton = new UIOver[3];

    private void Awake()
    {
        if (m_imageBandeau != null)
        {
            for (int i = 0; i < m_imageBandeau.Length; i++)
            {
                Material mat = Instantiate(m_imageBandeau[i].material);

                m_materialBandeauDissolve[i] = mat;
                m_imageBandeau[i].material = mat;
            }
        }
    }

    public void Start()
    {
         m_UiPlayerInfo = GameObject.Find("UI_Manager").GetComponent<SeekerOfSand.UI.UI_PlayerInfos>();
        for (int i = 0; i < uiUpgradeButton.Length; i++)
        {
            uiUpgradeButton[i].OnEnter += SetModifySpellStat;
        }
    }

    public void SetNewUpgradeData(UpgradeLevelingData data)
    {
        m_upgradeLevelingData = data;
        for (int i = 0; i < m_upgradeLevelingData.spellCount; i++)
        {
            spellInBar[i].sprite = m_upgradeLevelingData.iconSpell[i];
        }

        SetBaseSpellStat(0);

        upgradePointText.text = m_upgradeLevelingData.upgradePoint.ToString();
        spellUpgradeFocus.sprite = m_upgradeLevelingData.iconSpell[m_upgradeLevelingData.indexSpellFocus];
        for (int i = 0; i < 3; i++)
        {
            upgradeSelectable[i].text = ((m_upgradeLevelingData.upgradeChoose[i])).name;
            spellChoseUpgrade[i].sprite = spellUpgradeFocus.sprite;
            //if (gameObject.activeSelf) StartCoroutine(SpellFadeIn(i, Time.time));
        }
    }

    public void UpdateUpgradesAvailable(UpgradeObject[] upgradeGenerate)
    {
        m_upgradeLevelingData.upgradeChoose = upgradeGenerate;
    }

    public void ChooseUpgrade(int index)
    {
        m_upgradeManager.SendUpgrade(m_upgradeLevelingData.upgradeChoose[index]);
        m_upgradeManager.m_dropInventory.AddNewUpgrade(m_upgradeLevelingData.upgradeChoose[index], spellUpgradeFocus.sprite);

     
        
        for (int i = 0; i < upgradeSelectable.Length; i++)
        {
            if (i != index)
            {
                // if (gameObject.activeSelf) StartCoroutine(SpellFadeOut(i, Time.time));
            }
        }

        SetBaseSpellStat(index);

    }

    public IEnumerator SpellFadeOut(int upgradeNumber, float time)
    {
        //decorationHolder[upgradeNumber].SetActive(false);
        while (time + timeSolve - Time.time > 0)
        {
            m_materialBandeauDissolve[upgradeNumber].SetFloat("_Fade_Step", (time + timeSolve - Time.time) / timeSolve - 0.5f);
            yield return Time.deltaTime;
        }
        m_materialBandeauDissolve[upgradeNumber].SetFloat("_Fade_Step", -0.5f);

    }



    public IEnumerator SpellFadeIn(int upgradeNumber, float time)
    {
        while (Time.time - time < timeSolve)
        {
            m_materialBandeauDissolve[upgradeNumber].SetFloat("_Fade_Step", (Time.time - time) / timeSolve - 0.5f);
            yield return Time.deltaTime;
        }
        m_materialBandeauDissolve[upgradeNumber].SetFloat("_Fade_Step", 0.6f);
        //decorationHolder[upgradeNumber].SetActive(true);

    }


    public void SetBaseSpellStat(int index)
    {
        int indexSpell = m_upgradeLevelingData.upgradeChoose[index].indexSpellLink;
        SpellSystem.SpellProfil spellProfil = m_upgradeLevelingData.spellState[indexSpell];
        //if (spellProfil.tagData.EqualsSpellNature(SpellNature.PROJECTILE))
        //{
        //    upgradeTextStatBase[0].text = "Damage : " + spellProfil.GetIntStat(StatType.Damage);
        //    upgradeTextStatBase[1].text = "Speed : " + (spellProfil.GetFloatStat(StatType.Range) / spellProfil.GetFloatStat(StatType.LifeTime));
        //    upgradeTextStatBase[2].text = "Projectile : " + spellProfil.GetIntStat(StatType.Projectile);
        //    upgradeTextStatBase[3].text = "Salve : " + spellProfil.GetIntStat(StatType.ShootNumber);
        //    upgradeTextStatBase[4].text = "";
        //    upgradeTextStatBase[5].text = "";
        //}

        string textStatUpgrade = "";
        for (int i = 0; i < spellProfil.statDatas.Count; i++)
        {
            SpellSystem.StatData statData = spellProfil.statDatas[i];
            if(statData.isVisible) textStatUpgrade += statData.stat.ToString() + " : " + spellProfil.GetStatValueToString(statData.stat) + "\n";
        }
        upgradeTextStatBase[0].text = textStatUpgrade;
    }

    public void SetModifySpellStat(int index)
    {
        int indexSpell = m_upgradeLevelingData.upgradeChoose[index].indexSpellLink;

        SpellSystem.SpellProfil spellProfil = m_upgradeLevelingData.spellState[indexSpell];
        UpgradeObject stats = m_upgradeLevelingData.upgradeChoose[index];


        string textStatUpgrade = "";
        for (int i = 0; i < spellProfil.statDatas.Count; i++)
        {

            SpellSystem.StatData statDataSpell = spellProfil.statDatas[i];
            if (statDataSpell.isVisible)
            {
                if (stats.HasThisStat(statDataSpell.stat)) textStatUpgrade += "-->" + stats.PrewiewApplyValue(statDataSpell) + "\n";
                else textStatUpgrade += "\n";
            }
        }
        upgradeTextStatModify[0].text = textStatUpgrade;

       
    }


    public void Update()
    {

    }
}