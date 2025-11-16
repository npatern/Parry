using UnityEngine;

[CreateAssetMenu(fileName = "ListOfAssets", menuName = "ScriptableObjects/List of Assets", order = 1)]
public class ListOfAssetsAndValuesScriptableObject : ScriptableObject
{
    [Space(10), Header("Level")]
    public float ambientLightValue = .1f;

    [Space(10), Header("Weapons")]
    public GameObject PickableTemplate;
    public Bullet BulletThrowTemplate;
    public ItemWeaponScriptableObject EmptyWeapon;
    public ItemWeaponScriptableObject[] Weapons;
    public ItemWeaponScriptableObject GetRandomWeapon()
    {
        return Weapons[Random.Range(0,Weapons.Length)];
    }

    [Space(10), Header("Disguises")]
    public DisguiseScriptable[] Disguises;
    public DisguiseScriptable GetRandomDisguise()
    {
        return Disguises[Random.Range(0, Disguises.Length)];
    }
    [Space(10), Header("Needs")]
    public NeedScriptableObject[] Needs;
    public NeedScriptableObject ExitNeed;

    [Space(10), Header("Entities")]
    public EntityValuesScriptableObject DefaultEntityValues;
    public StatsScriptable DefaultEffectStats;
    public EffectVisuals[] EffectVisuals;
    public GameObject[] enemies;

    [Space(10), Header("Barks")]
    public Bark[] random;
    public Bark[] onPain;
    public Bark[] onDeath;
    public Bark[] onParry;
    public Bark[] onInvestigationStarted;
    public Bark[] onInvestigationEnded;
    public Bark[] inCombatNoticeDisbelief;
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
[System.Serializable]
public class EffectVisuals
{
    public string name;
    public Stat.Types type;
    public Sprite sprite;
    public Color iconColor = Color.white;
    public Color color = Color.white;
    public Color activeColor = Color.white;
    public ParticleSystem particles;
}
public static class Barks
{
    public enum BarkTypes { onPain, onDeath, onParry , onInvestigationStarted , onInvestigationEnded , inCombatNoticeDisbelief, inCombatNotice, onSearchStart , onSearchFail , inFleeEnterNotice , onFleeEnded };
    public static string GetBark(BarkTypes barkType)
    {
        Bark[] barkList = GameplayController.Instance.ListOfAssets.random;
        switch (barkType)
        {
            case BarkTypes.onPain:
                barkList = GameplayController.Instance.ListOfAssets.onPain;
                break;
            case BarkTypes.onDeath:
                barkList = GameplayController.Instance.ListOfAssets.onDeath;
                break;
            case BarkTypes.onParry:
                barkList = GameplayController.Instance.ListOfAssets.onParry;
                break;
            case BarkTypes.onInvestigationStarted:
                barkList = GameplayController.Instance.ListOfAssets.onInvestigationStarted;
                break;
            case BarkTypes.onInvestigationEnded:
                barkList = GameplayController.Instance.ListOfAssets.onInvestigationEnded;
                break;
            case BarkTypes.inCombatNotice:
                barkList = GameplayController.Instance.ListOfAssets.inCombatNotice;
                break; 
            case BarkTypes.inCombatNoticeDisbelief:
                barkList = GameplayController.Instance.ListOfAssets.inCombatNoticeDisbelief;
                break;
            case BarkTypes.onSearchStart:
                barkList = GameplayController.Instance.ListOfAssets.onSearchStart;
                break;
            case BarkTypes.onSearchFail:
                barkList = GameplayController.Instance.ListOfAssets.onSearchFail;
                break;
            case BarkTypes.inFleeEnterNotice:
                barkList = GameplayController.Instance.ListOfAssets.inFleeEnterNotice;
                break;
            case BarkTypes.onFleeEnded:
                barkList = GameplayController.Instance.ListOfAssets.onFleeEnded;
                break;
        }
        if (barkList.Length <= 0) return "Whatever...";
        int index = Random.Range(0, barkList.Length);
        
        return barkList[index].name;
    }
}