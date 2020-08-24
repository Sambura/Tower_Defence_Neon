using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /// <summary>
    /// GameObject of point that the tower standing on (needed in case of tower deletion)
    /// </summary>
    [HideInInspector] public GameObject buildPoint;
    public Animator animator;
    public GameObject bullet;
    public GameObject gun;
    public GameObject circlePrefab;
    public GameObject upgrade;
    public GameObject fx;
    public float radius = 2;
    public float damage = 25;
    public float fireRate = 3;
    public float bulletVelocity = 25;
    public float bulletDestructionDelay = 1;

    private bool _popUpMenu = false;
    public static Tower SelectedTower { get; set; }

    public bool PopUpMenu
    {
        get
        {
            return _popUpMenu;
        }
        set
        {
            _popUpMenu = value;
            if (value)
            {
                var popup = Controller.Instance.popupTowerMenu;
                if (SelectedTower != null)
                {
                    SelectedTower.Deselect();
                }
                SelectedTower = this;
                popup.SetActive(true);
                popup.transform.position = transform.position;
                var script = popup.GetComponent<TowerPopUp>();
                script.ResetButtons();
                if (upgrade == null) script.upgradeButton.interactable = false;
            } else
            {
                Controller.Instance.popupTowerMenu.SetActive(false);
                SelectedTower = null;
            }
        }
    }

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
            if (fx != null)
            {
                var sr = fx.GetComponent<SpriteRenderer>();
                sr.size = new Vector2(Vector2.Distance(transform.position, target.transform.position) - 0.12f, sr.size.y);
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
                foreach (var i in Controller.Instance.SpawnedEnemies)
                {
                    if (Vector2.Distance(transform.position, i.transform.position) <= radius)
                    {
                        target = i;
                        if (animator != null)
                        {
                            animator.SetTrigger("Fire");
                        }
                        else FireBullet();
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1 / fireRate);
        }
    }

    public void FireBullet()
    {
        if (target == null) return;
        if (bullet != null)
        {
            var newBullet = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
            newBullet.Target = target;
            newBullet.Damage = damage;
            newBullet.Velocity = bulletVelocity;
            newBullet.DestructionDelay = bulletDestructionDelay;
        } else
        {
            target.TakeDamage(damage);
        }
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
        if (SelectedTower != this)
            circle.SetActive(false);
    }

    private void OnMouseUpAsButton()
    {
        PopUpMenu = !PopUpMenu;
    }

    private void Deselect()
    {
        circle.SetActive(false);
        PopUpMenu = false;
    }
}
