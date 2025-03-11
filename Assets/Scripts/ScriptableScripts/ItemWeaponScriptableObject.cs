using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item ", menuName = "ScriptableObjects/Items", order = 2)]
public class ItemWeaponScriptableObject : ScriptableObject
{
    //later will be moved to Item>>
    public string ItemName = "Weapon";
    public string ID = "weapon";
    public string Description = "You can hack'n slash with it.";

    //weapon specific>>
    public AttackPattern attackPattern;
    public GameObject weaponObject;

    public float AttackDistance = 3f;
    public float Damage = 10;
    public Bullet bullet;

    public AttackScriptableObject LightAttack;
    public AttackScriptableObject HeavyAttack;
    public AttackScriptableObject Parry;
    public AttackScriptableObject Equip;
    public AttackScriptableObject Dequip;
    public AttackScriptableObject Stunned;
    public AttackScriptableObject Attacked;
}