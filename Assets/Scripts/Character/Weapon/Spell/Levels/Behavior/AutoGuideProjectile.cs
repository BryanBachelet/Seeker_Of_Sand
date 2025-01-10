using GuerhoubaGames.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{

    [CreateAssetMenu(fileName = "Target Projectile Level", menuName = "Spell/Target Projectile Level")]
    public class AutoGuideProjectile : BehaviorLevel
    {
        public int projectileCountBeforeEffect = 0;

        private int countBeforeEffect;
        public AnimationCurve directionCurve;
        public float timeOfRotate = 1;
        public float autoGuideSpeed = 50;

        [Range(0.1f, 10)]
        public float sizeMultiplier = 1;
        public override void OnProjectileShoot(ProjectileShootData projectileEffectData, GameObject lastProjectilSpawn)
        {
            countBeforeEffect++;
            if (countBeforeEffect < projectileCountBeforeEffect) return;

            countBeforeEffect = 0;
            GameObject instanceSpawn = GamePullingSystem.SpawnObject(projectileEffectData.profil.objectToSpawn,
                projectileEffectData.position, projectileEffectData.rotation * Quaternion.Euler(-135f, 0, Random.Range(-45f, -135f)));

            Vector3 currentScale = instanceSpawn.transform.localScale;
            instanceSpawn.transform.localScale = currentScale * sizeMultiplier;
            // Add script auto guide 
            Projectile baseProjectileBehavior = instanceSpawn.GetComponent<Projectile>();
            baseProjectileBehavior.enabled = false;
            ProjectileAutoTarget projectileAutoTarget = instanceSpawn.AddComponent<ProjectileAutoTarget>();

            projectileAutoTarget.SetProjectile(projectileEffectData.projectileData, null);
            projectileAutoTarget.isTemporary = true;
            projectileAutoTarget.directionCurve = directionCurve;
            projectileAutoTarget.timeDirection = timeOfRotate;
            projectileAutoTarget.autoGuideSpeed = autoGuideSpeed;

        }
    }

}