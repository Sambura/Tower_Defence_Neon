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

    private void Start()
    {
        buttons.SetActive(false);        
    }

    public void PointPressed(TowerPoint point)
    {
        if (SelectedPoint != null)
        {
            if (SelectedPoint == point)
            {
                SelectedPoint.Selected = false;
                SelectedPoint = null;
                return;
            }
            else {
                SelectedPoint.Selected = false;
                SelectedPoint.SetHighlight(false);
            }
        }

        SelectedPoint = point;
        point.Selected = true;
    }

    public void BuildTower(GameObject towerPrefab)
    {
        int towerCost = towerPrefab.GetComponent<LaserTower>().cost;
        if (Controller.Instance.Money >= towerCost)
        {
            Controller.Instance.Money -= towerCost;
            StartCoroutine(TowerBuilding(towerPrefab, SelectedPoint));
            SelectedPoint = null;
        }
    }

    private IEnumerator TowerBuilding(GameObject newTower, TowerPoint point)
    {
        // Tower stuff
        var towerScript = newTower.GetComponent<LaserTower>();
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
        tower.GetComponent<LaserTower>().buildPoint = point;
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
    }
} 
