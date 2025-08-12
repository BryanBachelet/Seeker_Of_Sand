using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using UnityEngine.VFX;
public class CristalHealth : MonoBehaviour
{
    [HideInInspector] static public Transform playerPosition;

    private float m_healthMax = 10;
    private float m_currentHealth = 10;
    [SerializeField] private GameObject m_cristalLootPrefab;
    private bool[] state = new bool[3];
    [SerializeField] public  GameElement cristalElement; //0 --> Water | 1 --> Aer | 2 --> Fire | 3 --> Ground
    [SerializeField] private VisualEffect m_hitPrefab;
    [SerializeField] private GameObject[] cristalPart;
    private bool m_activeDeath = false;

    private float radiusPlayerCollect = 10;
    private SphereCollider sphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        if (playerPosition == null) { playerPosition = GameObject.Find("Player").transform; }
        m_currentHealth = m_healthMax;
        if(!sphereCollider) { sphereCollider = this.GetComponent<SphereCollider>(); sphereCollider.radius = radiusPlayerCollect; }
        m_hitPrefab = GetComponentInChildren<VisualEffect>();
    }

    public void ReceiveHit(int damage)
    {
        damage = (int)m_currentHealth;
        if (m_hitPrefab) { m_hitPrefab.Play(); }
        m_currentHealth -= damage;
        if (m_currentHealth < m_healthMax * 0.66f && state[0] == false)
        {
            cristalPart[0].SetActive(false);
            state[0] = true;
            GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
            ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
            expMouvementScript.ActiveExperienceParticule(playerPosition);
            expMouvementScript.cristalType = (int)cristalElement;
        }
        if (m_currentHealth < m_healthMax * 0.33f && state[1] == false)
        {
            //cristalPart[0].SetActive(false);
            cristalPart[1].SetActive(false);
            state[1] = true;
            for (int i = 0; i < 3; i++)
            {
                GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
                ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
                expMouvementScript.ActiveExperienceParticule(playerPosition);
                expMouvementScript.cristalType = (int)cristalElement;
            }
        }
        if (m_currentHealth <= 0 && state[2] == false)
        {
            state[2] = true;
            StartCoroutine(DestroyAfterDelay(1.5f));
            for (int i = 0; i < 6; i++)
            {
                GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
                ExperienceMouvement expMouvementScript = cristalInstantiate.GetComponent<ExperienceMouvement>();
                expMouvementScript.ActiveExperienceParticule(playerPosition);
                expMouvementScript.cristalType = (int)cristalElement;
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

  

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ReceiveHit((int)m_currentHealth);
        }
    }

    private void OnDestroy()
    {
        //ReceiveHit((int)m_currentHealth);
    }
}
