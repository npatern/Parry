using UnityEngine;

[CreateAssetMenu(fileName = "ListOfAssets", menuName = "ScriptableObjects/List of Assets", order = 1)]
public class ListOfAssetsScriptableObject : ScriptableObject
{
    [Space(10), Header("Weapons")]
    public ItemWeaponScriptableObject[] Weapons;
    public ItemWeaponScriptableObject GetRandomWeapon()
    {
        return Weapons[Random.Range(0,Weapons.Length)];
    }

    [Space(10), Header("Entities")]
    public EntityStatsScriptableObject DefaultEntityStats;
    public GameObject[] enemies;

}