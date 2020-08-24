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
    public GameObject gun;
    public GameObject circlePrefab;
    public float radius = 2;
    public float damage = 25;
    public float fireRate = 3;
    public float bulletVelocity = 25;
    public float bulletDestructionDelay = 1;

    private Enemy target;
    private GameObject circle;

    void Start()
    {
        StartCoroutine(Fire());
        circle = Instantiate(circlePrefab, transform);
        circle.GetComponent<CircleDrawer>().Radius = radius;
        circle.SetActive(false);
    }

    private void Update()
    {
        if (target != null)
        {
            if (gun != null)
                gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y,
                    target.transform.position.x - transform.position.x));
        }
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
        newBullet.DestructionDelay = bulletDestructionDelay;
        if (gun != null)
        gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y, 
            target.transform.position.x - transform.position.x));
    }

    private void OnMouseEnter()
    {
        circle.SetActive(true);
    }

    private void OnMouseExit()
    {
        circle.SetActive(false);
    }
}
