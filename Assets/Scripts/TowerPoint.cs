using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoint : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;

    private bool highlighted = true;
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
            if (highlighted) return;
            else value = true;
        if (value)
            spriteRenderer.color = highlightedColor;
        else
            spriteRenderer.color = normalColor;
        highlighted = value;
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
        TowerBuildController.Instance.PointPressed(this);
    }
}
