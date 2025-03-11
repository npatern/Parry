using UnityEngine;

[CreateAssetMenu(fileName = "Need", menuName = "ScriptableObjects/Needs/Need", order = 1)]
public class NeedScriptableObject : ScriptableObject
{
    public string ID;
    public string Name;
    public string Description;
}