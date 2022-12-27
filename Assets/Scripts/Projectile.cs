using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Vector3 m_direction;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_lifeTime;
    [SerializeField] private LayerMask m_layer;
    private float m_lifeTimer;

    void Update()
    {
        Move();
        Duration();
    }

    public void SetDirection(Vector3 direction ) 
    { 
        m_direction = direction;
    }
    private void Move()
    {

        if (Physics.Raycast(transform.position, m_direction.normalized, m_speed * Time.deltaTime, m_layer ))
        {
            Destroy(this.gameObject);
        }
        transform.position += m_direction.normalized * m_speed * Time.deltaTime;
    }
    private void Duration()
    {
        if (m_lifeTimer > m_lifeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }


}
