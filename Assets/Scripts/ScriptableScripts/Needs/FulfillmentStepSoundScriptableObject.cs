using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "StepSound", menuName = "ScriptableObjects/Needs/FulfillmentStepSound", order = 1)]
public class FulfillmentStepSoundScriptableObject : FulfillmentStepScriptableObject
{
    public AudioClip Sound;
    public AudioSource AudioSource;
    public bool RemoveSoundAfter = true;
    public override void StartStep(EntityController entity)
    {
        AudioSource = entity.gameObject.AddComponent<AudioSource>();
        AudioSource.clip = Sound;
        AudioSource.Play();
        AudioSource.loop = true;
    }
    public override void EndStep(EntityController entity)
    {
        if (RemoveSoundAfter)
        {
            Destroy(AudioSource);
        }
    }
}