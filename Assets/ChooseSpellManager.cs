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
        if(activeGeneration)
        {
            activeGeneration = false;
            m_animator.SetBool("ActiveChoose", false);
            ResetRandomSpell();
        }
    }

    public void ActiveAnimation()
    {
        m_animator.SetBool("ActiveChoose", true);
    }
    public void ResetRandomSpell()
    {
        if(vfxLastChooseSpell.Count > 0)
        {
            for(int i = 0; i < vfxLastChooseSpell.Count; i ++)
            {
                Destroy(vfxLastChooseSpell[i]);
            }
            vfxLastChooseSpell.Clear();
        }
        for(int i = 0; i < randomSpellToChoose.Length; i++)
        {
            randomSpellToChoose[i] = CapsuleManager.GetRandomCapsuleIndex();
            newSpell[i] = capsuleManager.capsules[randomSpellToChoose[i]];
            vfxSpell[i].sprite = newSpell[i].sprite;
            GameObject lastVFx = Instantiate(vfxChooseSpell[(int)newSpell[i].type], transform.position, transform.rotation, vfxHolder[i].transform);
            vfxLastChooseSpell.Add(lastVFx);
        }
        ActiveAnimation();
    }

    
}
