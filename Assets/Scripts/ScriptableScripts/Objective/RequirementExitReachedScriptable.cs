using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequirementExitReached", menuName = "ScriptableObjects/Objectives/RequirementExitReached", order = 1)]
public class RequirementExitReachedScriptable : RequirementScriptable
{
    public override RequirementWrapper CreateWrapper()
    {
        return new RequirementExitReachedWrapper(this);
    }
}
[System.Serializable]
public class RequirementExitReachedWrapper : RequirementWrapper
{
    public RequirementExitReachedWrapper(RequirementExitReachedScriptable requirementScriptable) : base(requirementScriptable)
    {

    }
    public override string GenerateText()
    {
       return "Get to the exit as fast as possible.";
    }
    public override bool CheckIfRequirementCompleted()
    {
        if (Completed) return true;
        if (LevelController.Instance.ExitReached)
        {
            Completed = true;
            return true;
        }
        return false;
    }
    public override bool CheckIfRequirementFailed()
    {
        return false;
    }
}