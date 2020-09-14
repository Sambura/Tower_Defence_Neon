using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float velocity = 1;
    public float maxHealth = 150;
    public float rotationTime = 0.7f;
    public Vector3 healthBarOffset;
    public int cost;
    public GameObject destructionEffect;
    /// <summary>
    /// Route is contained in positions of LineRenderer
    /// </summary>
    public LineRenderer Route { get; set; }
    public LinkedListNode<Enemy> ListNode { get; set; }
    /// <summary>
    /// Represents how close enemy to the route's end [0..positiveInfinity]
    /// </summary>
    public float DistanceToFinish { get; set; }
    public Vector2 LastPosition { get; set; }

    /// <summary>
    /// Node index in route to which unit is moving now
    /// </summary>
    private int nextNodeIndex = 1;
    private HealthBar healthBar;
    private float health;
    private List<Effect> velocityChange = new List<Effect>();
    private Vector2 routeOffset;

    public void InflictEffect(Effect effect)
    {
        switch (effect.effectType)
        {
            case Effect.EffectType.VelocityChange:
                velocityChange.Add(effect);
                break;
        }
    }

    public void RemoveEffect(GameObject source, Effect.EffectType effectType)
    {
        switch (effectType)
        {
            case Effect.EffectType.VelocityChange:
                for (int i = 0; i < velocityChange.Count; i++)
                {
                    if (velocityChange[i].source == source)
                    {
                        velocityChange.RemoveAt(i);
                        i--;
                    }
                }
                break;
        }
    }

    private void Start()
    {
        health = maxHealth;
        healthBar = Controller.Instance.PlaceHealthBar();
        healthBar.FollowTarget = transform;
        healthBar.HideOnFull = true;
        healthBar.AutoSelfDestruction = true;
        healthBar.PositionOffset = healthBarOffset;
        healthBar.SetValue(1);
        routeOffset = Random.insideUnitCircle * Route.GetPosition(0).z / 2;
        transform.position = transform.position;
        DistanceToFinish += Vector2.Distance(transform.position, (Vector2)Route.GetPosition(nextNodeIndex) + routeOffset) 
            - Vector2.Distance(transform.position, Route.GetPosition(nextNodeIndex));
        var direction = Route.GetPosition(nextNodeIndex) + (Vector3)routeOffset - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        LastPosition = transform.position;
    }

    void Update()
    {
        if (nextNodeIndex == Route.positionCount) return;
        LastPosition = transform.position;
        // Inflicting vlocity effects
        float maxVelocityFactor = 1;
        float minVelocityFactor = 1;
        for (var i = 0; i < velocityChange.Count; i++)
        {
            maxVelocityFactor = Mathf.Max(maxVelocityFactor, velocityChange[i].value);
            minVelocityFactor = Mathf.Min(minVelocityFactor, velocityChange[i].value);
            if (velocityChange[i].CheckExpiration())
            {
                velocityChange[i].Destroy();
                velocityChange.RemoveAt(i);
                i--;
            }
        }
        // Calculating new position
        float deltaDistance = velocity * Time.deltaTime * maxVelocityFactor * minVelocityFactor;
        DistanceToFinish -= deltaDistance;
        if (DistanceToFinish < 0) DistanceToFinish = 0; // In theory this value can run significantly less then zero due to route offset
        while (deltaDistance > 0)
        {
            Vector2 destination = (Vector2)Route.GetPosition(nextNodeIndex) + routeOffset;
             //   + (nextNodeIndex == Route.positionCount - 1 ? Vector2.zero : routeOffset);
            float distance = Vector2.Distance(transform.position, destination);
            if (deltaDistance < distance)
            {
                transform.position = Vector2.Lerp(transform.position, destination, deltaDistance / distance);
                break;
            }
            else
            {
                deltaDistance -= distance;
                nextNodeIndex++;
                if (nextNodeIndex == Route.positionCount)
                {
                    if (ListNode != null)
                        if (ListNode.List != null)
                            ListNode.List.Remove(ListNode);
                    Destroy(gameObject);
                    Controller.Instance.EnemyBreakthrough();
                    break;
                }
                else
                {
                    var direction = Route.GetPosition(nextNodeIndex) + (Vector3)routeOffset - transform.position;
                    StartCoroutine(Rotate(Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), rotationTime));
                }
            }
        }
    }

    private IEnumerator Rotate(Quaternion targetRotation, float time)
    {
        var initRotation = transform.rotation;
        float startTime = Time.time;
        while (Time.time - startTime < time)
        {
            transform.rotation = Quaternion.Lerp(initRotation, targetRotation, (Time.time - startTime) / time);
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0) return;
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        healthBar.SetValue(health / maxHealth);
        if (health <= 0)
        {
            Destroy(gameObject);
            var effect = Instantiate(destructionEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystemRenderer>();
            effect.material = GetComponent<SpriteRenderer>().sharedMaterial;
            Controller.Instance.Money += cost;
            if (ListNode != null)
                if (ListNode.List != null)
                    ListNode.List.Remove(ListNode);
        }
    }

#if UNITY_EDITOR

    private void FixedUpdate()
    {
        if (Controller.Instance.drawDebugLines)
        {
            var holder = new GameObject("Enemy", typeof(LineRenderer));
            var line = holder.GetComponent<LineRenderer>();
            line.sortingOrder = 5;
            line.positionCount = 2;
            line.widthMultiplier = 0.001f;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, LastPosition);
            Destroy(holder, Time.fixedDeltaTime);
        }
    }

#endif
}

public class Effect
{
    public enum EffectType { VelocityChange };

    public EffectType effectType;
    /// <summary>
    /// For velocity change it is number by which unit's velocity being multiplied, e.g. value = 2 means unit will move two times faster
    /// </summary>
    public float value;
    /// <summary>
    /// Point in time when effect is expired. -1 if it should not be controlled by unit itself (effect should be removed by some other object)
    /// </summary>
    public float endTime;
    /// <summary>
    /// GameObject that inflicted this effect
    /// </summary>
    public GameObject source;
    /// <summary>
    /// Gameobjects with visual of this effect. Usually it is particles
    /// </summary>
    public GameObject fx;

    public bool CheckExpiration()
    {
        if (endTime == -1) return false;
        return Time.time >= endTime;
    }

    public void Destroy()
    {
        if (fx != null)
        {
            var particleSystem = fx.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Stop();
                Object.Destroy(fx, particleSystem.main.startLifetime.constantMax);
            }
            else Object.Destroy(fx);
        }
    }

    public Effect(EffectType effectType, float value, float endTime, GameObject source, GameObject fx = null)
    {
        this.effectType = effectType;
        this.value = value;
        this.endTime = endTime;
        this.source = source;
        this.fx = fx;
    }
}
