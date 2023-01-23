using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudID : MonoBehaviour
{
    [SerializeField] private float m_speed = 3;
    [HideInInspector]
    public Transform m_playerTransform = null;
    [HideInInspector]
    public CloudGenerator m_CloudGenerator = null;

    // Start is called before the first frame update
    void Start()
    {
        float rndSpeedVariation = Random.Range(-1.1f, 1.1f);
        m_speed += rndSpeedVariation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(3, 0, 0) * Time.deltaTime;
        if(Vector3.Distance(transform.position, m_playerTransform.position) > 200)
        {
            m_CloudGenerator.DestroyListObject(this);
        }
    }
}
