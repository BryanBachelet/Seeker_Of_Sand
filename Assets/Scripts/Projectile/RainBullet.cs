using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class RainBullet : Projectile
{
    private const float m_maxTimeDamageTick = .3f;
    private const float m_minTimeDamageTick = 0.05f;
    [SerializeField] private float m_timeDamageTick = 0.3f;
    private float m_timerDamageTick = 0;

    [SerializeField] private float m_radiusArea = 2.0f;
    private List<Enemies.NpcHealthComponent> m_enemiesList = new List<Enemies.NpcHealthComponent>();
    public SphereCollider m_Spherecollider;
    private VisualEffect m_VisualEffect;
    private int[] m_EventNameIdentifier;
    public void Start()
    {
        m_Spherecollider = GetComponent<SphereCollider>();
        GlobalSoundManager.PlayOneShot(24, transform.position);
        InitUpgradeSpell();
    }

    public void InitUpgradeSpell()
    {
        m_Spherecollider.radius += m_size * m_sizeMultiplicateurFactor;
        m_timeDamageTick -= m_shootNumber * 0.02f;
        m_timeDamageTick = Mathf.Clamp(m_timeDamageTick, m_minTimeDamageTick, m_maxTimeDamageTick);
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
        if (Vector3.Distance(enemy.transform.position, transform.position) > ((m_Spherecollider.radius/2.0f) - m_radiusArea))
        {
            for (int i = 0; i < m_salveNumber; i++)
            {
                enemy.ReceiveDamage(m_damage, (enemy.transform.position - transform.position).normalized, m_power, -1);
            }
           
            
            if (enemy.m_npcInfo.state == Enemies.NpcState.DEATH)
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
        m_characterShoot.ActiveOnHit(other.transform.position, EntitiesTrigger.Enemies, other.gameObject);

        if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;
        m_enemiesList.Add(enemyTouch);
    }

    public void ExitCollisionEvent(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;

        Enemies.NpcHealthComponent enemyTouch = other.GetComponent<Enemies.NpcHealthComponent>();

        if (enemyTouch.m_npcInfo.state == Enemies.NpcState.DEATH) return;
        m_enemiesList.Remove(enemyTouch);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, (m_Spherecollider.radius/ 2.0f));
        Gizmos.DrawWireSphere(transform.position, (m_Spherecollider.radius -m_radiusArea)/2.0f);
    }

    public IEnumerator LaunchingPluieVfx()
    {
        var eventAttribute = m_VisualEffect.CreateVFXEventAttribute();
        yield return new WaitForSeconds(0.1f);
        m_VisualEffect.SendEvent(m_EventNameIdentifier[0], eventAttribute);
    }
}
