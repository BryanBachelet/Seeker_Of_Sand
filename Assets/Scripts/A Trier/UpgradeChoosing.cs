using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.UI;
using SpellSystem;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;


public class UpgradeChoosing : MonoBehaviour
{
    //[HideInInspector] private
    [Header("UI Object")]
    [SerializeField] private Image[] spellInBar = new Image[4];
    [SerializeField] private Image fillNextTierRankPoint;
    public spell_Attribution currentSpellSelected;
    public spell_Attribution currentspellSelected_NextRank;
    public Image spellUpgradeFocus;
    public Image spellCurrentTierFocus;
    public Image spellNextTierFocus;
    public Image cadreCurrentTier;
    public Image cadreNextTier;
    public Image[] spellChoseUpgrade = new Image[3];
    private Material[] materialIcon = new Material[4];
    private GuerhoubaGames.UI.TooltipTrigger tooltipCurrentTier;
    private GuerhoubaGames.UI.TooltipTrigger tooltipNextTier;
    public TMPro.TMP_Text upgradePointText;
    public TMPro.TMP_Text[] upgradeSelectable = new TMPro.TMP_Text[3];
    public TMPro.TMP_Text[] upgradeSelectableDescription = new TMPro.TMP_Text[3];

    public Sprite[] spellTier = new Sprite[4];

    public EventSystem eventSystem;
    private UpgradeLevelingData m_upgradeLevelingData;
    public UpgradeManager m_upgradeManager;

    [SerializeField] private Image[] m_imageBandeau = new Image[3];
    [SerializeField] private Material[] m_materialBandeauDissolve = new Material[3];
    public GameObject[] decorationHolder = new GameObject[3];
    public float timeSolve;
    private SeekerOfSand.UI.UI_PlayerInfos m_UiPlayerInfo;
    private UIDispatcher m_Dispatcher;
    // Stats Order 
    // - Damage
    // - Size
    // - Speed
    // - Projectile
    // - Shoot
    // - Pierce

    public TMPro.TMP_Text[] upgradeTextStatBase = new TMPro.TMP_Text[6];
    public TMPro.TMP_Text[] upgradeTextStatModify = new TMPro.TMP_Text[6];
    private bool[] stateStat = new bool[6];
    public UIOver[] uiUpgradeButton = new UIOver[3];
    public TMPro.TMP_Text rerollText;

    public GameObject firstObjectSelect;

    public GameObject panelUI;
    [HideInInspector] public SpellProfil[] spellProfils;
    public Character.CharacterSpellBook spellBook;
    public GameObject[] skillUiHolder = new GameObject[4];
    public GameObject[] referenceSkill = new GameObject[4];
    public List<GameObject> skillUiHolderTemp;


    public GameObject prefabRank;
    private void Awake()
    {
        //if (m_imageBandeau != null)
        //{
        //    for (int i = 0; i < m_imageBandeau.Length; i++)
        //    {
        //        Material mat = Instantiate(m_imageBandeau[i].material);
        //
        //        m_materialBandeauDissolve[i] = mat;
        //        m_imageBandeau[i].material = mat;
        //    }
        //}

        m_upgradeManager = GameState.m_enemyManager.GetComponent<UpgradeManager>();
    }

    public void Start()
    {
        m_UiPlayerInfo = GameObject.Find("UI_Manager").GetComponent<SeekerOfSand.UI.UI_PlayerInfos>();
        m_Dispatcher = m_UiPlayerInfo.gameObject.GetComponent<UIDispatcher>();
       
        for (int i = 0; i < uiUpgradeButton.Length; i++)
        {
            uiUpgradeButton[i].OnEnter += SetModifySpellStat;
        }
        tooltipCurrentTier = cadreCurrentTier.GetComponent<GuerhoubaGames.UI.TooltipTrigger>();
        tooltipNextTier = cadreNextTier.GetComponent<GuerhoubaGames.UI.TooltipTrigger>();
    }

    public void OpenUpgradeUI()
    {
        if (GameState.instance.IsGamepad()) UITools.instance.SetUIObjectSelect(firstObjectSelect);
        rerollText.text = m_upgradeManager.rerollPoint.ToString();
    }

