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
    [SerializeField] private TMPro.TextMeshProUGUI[] costTexts;
    [SerializeField] private string costFiller = "—";

    public void ShowPopUp(Vector2 position, int upgradeCost)
    {
        transform.position = position;
        if (transform.position.y < 0)
        {
            if (upgradeCost == -1)
            {
                costTexts[0].text = costFiller;
                upgradeButton[0].interactable = false;
            } else
            {
                costTexts[0].text = upgradeCost.ToString();
                upgradeButton[0].interactable = true;
            }

            UpMenu.SetActive(true);
        } else
        {
            if (upgradeCost == -1)
            {
                costTexts[1].text = costFiller;
                upgradeButton[1].interactable = false;
            }
            else
            {
                costTexts[1].text = upgradeCost.ToString();
                upgradeButton[1].interactable = true;
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
