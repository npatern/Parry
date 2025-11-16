using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : BaseManager
{
    public static ResourcesManager Instance { get; private set; }
    public ListOfAssetsAndValuesScriptableObject ListOfAssets { get; private set; }

private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void Initialize()
    {
        base.Initialize();
    }

}
