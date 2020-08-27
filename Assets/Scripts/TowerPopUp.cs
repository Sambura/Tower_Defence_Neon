using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPopUp : MonoBehaviour
{
    [SerializeField] private GameObject menuObject;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMPro.TextMeshProUGUI upgradeCostText;
    [SerializeField] private TMPro.TextMeshProUGUI removeCostText;
    [SerializeField] private float yOffset;
    [SerializeField] private string absentCostFiller = "";

    private bool isOpened = false;
    private int upgradeCostNow;

    private void Start()
    {
        Controller.Instance.OnMoneyChanged += OnMoneyChanged;
    }

    public void ShowPopUp(Vector2 position, int upgradeCost, int removeCost)
    {
        isOpened = true;
        menuObject.transform.position = position + new Vector2(0, yOffset * (position.y > 0 ? -1 : 1));

        if (upgradeCost == -1) // If there is no upgrade
        {
            upgradeCostText.text = absentCostFiller;
            upgradeButton.interactable = false;
        }
        else // If there is upgrade
        {
            upgradeCostText.text = upgradeCost.ToString(); // Display its cost
            upgradeCostNow = upgradeCost; // And save to the variable

            upgradeButton.interactable = Controller.Instance.Money >= upgradeCostNow; // If insufficient money, not interactable
        }

        if (removeCost == -1) // If cannot remove
        {
            removeCostText.text = absentCostFiller;
            removeButton.interactable = false;
        }
        else
        {
            removeCostText.text = removeCost.ToString();
            removeButton.interactable = true;
        }

        menuObject.SetActive(true);
    }

    private void OnMoneyChanged(int money)
    {
        if (isOpened)
            upgradeButton.interactable = money >= upgradeCostNow;
    }

    public void ClosePopUp()
    {
        isOpened = false;
        menuObject.SetActive(false);
    }
}
