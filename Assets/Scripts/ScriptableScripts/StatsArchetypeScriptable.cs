using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Stats Archetype", menuName = "ScriptableObjects/Stats/Stats Archetype", order = 2)]
public class StatsArchetypeScriptable : ScriptableObject
{
    [SerializeField]
    public Stat.Types[] statsPermitted;
    [SerializeField]
    public Stat.Types[] statsMortalOnEmpty;
    [SerializeField]
    public Stat.Types[] statsMortalOnFull;
    [SerializeField]
    public Stat.Types[] statsAlwaysFull;
    [SerializeField]
    public Stat.Types[] makeCountagous;

    public bool HasType(Stat.Types _typeToCheck)
    {
        if (statsPermitted == null) return false;
        foreach (Stat.Types _type in statsPermitted)
            if (_type == _typeToCheck) return true;
        return false;
    }
    public bool IsMortalOnEmpty(Stat.Types _typeToCheck)
    {
        if (statsMortalOnEmpty == null) return false;
        foreach (Stat.Types _type in statsMortalOnEmpty)
            if (_type == _typeToCheck) return true;
        return false;
    }
    public bool IsMortalOnFull(Stat.Types _typeToCheck)
    {
        if (statsMortalOnFull == null) return false;
        foreach (Stat.Types _type in statsMortalOnFull)
            if (_type == _typeToCheck) return true;
        return false;
    }
    public bool IsAlwaysFull(Stat.Types _typeToCheck)
    {
        if (statsAlwaysFull == null) return false;
        foreach (Stat.Types _type in statsAlwaysFull)
            if (_type == _typeToCheck) return true;
        return false;
    }
    public bool IsContagous(Stat.Types _typeToCheck)
    {
        if (makeCountagous == null) return false;
        foreach (Stat.Types _type in makeCountagous)
            if (_type == _typeToCheck) return true;
        return false;
    }
}