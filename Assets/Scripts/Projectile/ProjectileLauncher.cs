using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct ProjectileLauncherData
{
    public GameObject ownerGo;
    public float reloadTime;
    public float timeBetweenShoot;
    public int shootPerAttack;
    public int projectilePerShoot;
    public float angleTotal;
}

[System.Serializable]
// This a class that allow to have a projectile launcher
public class ProjectileLauncher
{
    public bool isAutomaticShoot = true;
    [HideInInspector] public GameObject ownerGo;
    public GameObject projectileToShoot;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public float timeBetweenShoot;
    [HideInInspector] public int shootPerAttack;
    [HideInInspector] public int projectilePerShoot;
    [HideInInspector] public float angleTotal;


    [HideInInspector] public Collider[] targetList;
    // Projecitle variable
    private int m_currentShootCount;
    private int m_projectileCount;

    private bool m_isBetweenShoot;
    private bool m_isReloading;

    private float m_reloadTimer;
    private float m_shootTimer;


    public void Start(ProjectileLauncherData ProjectileLauncherData)
    {
        ownerGo = ProjectileLauncherData.ownerGo;
        reloadTime = ProjectileLauncherData.reloadTime;
        timeBetweenShoot = ProjectileLauncherData.timeBetweenShoot;
        shootPerAttack = ProjectileLauncherData.shootPerAttack;
        projectilePerShoot = ProjectileLauncherData.projectilePerShoot;
        angleTotal = ProjectileLauncherData.angleTotal;
    }

    public void Update()
    {
        if (m_reloadTimer >= reloadTime)
        {
            m_isReloading = false;
            if (isAutomaticShoot) ShootAttack();
        }
        else
        {

            m_reloadTimer += Time.deltaTime;
        }
    }

    #region Shoot Functions

    public bool CanShoot()
    {
        return (m_reloadTimer >= reloadTime);
    }

    public void ShootAttack()
    {
        if (m_isReloading) return;

        if (m_currentShootCount == shootPerAttack || targetList.Length == 0)
        {
            m_isReloading = true;
            m_currentShootCount = 0;
            m_reloadTimer = 0.0f;
            return;
        }
        if (m_shootTimer > timeBetweenShoot)
        {
            m_isBetweenShoot = false;
            m_shootTimer = 0.0f;
            m_currentShootCount++;

            int indexTarget = Random.Range(0, targetList.Length);
            LaunchProjectile(targetList[indexTarget].transform);
        }
        else
        {
            m_shootTimer += Time.deltaTime;
        }
    }

    public void LaunchProjectile(Transform target,bool isSpecial = false)
    {
        if (target) return;

        float angle = angleTotal / projectilePerShoot;

        for (int i = 0; i < projectilePerShoot; i++)
        {

            GameObject projectileSpawn = GameObject.Instantiate(projectileToShoot, ownerGo.transform.position, Quaternion.identity);
            SpellSystem.ProjectileMeta projectileMeta = projectileSpawn.GetComponent<SpellSystem.ProjectileMeta>();

            SpellSystem.ProjectileMetaData projectileMetaData = new SpellSystem.ProjectileMetaData();
            projectileMetaData.isFromSpell = false;
            projectileMetaData.lifetime = 0;
            projectileMetaData.range = 0;
            projectileMetaData.target = target;
            projectileMetaData.direction = Quaternion.AngleAxis(angle * ((i + 1) / 2), ownerGo.transform.up) * (target.transform.position - projectileSpawn.transform.position);
            projectileMetaData.angle = angle * ((i + 1) / 2);
            projectileMeta.metaData = projectileMetaData;
        }

      if(!isSpecial )  m_isBetweenShoot = true;
    }



    #endregion
}
