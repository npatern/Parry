using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "StepParticles", menuName = "ScriptableObjects/Needs/FulfillmentStepParticles", order = 1)]
public class FulfillmentStepParticleScriptableObject : FulfillmentStepScriptableObject
{
    public GameObject ParticlesToSpawn;
    public ParticleSystem SpawnedParticles;
    public bool LetThemDieOut = true;
    public override void StartStep(EntityController entity)
    {
        SpawnedParticles = Instantiate(ParticlesToSpawn, entity.transform).GetComponent<ParticleSystem>();
        SpawnedParticles.Play();
        //Debug.Log("Addedd custom particles");
        //Debug.Log("Starting " + WaitTime + " seconds wait.");

    }
    public override void EndStep(EntityController entity)
    {
        //Debug.Log("Step executed after " + WaitTime + " seconds.");
        //Debug.Log("Stoped emiting particles");
        SpawnedParticles.Stop();
        if (LetThemDieOut)
            Destroy(SpawnedParticles.gameObject, SpawnedParticles.main.startLifetime.constantMax);
        else
            Destroy(SpawnedParticles.gameObject);
        
        

    }
}