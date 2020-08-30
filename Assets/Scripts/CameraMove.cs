using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float translationSpeed = 5;
    [SerializeField] float minSize = 2;
    [SerializeField] float scalingSpeed = 0.9f;
    [SerializeField] float scalingStep = 0.5f;
    public float maxSize;

    private float targetSize;
    private Camera cam;
    private float scalingStartTime;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetSize = maxSize;
        cam.orthographicSize = maxSize;
    }

    void Update()
    {
        // Get input and set necessary values
        if (Input.mouseScrollDelta.y != 0)
        {
            float oldTarget = targetSize;
            if (Input.mouseScrollDelta.y > 0)
            {
                targetSize = Mathf.Clamp(targetSize - scalingStep, minSize, maxSize);
            } else
            {
                targetSize = Mathf.Clamp(targetSize + scalingStep, minSize, maxSize);
            }
            if (oldTarget != targetSize)
                scalingStartTime = Time.time;
        }

        // Do scling
        if (cam.orthographicSize != targetSize)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, (Time.time - scalingStartTime) * scalingSpeed);
        }

        // Do translation
        float xDelta = Input.GetAxisRaw("Horizontal");
        float yDelta = Input.GetAxisRaw("Vertical");
        if (xDelta != 0 || yDelta != 0)
        {
            transform.position += new Vector3(xDelta, yDelta) * Time.deltaTime * translationSpeed;
        }

        // Do correction
        if (Mathf.Abs(transform.position.y) + cam.orthographicSize > maxSize)
        {
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Sign(transform.position.y) * (maxSize - cam.orthographicSize),
                transform.position.z
                );
        }
    }
}
