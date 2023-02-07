using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileData
{
    public Vector3 direction;
    public float speed;
    public float life;
}



public class Projectile : MonoBehaviour
{
    [SerializeField] protected Vector3 m_direction;
    [SerializeField] protected float m_speed;
    [SerializeField] protected float m_lifeTime;
    [SerializeField] protected LayerMask m_layer;
    [SerializeField] protected float m_power;
    protected float m_lifeTimer;

    void  Update()
    {
        Move();
        Duration();
    }


    public virtual void SetProjectile(ProjectileData data)
    {
        m_direction = data.direction;
        m_speed = data.speed;
        m_lifeTime = data.life;
    }
    protected virtual void Move()
    {

        if (Physics.Raycast(transform.position, m_direction.normalized, m_speed * Time.deltaTime, m_layer))
        {

            Destroy(this.gameObject);
        }
        transform.position += m_direction.normalized * m_speed * Time.deltaTime;
    }
    protected virtual void Duration()
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

    private void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }

    protected virtual void CollisionEvent(Collider other)
    {
        if (other.tag != "Enemy") return;

        Enemies.Enemy enemyTouch = other.GetComponent<Enemies.Enemy>();

        if (enemyTouch.IsDestroing()) return;

        other.GetComponent<Enemies.Enemy>().GetDestroy(other.transform.position - transform.position, m_power);
        Destroy(this.gameObject);
    }

}
