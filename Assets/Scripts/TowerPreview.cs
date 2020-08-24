using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    public GameObject Tower { get; set; }

    private GameObject circle;

    public void Init()
    {
        Tower.GetComponent<Tower>().enabled = false;
        circle = Instantiate(Tower.GetComponent<Tower>().circlePrefab);
        circle.GetComponent<CircleDrawer>().Radius = Tower.GetComponent<Tower>().radius;
        Tower.transform.parent = transform;
        circle.transform.parent = transform;
    }

    void Update()
    {
        transform.position = Controller.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y);
    }
}
