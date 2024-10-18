using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackComponent : MonoBehaviour
{
    public Transform basePosition;
    public int damage = 1;
    private List<GameObject> targetTouch = new List<GameObject>();
    private Collider colliderAttack;
    public bool ActiveDebug = false;
    public string attackName;

    [Range(0, 180)]
    public float angleOfAttack = 180.0f;

    private float dotRatio;

    public void OnEnable()
    {
        targetTouch.Clear();
        colliderAttack = GetComponent<Collider>();

        if (angleOfAttack == 0) angleOfAttack = 180.0f;
        dotRatio = 1.0f - (angleOfAttack / 180f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        HealthPlayerComponent healthPlayerComponent = other.GetComponent<HealthPlayerComponent>();

        Vector3 dirTarget = healthPlayerComponent.transform.position - basePosition.position;
        float dot = Vector3.Dot(basePosition.forward, dirTarget.normalized);
        if (dot < dotRatio) return;
             


        if(!targetTouch.Contains(other.gameObject))
        {

            AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
            attackDamageInfo.attackName = attackName;
            attackDamageInfo.position = basePosition.position;
            attackDamageInfo.damage = damage;
             healthPlayerComponent.ApplyDamage(attackDamageInfo);

            targetTouch.Add(other.gameObject);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
       if(ActiveDebug) Gizmos.DrawCube( colliderAttack.bounds.center, colliderAttack.bounds.size);
    }


}
