using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /// <summary>
    /// GameObject of point that the tower standing on (needed in case of tower deletion)
    /// </summary>
    public GameObject point;
    public GameObject bullet;
    public float radius = 2;
    public float damage = 25;
    public float fireRate = 3;
    public float bulletVelocity = 25;

    private Enemy target;

    void Start()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            if (target != null && Vector2.Distance(transform.position, target.transform.position) <= radius)
            {
                FireBullet();
            }
            else
            {
                target = null;
                foreach (var i in Controller.Instance.SpawnedEnemies)
                {
                    if (Vector2.Distance(transform.position, i.transform.position) <= radius)
                    {
                        target = i;
                        FireBullet();
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1 / fireRate);
        }
    }

    private void FireBullet()
    {
        var newBullet = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
        newBullet.Target = target;
        newBullet.Damage = damage;
        newBullet.Velocity = bulletVelocity;
    }
}
