using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosif : MonoBehaviour
{
    [SerializeField] private float m_timeBeforeExplosion = 1.0f; //Remove const declaration
    [SerializeField] private float m_explosionSize;
    [SerializeField] private Vector3 m_direction;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_lifeTime;
    [SerializeField] private LayerMask m_layer;
    [SerializeField] private float m_power;
    [SerializeField] private LayerMask m_explosionMask;
    [SerializeField] private Material m_explosionMatToUse;
    private bool m_isStick;
    private Transform m_stickTransform;
    private Vector3 m_stickPosition;
    private float m_lifeTimer;
    private Transform m_transform;
    private Material m_mat_explosion;
    public AnimationCurve scaleByTime_Curve;
    public AnimationCurve coloriseByTime_Curve;
    public Color baseEmissiveColor;

    private void Start()
    {
        m_transform = gameObject.GetComponent<Transform>();
        m_mat_explosion = new Material(m_explosionMatToUse);
        gameObject.GetComponent<MeshRenderer>().material = m_mat_explosion;

    }
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
            if(m_stickTransform == null)
            {
                m_isStick = false;
            }else
            { 
                transform.position = m_stickTransform.position + m_stickPosition; 
            }
           
            ScaleByTime();
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
        GlobalSoundManager.PlayOneShot(0, transform.position);
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemies.Enemy enemyTouch = enemies[i].GetComponent<Enemies.Enemy>();
            if (enemyTouch.IsDestroing()) return;

            enemyTouch.GetDestroy(enemyTouch.transform.position - transform.position, m_power);
        }
        Destroy(this.gameObject);
    }

    private void ScaleByTime()
    {
        m_lifeTimer += Time.deltaTime;
        float scaleByTime = scaleByTime_Curve.Evaluate(m_lifeTimer / m_timeBeforeExplosion) / 2;
        m_transform.localScale = new Vector3(scaleByTime, scaleByTime, scaleByTime);
        m_mat_explosion.SetColor("_EmissiveColor", baseEmissiveColor * coloriseByTime_Curve.Evaluate(m_lifeTimer / m_timeBeforeExplosion));
        //m_mat_explosion.SetFloat("_EmissiveIntensity", 1.3f * scaleByTime * 200);
    }


}
