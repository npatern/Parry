using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCasterCollider : MonoBehaviour
{
    public DamageEffects effects;
    List<StatusController> statuses = new List<StatusController>();
    public float TickTime = 0.5f;
    float TickTimer = 0;
    private void FixedUpdate()
    {
        TickTimer += Time.fixedDeltaTime;
        if (TickTimer >= TickTime)
        {
            TickTimer -= TickTime;
            Tick();
        }
    }
    private void Tick()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i] == null) continue;
            effects.ApplyEffects(statuses[i].stats);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<StatusController>(out StatusController status))
        {
            statuses.Add(status);
            if (status.stats != null)
                effects.ApplyEffects(status.stats);
        }
            
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<StatusController>(out StatusController status) != null)
            statuses.Remove(status);
    }
}
