using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    
    public Transform StartPoint;
    public Transform EndPoint;
    public TrailRenderer[] trails;
    public ParticleSystem[] particles;

    public GameObject heavyAttackEffect;
    private ParticleSystem heavyAttackParticles;
    private void Awake()
    {
        trails = GetComponentsInChildren<TrailRenderer>();
        particles = GetComponentsInChildren<ParticleSystem>();
    }
    private void Start()
    {
        SetWeapon(false);
    }
    public void SetWeapon(bool state, bool heavy = false)
    {
        SetTrail(state);
        SetParticles(state);
        SetHeavyAttack(state, heavy);
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
    
    private void SetHeavyAttack(bool state, bool heavy)
    {
        if (heavyAttackEffect == null) heavyAttackEffect = Instantiate( GameplayController.Instance.ListOfAssets.HeavyAttackEffect,StartPoint);
        if (heavyAttackParticles == null) heavyAttackParticles = heavyAttackEffect.GetComponent<ParticleSystem>();
        if (!heavy) heavyAttackParticles.Stop();
        else heavyAttackParticles.Play();
    }
}
