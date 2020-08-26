using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AreaTower : Tower
{
    [Header("Area tower exclusives")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileVelocity = 30;
    [SerializeField] private float damageRadius = 0.5f;
    [SerializeField] private float damageDecay = 1; // Linear decay, 0 - no decay, 1 - no damage on area edges

    protected override void Update()
    {
        if (target != null)
        {
            if (Vector2.Distance(target.transform.position, transform.position) > radius)
                target = null;
        }
        if (target == null)
        {
            float lastProgress = 0;
            foreach (var i in Controller.Instance.SpawnedEnemies)
            {
                if (Vector2.Distance(transform.position, i.transform.position) <= radius)
                {
                    if (target == null || lastProgress < i.RouteProgress)
                    {
                        target = i;
                        lastProgress = i.RouteProgress;
                    }
                }
            }
        }
        if (target != null)
        {
            gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y,
                            target.transform.position.x - transform.position.x));
            if (nextShot <= Time.time)
            {
                FireBullet();
                nextShot = Time.time + 1 / fireRate;
            }
        }
    }

    public void FireBullet()
    {
        if (target == null) return;
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<AreaDamageProjectile>();
        projectile.Velocity = projectileVelocity;
        projectile.Damage = damage;
        projectile.Destination = target.transform.position;
        projectile.DamageDecay = damageDecay;
        projectile.DamageRadius = damageRadius;
    }
}
