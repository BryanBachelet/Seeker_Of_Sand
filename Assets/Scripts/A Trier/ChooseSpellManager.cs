using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpellSystem;
using TMPro;
using GuerhoubaGames.UI;
using GuerhoubaGames.GameEnum;

public class ChooseSpellManager : MonoBehaviour
{
    private const double minAnimTime = 0.20;
    public GameObject[] vfxChooseSpell = new GameObject[4];
    public int[] randomSpellToChoose = new int[3];
    public SpellManager spellManager;
    public SpellSystem.SpellProfil[] newSpell = new SpellSystem.SpellProfil[3];
    public Image[] vfxSpell = new Image[3];
    public GameObject[] spellHolder = new GameObject[3];
    public TMPro.TMP_Text[] textObject = new TMPro.TMP_Text[3];
    public string[] nextSpellName = new string[3];
    [SerializeField] private Image[] m_imageBandeau = new Image[3];
    [SerializeField] private Material[] m_materialBandeauDissolve = new Material[3];
    public GameObject[] decorationHolder = new GameObject[3];
    public float timeSolve;


    public bool activeGeneration = false;

    public GameObject[] vfxHolder = new GameObject[3];
    public List<GameObject> vfxLastChooseSpell = new List<GameObject>();
    public PopupFunction[] popupFunction = new PopupFunction[3];
    public TMP_Text name;
    public TMP_Text description;
    public Image iconSpell;
    private PopupFunction lastSpell = null;
    public GameObject descriptionHolder = null;
    private Animator descriptionHolderAnimator;
    public bool overable = false;
    public TMP_Text textSpellFeedback;
    [HideInInspector] public UpgradeManager m_upgradeManagerComponenet;


    private Animator m_animator;
    private bool m_hasChooseSpell = false;
    private int m_indexSpellChoose = -1;

    public GameObject FirstObjectSelect;

    [Header("Elements Reward ")]
    public GameElement lastRoomElement;
    public float[] percentForUpgradeMatchingElementRoom = new float[4] { 100, 75, 50, 25 };
    private int countSpellDraw = 0;

    #region Unity Functions

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
                ClearVfxInstance();
                m_hasChooseSpell = false;
                textSpellFeedback.alpha = 0;

                for (int i = 0; i < vfxHolder.Length; i++)
                {
                    vfxHolder[i].SetActive(false);
                    spellHolder[i].SetActive(false);
                    vfxSpell[i].enabled = false;
                    vfxSpell[i].sprite = null;
                    m_materialBandeauDissolve[i].SetFloat("_Fade_Step", -0.5f);



                }
                m_upgradeManagerComponenet.SendSpell(newSpell[m_indexSpellChoose]);

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


    private void ClearVfxInstance()
    {
        if (vfxLastChooseSpell.Count > 0)
        {
            for (int i = 0; i < vfxLastChooseSpell.Count; i++)
            {
                Destroy(vfxLastChooseSpell[i]);
            }
            vfxLastChooseSpell.Clear();
        }
    }


    public void OpenSpellChoice()
    {
        if (!GameState.instance.IsGamepad()) return;
        countSpellDraw = 0;
        UITools.instance.SetUIObjectSelect(FirstObjectSelect);
    }

    public void ResetRandomSpell()
    {
        ClearVfxInstance();

        for (int i = 0; i < randomSpellToChoose.Length; i++)
        {
            // ---------------------
            randomSpellToChoose[i] = DrawRandomSpell();


            newSpell[i] = spellManager.spellProfils[randomSpellToChoose[i]];
            SpellManager.RemoveSpecificSpellFromSpellPool(randomSpellToChoose[i]);
            countSpellDraw++;
            // --------------------
            StartCoroutine(SpellFadeIn(i, Time.time));
            vfxSpell[i].sprite = newSpell[i].spell_Icon;
            vfxSpell[i].material = newSpell[i].matToUse;
            vfxHolder[i].SetActive(true);
            spellHolder[i].SetActive(true);
            vfxSpell[i].enabled = true;
            newSpell[i].tagData.element = spellManager.spellProfils[randomSpellToChoose[i]].tagData.element;
            nextSpellName[i] = newSpell[i].name;
            textObject[i].text = nextSpellName[i];
            int indexVFX = (int)newSpell[i].tagData.element - 1;
            indexVFX = Mathf.Clamp(indexVFX, 0, 100);
            GameObject lastVFx = Instantiate(vfxChooseSpell[indexVFX], vfxHolder[i].transform.position, vfxHolder[i].transform.rotation, vfxHolder[i].transform);
            vfxLastChooseSpell.Add(lastVFx);
        }
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

        if (lastRoomElement == GameElement.NONE) return SpellManager.GetRandomCapsuleIndex();

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
        for (int i = 0; i < vfxHolder.Length; i++)
        {
            if (i != indexChoice)
            {
                vfxHolder[i].SetActive(false);
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

    }

    public void SpellOverring(int spellIndex, PopupFunction popup)
    {
        if (lastSpell == popup) return;
        if (!overable) return;
        descriptionHolder.SetActive(true);
        if (descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", true); }
        lastSpell = popup;
        name.text = newSpell[spellIndex].name;
        description.text = newSpell[spellIndex].description;
        iconSpell.sprite = newSpell[spellIndex].spell_Icon;
        //description.text = popup.des
    }

    public void SpellOverringExit()
    {
        descriptionHolder.SetActive(false);
        if (descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", false); }
    }

    public IEnumerator SpellFadeOut(int spellNumber, float time)
    {
        decorationHolder[spellNumber].SetActive(false);

        while (time + timeSolve - Time.time > 0)
        {
            m_materialBandeauDissolve[spellNumber].SetFloat("_Fade_Step", (time + timeSolve - Time.time) / timeSolve - 0.5f);
            yield return Time.deltaTime;
        }
        m_materialBandeauDissolve[spellNumber].SetFloat("_Fade_Step", -0.5f);

    }

    public IEnumerator SpellFadeIn(int spellNumber, float time)
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
