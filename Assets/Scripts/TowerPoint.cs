using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isHighlighted = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (Controller.Instance.IsBuilding != -1)
        {
            spriteRenderer.color = Color.green;
            isHighlighted = true;
        }
    }

    private void OnMouseExit()
    {
        if (isHighlighted)
        {
            spriteRenderer.color = Color.white;
            isHighlighted = false;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (Controller.Instance.IsBuilding != -1)
        {
            Controller.Instance.BuildTower(gameObject);
        }
    }
}
