using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFadeOut : MonoBehaviour
{
    public ParticleSystem emit;

    public void detachParticles()
    {
        // This splits the particle off so it doesn't get deleted with the parent
        emit.transform.parent = null;

        // this stops the particle from creating more bits and deletes the particle system automatically after all remaining particles vanished
        emit.Stop();

    }
}
