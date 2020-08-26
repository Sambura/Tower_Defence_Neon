using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] new private ParticleSystem particleSystem;

    public void StartParticleSystem()
    {
        if (particleSystem == null)
        {
            Debug.LogError("Particle system hasn't beem assigned or has been destroyed");
            return;
        }
        particleSystem.Play();
    }
}
