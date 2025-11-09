using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    List<ParticleSystem> particleSystems;
    public float timer = 10;
    public float whenToDetatchParticles = 0;
    public GameObject spawnOnDestroy;
    private void Awake()
    {
        particleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }
    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer <= whenToDetatchParticles)
            DetachParticles();
        if (timer <= 0)
        {
            if (spawnOnDestroy != null) Instantiate(spawnOnDestroy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
            
    }
    void DetachParticles()
    {
        foreach (ParticleSystem particle in particleSystems)
        {
            particle.transform.parent = null;
            particle.Stop();
            Destroy(particle.gameObject,5);
        }
        
    }
}
