using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Weapon Item ", menuName = "ScriptableObjects/Items", order = 2)]
public class ItemWeaponScriptableObject : ScriptableObject
{
    //later will be moved to Item>>
    public string ItemName = "Weapon";
    public string ID = "weapon";
    public string Description = "You can hack'n slash with it.";
    public bool Stackable = false;
    public bool Big = false;
    public GameObject weaponObject;


    //weapon specific>>
    public AttackPattern attackPattern;


    public float AttackDistance = 3f;
    public float Damage = 10;
    public float BulletDamage = 10;
    public Bullet bullet;
    public int stack;

    public AttackScriptableObject LightAttack;
    public AttackScriptableObject HeavyAttack;
    public AttackScriptableObject Parry;
    public AttackScriptableObject Equip;
    public AttackScriptableObject Dequip;
    public AttackScriptableObject Stunned;
    public AttackScriptableObject Attacked;
}
 
public class ItemWeaponWrapper
{
    public ItemWeaponScriptableObject itemType;
    public string ItemName;
    public string ID;
    public string Description;
    public bool Stackable;
    public int stack;
    public bool Big;
    public GameObject weaponObject;
    public Transform CurrentWeaponObject = null;
    public Pickable pickable = null; 
    //weapon specific>>
    public AttackPattern attackPattern;


    public float AttackDistance = 3f;
    public float Damage = 10;
    public float BulletDamage = 10;
    public Bullet Bullet;
    
    
    public ItemWeaponWrapper(ItemWeaponScriptableObject scriptableObject)
    {
        itemType = scriptableObject;
        Damage = scriptableObject.Damage;
        ItemName = scriptableObject.ItemName;
        ID = scriptableObject.ID;
        Description = scriptableObject.Description;
        Stackable = scriptableObject.Stackable;
        stack = scriptableObject.stack;
        Big = scriptableObject.Big;
        attackPattern = scriptableObject.attackPattern;
        weaponObject = scriptableObject.weaponObject;
        AttackDistance = scriptableObject.AttackDistance;
        Damage = scriptableObject.Damage;
        BulletDamage = scriptableObject.BulletDamage;
        Bullet = scriptableObject.bullet;
        
    }
    public Transform SpawnWeaponObject(Transform parentTransform = null)
    {
        CurrentWeaponObject = GameObject.Instantiate(weaponObject, parentTransform).transform;
        return CurrentWeaponObject;
    }
    public void MakePickable()
    {
        if (CurrentWeaponObject == null) return;

        CurrentWeaponObject.GetComponent<Collider>().enabled = true;
        CurrentWeaponObject.GetComponent<Collider>().isTrigger = false;
        pickable = GameObject.Instantiate(GameController.Instance.ListOfAssets.PickableTemplate, CurrentWeaponObject.position, CurrentWeaponObject.rotation).GetComponent<Pickable>();
        CurrentWeaponObject.transform.parent = pickable.transform;
        pickable.weaponObject = CurrentWeaponObject.gameObject;
        pickable.weaponWrapper = this;
    }
    public void RemovePickable(Transform newParent = null)
    {
        if (CurrentWeaponObject == null) return;
        if (pickable == null) return;
        CurrentWeaponObject.GetComponent<Collider>().enabled = false;
        CurrentWeaponObject.GetComponent<Collider>().isTrigger = true;
        CurrentWeaponObject.transform.parent = newParent;

        GameObject.Destroy(pickable.gameObject);
    }
    public void DestroyPhysicalPresence()
    {
        if (CurrentWeaponObject != null) CurrentWeaponObject.parent = null;
        if (pickable != null) GameObject.Destroy(pickable.gameObject);
        if (CurrentWeaponObject != null) GameObject.Destroy(CurrentWeaponObject.gameObject);
    }
}