using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    public GameObject Tower { get; set; }
    public GameObject Circle { get; set; }

    public void Init()
    {
        Tower.GetComponent<Tower>().enabled = false;
        Tower.GetComponent<Collider2D>().enabled = false;
        Circle.GetComponent<CircleDrawer>().Radius = Tower.GetComponent<Tower>().radius;
        Tower.transform.parent = transform;
        Circle.transform.parent = transform;
    }

    void Update()
    {
        transform.position = Controller.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y);
    }
}
