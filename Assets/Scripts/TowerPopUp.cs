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

    public void ShowPopUp(bool removeB, bool infoB, bool upgradeB, Vector2 position)
    {
        transform.position = position;
        if (transform.position.y < 0)
        {
            removeButton[0].interactable = removeB;
            infoButton[0].interactable = infoB;
            upgradeButton[0].interactable = upgradeB;
            UpMenu.SetActive(true);
        } else
        {
            removeButton[1].interactable = removeB;
            infoButton[1].interactable = infoB;
            upgradeButton[1].interactable = upgradeB;
            DownMenu.SetActive(true);
        }
    }

    public void ClosePopUp()
    {
        UpMenu.SetActive(false);
        DownMenu.SetActive(false);
    }
}
