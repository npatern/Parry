using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCasterCollider : MonoBehaviour
{
    public DamageEffects effects;
    List<StatusController> statuses = new List<StatusController>();
    public float TickTime = 1f;
    float TickTimer = 0;
    public bool ApplyOnlyOnce = false;

    public bool applyExplosionForce = false;
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();
    public float explosionForce = 10;
    private void Start()
    {
        //if (ApplyOnlyOnce) Tick();
        
    }
    private void FixedUpdate()
    {
        if (ApplyOnlyOnce) return;
        TickTimer -= Time.fixedDeltaTime;
        if (TickTimer <= 0)
        {
            TickTimer += TickTime;
            Tick();
        }
    }
    private void Tick()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i] == null) continue;
            statuses[i].TakeDamageWithEffects(effects);
            //effects.ApplyEffects(statuses[i].stats);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<StatusController>(out StatusController status))
        {
            statuses.Add(status);
            status.TakeDamageWithEffects(effects);
            //if (status.stats != null)
            //effects.ApplyEffects(status.stats);
        }
        if (applyExplosionForce)
        {
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rigidbodies.Add(rb);
                if (!rb.isKinematic)
                    rb.AddExplosionForce(explosionForce, transform.position, 10);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<StatusController>(out StatusController status))
            statuses.Remove(status);
        if (applyExplosionForce)
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rigidbodies.Remove(rb);
    }
}
