using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaDamageProjectile : MonoBehaviour
{
    public float Velocity { get; set; }
    public float Damage { get; set; }
    public float DamageDecay { get; set; }
    public float DamageRadius { get; set; }
    public Vector3 Destination { get; set; }
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private bool enableTorgue;
    [SerializeField] private float torgueMultiplier;
    [SerializeField] private GameObject trail;

    private Vector3 movementDirection;

    private void Start()
    {
        movementDirection = (Destination - transform.position).normalized;
    }

    void Update()
    {
        float estimatedDistance = Velocity * Time.deltaTime;
        float remainingDistance = Vector2.Distance(transform.position, Destination);
        if (estimatedDistance >= remainingDistance)
        {
            InflictDamage();
            Instantiate(explosionEffect, Destination, Quaternion.identity);
            trail.transform.parent = null;
            Destroy(gameObject);
            return;
        }
        transform.Translate(movementDirection * estimatedDistance, Space.World);
        if (enableTorgue)
            transform.Rotate(0, 0, estimatedDistance * torgueMultiplier);
    }

    void InflictDamage()
    {
        var array = Controller.Instance.SpawnedEnemies.ToArray();
        foreach (var i in array)
        {
            float distance = Vector2.Distance(Destination, i.transform.position);
            if (distance <= DamageRadius)
            {
                i.TakeDamage(Damage * Mathf.Lerp(1, DamageDecay, distance / DamageRadius));
            }
        }
    }
}
