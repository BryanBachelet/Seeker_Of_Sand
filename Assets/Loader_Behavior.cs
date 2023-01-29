using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Behavior : MonoBehaviour
{
    [SerializeField] private GameObject[] m_capsuleDisplayPrefab = new GameObject[2];
    [SerializeField] private Transform[] m_capsulePosition = new Transform[6];
    [SerializeField] private GameObject[] m_capsuleObject = new GameObject[6];
    [SerializeField] private int m_currentCapsule = 0;
    private Transform m_Objeect;
    private Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_Objeect = this.gameObject.transform;
        m_animator = this.gameObject.GetComponent<Animator>();
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
            ReloadCapsule();
            return;
        }
        m_Objeect.rotation *= Quaternion.Euler(0, 0, 60);
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
    }

    public void SetCapsuleOrder(int[] weaponOrder)
    {
        for(int i = 0; i < weaponOrder.Length; i++)
        {
            m_capsuleObject[i] = Instantiate(m_capsuleDisplayPrefab[weaponOrder[i]], m_capsulePosition[i].position, Quaternion.identity, m_Objeect);
        }
    }
}
