using UnityEngine;

public class DecayAndDie : MonoBehaviour
{
    [SerializeField] private SpriteRenderer decayImage;
    [SerializeField] private float decayTime;
    [SerializeField] private Gradient decayGradient;
    [SerializeField] private bool randomRotation;

    private float startTime;

    private void Awake()
    {
        if (randomRotation)
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        startTime = Time.time;
    }

    private void Update()
    {
        float progress = (Time.time - startTime) / decayTime;
        decayImage.color = decayGradient.Evaluate(progress);
        if (progress >= 1) Destroy(gameObject);
    }
}
