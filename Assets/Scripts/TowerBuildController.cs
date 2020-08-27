﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuildController : MonoBehaviour
{
    [SerializeField] private GameObject menuObject;
    [SerializeField] private float yOffset;
    [SerializeField] private GameObject buildEffect;
    [SerializeField] private GameObject upgradeEffect;
    [SerializeField] private GameObject removeEffect;

    private TowerPoint _selectedPoint;

    public TowerPoint SelectedPoint
    {
        get
        {
            return _selectedPoint;
        }
        set
        {
            if (value == null && _selectedPoint != null)
                _selectedPoint.Selected = false;
            else if (value != null)
                value.Selected = true;
            _selectedPoint = value;
            if (value != null)
                menuObject.transform.position = _selectedPoint.transform.position + new Vector3(0, yOffset * (_selectedPoint.transform.position.y > 0 ? -1 : 1));
            menuObject.SetActive(value != null);
        }
    }

	#region singleton
	public static TowerBuildController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        else Destroy(this);
    }
    #endregion

    public void PointPressed(TowerPoint point)
    {
        if (SelectedPoint != null)
        {
            if (SelectedPoint == point)
            {
                SelectedPoint = null;
                return;
            }
            else {
                SelectedPoint.Selected = false;
                SelectedPoint.SetHighlight(false);
            }
        }

        SelectedPoint = point;
    }

    public void BuildTower(GameObject towerPrefab)
    {
        int towerCost = towerPrefab.GetComponent<Tower>().cost;
        if (Controller.Instance.Money >= towerCost)
        {
            Controller.Instance.Money -= towerCost;
            SelectedPoint.Selected = false; // Do not move
            StartCoroutine(TowerBuilding(towerPrefab, SelectedPoint));
            SelectedPoint = null;
        }
    }

    public void RemoveTower()
    {
        Controller.Instance.Money += Tower.SelectedTower.removeCost;
        Instantiate(removeEffect, Tower.SelectedTower.transform.position, Quaternion.identity)
            .GetComponent<EffectController>().SetParticleMaterial(Tower.SelectedTower.particleMaterial);
        Destroy(Tower.SelectedTower.gameObject);
        Tower.SelectedTower.buildPoint.gameObject.SetActive(true);
        Tower.SelectedTower.PopUpMenu = false;
    }

    public void UpgradeTower()
    {
        var upgrade = Tower.SelectedTower.upgrade;
        var upgradeCost = upgrade.GetComponent<Tower>().cost;
        if (Controller.Instance.Money >= upgradeCost)
        {
            Controller.Instance.Money -= upgradeCost;
            StartCoroutine(TowerUpgrading(Tower.SelectedTower.gameObject, upgrade));
            Tower.SelectedTower.PopUpMenu = false;
            Tower.SelectedTower = null;
        }
    }

    private void Start()
    {
        menuObject.SetActive(false);
    }

    private IEnumerator TowerBuilding(GameObject newTower, TowerPoint point)
    {
        // Point stuff
        point.gameObject.SetActive(false);
        point.SetHighlight(false);
        // Tower stuff
        var towerScript = newTower.GetComponent<Tower>();
        var buildTime = towerScript.buildTime;
        var progressBarOffset = towerScript.buildBarOffset;
        // Effect stuff
        var effect = Instantiate(buildEffect, point.transform.position, Quaternion.identity).GetComponent<EffectController>();
        effect.GetComponent<TimedDestruction>().StartDestruction(effect.SetDuration(buildTime));
        effect.SetParticleMaterial(towerScript.particleMaterial);
        effect.PlayEffect();
        // ProgressBar stuff
        var progressBar = Controller.Instance.PlaceHealthBar();
        progressBar.transform.position = point.transform.position + progressBarOffset;
        progressBar.SetValue(0);
        // Start
        float startTime = Time.time;
        for (float progress = 0; progress < 1;)
        {
            progress = Mathf.Lerp(0, 1, (Time.time - startTime) / buildTime);
            progressBar.SetValue(progress);
            yield return null;
        }

        var tower = Instantiate(newTower, point.transform.position, Quaternion.identity);
        tower.GetComponent<Tower>().buildPoint = point;
        Destroy(progressBar.gameObject);
    }

    private IEnumerator TowerUpgrading(GameObject oldTower, GameObject newTower)
    {
        // Tower stuff
        var upgradeScript = newTower.GetComponent<Tower>();
        var buildTime = upgradeScript.buildTime;
        var progressBarOffset = upgradeScript.buildBarOffset;
        var oldTowerScript = oldTower.GetComponent<Tower>();
        var point = oldTowerScript.buildPoint;
        var removeCost = oldTowerScript.removeCost;
        oldTowerScript.PrepareForUpgrade();
        // Effect stuff
        var effect = Instantiate(upgradeEffect, point.transform.position, Quaternion.identity).GetComponent<EffectController>();
        effect.GetComponent<TimedDestruction>().StartDestruction(effect.SetDuration(buildTime));
        effect.SetParticleMaterial(oldTowerScript.particleMaterial);
        effect.PlayEffect();
        // ProgressBar stuff
        var progressBar = Controller.Instance.PlaceHealthBar();
        progressBar.transform.position = point.transform.position + progressBarOffset;
        progressBar.SetValue(0);
        // Start
        float startTime = Time.time;

        for (float progress = 0; progress < 1;)
        {
            progress = Mathf.Lerp(0, 1, (Time.time - startTime) / buildTime);
            progressBar.SetValue(progress);
            yield return null;
        }

        var tower = Instantiate(newTower, point.transform.position, Quaternion.identity);
        var newTowerScript = tower.GetComponent<Tower>();
        newTowerScript.buildPoint = point;
        newTowerScript.removeCost += removeCost;
        Destroy(oldTower);

        Destroy(progressBar.gameObject);
    }
} 
