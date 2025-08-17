using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePhoenix : Projectile
{

    public bool activeRetunrMovement;
    private float ratioDirection = 1;

    public override void SetProjectile(ProjectileData data, CharacterProfile charaProfil)
    {
       
        base.SetProjectile(data, charaProfil);
        if (spellProfil.currentSpellTier == 3)
        {
            activeRetunrMovement = true;
        }
        else
        {
            activeRetunrMovement = false;
        }
    }

    protected override void ResetProjectile()
    {
        base.ResetProjectile();
        ratioDirection = 1;
    }
    protected override void Move()
    {

        if (activeRetunrMovement)
        {
            if (Physics.Raycast(transform.position, m_direction.normalized, m_speed * Time.deltaTime, m_layer))
            {
                ActiveDeath();
            }
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
            {
                normalHit = hit.normal;
                hitPoint = hit.point;

                SetSlopeRotation(hit.normal);


            }
            transform.position += ratioDirection * transform.forward * 2.0f * m_speed * Time.deltaTime;
        }
        else
        {
            base.Move();
        }
    }


    protected override void Duration()
    {
        base.Duration();

        if (m_lifeTimer > m_lifeTime / 2.0f)
        {
            ratioDirection = -1;
        }

    }


    protected override void PiercingUpdate()
    {
        if (objectType == CharacterObjectType.FRAGMENT) return;

        if (isStartToMove)
        {
            if (spellProfil.TagList.EqualsSpellParticularity(SpellParticualarity.Piercing))
                piercingCount++;
        }
    }
}