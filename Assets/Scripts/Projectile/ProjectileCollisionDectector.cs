using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionDectector : MonoBehaviour
{
    [SerializeField] private Projectile m_projectileScript;

 

    private void OnTriggerEnter(Collider other)
    {
        m_projectileScript.CollisionEvent(other);
    }
}
