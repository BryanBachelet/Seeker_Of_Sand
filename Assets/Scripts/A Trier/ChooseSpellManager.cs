using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpellSystem;
using TMPro;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using Unity.VisualScripting;

public class ChooseSpellManager : MonoBehaviour
{
    [HideInInspector] private int[] randomSpellToChoose = new int[3];
    [HideInInspector] private SpellManager spellManager;
    [HideInInspector] private SpellSystem.SpellProfil[] newSpell = new SpellSystem.SpellProfil[3];
    [SerializeField] private Image[] vfxSpell = new Image[3];
    [SerializeField] private GameObject[] spellHolder = new GameObject[3];
    [SerializeField] private TMPro.TMP_Text[] textObject = new TMPro.TMP_Text[3];
    [HideInInspector] private string[] nextSpellName = new string[3];
    [SerializeField] private Image[] m_imageBandeau = new Image[3];
    [HideInInspector] private Material[] m_materialBandeauDissolve = new Material[3];
    [SerializeField] private GameObject[] decorationHolder = new GameObject[3];
    [HideInInspector] private float timeSolve = 1;

    [SerializeField] private TMP_Text spellName;
    [SerializeField] private TMP_Text spellDescription;
    [SerializeField] private Image iconSpell;
    [HideInInspector] private PopupFunction lastSpell = null;
    [SerializeField] private GameObject descriptionHolder = null;
    [SerializeField] private Animator descriptionHolderAnimator;
    [SerializeField] private TMPro.TMP_Text[] tagText = new TMPro.TMP_Text[3];
    [HideInInspector] private bool overable = false;
    [SerializeField] private TMP_Text textSpellFeedback;
    [HideInInspector] public UpgradeManager m_upgradeManagerComponenet;


    [HideInInspector] private Animator m_animator;
    [HideInInspector] private bool m_hasChooseSpell = false;
    [HideInInspector] private int m_indexSpellChoose = -1;

    [SerializeField] private GameObject FirstObjectSelect;

    [Header("Elements Reward ")]
    [HideInInspector] public GameElement lastRoomElement;
    [SerializeField] private float[] percentForUpgradeMatchingElementRoom = new float[4] { 100, 75, 50, 25 };
    [HideInInspector] private int countSpellDraw = 0;

    [SerializeField] private spell_Attribution[] rankSpell_Preview = new spell_Attribution[3];
    [SerializeField] private Image spellTier1Image;
    [SerializeField] private Image spellTier2Image;
    [SerializeField] private Image spellTier3Image;
    [SerializeField] private Image cadreTier1Image;
    [SerializeField] private Image cadreTier2Image;
    [SerializeField] private Image cadreTier3Image;
    [SerializeField] private TooltipTrigger[] tooltipNextTier = new TooltipTrigger[3];

    #region Unity Functions

    private void Awake()
    {
        spellManager = GameObject.Find("General_Manager").GetComponent<SpellManager>();
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
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
        if (descriptionHolder != null) { descriptionHolderAnimator = descriptionHolder.GetComponent<Animator>(); }


    }
    #endregion


