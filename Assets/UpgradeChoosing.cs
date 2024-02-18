using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChoosing : MonoBehaviour
{
    public int indexSpellBar = 0;
    public GameObject[] spellInBar = new GameObject[4];
    public GameObject[] upgradeSelectable = new GameObject[3];
    private float m_durationOfCuve = 1f;
    private bool m_isGoingToSlot = false;
    [SerializeField] private float m_speed = 15;
    [SerializeField] private float m_speedUp = 40;

    public Transform m_upgradePosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_upgradePosition)
        {
            MoveDestination();

        }
    }

    public void MoveDestination()
    {
        Vector3 direction = m_upgradePosition.position - transform.position;
        if (direction.magnitude < 1)
        {
            return;
        }
        Debug.Log(direction.magnitude);
        if (!m_isGoingToSlot)
        {
            transform.position += Vector3.up * m_speedUp * Time.deltaTime;
            m_speedUp -= m_speedUp * 1.0f / m_durationOfCuve * Time.deltaTime;
        }

        transform.position += direction.normalized * m_speed * Time.deltaTime;
        m_speed += (10.0f * Time.deltaTime);
    }
    public void SelectUpgrade(int indexSpellAssociated)
    {
        transform.position = upgradeSelectable[indexSpellAssociated].transform.position;
        m_upgradePosition = spellInBar[indexSpellBar].transform;
        StartCoroutine(MoveToDestination());
    }

    public IEnumerator MoveToDestination()
    {
        yield return new WaitForSeconds(m_durationOfCuve);
        yield return new WaitForSeconds(m_durationOfCuve * 2);
        m_isGoingToSlot = true;
    }
}
