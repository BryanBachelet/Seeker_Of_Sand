using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceMouvement : MonoBehaviour
{
    [HideInInspector] public Transform playerPosition;
    [SerializeField] private float m_speed = 15;
    // Update is called once per frame
    void Update()
    {
        if(playerPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition.position, m_speed * Time.deltaTime);
        }
    }
}
