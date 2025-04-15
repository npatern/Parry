using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    
    public Transform StartPoint;
    public Transform EndPoint;
    //public Collider collider;
    public TrailRenderer[] trails;
    public ParticleSystem[] particles;
    private void Awake()
    {
        trails = GetComponentsInChildren<TrailRenderer>();
        particles = GetComponentsInChildren<ParticleSystem>();
    }
    private void Start()
    {
        SetWeapon(false);


    }
    public void SetWeapon(bool state)
    {
        SetTrail(state);
        SetParticles(state);
    }
    public void SetTrail(bool state)
    {
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = state;
        }
    }
    public void SetParticles(bool state)
    {
        foreach (ParticleSystem particle in particles)
        {
            if (state)
                particle.Play();
            else
                particle.Stop();
        }
    }
}
