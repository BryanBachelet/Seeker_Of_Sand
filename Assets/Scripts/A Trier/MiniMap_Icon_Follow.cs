using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap_Icon_Follow : MonoBehaviour
{
    [SerializeField] private Transform m_ObjectAssociat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_ObjectAssociat.position.x, 675, m_ObjectAssociat.position.z);
    }
}
