using GuerhoubaGames.Resources;
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
    [HideInInspector] public float timeBeforeDestruction = 3;
    public int quantity;

    public int cristalType = 0;

    public bool m_destruction = false;
    private bool m_isFollowPlayer = false;
    private bool m_isGrounded = false;
    private bool m_isDropping = false;
    public float m_durationOfCuve = 1f;

    private float m_timeSpawned = 0;
    private float m_tempsEcoule;
    private ObjectState m_state;
    private Collider m_coll;
    private PullingMetaData m_pullingMetaData;
    private float m_timerBeforeDestruction;

    private void Awake()
    {

        m_pullingMetaData = GetComponent<PullingMetaData>();
        if (m_pullingMetaData) m_pullingMetaData.OnSpawn += InitComponent;
    }

    private void Start()
    {
        m_state = new ObjectState();
        GameState.AddObject(m_state);

        if (m_pullingMetaData == null)
        {
            m_timeSpawned = Time.time;
            m_coll = this.GetComponent<Collider>();
            m_timerBeforeDestruction = timeBeforeDestruction;
        }
    }



    public void InitComponent()
    {
        m_timeSpawned = Time.time;
        m_coll = this.GetComponent<Collider>();
        m_timerBeforeDestruction = timeBeforeDestruction;
        m_destruction = false;
        if (m_coll) m_coll.enabled = true;
        m_isGrounded = false;
        m_tempsEcoule = 0.0f;
        m_speed = 15.0f;
    }

    void Update()
    {
        if (!m_isGrounded && GroundPosition != Vector3.zero)
        {
            MoveGround();
            if (m_tempsEcoule < 2)
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
                if (Vector3.Distance(transform.position, m_playerPosition.position) < 1)
                {
                    m_isGrounded = true;
                }
                else
                {
                    MoveDestination();
                }


            }
        }

        if (m_destruction)
        {
            m_timerBeforeDestruction -= Time.deltaTime;
            if (m_trail) m_trail.time = m_timerBeforeDestruction;
            if (m_timerBeforeDestruction < 0) DestroyObject();
        }
    }

    public void DestroyObject()
    {
        if (m_pullingMetaData == null)
        {
            Destroy(this.gameObject);
        }
        else if (m_pullingMetaData.isActive)
        {
            int idData = GetComponent<PullingMetaData>().id;
            GamePullingSystem.instance.ResetObject(this.gameObject, idData);
            ResetComponent();
        }
    }

    private void ResetComponent()
    {
        m_tempsEcoule = 0;
        m_isGrounded = false;
        m_timerBeforeDestruction = timeBeforeDestruction;
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
        GameState.RemoveObject(m_state);
        m_destruction = true;
        if (m_coll) m_coll.enabled = false;
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
