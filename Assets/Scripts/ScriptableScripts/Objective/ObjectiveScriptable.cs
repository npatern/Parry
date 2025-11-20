using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "ScriptableObjects/Objectives/Objective", order = 1)]
public class ObjectiveScriptable : ScriptableObject
{
    public string Name;
    public string Description;
    //requirements scriptable
    ////get item
    ////get to exit
    ////kill target
    ////fire kill..?
    ////set on fire
    ////poison
    ////electrify
    //fail states
    ////only kill targets
    ////no witnesses
    ////dont kill anyone
    ////dont get spotted
    public List<RequirementScriptable> RequirementScriptables;
}
[System.Serializable]
public class ObjectiveWrapper
{
    public string Name;
    public string Description;
    public List<RequirementWrapper> RequirementWrappers;
    public bool Failed;
    public bool Completed;

    public ObjectiveWrapper(ObjectiveScriptable scriptableObjective)
    {
        Name = scriptableObjective.Name;
        Description = scriptableObjective.Description;
        Failed = false;
        Completed = false;
        RequirementWrappers = new List<RequirementWrapper>();
        foreach (RequirementScriptable requirementScriptable in scriptableObjective.RequirementScriptables)
            RequirementWrappers.Add(requirementScriptable.CreateWrapper());
    }
    public bool CheckIfCompleted()
    {
        if (Completed) return true;
        foreach (var requirementWrapper in RequirementWrappers)
        {
            if (!requirementWrapper.CheckIfRequirementCompleted()) return false;
        }
        Completed = true;
            return true;
    }
     
}