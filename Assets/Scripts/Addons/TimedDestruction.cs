using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    [SerializeField] float destructionDelay;
    [SerializeField] bool playOnAwake;

    public void StartDestruction()
    {
        Destroy(gameObject, destructionDelay);
    }

    public void StartDestruction(float delay)
    {
        Destroy(gameObject, delay);
    }

    private void Awake()
    {
        if (playOnAwake)
            StartDestruction();
    }
}
