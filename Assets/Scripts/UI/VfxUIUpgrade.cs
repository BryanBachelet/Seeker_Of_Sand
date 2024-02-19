using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VfxUIUpgrade : MonoBehaviour
{
    public int indexSpellBar = 0;
    public Image[] spellInBar = new Image[4];
    public TMPro.TMP_Text[] upgradeSelectable = new TMPro.TMP_Text[3];

    [Header("Animation Select Paramaters")]
    private float m_durationOfCuve = 1f;
    private bool m_isGoingToSlot = false;
    [SerializeField] private float m_speed = 15;
    [SerializeField] private float m_speedUp = 40;


    public Transform m_upgradePosition;
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

