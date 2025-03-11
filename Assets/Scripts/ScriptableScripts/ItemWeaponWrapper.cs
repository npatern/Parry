using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeaponWrapper
{
    public ItemWeaponScriptableObject itemType;
    public float Damage;
    
    public ItemWeaponWrapper(ItemWeaponScriptableObject scriptableObject)
    {
        itemType = scriptableObject;
        Damage = scriptableObject.Damage;
    }
}
