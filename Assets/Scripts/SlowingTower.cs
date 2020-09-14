using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingTower : Tower
{
    [SerializeField] private float slowingFactor;
    [SerializeField] private GameObject slowingEffectPrefab;

    private ParticleSystem gunParticles;
    private List<Enemy> slowedEnemies = new List<Enemy>();

    protected override void Start()
    {
        base.Start();
        gunParticles = gun.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Time.time >= nextShot)
        {
            bool flag = false;
            foreach (var i in Controller.Instance.SpawnedEnemies)
            {
                if (Vector2.Distance(transform.position, i.transform.position) <= radius)
                {
                    flag = true;
                    if (slowedEnemies.Contains(i)) continue;
                    var fx = Instantiate(slowingEffectPrefab, i.transform);
                    i.InflictEffect(new Effect(Effect.EffectType.VelocityChange, slowingFactor, -1, gameObject, fx));
                    slowedEnemies.Add(i);
                } else if (slowedEnemies.Contains(i))
                {
                    i.RemoveEffect(gameObject, Effect.EffectType.VelocityChange);
                    slowedEnemies.Remove(i);
                }
            }
            if (flag && !gunParticles.isPlaying) gunParticles.Play(); 
            if (!flag && gunParticles.isPlaying) gunParticles.Stop();
            nextShot = Time.time + 1 / fireRate;
        }
    }

    public override void PrepareForUpgrade()
    {
        gunParticles.Stop();
        Circle.SetActive(false);
        Destroy(this);
    }
}