    public void SetNewUpgradeData(UpgradeLevelingData data)
    {
        m_upgradeLevelingData = data;
        for (int i = 0; i < m_upgradeLevelingData.spellCount; i++)
        {
            spellInBar[i].sprite = m_upgradeLevelingData.iconSpell[i];
            spellInBar[i].material = m_upgradeLevelingData.materialIconSpell[i];
            materialIcon[i] = m_upgradeLevelingData.materialIconSpell[i];
        }
        OpenRotation();
        //m_Dispatcher.HideOrShowFixeUi(false);
        SetBaseSpellStat(0);

        upgradePointText.text = m_upgradeLevelingData.upgradePoint.ToString();

        for (int i = 0; i < 3; i++)
        {
            upgradeSelectable[i].text = ((m_upgradeLevelingData.upgradeChoose[i])).name;
            upgradeSelectableDescription[i].text = ((m_upgradeLevelingData.upgradeChoose[i])).description;
            //spellChoseUpgrade[i].material = materialIcon[data.indexSpellFocus];
            //if (gameObject.activeSelf) StartCoroutine(SpellFadeIn(i, Time.time));
            SetModifySpellStat(i);
        }
        currentSpellSelected.AcquireSpellData(m_upgradeLevelingData.spellState[m_upgradeLevelingData.indexSpellFocus]);
        SpellProfil tempSpellProfil = m_upgradeLevelingData.spellState[m_upgradeLevelingData.indexSpellFocus];
        if(tempSpellProfil.currentSpellTier <= 0)
        {
            if(tempSpellProfil.spellExp > 0)
            {
                fillNextTierRankPoint.fillAmount = ((float)tempSpellProfil.spellExp / 4);
            }
            else
            {
                fillNextTierRankPoint.fillAmount = 0;
            }

        }
        else
        {
            fillNextTierRankPoint.fillAmount = (((float)tempSpellProfil.spellExp - ((float)tempSpellProfil.currentSpellTier * 4))/4);
        }
        tempSpellProfil.spellExp += 4;
        currentspellSelected_NextRank.AcquireSpellData(tempSpellProfil);
        tempSpellProfil.spellExp -= 4;
    }

    public void UpdateUpgradesAvailable(UpgradeObject[] upgradeGenerate)
    {
        m_upgradeLevelingData.upgradeChoose = upgradeGenerate;
    }

    public void ChooseUpgrade(int index)
    {
        m_upgradeManager.SendUpgrade(m_upgradeLevelingData.upgradeChoose[index]);
        //m_upgradeManager.m_dropInventory.AddNewUpgrade(m_upgradeLevelingData.upgradeChoose[index], spellUpgradeFocus.sprite);
        GlobalSoundManager.PlayOneShot(31, Vector3.zero);
        //GameObject rankMoving = Instantiate(prefabRank, transform.position, transform.rotation); 

        for (int i = 0; i < upgradeSelectable.Length; i++)
        {
            if (i != index)
            {
                // if (gameObject.activeSelf) StartCoroutine(SpellFadeOut(i, Time.time));
            }
        }

        SetBaseSpellStat(index);

    }
    public void RerollUpgrade()
    {
        m_upgradeManager.ReDrawUpgrade();
        rerollText.text = m_upgradeManager.rerollPoint.ToString();
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
        for (int i = 0; i < spellProfil.gameEffectStats.statDatas.Count; i++)
        {
            SpellSystem.StatData statData = spellProfil.gameEffectStats.statDatas[i];
            if (statData.isVisible) textStatUpgrade += statData.stat.ToString() + " : " + spellProfil.gameEffectStats.GetStatValueToString(statData.stat) + "\n";
        }
        //ChangeTierCadre(spellProfil);
        upgradeTextStatBase[0].text = textStatUpgrade;
    }

