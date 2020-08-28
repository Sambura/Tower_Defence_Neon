using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerBuildButton : MonoBehaviour
{
    public GameObject towerPrefab;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private float activeAlpha;
    [SerializeField] private float inactiveAlpha;

    private int towerCost;

    private void Start()
    {
        towerCost = towerPrefab.GetComponent<Tower>().cost;
        costText.text = towerCost.ToString();
        Controller.Instance.OnMoneyChanged += OnMoneyChanged;
        OnMoneyChanged(Controller.Instance.Money);
    }

    private void OnEnable()
    {
        OnMoneyChanged(Controller.Instance.Money);
    }

    private void OnMoneyChanged(int money)
    {
        if (money >= towerCost)
        {
            button.interactable = true;
            canvasGroup.alpha = activeAlpha;
        } else
        {
            button.interactable = false;
            canvasGroup.alpha = inactiveAlpha;
        }
    }
}
