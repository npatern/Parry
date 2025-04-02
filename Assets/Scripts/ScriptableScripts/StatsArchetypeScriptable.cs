using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Stats Archetype", menuName = "ScriptableObjects/Stats/Stats Archetype", order = 2)]
public class StatsArchetypeScriptable : ScriptableObject
{
    [SerializeField]
    public Stat.Types[] statsPermitted;
    public bool HasType(Stat.Types _typeToCheck)
    {
        foreach (Stat.Types _type in statsPermitted)
            if (_type == _typeToCheck) return true;
        return false;
    }
}