using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequirementKill", menuName = "ScriptableObjects/Objectives/RequirementKill", order = 1)]
public class RequirementKillScriptable : RequirementScriptable
{
    public override RequirementWrapper CreateWrapper()
    {
        return new RequirementKillWrapper(this);
    }
}
[System.Serializable]
public class RequirementKillWrapper : RequirementWrapper
{
    public StatusController targetStatus;
    public RequirementKillWrapper(RequirementKillScriptable requirementScriptable) : base(requirementScriptable)
    {

    }
    public override string GenerateText()
    {
       return "Get to the exit as fast as possible.";
    }
    public override bool CheckIfRequirementCompleted()
    {
        if (Completed) return true;
        if (targetStatus == null)
        {
            Failed = true;
            Completed = false;
            return false;
        }
        if (targetStatus.IsKilled)
        {
            Completed = true;
            return true;
        }
        return false;
    }
    public override bool CheckIfRequirementFailed()
    {
        if (Failed) return true;
        return false;
    }
}