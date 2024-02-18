using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalHealth : MonoBehaviour
{
    [HideInInspector] static public Transform playerPosition;

    [SerializeField] private float m_healthMax;
    [SerializeField] private float m_currentHealth;
    [SerializeField] private GameObject m_cristalLootPrefab;
    private bool[] state = new bool[3];
    [Range(0, 3)]
    [SerializeField] public int m_cristalType = 0; //0 --> Water | 1 --> Fire | 2 --> Aer | 3 --> Ground
    [SerializeField] private UnityEngine.VFX.VisualEffect m_hitPrefab;
    [SerializeField] private GameObject[] cristalPart;
    private bool m_activeDeath;

    // Start is called before the first frame update
    void Start()
    {
        if (playerPosition == null) { playerPosition = GameObject.Find("Player").transform; }
        m_currentHealth = m_healthMax;
    }

    public void ReceiveHit(int damage)
    {
        m_hitPrefab.Play();
        m_currentHealth -= damage;
        if (m_currentHealth < m_healthMax * 0.66f || state[0] == false)
        {
            cristalPart[0].SetActive(false);
            state[0] = true;
                GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
                ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
                expMouvementScript.ActiveExperienceParticule(playerPosition);
                expMouvementScript.cristalType = m_cristalType;
        }
        if (m_currentHealth < m_healthMax * 0.33f || state[1] == false)
        {
            //cristalPart[0].SetActive(false);
            cristalPart[1].SetActive(false);
            state[1] = true;
            for (int i = 0; i < 3; i++)
            {
                GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
                ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
                expMouvementScript.ActiveExperienceParticule(playerPosition);
                expMouvementScript.cristalType = m_cristalType;
            }
        }
        if (m_currentHealth <= 0 || state[2] == false)
        {
            state[2] = true;
            StartCoroutine(DestroyAfterDelay(3));
            for (int i = 0; i < 6; i++)
            {
                GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
                ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
                expMouvementScript.ActiveExperienceParticule(playerPosition);
                expMouvementScript.cristalType = m_cristalType;
            }
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
