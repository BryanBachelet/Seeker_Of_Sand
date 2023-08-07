using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class RainBullet : Projectile
{
    [SerializeField] private float m_timeDamageTick = 0.3f;
    private float m_timerDamageTick = 0;

    [SerializeField] private float m_radiusArea = 2.0f;
    private List<Enemies.NpcHealthComponent> m_enemiesList = new List<Enemies.NpcHealthComponent>();
    public SphereCollider m_collider;
    private VisualEffect m_VisualEffect;
    private int[] m_EventNameIdentifier;
    public void Start()
    {
        m_collider = GetComponent<SphereCollider>();
        //m_VisualEffect = transform.GetComponentInChildren<VisualEffect>();
        //m_EventNameIdentifier[0] = Shader.PropertyToID("StartPluie");
        //m_EventNameIdentifier[1] = Shader.PropertyToID("StopPluie");
        //StartCoroutine(LaunchingPluieVfx());
        //GlobalSoundManager.PlayOneShot(11, transform.position);
    }

    public void Update()
    {
        if (TimerDamageTick())
        {
            for (int i = 0; i < m_enemiesList.Count; i++)
            {
                ApplyDamage(m_enemiesList[i]);
            }
        }
        Duration();
    }

    private bool TimerDamageTick()
    {
        if (m_timerDamageTick > m_timeDamageTick)
        {
            m_timerDamageTick = 0;
            return true;
        }
        else
        {
            m_timerDamageTick += Time.deltaTime;
            return false;
        }
    }
    private void ApplyDamage(Enemies.NpcHealthComponent enemy)
    {
        if (enemy == null) return;
        if (Vector3.Distance(enemy.transform.position, transform.position) > ((m_collider.radius/2.0f) - m_radiusArea))
        {
            enemy.ReceiveDamage(m_damage, (enemy.transform.position - transform.position).normalized, m_power);
            
            if (enemy.npcState == Enemies.NpcState.DEATH)
                m_enemiesList.Remove(enemy);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        CollisionEvent(other);
    }

    public void OnTriggerExit(Collider other)
    {
        ExitCollisionEvent(other);
    }

    public override void CollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;

        Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();

        if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;
        m_enemiesList.Add(enemyTouch);
    }

    public void ExitCollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;

        Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();

        if (enemyTouch.npcState == Enemies.NpcState.DEATH) return;
        m_enemiesList.Remove(enemyTouch);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, (m_collider.radius/ 2.0f));
        Gizmos.DrawWireSphere(transform.position, (m_collider.radius -m_radiusArea)/2.0f);
    }

    public IEnumerator LaunchingPluieVfx()
    {
        var eventAttribute = m_VisualEffect.CreateVFXEventAttribute();
        yield return new WaitForSeconds(0.1f);
        m_VisualEffect.SendEvent(m_EventNameIdentifier[0], eventAttribute);
    }
}
