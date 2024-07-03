using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackComponent : MonoBehaviour
{
    public Transform basePosition;
    public int damage = 1;
    public bool isHeavyAttack = false;
    private List<GameObject> targetTouch = new List<GameObject>();
    private Collider colliderAttack;
    public bool ActiveDebug = false;

    public void OnEnable()
    {
        targetTouch.Clear();
        colliderAttack = GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        HealthPlayerComponent healthPlayerComponent = other.GetComponent<HealthPlayerComponent>();
       
        if(!targetTouch.Contains(other.gameObject))
        {
            if(!isHeavyAttack) healthPlayerComponent.GetLightDamage(damage, basePosition.position);
            if(isHeavyAttack) healthPlayerComponent.GetHeavyDamage(damage);
            targetTouch.Add(other.gameObject);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
       if(ActiveDebug) Gizmos.DrawCube( colliderAttack.bounds.center, colliderAttack.bounds.size);
    }


}
