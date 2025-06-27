using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditText : MonoBehaviour
{
    [Header("Credits Variables ")]
    [SerializeField] private float m_creditDuration = 3;
    [SerializeField] private float m_creditSpeed = 50;
    [SerializeField] private float m_creditStartDelayTime = 1;
    [SerializeField] private int m_sceneIndexMenu = 1;

    [Header("Credits Objects")]
    [SerializeField] private GameObject m_creditTextContainer;

    private float m_creditTime;

   
    // Update is called once per frame
    void Update()
    {
        UpdateCredit();
    }

    private void UpdateCredit()
    {
        if (m_creditTime > m_creditStartDelayTime)
        {
            m_creditTextContainer.transform.position += Vector3.up * m_creditSpeed * Time.deltaTime;
        }
        if (m_creditTime > (m_creditStartDelayTime + m_creditDuration))
        {
            SceneManager.LoadScene(m_sceneIndexMenu);

        }
        m_creditTime += Time.deltaTime;
    }
}
