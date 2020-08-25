using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPopUp : MonoBehaviour
{
    [SerializeField] private Button[] removeButton;
    [SerializeField] private Button[] infoButton;
    [SerializeField] private Button[] upgradeButton;
    [SerializeField] private GameObject UpMenu;
    [SerializeField] private GameObject DownMenu;
    [SerializeField] private TMPro.TextMeshProUGUI[] upgradeCostTexts;
    [SerializeField] private TMPro.TextMeshProUGUI[] removeCostTexts;
    [SerializeField] private string costFiller = "—";

    public void ShowPopUp(Vector2 position, int upgradeCost, int removeCost)
    {
        transform.position = position;
        if (transform.position.y < 0)
        {
            if (upgradeCost == -1)
            {
                upgradeCostTexts[0].text = costFiller;
                upgradeButton[0].interactable = false;
            } 
            else
            {
                upgradeCostTexts[0].text = upgradeCost.ToString();
                upgradeButton[0].interactable = true;
            }

            if (removeCost == -1)
            {
                removeCostTexts[0].text = costFiller;
                removeButton[0].interactable = false;
            }
            else
            {
                removeCostTexts[0].text = removeCost.ToString();
                removeButton[0].interactable = true;
            }

            UpMenu.SetActive(true);
        } else
        {
            if (upgradeCost == -1)
            {
                upgradeCostTexts[1].text = costFiller;
                upgradeButton[1].interactable = false;
            }
            else
            {
                upgradeCostTexts[1].text = upgradeCost.ToString();
                upgradeButton[1].interactable = true;
            }

            if (removeCost == -1)
            {
                removeCostTexts[1].text = costFiller;
                removeButton[1].interactable = false;
            }
            else
            {
                removeCostTexts[1].text = removeCost.ToString();
                removeButton[1].interactable = true;
            }

            DownMenu.SetActive(true);
        }
    }

    public void ClosePopUp()
    {
        UpMenu.SetActive(false);
        DownMenu.SetActive(false);
    }
}
