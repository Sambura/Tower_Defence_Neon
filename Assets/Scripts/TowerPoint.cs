using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoint : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color selectedColor;

    private bool _selected = false;
    private SpriteRenderer spriteRenderer;

    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if (value) SetHighlight(true);
        }
    }
    
    public void SetHighlight(bool value)
    {
        if (Selected)
        {
            spriteRenderer.color = selectedColor;
        }
        else
        {
            if (value)
                spriteRenderer.color = hoverColor;
            else
                spriteRenderer.color = normalColor;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetHighlight(false); // Do not remove
    }

    private void OnMouseOver()
    {
        SetHighlight(true);
    }

    private void OnMouseExit()
    {
        SetHighlight(false);
    }

    private void OnMouseUpAsButton()
    {
        TowerManager.Instance.PointPressed(this);
    }
}
