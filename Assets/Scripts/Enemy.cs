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
    /// <summary>
    /// Route is contained in positions of LineRenderer
    /// </summary>
    public LineRenderer Route { get; set; }
    public LinkedListNode<Enemy> ListNode { get; set; }
    /// <summary>
    /// Represents how close enemy to the route's end [0..1]
    /// </summary>
    public float RouteProgress { get; set; }

    /// <summary>
    /// Node index in route to which unit is moving now
    /// </summary>
    private int nextNodeIndex = 1;
    private HealthBar healthBar;
    private float health;

    private void Start()
    {
        health = maxHealth;
        var direction = Route.GetPosition(nextNodeIndex) - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        healthBar = Controller.Instance.PlaceHealthBar();
        healthBar.FollowTarget = transform;
        healthBar.HideOnFull = true;
        healthBar.AutoSelfDestruction = true;
        healthBar.PositionOffset = healthBarOffset;
        healthBar.SetHealth(1);
    }

    void Update()
    {
        if (nextNodeIndex == Route.positionCount) return;
        float deltaDistance = velocity * Time.deltaTime;
        while (deltaDistance > 0)
        {
            float distance = Vector2.Distance(transform.position, Route.GetPosition(nextNodeIndex));
            if (deltaDistance < distance)
            {
                transform.position = Vector2.Lerp(transform.position, Route.GetPosition(nextNodeIndex), deltaDistance / distance);
                break;
            } else
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
                } else
                {
                    var direction = Route.GetPosition(nextNodeIndex) - transform.position;
                    StartCoroutine(Rotate(Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), rotationTime));
                }
            }
        }
        if (Route.positionCount == nextNodeIndex) RouteProgress = 1; 
        else
            RouteProgress = 
            (
            nextNodeIndex - 1 // Progress among route segments, plus...
            + Vector2.Distance(transform.position, Route.GetPosition(nextNodeIndex - 1)) // ...Progress on current segment (less then 1 / segmentsCount)
            / 
            Vector2.Distance(Route.GetPosition(nextNodeIndex - 1), Route.GetPosition(nextNodeIndex))
            )
            / (Route.positionCount - 1); // Over segmentsCount
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
        healthBar.SetHealth(health / maxHealth);
        if (health <= 0)
        {
            Destroy(gameObject);
            Controller.Instance.Money += cost;
            Controller.Instance.kills++;
            if (ListNode != null)
                if (ListNode.List != null)
                    ListNode.List.Remove(ListNode);
        }
    }
}
