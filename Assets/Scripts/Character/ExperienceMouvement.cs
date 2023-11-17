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
    private float m_timeBeforeDestruction = 3;
    [SerializeField] private TrailRenderer m_trail; 
    // Update is called once per frame
    void Update()
    {
        if(playerPosition)
        {
            if(!startCoroutine)
            {
                StartCoroutine(MoveToDestination(0.5f));
            }
            if(!checkDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerPosition.position, m_speed * Time.deltaTime);
                m_speed += (float)(10 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, m_speed * 5 * Time.deltaTime);
                float distance = Vector3.Distance(transform.position, destination);
            }
            if(distance < 3)
            {
                destination.y = Mathf.Lerp(destination.y, playerPosition.position.y, distance / 5);
            }
            else if (distance < 1)
            {
                checkDistance = false;
            }

        }
        if(m_destruction)
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
