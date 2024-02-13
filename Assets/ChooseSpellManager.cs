using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpellSystem;
public class ChooseSpellManager : MonoBehaviour
{
    public GameObject[] vfxChooseSpell = new GameObject[4];
    public int[] randomSpellToChoose = new int[3];
    public CapsuleManager capsuleManager;
    public Capsule[] newSpell = new Capsule[3];
    public Image[] vfxSpell = new Image[3];
    public GameObject[] spellHolder = new GameObject[3];

    public bool activeGeneration = false;

    public GameObject[] vfxHolder = new GameObject[3];
    public List<GameObject> vfxLastChooseSpell = new List<GameObject>();

    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ActiveAnimation()
    {
        m_animator.ResetTrigger("Choosed");
        m_animator.SetTrigger("ActiveChoose");
    }
    public void ResetRandomSpell()
    {
        if (vfxLastChooseSpell.Count > 0)
        {
            for(int i = 0; i < vfxLastChooseSpell.Count; i ++)
            {
                Destroy(vfxLastChooseSpell[i]);
            }
            vfxLastChooseSpell.Clear();
        }
        for(int i = 0; i < randomSpellToChoose.Length; i++)
        {
            vfxHolder[i].SetActive(true);
            spellHolder[i].SetActive(true);
            randomSpellToChoose[i] = CapsuleManager.GetRandomCapsuleIndex();
            newSpell[i] = capsuleManager.capsules[randomSpellToChoose[i]];
            vfxSpell[i].sprite = newSpell[i].sprite;
            newSpell[i].elementType = capsuleManager.capsules[randomSpellToChoose[i]].elementType;
            Debug.Log(newSpell[i].elementType);
            GameObject lastVFx = Instantiate(vfxChooseSpell[(int)newSpell[i].elementType], vfxHolder[i].transform.position, vfxHolder[i].transform.rotation, vfxHolder[i].transform);
            vfxLastChooseSpell.Add(lastVFx);
        }
        ActiveAnimation();
    }

    public void Choose(int indexChoice)
    {
        m_animator.ResetTrigger("ActiveChoose");
        m_animator.SetTrigger("Choosed");
        for(int i = 0; i < vfxHolder.Length; i++)
        {
            if(i != indexChoice)
            {
                vfxHolder[i].SetActive(false);
                spellHolder[i].SetActive(false);
            }
        }
    }
    
}