    public void SetModifySpellStat(int index)
    {
        int indexSpell = m_upgradeLevelingData.upgradeChoose[index].indexSpellLink;

        SpellSystem.SpellProfil spellProfil = m_upgradeLevelingData.spellState[indexSpell];

        UpgradeObject stats = m_upgradeLevelingData.upgradeChoose[index];

        GlobalSoundManager.PlayOneShot(58, Vector3.zero);
        string textStatUpgrade = "";
        for (int i = 0; i < stats.gameEffectStats.statDatas.Count; i++)
        {

            SpellSystem.StatData statDataUpgrade = stats.gameEffectStats.statDatas[i];
            int indexStatSpell = spellProfil.gameEffectStats.GetStats(statDataUpgrade.stat);

            if (indexStatSpell != -1)
            {
                if(stats.IsAddingTag && stats.gameEffectStats.statDatas[i].isOnlyAddWithTag)
                {
                    continue;
                }
                SpellSystem.StatData statDataSpell = spellProfil.gameEffectStats.statDatas[indexStatSpell];

                textStatUpgrade += "" + stats.PreviewApplyValue(statDataSpell) + "\n";
                stateStat[0] = true;
            }
            else if(stats.IsAddingTag)
            {
                textStatUpgrade += "" + stats.PreviewNewValue(i) + "\n";
            }
            else textStatUpgrade += "";
        }
        for (int i = 0; i < upgradeTextStatModify.Length; i++)
        {
            for (int j = 0; j < stateStat.Length; j++)
            {
                if (i == index)
                {
                    upgradeTextStatModify[index].text = textStatUpgrade;
                }
                else if (i != index)
                {
                    //upgradeTextStatModify[index].text = "";
                }
            }
        }




    }

    public void ChangeTierCadre(SpellProfil data)
    {
        Sprite spellFocus = data.spell_Icon;
        spellUpgradeFocus.sprite = spellFocus;
        spellCurrentTierFocus.sprite = spellFocus;
        spellNextTierFocus.sprite = spellFocus;
        int currentTier = data.currentSpellTier % 4;
        if (currentTier >= 3)
        {
            cadreCurrentTier.sprite = spellTier[currentTier];
            cadreNextTier.sprite = spellTier[currentTier];


        }
        else
        {
            cadreCurrentTier.sprite = spellTier[currentTier];
            cadreNextTier.sprite = spellTier[currentTier + 1];

        }



        if (tooltipCurrentTier == null) { tooltipCurrentTier = spellCurrentTierFocus.GetComponent<GuerhoubaGames.UI.TooltipTrigger>(); }
        if (tooltipNextTier == null) { tooltipNextTier = spellNextTierFocus.GetComponent<GuerhoubaGames.UI.TooltipTrigger>(); }

        if (currentTier == 0)
        {
            if (data.levelSpellsProfiles[currentTier] == null || data.levelSpellsProfiles[currentTier + 1] == null) return;

            tooltipCurrentTier.content = "No additional effect yet";
            tooltipNextTier.content = data.levelSpellsProfiles[currentTier].description;
        }
        else if (currentTier > 0 && currentTier < 3)
        {
            if (data.levelSpellsProfiles[currentTier - 1] == null || data.levelSpellsProfiles[currentTier] == null) return;

            tooltipCurrentTier.content = data.levelSpellsProfiles[currentTier - 1].description;
            tooltipNextTier.content = data.levelSpellsProfiles[currentTier].description;
        }
        else if (currentTier >= 3)
        {
            if (data.levelSpellsProfiles[2] == null) return;

            tooltipCurrentTier.content = data.levelSpellsProfiles[2].description;
            tooltipNextTier.content = "Already max. No more additional effect";
        }

    }
    public void Update()
    {

    }

    public void OpenRotation()
    {
        //GameState.ChangeState();

        panelUI.SetActive(true);
        spellProfils = spellBook.GetSpellsRotations();
        //for (int i = 0; i < spellProfils.Length; i++)
        //{
        //    skillUiHolderTemp.Add(Instantiate(referenceSkill[i], skillUiHolder[i].transform.position, skillUiHolder[i].transform.rotation, skillUiHolder[i].transform));
        //}
    }

    public void CloseRotation()
    {
        GameState.ChangeState();
        for (int i = 0; i < skillUiHolderTemp.Count; i++)
        {
            Destroy(skillUiHolderTemp[i]);
        }
        skillUiHolderTemp.Clear();
        panelUI.SetActive(false);
        spellBook.CloseUiExchange();
    }
}