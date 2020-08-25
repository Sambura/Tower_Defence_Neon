using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    public Animator animator;
    public GameObject[] lasers;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(Fire());
    }

    protected override void Update()
    {
        if (target != null)
        {
            if (gun != null)
                gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y,
                    target.transform.position.x - transform.position.x));
            if (lasers != null)
            {
                foreach (var i in lasers)
                {
                    var sr = i.GetComponent<SpriteRenderer>();
                    sr.size = new Vector2(Vector2.Distance(sr.transform.position, target.transform.position), sr.size.y);
                }
            }
        }
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            if (target != null && Vector2.Distance(transform.position, target.transform.position) <= radius)
            {
                if (animator != null)
                {
                    animator.SetTrigger("Fire");
                }
                else FireBullet();
            }
            else
            {
                target = null;
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
                if (target != null)
                {
                    if (lasers != null)
                    {
                        foreach (var i in lasers)
                        {
                            var sr = i.GetComponent<SpriteRenderer>();
                            sr.size = new Vector2(Vector2.Distance(sr.transform.position, target.transform.position), sr.size.y);
                        }
                    }
                    if (gun != null)
                        gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y,
                            target.transform.position.x - transform.position.x));
                    if (animator != null)
                    {
                        animator.SetTrigger("Fire");
                    }
                }
            }
            yield return new WaitForSeconds(1 / fireRate);
        }
    }

    public void FireBullet()
    {
        if (target == null) return;
        target.TakeDamage(damage);
    }
}
