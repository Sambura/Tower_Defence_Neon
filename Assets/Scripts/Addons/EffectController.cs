using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private float scalableDuration;
    [SerializeField] private float unscalableDuration;

    public void SetParticleMaterial(Material newMat)
    {
        if (particleSystem != null)
        {
            particleSystem.GetComponent<ParticleSystemRenderer>().material = newMat;
            particleSystem.GetComponent<ParticleSystemRenderer>().trailMaterial = newMat;
        }
    } 

    public float SetDuration(float newDuration)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", scalableDuration / newDuration);
        }

        if (particleSystem != null)
        {
            var mainEffector = particleSystem.main;
            mainEffector.duration = newDuration;
        }

        return newDuration + unscalableDuration;
    }

    public void PlayEffect()
    {
        if (animator != null) animator.SetTrigger("Play");
        if (particleSystem != null) particleSystem.Play();
    }
}
