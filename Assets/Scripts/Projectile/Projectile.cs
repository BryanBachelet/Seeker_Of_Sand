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
    private const int m_timeBeforeDestruction = 3;
    private const float m_timeStartSizeShrinking = 0.75f;
    private const int maxSlopeAngle = 90;
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
    private bool willDestroy = false;
    protected Collider m_collider;
    private Vector3 m_initialScale;

    protected bool isStartToMove = false;

    private Vector3 normalHit;
    private Vector3 hitPoint;
    void Update()
    {
        if (!checkSpawnTime) { spawnTime = Time.time; checkSpawnTime = true; GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position); }
        else
        {
            float currentTime = Time.time;
            if (currentTime > spawnTime + m_deltaTimeMove)
            {
                if (willDestroy)
                {
                  if(m_collider)  m_collider.enabled = false;
                }
                else
                {
                    isStartToMove = true;
                    Move();

                }
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
        m_initialScale = transform.localScale;
        m_collider = this.GetComponent<Collider>();
    }
    protected virtual void Move()
    {
        //Debug.Log("Test");
        if (Physics.Raycast(transform.position, m_direction.normalized, m_speed * Time.deltaTime, m_layer))
        {
            Destroy(this.gameObject);
        }
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            normalHit = hit.normal;
            hitPoint = hit.point;

            SetSlopeRotation(hit.normal);


        }
        transform.position += transform.forward * m_speed * Time.deltaTime;
    }
    protected virtual void Duration()
    {
        if (m_lifeTimer > m_lifeTime + m_timeBeforeDestruction)
        {
            Destroy(this.gameObject);
            return;
        }
        if (m_lifeTimer > m_lifeTime)
        {
            willDestroy = true;
        }
        if (m_lifeTimer > m_lifeTime - m_timeStartSizeShrinking)
        {
            transform.localScale = Vector3.Lerp(m_initialScale, Vector3.zero, m_lifeTimer - m_lifeTime);
        }
       
        m_lifeTimer += Time.deltaTime;

    }

    private void ActiveDeath()
    {

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

            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject);
            if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;

            enemyTouch.ReceiveDamage(m_damage, other.transform.position - transform.position, m_power,-1);

            PiercingUpdate();
            if (piercingCount >= m_piercingMax) 
            {

                //Destroy(this.gameObject);
                m_lifeTimer = m_lifeTime;
                //willDestroy = true;
            }
        }
        else if (other.gameObject.tag == "Cristal")
        {
            other.GetComponent<CristalHealth>().ReceiveHit((int)m_damage);
            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                m_lifeTimer = m_lifeTime;
                //willDestroy = true;
            }
        }
        else if (other.gameObject.tag == "DPSCheck")
        {
            Punketone enemyTouch = other.GetComponent<Punketone>();

            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject);

            enemyTouch.currentHP -= m_damage;

            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                //willDestroy = true;
                m_lifeTimer = m_lifeTime;
            }
        }
        else if (other.gameObject.tag == "Dummy")
        {
            dummy enemyTouch = other.GetComponent<dummy>();

            m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject);
            enemyTouch.ReceiveDamage(m_damage, other.transform.position - transform.position, m_power, -1);

            enemyTouch.currentHP -= m_damage;

            PiercingUpdate();
            if (piercingCount >= m_piercingMax)
            {

                //Destroy(this.gameObject);
                //willDestroy = true;
                m_lifeTimer = m_lifeTime;
            }
        }
        else return;



    }

    protected virtual void PiercingUpdate()
    {
        piercingCount++;
    }
    protected virtual void UpdateTravelTime()
    {
    }

    public void SetSlopeRotation(Vector3 hitNormal)
    {
        Vector3 axis = Vector3.Cross(transform.right, hitNormal);
        Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        float angle = Vector3.SignedAngle(rotTest * Vector3.forward, axis, transform.right);
        if (Mathf.Abs(angle) > maxSlopeAngle)
            return;
        transform.rotation = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, 0);
    }

}
