using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : BaseController
{
    public static LevelController _instance;
    public static LevelController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LevelController>();
            }

            return _instance;
        }
    }

    [Header("Runtime")]
    public GameObject PlayerEntity;
    public Transform EntitiesParent;
    public GameObject EntitySelector;
    public StatusController CurrentPlayer;
    public List<EntityController> EntitiesInGame = new List<EntityController>();
    public List<LightController> lightControllers = new List<LightController>();
    public GameObject[] LegacySpawners;
    public SpawnerNPC[] Spawners;
    [Header("Needs")]
    public NeedFulfiller[] NeedFulfillers;
    protected override void OnInitialize()
    {
        CollectLevelElements();
        lightControllers = new List<LightController>();
        base.OnInitialize();
    }
    void CollectLevelElements()
    {
        LegacySpawners = GameObject.FindGameObjectsWithTag("Spawn");
        Spawners = GameObject.FindObjectsOfType<SpawnerNPC>();
        Debug.Log("Znaleziono spawnerów: " + Spawners.Length);
        CollectNeedFulfillers();
        //Level.GetComponentsInChildren(lightControllers);
    }
    void CollectNeedFulfillers()
    {
        NeedFulfillers = FindObjectsOfType(typeof(NeedFulfiller)) as NeedFulfiller[];
    }
}
