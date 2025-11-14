using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "StepPostProcess", menuName = "ScriptableObjects/Needs/FulfillmentStepPostProcess", order = 1)]
public class FulfillmentStepPostProcessScriptableObject : FulfillmentStepScriptableObject
{
    public PostProcessProfile Profile;
    public PostProcessVolume Volume;
    public bool RemovePostProcessAfter = true;
    public override void StartStep(EntityController entity)
    {
        Volume = GameplayController.Instance.PostProcess.AddComponent<PostProcessVolume>();
        Volume.sharedProfile = Profile;
        Volume.isGlobal = true;
        //Debug.Log("Addedd custom profile" + Profile.name);
        //Debug.Log("Starting " + WaitTime + " seconds wait.");

    }
    public override void EndStep(EntityController entity)
    {

        //Debug.Log("Step executed after " + WaitTime + " seconds.");
        if (RemovePostProcessAfter)
        {
            Destroy(Volume);
            //Debug.Log("Removed custom profile" + Profile.name);
        }
    }
}