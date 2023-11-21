using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceMouvement : MonoBehaviour
{
    [HideInInspector] public Transform playerPosition;
    [SerializeField] private float m_speed = 15;
    private bool checkDistance = true;
    private bool startCoroutine = false;
    private float distance = 0;
    Vector3 destination = Vector3.zero;

    public int cristalType = 0;

    private bool m_destruction = false;
    private bool m_isFollowPlayer = false;
    private bool m_isGrounded = false;
    private bool m_isDropping = false;
    private float m_durationOfCuve = 1f;

    private float m_timeSpawned = 0;
    private float m_tempsEcoule;
    private void Start()
    {
        m_timeSpawned = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        if(playerPosition)
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

    public IEnumerator MoveToDestination(float time)
    {
        startCoroutine = true;
        Vector2 rnd = Random.insideUnitCircle;
        destination = new Vector3(transform.position.x + (rnd.x * 10), transform.position.y + 30, transform.position.z + (rnd.y * 10));
        yield return new WaitForSeconds(time);
        checkDistance = false;
    }
    
    public void initDestruction()
    {
        m_destruction = true;
    }


}
