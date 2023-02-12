using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpJauge : MonoBehaviour
{
    [SerializeField] private float m_posXInit;
    [SerializeField] private float m_posXFinal;
    [SerializeField] private GameObject m_xpPointer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePointer(float xpFillAmount)
    {
        m_xpPointer.transform.position = new Vector3(Mathf.Lerp(m_posXInit, m_posXFinal, xpFillAmount), 515, 0); 
    }

}
