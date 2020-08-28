﻿using System.Collections;
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
    }

    void Update()
    {
        if (nextNodeIndex == Route.positionCount) return;
        // Inflicting vlocity effects
        float velocityFactor = 1;
        for (var i = 0; i < velocityChange.Count; i++)
        {
            velocityFactor *= velocityChange[i].factor;
            if (velocityChange[i].endTime <= Time.time)
            {
                velocityChange.RemoveAt(i);
                i--;
            }
        }
        // Calculating new position
        float deltaDistance = velocity * Time.deltaTime * velocityFactor;
        DistanceToFinish -= deltaDistance;
        if (DistanceToFinish < 0) DistanceToFinish = 0; // In theory this value can run significantly less then zero due to route offset
        while (deltaDistance > 0)
        {
            Vector2 destination = (Vector2)Route.GetPosition(nextNodeIndex) + (nextNodeIndex == Route.positionCount - 1 ? Vector2.zero : routeOffset);
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
}

public class Effect
{
    public enum EffectType { VelocityChange };

    public EffectType effectType;
    public float factor;
    public float endTime;

    public Effect(EffectType effectType, float factor, float endTime)
    {
        this.effectType = effectType;
        this.factor = factor;
        this.endTime = endTime;
    }
}
