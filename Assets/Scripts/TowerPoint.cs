using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool _highlihted = false;

    private bool Highlighted
    {
        get
        {
            return _highlihted;
        }
        set
        {
            if (value == _highlihted) return;
            if (value)
                spriteRenderer.color = Color.green;
            else
                spriteRenderer.color = Color.white;
            _highlihted = value;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (Controller.Instance.IsBuilding != -1)
        {
            Highlighted = true;
        }
    }

    private void OnMouseExit()
    {
        Highlighted = false;
    }

    private void OnMouseUpAsButton()
    {
        if (Controller.Instance.IsBuilding != -1)
        {
            Highlighted = false;
            Controller.Instance.BuildTower(gameObject);
            gameObject.SetActive(false);
        }
    }
}
