using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class UpgradeChoosing : MonoBehaviour
{
    public int indexSpellBar = 0;
    [Header("UI Object")]
    public Image[] spellInBar = new Image[4];
    public Image spellUpgradeFocus;
    public TMPro.TMP_Text upgradePointText;
    public TMPro.TMP_Text[] upgradeSelectable = new TMPro.TMP_Text[3];

    public EventSystem eventSystem;
    private UpgradeLevelingData m_upgradeLevelingData;
    [HideInInspector] public UpgradeManager m_upgradeManager;

    [SerializeField] private Image[] m_imageBandeau = new Image[3];
    [SerializeField] private Material[] m_materialBandeauDissolve = new Material[3];
    public GameObject[] decorationHolder = new GameObject[3];
    public float timeSolve;


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
            upgradeSelectable[i].text = ((m_upgradeLevelingData.upgradeChoose[i])).gain.nameUpgrade;
            if (gameObject.activeSelf) StartCoroutine(SpellFadeIn(i, Time.time));
        }
    }

    public void UpdateUpgradesAvailable(Upgrade[] upgradeGenerate)
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
                if (gameObject.activeSelf) StartCoroutine(SpellFadeOut(i, Time.time));
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
        int indexSpell = m_upgradeLevelingData.upgradeChoose[index].capsuleIndex;
        CapsuleStats stats = m_upgradeLevelingData.spellState[indexSpell];
        upgradeTextStatBase[0].text = "Damage : " + stats.damage;
        upgradeTextStatBase[1].text = "Size : " + stats.size;
        upgradeTextStatBase[2].text = "Speed : " + stats.speed;
        upgradeTextStatBase[3].text = "Projectile : " + stats.projectileNumber;
        upgradeTextStatBase[4].text = "Salve : " + stats.shootNumber;
        upgradeTextStatBase[5].text = "Piercing : " + stats.piercingMax;
    }
    public void SetModifySpellStat(int index)
    {
        int indexSpell = m_upgradeLevelingData.upgradeChoose[index].capsuleIndex;

        CapsuleStats baseStats = m_upgradeLevelingData.spellState[indexSpell];
        CapsuleStats stats = m_upgradeLevelingData.upgradeChoose[index].gain.capsulsStats;

        float[] baseSpellStats = baseStats.GetVisibleStat();
        float[] spellStatUpgrade = stats.GetVisibleStat();

        for (int i = 0; i < spellStatUpgrade.Length; i++)
        {
            if (spellStatUpgrade[i] != 0)
            {
                upgradeTextStatModify[i].text = "-->" + (baseSpellStats[i] + spellStatUpgrade[i]);
            }else
            {
                upgradeTextStatModify[i].text = "";
            }
        }
    }


    public void Update()
    {
       
    }
}