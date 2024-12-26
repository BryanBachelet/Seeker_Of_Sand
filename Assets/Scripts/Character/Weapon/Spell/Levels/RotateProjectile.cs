using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rotate Projectile Behavior", menuName = "Spell/CustomBehavior/Rotate Projectile")]
public class RotateProjectile : CustomStatBehavior
{
    public override void Apply(SpellProfil spellProfil)
    {
        spellProfil.angleRotation =  new Vector3(0, 0f, 90f);
    }
}
