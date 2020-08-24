using System.Collections;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    public int vertexCountFactor = 20;

    [SerializeField] private float _radius;

    public float Radius
    {
        get
        {
            return _radius;
        }
        set
        {
            _radius = value;
            RedrawCircle();
        }
    }

    private void Start()
    {
        RedrawCircle();
    }

    private void RedrawCircle()
    {
        int vertexCount = Mathf.RoundToInt(Radius * vertexCountFactor);
        lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < vertexCount; i++)
        {
            float angle = -i * 2 * Mathf.PI / vertexCount;
            lineRenderer.SetPosition(i, new Vector3(Radius * Mathf.Cos(angle), Radius * Mathf.Sin(angle)));
        }
    }
}
