using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Projectile
{

    [SerializeField] private float m_damagePerSpeed = 0.5f;
    private Rigidbody m_rigidbody;
    private bool m_hasRoll;
    [SerializeField] private float initialForce = 50; 
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponentInChildren<Rigidbody>();
        transform.position += m_direction.normalized * 1.5f;
        GlobalSoundManager.PlayOneShot(m_indexSFX, transform.position);
        m_rigidbody.AddForce(Vector3.forward * initialForce, ForceMode.Impulse);
    }


    public void FixedUpdate()
    {
    }
    // Update is called once per frame
    void Update()
    {
        ControlSpeed();
        Duration();
    }

    private void ControlSpeed()
    {
        if (m_rigidbody.velocity.magnitude > 1)
        {
            m_hasRoll = true;
            return;
        }
        if (m_hasRoll && m_rigidbody.velocity.magnitude < 0.5f)
        {
            Destroy(this.gameObject);
        }
    }

    protected override void Duration()
    {
        if (m_hasRoll) return;

        if (m_lifeTimer > m_lifeTime)
        {

            Destroy(this.gameObject);
        }
        else
        {
            m_lifeTimer += Time.deltaTime;
        }
    }

    public void OnTriggerEnter(Collider other)
    {

    }


    public override void CollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;
        //GlobalSoundManager.PlayOneShot(10, transform.position);
        Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();

        if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;
        enemyTouch.ReceiveDamage(m_rigidbody.velocity.magnitude * m_damage, (other.transform.position - transform.position).normalized, m_power);
    }

}
