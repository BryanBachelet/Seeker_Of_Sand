using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalHealth : MonoBehaviour
{
    [HideInInspector] static public Transform playerPosition;

    [SerializeField] private float m_healthMax;
    [SerializeField] private float m_currentHealth;
    [SerializeField] private int m_cristalToDropPerHealth = 1;
    [SerializeField] private GameObject m_cristalLootPrefab;
    [Range(0,3)]
    [SerializeField] public int m_cristalType = 0; //0 --> Water | 1 --> Fire | 2 --> Aer | 3 --> Ground
    [SerializeField] private UnityEngine.VFX.VisualEffect m_hitPrefab;
    [SerializeField] private GameObject[] cristalPart;
    private bool m_activeDeath;

    // Start is called before the first frame update
    void Start()
    {
        if(playerPosition == null) { playerPosition = GameObject.Find("Player").transform; }
        m_currentHealth = m_healthMax;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceiveHit(int damage)
    {
        for(int i = 0; i < damage; i++)
        {
            m_hitPrefab.Play();
            GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
            ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
            expMouvementScript.playerPosition = playerPosition;
            expMouvementScript.cristalType = m_cristalType;
        }
        m_currentHealth -= damage;
        if (m_currentHealth < m_healthMax * 0.66f)
        {
            cristalPart[0].SetActive(false);
        }
        if (m_currentHealth < m_healthMax * 0.33f)
        {
            //cristalPart[0].SetActive(false);
            cristalPart[1].SetActive(false);
        }
        if (m_currentHealth <= 0)
        {
            StartCoroutine(DestroyAfterDelay(3));
        }
    }

    public IEnumerator DestroyAfterDelay(float time)
    {
        //cristalPart[0].SetActive(false);
        //cristalPart[1].SetActive(false);
        cristalPart[2].SetActive(false);
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
