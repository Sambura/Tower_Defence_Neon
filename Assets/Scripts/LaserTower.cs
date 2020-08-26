using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    public Animator animator;
    public GameObject[] lasers;

    protected void Update()
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
            foreach (var i in lasers)
            {
                var sr = i.GetComponent<SpriteRenderer>();
                sr.size = new Vector2(Vector2.Distance(sr.transform.position, target.transform.position), sr.size.y);
            }
            if (nextShot <= Time.time)
            {
                animator.SetTrigger("Fire");
                nextShot = Time.time + 1 / fireRate;
            }
        }
    }

    public void FireBullet()
    {
        if (target == null) return;
        target.TakeDamage(damage);
    }

    public override void PrepareForUpgrade()
    {
        circle.SetActive(false);
        enabled = false;
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Stop");
    }
}
