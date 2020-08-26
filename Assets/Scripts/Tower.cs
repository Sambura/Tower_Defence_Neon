﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                Controller.Instance.popupTowerMenu.ShowPopUp(transform.position, (upgrade == null) ? -1 : (upgrade.GetComponent<Tower>().cost), removeCost);
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

    protected virtual void Start()
    {
        circle = Instantiate(circlePrefab, transform);
        circle.GetComponent<CircleDrawer>().Radius = radius;
        circle.SetActive(false);
        removeCost += (int)(cost * Controller.Instance.removeCostRatio);
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            if (gun != null)
                gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y,
                    target.transform.position.x - transform.position.x));
        }
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

    protected virtual void Deselect()
    {
        circle.SetActive(false);
        PopUpMenu = false;
    }
}