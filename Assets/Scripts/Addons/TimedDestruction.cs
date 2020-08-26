using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    [SerializeField] float minDestructionDelay;
    [SerializeField] float maxDestructionDelay;
    [SerializeField] bool playOnAwake;

    public void StartDestruction()
    {
        Destroy(gameObject, Random.Range(minDestructionDelay, maxDestructionDelay));
    }

    private void Awake()
    {
        if (playOnAwake)
            StartDestruction();
    }
}
