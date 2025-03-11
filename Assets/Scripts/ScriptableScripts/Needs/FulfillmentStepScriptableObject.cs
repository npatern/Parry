using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Step", menuName = "ScriptableObjects/Needs/FulfillmentStep", order = 1)]
public class FulfillmentStepScriptableObject : ScriptableObject
{
    public float WaitTime = 0;
    public bool DestroyEntityAfter = false;
    public IEnumerator CountDown(EntityController entity)
    {
        StartStep(entity);
        yield return new WaitForSeconds(WaitTime);
        EndStep(entity);
        if (DestroyEntityAfter)
        {
            Debug.Log("Marking for destruction " + entity.gameObject.name);
            entity.MarkedForDestruction = true;
        }
    }
    public virtual void StartStep(EntityController entity)
    {
        //Debug.Log("Starting " + WaitTime + " seconds wait.");
    }
    public virtual void EndStep(EntityController entity)
    {
        //Debug.Log("Step executed after " + WaitTime + " seconds.");
    }
}