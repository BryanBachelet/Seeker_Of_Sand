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

   
    private bool m_activeDeath;

    // Start is called before the first frame update
    void Start()
    {
        if(playerPosition == null) { playerPosition = GameObject.Find("Player").transform; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveHit(int damage)
    {
        for(int i = 0; i < damage; i++)
        {
            GameObject cristalInstantiate = Instantiate(m_cristalLootPrefab, transform.position, transform.rotation);
            cristalInstantiate.GetComponent<ExperienceMouvement>().playerPosition = playerPosition;
        }
    }

}
