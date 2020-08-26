using UnityEngine;

public class RandomTransform : MonoBehaviour
{
    [SerializeField] private bool randomizePosition;
    [SerializeField] private bool randomizeRotation;
    [SerializeField] private bool randomizeScale;

    [SerializeField] private float minTranslationDistance;
    [SerializeField] private float maxTranslationDistance;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;

    [SerializeField] private bool playOnAwake;

    public void RandomizeTransform()
    {
        if (randomizePosition)
        {
            Vector3 direction = Random.insideUnitCircle.normalized;
            transform.position = transform.position + direction * Random.Range(minTranslationDistance, maxTranslationDistance);
        }

        if (randomizeRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(minAngle, maxAngle));
        }

        if (randomizeScale)
        {
            float scale = Random.Range(minScale, maxScale);
            transform.localScale = new Vector2(scale, scale);
        }
    }

    private void Awake()
    {
        if (playOnAwake)
            RandomizeTransform();
    }
}
