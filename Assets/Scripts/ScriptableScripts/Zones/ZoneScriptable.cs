using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Zone ", menuName = "ScriptableObjects/Disguises/Zone", order = 2)]

public class ZoneScriptable : ScriptableObject
{
    public RestrictionType DefaultRestriction = RestrictionType.FREE;
    [Tooltip("Zone with highest nr will affect player")]
    public int Depth = 0;

    public List<DisguiseScriptable> FREEDisguises;
    public List<DisguiseScriptable> RESTRICTEDDisguises;
    public List<DisguiseScriptable> HOSTILEDisguises;
}
public enum RestrictionType { FREE, RESTRICTED, HOSTILE };

[System.Serializable]
public class ZoneWrapper
{
    
    public RestrictionType DefaultRestriction = RestrictionType.FREE;
    [Tooltip("Zone with highest nr will affect player")]
    public int Depth = 0;

    public List<DisguiseScriptable> FREEDisguises;
    public List<DisguiseScriptable> RESTRICTEDDisguises;
    public List<DisguiseScriptable> HOSTILEDisguises;
    public ZoneWrapper()
    {
        DefaultRestriction = RestrictionType.FREE;
        Depth = 0;
    }
    public ZoneWrapper(ZoneScriptable _source)
    {
        DefaultRestriction = _source.DefaultRestriction;
        Depth = _source.Depth;
        FREEDisguises = _source.FREEDisguises;
        RESTRICTEDDisguises = _source.RESTRICTEDDisguises;
        HOSTILEDisguises = _source.HOSTILEDisguises;
    }
}