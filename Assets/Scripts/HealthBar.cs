using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject graphics;

    private bool _hideOnFull;

    /// <summary>
    /// Heatlhbar will follow this target if not null
    /// </summary>
    public Transform FollowTarget { get; set; }
    /// <summary>
    /// Offset from the target object
    /// </summary>
    public Vector3 PositionOffset { get; set; }
    /// <summary>
    /// Whether healthbar should destroy itself when target became null or not
    /// </summary>
    public bool AutoSelfDestruction { get; set; }
    /// <summary>
    /// Whether healthbar should be hidden on full health or not
    /// </summary>
    public bool HideOnFull
    {
        get
        {
            return _hideOnFull;
        }
        set
        {
            _hideOnFull = value;
            if (value && slider.value == 1)
            {
                graphics.SetActive(false);
            }
            else graphics.SetActive(true);
        }
    }

    void Update()
    {
        if (FollowTarget != null)
            transform.position = FollowTarget.position + PositionOffset;
        else if (AutoSelfDestruction)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets health to the given value (between 0 and 1)
    /// </summary>
   public void SetValue(float newHealth) 
    {
        slider.SetValueWithoutNotify(newHealth);
        if (HideOnFull)
        {
            graphics.SetActive(newHealth != 1);
        }
    }
}
