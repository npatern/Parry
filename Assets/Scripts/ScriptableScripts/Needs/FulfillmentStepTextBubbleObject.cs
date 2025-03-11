using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "StepTextBubble", menuName = "ScriptableObjects/FulfillmentStepTextBubble", order = 1)]
public class FulfillmentStepTextBubbleObject : FulfillmentStepScriptableObject
{
    public string TextToSpawn = "Hello world!";
    public override void StartStep(EntityController entity)
    {
        UIController.Instance.SpawnTextBubble(TextToSpawn, entity.transform);
    }
    public override void EndStep(EntityController entity)
    { 
    }
}