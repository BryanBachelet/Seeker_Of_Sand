using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpellSystem;
using TMPro;

public class ChooseSpellManager : MonoBehaviour
{
    private const double minAnimTime = 0.20;
    public GameObject[] vfxChooseSpell = new GameObject[4];
    public int[] randomSpellToChoose = new int[3];
    public CapsuleManager capsuleManager;
    public Capsule[] newSpell = new Capsule[3];
    public Image[] vfxSpell = new Image[3];
    public GameObject[] spellHolder = new GameObject[3];
    public TMPro.TMP_Text[] textObject = new TMPro.TMP_Text[3];
    public string[] nextSpellName = new string[3];


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


    #region Unity Functions
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
        if(descriptionHolder !=  null) { descriptionHolderAnimator = descriptionHolder.GetComponent<Animator>(); }

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
            if ( m_hasChooseSpell)
            {
                ClearVfxInstance();
                m_hasChooseSpell = false;
                textSpellFeedback.alpha = 0;

                for (int i = 0; i < vfxHolder.Length; i++)
                {
                    vfxHolder[i].SetActive(false) ;
                    spellHolder[i].SetActive(false);
                    vfxSpell[i].enabled = false;
                    vfxSpell[i].sprite = null;
                    
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

        if(!m_animator) m_animator = this.GetComponent<Animator>();
        m_animator.ResetTrigger("Choosed");
        m_animator.SetBool("ActiveChoice",true);
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


    public void ResetRandomSpell()
    {
        ClearVfxInstance();

        for (int i = 0; i < randomSpellToChoose.Length; i++)
        {
            // ---------------------
            randomSpellToChoose[i] = CapsuleManager.GetRandomCapsuleIndex();
            newSpell[i] = capsuleManager.capsules[randomSpellToChoose[i]];
            // --------------------

            vfxSpell[i].sprite = newSpell[i].sprite;
            vfxSpell[i].material = newSpell[i].materialToUse;
            vfxHolder[i].SetActive(true);
            spellHolder[i].SetActive(true);
            vfxSpell[i].enabled = true;
            newSpell[i].elementType = capsuleManager.capsules[randomSpellToChoose[i]].elementType;
            nextSpellName[i] = newSpell[i].name;
            textObject[i].text = nextSpellName[i];
            Debug.Log(newSpell[i].elementType);
            GameObject lastVFx = Instantiate(vfxChooseSpell[(int)newSpell[i].elementType], vfxHolder[i].transform.position, vfxHolder[i].transform.rotation, vfxHolder[i].transform);
            vfxLastChooseSpell.Add(lastVFx);
        }
        ResetUIAnimation();
    }

    public void Choose(int indexChoice)
    {
        ActivateChoiceAnimation();
        for (int i = 0; i < vfxHolder.Length; i++)
        {
            if (i != indexChoice)
            {
                vfxHolder[i].SetActive(false);
                spellHolder[i].SetActive(false);
              
            }
        }
        m_indexSpellChoose = indexChoice;
        int capsuleIndex = randomSpellToChoose[indexChoice];
        CapsuleManager.RemoveSpecificSpellFromSpellPool(capsuleIndex);

    }

    public void SpellOverrring(int spellIndex, PopupFunction popup)
    {
        if (lastSpell == popup) return;
        if (!overable) return;
        descriptionHolder.SetActive(true);
        if(descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", true); }
        lastSpell = popup;
        name.text = newSpell[spellIndex].name;
        description.text = newSpell[spellIndex].description;
        iconSpell.sprite = newSpell[spellIndex].sprite;
        //description.text = popup.des
    }

    public void SpellOverringExit()
    {
        descriptionHolder.SetActive(false);
        if (descriptionHolderAnimator != null) { descriptionHolderAnimator.SetBool("Open", false); }
    }
}
