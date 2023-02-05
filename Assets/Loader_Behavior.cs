using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loader_Behavior : MonoBehaviour
{
    [SerializeField] private GameObject[] m_capsuleDisplayPrefab = new GameObject[2];
    [SerializeField] private Transform[] m_capsulePosition = new Transform[6];
    [SerializeField] private GameObject[] m_capsuleObject = new GameObject[6];
    [SerializeField] private int m_currentCapsule = 0;
    [SerializeField] private RectTransform[] m_NumberSlotPosition = new RectTransform[6];
    [SerializeField] private Text[] m_TextObjectNumberSlot = new Text[6];
    [SerializeField] private Vector2 relativePositionOfLoader;
    private Transform m_Objeect;
    private Animator m_animator;
    private bool m_reloading = false;
    // Start is called before the first frame update
    void Start()
    {
        m_Objeect = this.gameObject.transform;
        m_animator = this.gameObject.GetComponent<Animator>();
        relativePositionOfLoader = this.GetComponent<RectTransform>().anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveCapsule()
    {
        m_capsuleObject[m_currentCapsule].SetActive(false);
        m_currentCapsule += 1;
        if(m_currentCapsule >= m_capsuleObject.Length)
        {
            m_reloading = true;
            ReloadCapsule();
            return;
        }
        GlobalSoundManager.PlayOneShot(4, Vector3.zero);
        m_Objeect.rotation *= Quaternion.Euler(0, 0, 60);
        ReplaceNumberSlotText(m_currentCapsule);
    }

    public void ReloadCapsule()
    {
        m_animator.SetTrigger("Reloading");
        StartCoroutine(DelayReload());
    }

    public IEnumerator DelayReload()
    {
        GlobalSoundManager.PlayOneShot(2, Vector3.zero);
        yield return new WaitForSeconds(1.1f);
        m_Objeect.rotation = Quaternion.Euler(0, 0, 0);
        m_currentCapsule = 0;
        for (int i = 0; i < m_capsuleObject.Length; i++)
        {
            m_capsuleObject[i].SetActive(true);
        }
        ReplaceNumberSlotText(0);
        m_reloading = false;
    }

    public void SetCapsuleOrder(int[] weaponOrder)
    {
        for(int i = 0; i < weaponOrder.Length; i++)
        {
            m_capsuleObject[i] = Instantiate(m_capsuleDisplayPrefab[weaponOrder[i]], m_capsulePosition[i].position, Quaternion.identity, this.transform);
            //m_capsuleObject[i].transform.parent = m_Objeect;
        }
    }

    public bool GetReloadingstate()
    {
        return m_reloading;
    }

    public void ReplaceNumberSlotText(int currentIndex)
    {
        int compteur = currentIndex + 1;
        for(int i = 0; i < m_TextObjectNumberSlot.Length; i++)
        {
            m_TextObjectNumberSlot[i].text = "" + (compteur);
            if(i > compteur)
            {
                m_TextObjectNumberSlot[i].color = Color.gray;
            }
            else
            {
                m_TextObjectNumberSlot[i].color = Color.white;
            }
            compteur++;
            if(compteur > m_TextObjectNumberSlot.Length)
            {
                compteur = 1;
            }
        }
    }
}
