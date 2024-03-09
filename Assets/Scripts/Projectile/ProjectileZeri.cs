using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileZeri : Projectile
{
    protected override void Duration()
    {
        base.Duration();
    }

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
    }


}
