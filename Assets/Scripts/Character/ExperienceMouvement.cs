using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceMouvement : MonoBehaviour
{
    public Transform m_playerPosition;
    [HideInInspector] public Vector3 GroundPosition;
    [Header("Particule Parameters")]
    [SerializeField] private float m_speed = 15;
    [SerializeField] private float m_speedUp = 40;
    [SerializeField] private TrailRenderer m_trail;
    [SerializeField] private float m_timeBeforeDestruction = 3;

    public int cristalType = 0;

    public bool m_destruction = false;
    private bool m_isFollowPlayer = false;
    private bool m_isGrounded = false;
    private bool m_isDropping = false;
    private float m_durationOfCuve = 1f;

    private float m_timeSpawned = 0;
    private float m_tempsEcoule;

    private Collider m_coll;
    private void Start()
    {
        m_timeSpawned = Time.time;
        m_coll = this.GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!m_isGrounded && GroundPosition != Vector3.zero)
        {
            MoveGround();
            if(m_tempsEcoule < 2)
            {
                m_tempsEcoule += Time.deltaTime;
            }
            else
            {
                m_isGrounded = true;
                
            }
        }
        else
        {
            if (m_playerPosition)
            {
                MoveDestination();

            }
        }
        
        if (m_destruction)
        {
            m_timeBeforeDestruction -= Time.deltaTime;
            m_trail.time = m_timeBeforeDestruction;
            if (m_timeBeforeDestruction < 0) Destroy(this.gameObject);
        }
    }

    public IEnumerator MoveToDestination()
    {
        yield return new WaitForSeconds(m_durationOfCuve);
        yield return new WaitForSeconds(m_durationOfCuve * 2);
        m_isFollowPlayer = true;
    }

    public IEnumerator MoveToGround()
    {
        yield return new WaitForSeconds(m_durationOfCuve);
        yield return new WaitForSeconds(m_durationOfCuve * 2);
        m_isDropping = true;
    }

    public void InitDestruction()
    {
        m_destruction = true;
        m_coll.enabled = false;
    }

    public void MoveDestination()
    {
        Vector3 direction = m_playerPosition.position - transform.position;

        if (!m_isFollowPlayer)
        {
            transform.position += Vector3.up * m_speedUp * Time.deltaTime;
            m_speedUp -= m_speedUp * 1.0f / m_durationOfCuve * Time.deltaTime;
        }

        transform.position += direction.normalized * m_speed * Time.deltaTime;
        m_speed += (10.0f * Time.deltaTime);
    }

    public void MoveGround()
    {
        Vector3 direction = GroundPosition - transform.position;

        if (!m_isDropping)
        {
            transform.position += Vector3.up * 3 * m_speedUp * Time.deltaTime;
            m_speedUp -= m_speedUp * 1.0f / m_durationOfCuve * Time.deltaTime;
        }
        else
        {
            if (Vector3.Distance(transform.position, GroundPosition) < 1) { m_isGrounded = true; }
        }
        transform.position += direction.normalized * m_speed * Time.deltaTime;
        m_speed += (10.0f * Time.deltaTime);

    }

    public void ActiveExperienceParticule(Transform target)
    {
        m_playerPosition = target;
        StartCoroutine(MoveToDestination());
    }
}
