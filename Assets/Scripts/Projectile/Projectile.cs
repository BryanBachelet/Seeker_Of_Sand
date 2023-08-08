using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileData
{
    public Vector3 direction;
    public float speed;
    public float life;
    public float damage;
    public Vector3 destination;
    public GameObject area_Feedback;
}



public class Projectile : MonoBehaviour
{   
    protected Vector3 m_direction;
    [SerializeField] protected float m_speed;
    [SerializeField] protected float m_lifeTime;
    [SerializeField] protected LayerMask m_layer;
    [SerializeField] protected float m_power;
    [SerializeField] protected float m_damage = 1;
    [SerializeField] public int m_indexSFX;

    protected Vector3 m_destination;
    protected float m_lifeTimer;
    public int piercingMax;
    private int piercingCount;

    private float spawnTime;
    private bool checkSpawnTime = false;
    [SerializeField] private float m_deltaTimeMove;
    void  Update()
    {
        if(!checkSpawnTime) { spawnTime = Time.time; checkSpawnTime = true; GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position); }
        else
        {
            if(Time.time > spawnTime + m_deltaTimeMove)
            {
                Move();
                Duration();
            }
        }
    }


    public virtual void SetProjectile(ProjectileData data)
    {
        m_direction = data.direction;
        m_speed = data.speed;
        m_lifeTime = data.life;
        m_damage = data.damage;
        m_destination = data.destination;


    }
    protected virtual void Move()
    {
        //Debug.Log("Test");
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
    public virtual void CollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;

        Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();

        if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;

        enemyTouch.ReceiveDamage(m_damage,other.transform.position - transform.position, m_power);

        piercingCount++;
        if (piercingCount >= piercingMax)
        {
            Destroy(this.gameObject);
        }

    }

}