    public void Update()
    {
        AnimatorStateInfo animatorStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("StartState"))
        {
            if (m_hasChooseSpell)
            {
                m_animator.SetBool("ActiveChoice", true);
            }
        }
        if (animatorStateInfo.IsName("NewSpellChoice_Open"))
        {
            if (m_hasChooseSpell)
            {
                m_hasChooseSpell = false;
                //textSpellFeedback.alpha = 0;

                //for (int i = 0; i < 3; i++)
                //{
                //    //vfxHolder[i].SetActive(false);
                //    spellHolder[i].SetActive(false);
                //    vfxSpell[i].enabled = false;
                //    vfxSpell[i].sprite = null;
                //    m_materialBandeauDissolve[i].SetFloat("_Fade_Step", -0.5f);
                //}
                //m_upgradeManagerComponenet.SendSpell(newSpell[m_indexSpellChoose]);

            }
        }

    }

    // Reset the animation state of the UI at his first state
    private void ResetUIAnimation()
    {
        m_hasChooseSpell = false;
        m_indexSpellChoose = -1;

        if (!m_animator) m_animator = this.GetComponent<Animator>();
        m_animator.ResetTrigger("Choosed");
        m_animator.SetBool("ActiveChoice", true);
        overable = true; // --> Je sais pas
    }

    private void ActivateChoiceAnimation()
    {
        m_hasChooseSpell = true;
        m_animator.SetBool("ActiveChoice", false);
        if (descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", false); }
        m_animator.SetTrigger("Choosed");
    }



    public void OpenSpellChoice()
    {
        if (!GameState.instance.IsGamepad()) return;
        countSpellDraw = 0;
        UITools.instance.SetUIObjectSelect(FirstObjectSelect);
    }

    public void ResetRandomSpell()
    {

        for (int i = 0; i < randomSpellToChoose.Length; i++)
        {
            // ---------------------
            randomSpellToChoose[i] = DrawRandomSpell();
           

            newSpell[i] = spellManager.spellProfils[randomSpellToChoose[i]];
           if(newSpell[i].idFamily!=0) SpellManager.idFamilyBan.Add(newSpell[i].idFamily);
            SpellManager.RemoveSpecificSpellFromSpellPool(randomSpellToChoose[i]);
            countSpellDraw++;
            // --------------------
            StartCoroutine(SpellFadeIn(i, Time.time));
            vfxSpell[i].sprite = newSpell[i].spell_Icon;
            vfxSpell[i].material = newSpell[i].matToUse;
            //vfxHolder[i].SetActive(true);
            spellHolder[i].SetActive(true);
            vfxSpell[i].enabled = true;
            newSpell[i].tagData.element = spellManager.spellProfils[randomSpellToChoose[i]].tagData.element;
            nextSpellName[i] = newSpell[i].name;
            textObject[i].text = nextSpellName[i];
            int indexVFX = (int)GeneralTools.GetElementalArrayIndex( newSpell[i].tagData.element);
            indexVFX = Mathf.Clamp(indexVFX, 0, 100);
            //GameObject lastVFx = Instantiate(vfxChooseSpell[indexVFX], vfxHolder[i].transform.position, vfxHolder[i].transform.rotation, vfxHolder[i].transform);
            //vfxLastChooseSpell.Add(lastVFx);
        }

        SpellManager.idFamilyBan.Clear();

        ResetUIAnimation();
    }


    private int DrawRandomSpell()
    {
        GameElement[] elements = m_upgradeManagerComponenet.m_characterUpgradeComponent.m_characterInventory.GetElementSpellInRotation();
        bool isElementOwn = false;

        for (int i = 0; i < elements.Length; i++)
        {
            if (lastRoomElement == elements[i])
            {
                isElementOwn = true;
                break;
            }
        }

        if (lastRoomElement == GameElement.NONE) return SpellManager.GetRandomSpellIndex();

        float percent = Random.Range(0.0f, 100.0f);

        if(percent< percentForUpgradeMatchingElementRoom[countSpellDraw])
        {
            return SpellManager.GetElementRandomSpellIndex(lastRoomElement);
        }else
        {
            return  SpellManager.GetRandomSpellIndexWithoutOneElememt(lastRoomElement);
        }

    }



    public void Choose(int indexChoice)
    {
        ActivateChoiceAnimation();
        GlobalSoundManager.PlayOneShot(31, Vector3.zero);
        for (int i = 0; i < 3; i++)
        {
            if (i != indexChoice)
            {
                //vfxHolder[i].SetActive(false);
                //spellHolder[i].SetActive(false);
                StartCoroutine(SpellFadeOut(i, Time.time));
            }
        }

        for (int i = 0; i < randomSpellToChoose.Length; i++)
        {
            if (indexChoice != i)
            {
                SpellManager.AddSpecificSpellFromSpellPool(randomSpellToChoose[i]);
            }
        }
        m_indexSpellChoose = indexChoice;
        countSpellDraw = 0;

        // Close the UI
        textSpellFeedback.alpha = 0;

        for (int i = 0; i < 3; i++)
        {
            //vfxHolder[i].SetActive(false);
            spellHolder[i].SetActive(false);
            vfxSpell[i].enabled = false;
            vfxSpell[i].sprite = null;
            m_materialBandeauDissolve[i].SetFloat("_Fade_Step", -0.5f);
        }
        m_upgradeManagerComponenet.SendSpell(newSpell[m_indexSpellChoose]);

    }

    public void SpellOverring(int spellIndex, PopupFunction popup)
    {
        if (lastSpell == popup && descriptionHolder.activeSelf)  return;
        if (!overable) return;
        descriptionHolder.SetActive(true);
        if (descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", true); }
        lastSpell = popup;
        spellName.text = newSpell[spellIndex].name;
        string[] tagLines = newSpell[spellIndex].tagData.GetUIInfosValue();
        for (int i = 0; i < tagText.Length; i++)
        {
            tagText[i].text = tagLines[i];
        }
        spellDescription.text = newSpell[spellIndex].description + "\n" + newSpell[spellIndex].DebugStat();
        Material materialSpell = newSpell[spellIndex].matToUse;
        iconSpell.material = materialSpell;
        for(int i = 0; i < rankSpell_Preview.Length; i++)
        {
            rankSpell_Preview[i].AcquireSpellData(newSpell[spellIndex]);
            rankSpell_Preview[i].ChangeNameForSpecificText(newSpell[spellIndex].levelSpellsProfiles[i].nameLevelUprade + " " + newSpell[spellIndex].levelSpellsProfiles[i].LevelType);
            //newSpell[spellIndex].levelSpellsProfiles[i].nameLevelUprade + " " + newSpell[spellIndex].levelSpellsProfiles[i].LevelType;
        }
        for (int i = 0; i < newSpell[spellIndex].levelSpellsProfiles.Length; i++)
        {
            tooltipNextTier[i].content = newSpell[spellIndex].levelSpellsProfiles[i].description;
        }


        GlobalSoundManager.PlayOneShot(58, Vector3.zero);
        //description.text = popup.des
    }


    public void CloseSpellChoiceUI()
    {
        SpellOverringExit();
    }

    public void SpellOverringExit()
    {
        descriptionHolder.SetActive(false);
        if (descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", false); }
    }

    public IEnumerator SpellFadeOut(int spellNumber, float time)
    {
        if (decorationHolder.Length > 0) 
        {
            decorationHolder[spellNumber].SetActive(false);

            while (time + timeSolve - Time.time > 0)
            {
                m_materialBandeauDissolve[spellNumber].SetFloat("_Fade_Step", (time + timeSolve - Time.time) / timeSolve - 0.5f);
                yield return Time.deltaTime;
            }
            m_materialBandeauDissolve[spellNumber].SetFloat("_Fade_Step", -0.5f);
        }


    }

    public IEnumerator SpellFadeIn(int spellNumber, float time)
    {
        if (decorationHolder.Length > 0)
        {
            while (Time.time - time < timeSolve)
            {
                m_materialBandeauDissolve[spellNumber].SetFloat("_Fade_Step", (Time.time - time) / timeSolve - 0.5f);
                yield return Time.deltaTime;
            }
            m_materialBandeauDissolve[spellNumber].SetFloat("_Fade_Step", 0.6f);
            decorationHolder[spellNumber].SetActive(true);
        }
    }

}
