using UnityEngine;

public class Decay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer decayImage;
    [SerializeField] private float decayTime;
    [SerializeField] private Gradient decayGradient;

    private float startTime;

    private void Awake()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        float progress = (Time.time - startTime) / decayTime;
        decayImage.color = decayGradient.Evaluate(progress);
    }
}
