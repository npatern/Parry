using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Disguise ", menuName = "ScriptableObjects/Disguises/Disguise", order = 2)]
public class DisguiseScriptable : ScriptableObject
{
    public ItemTypes LegalItems = ItemTypes.UTILITY;
    public List<GameObject> defaultClothes;
    public List<GameObject> defaultHeadgear;

    public ItemWeaponScriptableObject item = null;
    public List<DisguiseScriptable> enforcesDisguises;

    public bool OverrideRestrictions = false;
    public RestrictionType OverridenRestriction = RestrictionType.FREE;

    public ListOfNeedsScriptable randomNeeds;

    public int GetLegality(ZoneScriptable _zone)
    {
        if (OverrideRestrictions)
            return (int)OverridenRestriction;
        if (_zone == null) return 0;
        if (_zone.FREEDisguises.Contains(this)) return 0;
        if (_zone.RESTRICTEDDisguises.Contains(this)) return 1;
        if (_zone.HOSTILEDisguises.Contains(this)) return 2;
        return (int)_zone.DefaultRestriction;
    }
}

[System.Serializable]
public class DisguiseWrapper
{
    public DisguiseScriptable disguiseType;
    public ItemTypes LegalItems;
    public List<GameObject> defaultClothes;
    public List<GameObject> defaultHeadgear;

    public ItemWeaponScriptableObject item = null;
    public List<DisguiseScriptable> enforcesDisguises;

    public bool OverrideRestrictions = false;
    public RestrictionType OverridenRestriction = RestrictionType.FREE;

    public ListOfNeedsScriptable randomNeeds;
    public DisguiseWrapper(DisguiseScriptable _source)
    {
        disguiseType = _source;
        LegalItems = _source.LegalItems;
        defaultClothes = new List<GameObject>(_source.defaultClothes);
        defaultHeadgear = new List<GameObject>(_source.defaultHeadgear);
        item = _source.item;
        OverrideRestrictions = _source.OverrideRestrictions;
        OverridenRestriction = _source.OverridenRestriction;
        enforcesDisguises = new List<DisguiseScriptable>(_source.enforcesDisguises);
        randomNeeds = _source.randomNeeds;
    }
}
[System.Flags]
public enum ItemTypes
{
    None = 0,
    UTILITY = 1 << 0,
    KNIFE = 1 << 1,
    PISTOL = 1 << 2,
    RIFLE = 1 << 3,
    SWORD = 1 << 4,
    EXPLOSIVES = 1 << 5,
    Everything = ~0
}