using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPopUp : MonoBehaviour
{
    public Button removeButton;
    public Button infoButton;
    public Button upgradeButton;

    public void ResetButtons()
    {
        removeButton.interactable = true;
        infoButton.interactable = true;
        upgradeButton.interactable = true;
    }
}
