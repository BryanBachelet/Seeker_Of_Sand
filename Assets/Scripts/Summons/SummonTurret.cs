using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace SpellSystem
{
    public class SummonTurret : MonoBehaviour
    {
        private SummonsMeta m_summonsMeta;
        public ProjectileLauncher projectileLauncher;

        private float m_range;
        private SpellProfil spellProfil;


        // TODO :
        // 1. Allow to special attack

        void Start()
        {
            m_summonsMeta = GetComponent<SummonsMeta>();
            m_summonsMeta.OnSpecialSkill += AttackSpecial;
            spellProfil = m_summonsMeta.summonData.spellProfil;
            InitProjectileLauncher();
        }

        private void InitProjectileLauncher()
        {
            ProjectileLauncherData projectileLauncherData = new ProjectileLauncherData();
            projectileLauncherData.projectilePerShoot = spellProfil.GetIntStat(StatType.Projectile);
            projectileLauncherData.shootPerAttack = spellProfil.GetIntStat(StatType.ShootNumber);
            projectileLauncherData.timeBetweenShoot = spellProfil.GetFloatStat(StatType.TimeBetweenShot);
            projectileLauncherData.angleTotal = spellProfil.GetFloatStat(StatType.ShootAngle);
            projectileLauncherData.reloadTime = spellProfil.GetFloatStat(StatType.AttackReload);

            projectileLauncher.Start(projectileLauncherData);
        }

        void Update()
        {
            projectileLauncher.Update();
            Attack();
        }

        public void Attack()
        {
            if (m_summonsMeta.summonData.spellProfil.TagList.EqualsSpellNature(GuerhoubaGames.GameEnum.SpellNature.PROJECTILE))
            {
                if (!projectileLauncher.CanShoot()) 
                    return;

                projectileLauncher.targetList = FindTarget(projectileLauncher.shootPerAttack);
                projectileLauncher.ShootAttack();


            }
        }

        public void AttackSpecial()
        {
            if (m_summonsMeta.summonData.spellProfil.TagList.EqualsSpellNature(GuerhoubaGames.GameEnum.SpellNature.PROJECTILE))
            {
                projectileLauncher.targetList = FindTarget(projectileLauncher.shootPerAttack);
                for (int i = 0; i < projectileLauncher.targetList.Length; i++)
                {
                    projectileLauncher.LaunchProjectile(projectileLauncher.targetList[i].transform, true);
                }
               
            }
        }

        #region Shoot Projectile Functions


        private Collider[] FindTarget(int targetCount)
        {
            Collider[] targetObj = new Collider[targetCount];
            Dictionary<int, float> rangeList = new Dictionary<int, float>();

            Collider[] allTarget = Physics.OverlapSphere(transform.position, m_range, GameLayer.instance.enemisLayerMask);

            for (int i = 0; i < allTarget.Length; i++)
            {
                float distance = (allTarget[i].transform.position - transform.position).magnitude;
                if (rangeList.Count < targetCount)
                {
                    rangeList.Add(i, distance);
                    targetObj[i] = allTarget[i];
                }
                else
                {
                    if (distance >= rangeList[targetCount - 1])
                        continue;

                    for (int j = 0; j < targetCount ; j++)
                    {
                        if (distance >= rangeList[j])
                            continue;

                        rangeList[j] = distance;
                        targetObj[j] = allTarget[i];
                        break;
                    }


                }
            }

            return targetObj;

        }

        #endregion
    }

}