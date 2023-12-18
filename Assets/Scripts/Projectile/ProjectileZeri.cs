using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileZeri : Projectile
{
    protected override void Duration()
    {
        base.Duration();
    }

    private Vector3 normalHit;
    private Vector3 hitPoint;

    private ApplyLightingStrike mLightingStrike;
    [Range(1,100)]
    public int probability;

    public void Start()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            //if (Vector3.Distance(transform.position, hit.point) < 1.2f)
            //{
            //    transform.position += (transform.position - hit.point).normalized *-3f;
            //    return;
            //}

            //if (Vector3.Distance(transform.position, hit.point) > 1.5f)
            //{
            //    transform.position += (hit.point - transform.position).normalized * -3f;
            //}
        }
        mLightingStrike = this.GetComponent<ApplyLightingStrike>();
    }

    protected override void Move()
    {
        RaycastHit hit = new RaycastHit();

        transform.position += transform.forward * m_speed * Time.deltaTime;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {
            normalHit = hit.normal;
            hitPoint = hit.point;
             
            SetSlopeRotation(hit.normal);
            //if (Vector3.Distance(transform.position, hit.point) < 0.9f)
            //{
            //    transform.position += (transform.position - hit.point).normalized * (1.0f - Vector3.Distance(transform.position, hit.point));
            //    return;
            //}

            //if (Vector3.Distance(transform.position, hit.point) > 1.1f)
            //{
            //    transform.position += (hit.point - transform.position).normalized * (Vector3.Distance(transform.position, hit.point) - 1.0f);
            //}
           
        }
    }

    private void OnDestroy()
    {
        int random = Random.Range(0, 100);
        if( random > probability) { return; }

        GlobalSoundManager.PlayOneShot(35, transform.position);
        mLightingStrike.CallLightingStrike(transform.position);
    }

    private void TriggerEffect()
    {

    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(hitPoint, hitPoint + normalHit.normalized * 5);
    }

}
