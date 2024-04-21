using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackComponent : MonoBehaviour
{
    public Transform basePosition;
    public int damage = 1;
    public bool isHeavyAttack = false;
    private List<GameObject> targetTouch = new List<GameObject>();

    public void OnEnable()
    {
        targetTouch.Clear();
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
            Debug.Log("As touch Target");
        }
    }

   
}
