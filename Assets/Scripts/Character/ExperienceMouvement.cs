using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceMouvement : MonoBehaviour
{
    [HideInInspector] public Transform playerPosition;
    [SerializeField] private float m_speed = 15;
    private bool checkDistance = false;
    private bool startCoroutine = false;
    private float distance = 0;
    Vector3 destination = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        if(playerPosition)
        {
            if(!startCoroutine)
            {
                StartCoroutine(MoveToDestination(3));
            }
            if(!checkDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerPosition.position, m_speed * Time.deltaTime);
                m_speed += (float)(10 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, m_speed * Time.deltaTime);
            }

        }
    }

    public IEnumerator MoveToDestination(float time)
    {
        startCoroutine = true;
        destination = new Vector3(playerPosition.position.x, playerPosition.position.y + 100, playerPosition.position.z);
        yield return new WaitForSeconds(time);
        checkDistance = false;
    }



    
}
