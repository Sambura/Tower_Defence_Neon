using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Velocity { get; set; }
    public float Damage { get; set; }
    public Enemy Target { get; set; }

    private Vector2 destinationPoint;

    void Update()
    {
        if (Target != null)
            destinationPoint = Target.transform.position;
        float distance = Vector2.Distance(transform.position, destinationPoint);
        float deltaDistance = Time.deltaTime * Velocity;
        float time = deltaDistance / distance;
        if (time >= 1)
        {
            Destroy(gameObject);
            if (Target != null)
                Target.TakeDamage(Damage);
        }
        else
            transform.position = Vector2.Lerp(transform.position, destinationPoint, time);
    }
}
