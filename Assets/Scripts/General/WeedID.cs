using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedID : MonoBehaviour
{
    [SerializeField] private float m_speed = 3;
    //[HideInInspector]
    public Transform m_playerTransform = null;
    [HideInInspector]
    public ThumbleWeedGenerator m_weedGenerator = null;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, m_playerTransform.position) > 200)
        {
            m_weedGenerator.DestroyListObject(this);
        }
    }
}
