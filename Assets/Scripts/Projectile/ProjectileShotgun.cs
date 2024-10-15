using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShotgun : Projectile
{



    protected override void Duration()
    {
        base.Duration();
    }


    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Test");
        }
    }

}
