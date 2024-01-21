using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileData
{
    public Vector3 direction;
    public float speed;
    public float life;
    public float travelTime;
    public float damage;
    public Vector3 destination;
    public GameObject area_Feedback;
    public int piercingMax;
    public int shootNumber;
    public int salveNumber;
    public float sizeFactor;
    public float size;
    public Character.CharacterShoot characterShoot;
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
    public int m_piercingMax;
    protected int piercingCount;

    protected float m_travelTime;
    protected float m_travelTimer;
    protected int m_shootNumber;
    protected int m_salveNumber;
    protected float m_size;
    protected float m_sizeMultiplicateurFactor;
    protected Character.CharacterShoot m_characterShoot;

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
        m_piercingMax = data.piercingMax;
        m_shootNumber = data.shootNumber;
        m_salveNumber = data.salveNumber;
        m_size = data.size;
        m_travelTime = data.travelTime;
        m_sizeMultiplicateurFactor = data.sizeFactor;
        m_characterShoot = data.characterShoot;
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
        if (other.gameObject.tag == "Enemy") 
        {
            Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();

            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies,other.gameObject);
            if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;

            enemyTouch.ReceiveDamage(m_damage, other.transform.position - transform.position, m_power);

            piercingCount++;
            if (piercingCount >= m_piercingMax)
            {

                Destroy(this.gameObject);
            }
        }
        else if (other.gameObject.tag == "Cristal")
        {
            other.GetComponent<CristalHealth>().ReceiveHit((int)m_damage);
            piercingCount++;
            if (piercingCount >= m_piercingMax)
            {

                Destroy(this.gameObject);
            }
        }
        else return;



    }
    protected virtual void UpdateTravelTime()
    {
    }

    public void SetSlopeRotation(Vector3 hitNormal)
    {
        Vector3 axis = Vector3.Cross(transform.right, hitNormal);
        Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        float angle = Vector3.SignedAngle( rotTest* Vector3.forward, axis,transform.right);
        transform.rotation = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, 0);
    }

}
