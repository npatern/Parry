using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wind : MonoBehaviour
{
    AnimationCurve WindPowerOverDistance;
    public float Force = 10;
    public Vector3 Direction;
    public List<NavMeshAgent> agents = new List<NavMeshAgent>();
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();
    public ForceMode forcemode;
    private void FixedUpdate()
    {
        Direction = transform.forward;
        AffectAgents();
    }
    void AffectAgents()
    {
        for (int i = 0; i < agents.Count; i++)
            if (agents[i] == null) agents.RemoveAt(i);
        
        foreach (NavMeshAgent agent in agents)
        {
            agent.velocity += Force * Direction * Time.fixedDeltaTime ;
        }

        for (int i = 0; i < rigidbodies.Count; i++)
            if (rigidbodies[i] == null) rigidbodies.RemoveAt(i);

        foreach (Rigidbody rb in rigidbodies)
        { 
            rb.AddForce( Force * Direction, forcemode);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agents.Add(agent);
        }
        if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rigidbodies.Add(rb);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agents.Remove(agent);
        }
        if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rigidbodies.Remove(rb);
        }
    }
}