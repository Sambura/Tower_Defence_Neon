using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Tower : MonoBehaviour
{
    /// <summary>
    /// GameObject of point that the tower standing on (needed in case of tower deletion)
    /// </summary>
    [HideInInspector] public TowerPoint buildPoint;
    public GameObject gun;
    public GameObject circlePrefab;
    public GameObject upgrade;
    public float radius = 2;
    public float damage = 25;
    public float fireRate = 3;
    public int cost = 150;
    public float buildTime = 4;
    public Vector3 buildBarOffset = new Vector3(0, -0.5f);
    public Material particleMaterial;
    [HideInInspector] public int removeCost = 0;

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
                if (SelectedTower != null)
                {
                    SelectedTower.Deselect();
                }
                Controller.Instance.popupTowerMenu.ShowPopUp(transform.position, (upgrade == null) ? -1 :
                    (upgrade.GetComponent<Tower>().cost), (int)(removeCost * Controller.Instance.CurrentRemoveRatio));
                SelectedTower = this;
            }
            else
            {
                Controller.Instance.popupTowerMenu.ClosePopUp();
                SelectedTower = null;
            }
        }
    }

    protected Enemy target;
    protected GameObject circle;
    protected float nextShot;

    protected virtual void Start()
    {
        circle = Instantiate(circlePrefab, transform);
        circle.GetComponent<CircleDrawer>().Radius = radius;
        circle.SetActive(false);
        removeCost += cost;
    }

    protected virtual void OnMouseEnter()
    {
        circle.SetActive(true);
    }

    protected virtual void OnMouseExit()
    {
        if (SelectedTower != this)
            circle.SetActive(false);
    }

    protected virtual void OnMouseUpAsButton()
    {
        PopUpMenu = !PopUpMenu;
    }

    public virtual void Deselect()
    {
        circle.SetActive(false);
        PopUpMenu = false;
    }

    public virtual void PrepareForUpgrade()
    {
        circle.SetActive(false);
        Destroy(this);
    }

#if UNITY_EDITOR

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    protected void FixedUpdate()
    {
        if (Controller.Instance.drawDebugLines)
        {
            if (target != null)
            {
                var holder = new GameObject("Pointer", typeof(LineRenderer));
                var line = holder.GetComponent<LineRenderer>();
                line.sortingOrder = 2;
                line.positionCount = 2;
                line.widthMultiplier = 0.03f;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, target.transform.position);
                Destroy(holder, Time.fixedDeltaTime);
            }
            circle.SetActive(true);
        }
    }

#endif
}
