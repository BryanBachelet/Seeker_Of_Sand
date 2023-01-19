using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosif : MonoBehaviour
{
    [SerializeField] private const float m_timeBeforeExplosion = 1.0f;
    [SerializeField] private float m_explosionSize;
    [SerializeField] private Vector3 m_direction;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_lifeTime;
    [SerializeField] private LayerMask m_layer;
    [SerializeField] private float m_power;
    [SerializeField] private LayerMask m_explosionMask;
    private bool m_isStick;
    private Transform m_stickTransform;
    private Vector3 m_stickPosition;
    private float m_lifeTimer;

    void Update()
    {
        Move();
        Duration();
    }

    public void SetDirection(Vector3 direction)
    {
        m_direction = direction;
    }
    private void Move()
    {
        if (m_isStick)
        {
            transform.position = m_stickTransform.position + m_stickPosition;
            return;
        }

        if (Physics.Raycast(transform.position, m_direction.normalized, m_speed * Time.deltaTime, m_layer))
        {
            Destroy(this.gameObject);
        }
        transform.position += m_direction.normalized * m_speed * Time.deltaTime;

    }
    private void Duration()
    {
        if (m_isStick) return;
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
        if (other.tag != "Enemy" || m_isStick) return;

        m_stickTransform = other.transform;
        m_stickPosition = other.transform.position - transform.position;
        m_isStick = true;
        StartCoroutine(TimeToExplose());
        //if (enemyTouch.IsDestroing()) return;

        //other.GetComponent<Enemies.Enemy>().GetDestroy(other.transform.position - transform.position, m_power);
        //Destroy(this.gameObject);

    }

    private IEnumerator TimeToExplose()
    {
        yield return new WaitForSeconds(m_timeBeforeExplosion);
        Explosion();
    }

    private void Explosion()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, m_explosionSize, m_explosionMask);
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemies.Enemy enemyTouch = enemies[i].GetComponent<Enemies.Enemy>();
            if (enemyTouch.IsDestroing()) return;

            enemyTouch.GetDestroy(enemyTouch.transform.position - transform.position, m_power);
        }
        Destroy(this.gameObject);
    }

}
