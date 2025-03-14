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

    [Space(10), Header("Barks")]
    public Bark[] random;
    public Bark[] onPain;
    public Bark[] onDeath;
    public Bark[] onInvestigationStarted;
    public Bark[] onInvestigationEnded;
    public Bark[] inCombatNotice;
    public Bark[] onSearchStart;
    public Bark[] onSearchFail;
    public Bark[] inFleeEnterNotice;
    public Bark[] onFleeEnded;
}
[System.Serializable]
public class Bark
{
    public string name;

    public Bark()
    {
        name = "Bark.";
    }
    public Bark(string _name)
    {
        name = _name;
    }
}
public static class Barks
{
    public enum BarkTypes { onPain, onDeath , onInvestigationStarted , onInvestigationEnded , inCombatNotice , onSearchStart , onSearchFail , inFleeEnterNotice , onFleeEnded };
    public static string GetBark(BarkTypes barkType)
    {
        Bark[] barkList = GameController.Instance.ListOfAssets.random;
        switch (barkType)
        {
            case BarkTypes.onPain:
                barkList = GameController.Instance.ListOfAssets.onPain;
                break;
            case BarkTypes.onDeath:
                barkList = GameController.Instance.ListOfAssets.onDeath;
                break;
            case BarkTypes.onInvestigationStarted:
                barkList = GameController.Instance.ListOfAssets.onInvestigationStarted;
                break;
            case BarkTypes.onInvestigationEnded:
                barkList = GameController.Instance.ListOfAssets.onInvestigationEnded;
                break;
            case BarkTypes.inCombatNotice:
                barkList = GameController.Instance.ListOfAssets.inCombatNotice;
                break;
            case BarkTypes.onSearchStart:
                barkList = GameController.Instance.ListOfAssets.onSearchStart;
                break;
            case BarkTypes.onSearchFail:
                barkList = GameController.Instance.ListOfAssets.onSearchFail;
                break;
            case BarkTypes.inFleeEnterNotice:
                barkList = GameController.Instance.ListOfAssets.inFleeEnterNotice;
                break;
            case BarkTypes.onFleeEnded:
                barkList = GameController.Instance.ListOfAssets.onFleeEnded;
                break;
        }
        int index = Random.Range(0, barkList.Length);
        return barkList[index].name;
    }
}