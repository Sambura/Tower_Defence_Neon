using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingTower : Tower
{
    [SerializeField] private float slowingFactor;
    [SerializeField] private float effectTime;

    void Update()
    {
        foreach (var i in Controller.Instance.SpawnedEnemies)
        {
            if (Vector2.Distance(transform.position, i.transform.position) <= radius)
            {
                i.InflictEffect(new Effect(Effect.EffectType.VelocityChange, slowingFactor, effectTime + Time.time));
            }
        }
    }

    public override void PrepareForUpgrade()
    {
        gun.GetComponent<ParticleSystem>().Stop();
        Circle.SetActive(false);
        Destroy(this);
    }
}
