using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour
{
    /// <summary>
    /// GameObject of point that the tower standing on (needed in case of tower deletion)
    /// </summary>
    [HideInInspector] public GameObject buildPoint;
    public Animator animator;
    public GameObject gun;
    public GameObject circlePrefab;
    public GameObject upgrade;
    public GameObject[] lasers;
    public float radius = 2;
    public float damage = 25;
    public float fireRate = 3;
    public int cost = 150;

    private bool _popUpMenu = false;
    public static LaserTower SelectedTower { get; set; }

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
                if (SelectedTower != null)
                {
                    SelectedTower.Deselect();
                }
                Controller.Instance.popupTowerMenu.ShowPopUp(true, true, upgrade != null, transform.position);
                SelectedTower = this;
            } else
            {
                Controller.Instance.popupTowerMenu.ClosePopUp();
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
