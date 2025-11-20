using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Requirement", menuName = "ScriptableObjects/Objectives/Requirement", order = 1)]
public abstract class RequirementScriptable : ScriptableObject
{
    public string Name;
    public abstract RequirementWrapper CreateWrapper();
    public string RequirementText;

    
}
[System.Serializable]
public class RequirementWrapper
{
    public string Name;
    public string Text;
    public bool Completed;
    public bool Failed;
    public RequirementWrapper(RequirementScriptable requirementScriptable)
    {
        Name = requirementScriptable.Name;
        Text = GenerateText();
    }
    public virtual string GenerateText()
    {
        return "";
    }
    public virtual bool CheckIfRequirementCompleted()
    {
        return true;
    }
    public virtual bool CheckIfRequirementFailed()
    {
        return false;
    }
    
}
