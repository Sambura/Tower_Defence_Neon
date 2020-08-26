using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private float scalableDuration;
    [SerializeField] private float unscalableDuration;

    public void SetDuration(float newDuration)
    {
        if (animator != null)
        {
            animator.speed = scalableDuration / newDuration;
        }

        if (particleSystem != null)
        {
            var mainEffector = particleSystem.main;
            mainEffector.duration = newDuration + unscalableDuration;
        }
    }

    public void PlayEffect()
    {
        if (animator != null) animator.SetTrigger("Main");
        if (particleSystem != null) particleSystem.Play();
    }
}
