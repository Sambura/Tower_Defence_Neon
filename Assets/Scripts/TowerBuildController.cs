using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuildController : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject buildEffect;

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
            buttons.SetActive(value != null);
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
            StartCoroutine(TowerBuilding(towerPrefab, SelectedPoint));
            SelectedPoint.Selected = false;
            SelectedPoint = null;
        }
    }

    public void RemoveTower()
    {
        Controller.Instance.Money += Tower.SelectedTower.removeCost;
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
        }
    }

    private void Start()
    {
        buttons.SetActive(false);
    }

    private IEnumerator TowerBuilding(GameObject newTower, TowerPoint point)
    {
        // Tower stuff
        var towerScript = newTower.GetComponent<Tower>();
        var buildTime = towerScript.buildTime;
        var progressBarOffset = towerScript.buildBarOffset;
        // Effect stuff
        var effect = Instantiate(buildEffect, point.transform.position, Quaternion.identity);
        var particleSystem = effect.GetComponent<ParticleSystem>();
        var mainEffector = particleSystem.main;
        mainEffector.duration = buildTime;
        particleSystem.Play();
        // ProgressBar stuff
        var progressBar = Controller.Instance.PlaceHealthBar();
        progressBar.transform.position = point.transform.position + progressBarOffset;
        progressBar.SetValue(0);
        // Point stuff
        point.enabled = false;
        var pointSr = point.GetComponent<SpriteRenderer>();
        // Start
        float startTime = Time.time;
        for (float progress = 0; progress < 1; )
        {
            progress = Mathf.Lerp(0, 1, (Time.time - startTime) / buildTime);
            progressBar.SetValue(progress);
            pointSr.color = point.buildColorChange.Evaluate(progress);
            yield return null;
        }

        var tower = Instantiate(newTower, point.transform.position, Quaternion.identity);
        tower.GetComponent<Tower>().buildPoint = point;
        Destroy(progressBar.gameObject);
        Destroy(effect, particleSystem.main.startLifetime.constantMax);
        startTime = Time.time;
        for (float progress = 1; progress > 0; progress = Mathf.Lerp(1, 0, (Time.time - startTime)))
        {
            pointSr.color = new Color(1, 1, 1, Mathf.Exp(progress * 8 - 8));
            yield return null;
        }
        point.enabled = true;
        point.gameObject.SetActive(false);
        point.SetHighlight(false);
    }

    private IEnumerator TowerUpgrading(GameObject oldTower, GameObject newTower)
    {
        // Tower stuff
        var oldTowerScript = oldTower.GetComponent<Tower>();
        var upgradeScript = newTower.GetComponent<Tower>();
        var buildTime = upgradeScript.buildTime;
        var progressBarOffset = upgradeScript.buildBarOffset;
        oldTowerScript.enabled = false;
        // Point stuff
        var point = oldTowerScript.buildPoint;
        // Effect stuff
        var effect = Instantiate(buildEffect, point.transform.position, Quaternion.identity);
        var particleSystem = effect.GetComponent<ParticleSystem>();
        var mainEffector = particleSystem.main;
        mainEffector.duration = buildTime;
        particleSystem.Play();
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
        newTowerScript.removeCost += oldTowerScript.removeCost;
        Destroy(oldTower);

        Destroy(progressBar.gameObject);
        Destroy(effect, particleSystem.main.startLifetime.constantMax);
    }
} 
