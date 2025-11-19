using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
public class GameplayController : BaseController
{
    [Header("References")]
    public Transform Level;
    public ListOfAssetsAndValuesScriptableObject ListOfAssets;
    public FollowTarget cameraController;
    public Transform GarbageCollector;
    public EntityController CurrentEntity = null;
    

    [Header("Settings")]
    public bool spawnOnce = false;
    public int MaxEntitiesNr = 10;
    public bool StopTimeOnStart = false;
    public float TimeBetweenSpawns = 5;
    
    [Header("Needs")]
    public NeedFulfiller[] NeedFulfillers;

    [Header("Post Processing")]
    public GameObject PostProcess;
    public PostProcessVolume SlowmoPostProcess;
    public PostProcessVolume StunnedPostProcess;
    public PostProcessVolume NoctovisionPostProcess;
    public bool NoctovisionOn = false;

    [Header("Runtime")]
    public GameObject PlayerEntity;
    public Transform EntitiesParent;
    public GameObject EntitySelector;
    public StatusController CurrentPlayer;
    public List<EntityController> EntitiesInGame = new List<EntityController>();
    public List<LightController> lightControllers = new List<LightController>();
    public GameObject[] LegacySpawners;
    public SpawnerNPC[] Spawners;

    [Header("Timers")]
    [SerializeField]
    float spawnTimer = 0;
    [SerializeField]
    float slowmoTimer = 0;
    bool slowmo = false;

    public static GameplayController _instance;
    public static GameplayController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameplayController>();
            }

            return _instance;
        }
    }
    protected override void OnInitialize()
    {
        base.OnInitialize();
    }
    private void Awake()
    {
        //Instance = this;
        //DontDestroyOnLoad(gameObject);
        if (StopTimeOnStart) StopTime();
        CollectLevelElements();
        lightControllers = new List<LightController>();
        
    }
    private void Start()
    {
      //  if (spawnOnce)
          //  SpawnNpcs();
    }
    void CollectLevelElements()
    {
        LegacySpawners = GameObject.FindGameObjectsWithTag("Spawn");
        Spawners = GameObject.FindObjectsOfType<SpawnerNPC>();
        Debug.Log("Znaleziono spawnerów: " + Spawners.Length);
        CollectNeedFulfillers();
        //Level.GetComponentsInChildren(lightControllers);
    }
    public void SpawnNpcs()
    {
        
        foreach (SpawnerNPC spawner in Spawners)
        {
            spawner.RunSpawner();

        }
        /*
            while (EntitiesInGame.Count < MaxEntitiesNr)
            SpawnEntity();
        */
    }
    public void SpawnNpcsRuntime()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            if (EntitiesInGame.Count < MaxEntitiesNr)
                SpawnEntityFromRandomSpawner();
            spawnTimer = TimeBetweenSpawns;
        }
    }
    private void FixedUpdate()
    {
        if (!spawnOnce)
            SpawnNpcsRuntime();
        ApplySlowmoPostprocess();
        ApplyStunnedPostprocess();
    }
    
    void ApplyStunnedPostprocess()
    {
        if (CurrentPlayer == null) return; 
    }
    public void SwitchNoctovision()
    {
        NoctovisionOn = !NoctovisionOn;
        if (NoctovisionOn) 
            NoctovisionPostProcess.weight = 1;
        else
            NoctovisionPostProcess.weight = 0;
    }
    void ApplySlowmoPostprocess()
    {
        float slowmospeed = .5f;
        SlowmoPostProcess.weight = slowmoTimer * 3;
        if (Time.timeScale > 0)
            if (slowmoTimer > 0)
            {
                if (!slowmo)
                {

                    SetTimeSpeed(slowmospeed);
                    CurrentPlayer.SlowmoSpeedMultiplier = CurrentPlayer.SlowmoSpeedMultiplier / slowmospeed;
                }
                slowmo = true;
                slowmoTimer -= Time.fixedDeltaTime;
            }
            else
            {

                if (slowmo)
                {
                    SetTimeSpeed(1);
                    CurrentPlayer.SlowmoSpeedMultiplier = CurrentPlayer.SlowmoSpeedMultiplier * slowmospeed;
                }
                slowmo = false;
            }
    }
    public void IncreaseSlowmoTimer()
    {
        slowmoTimer = 1f;
        SetTimeSpeed(.5f);
    }
    public void EndGame()
    {
        UIController.Instance.StartScreen.gameObject.SetActive(true);
    }
    public void ResetGame()
    {
        StatusController[] entities = EntitiesParent.GetComponentsInChildren<StatusController>();
        //foreach (EntityController entity in EntitiesInGame)
        foreach (StatusController entity in entities)
            Destroy(entity.gameObject);
    }
    public void RemoveFromListOfEntities(EntityController sentity)
    {
        int targetIndex = EntitiesInGame.IndexOf(sentity);
        if (targetIndex >= 0)
        {
            EntitiesInGame.RemoveAt(targetIndex);
        }
    }
    public void StartGame()
    {
        EntitiesInGame = new List<EntityController>();
        CurrentPlayer = Instantiate(PlayerEntity, Vector3.zero, Quaternion.identity, EntitiesParent).GetComponent<StatusController>() ;
        CurrentPlayer.GetComponent<ToolsController>().EquipItem(new ItemWeaponWrapper(ListOfAssets.GetRandomWeapon()));
        cameraController.ApplyTarget(CurrentPlayer.transform);
        SpawnNpcs();
    }
    public void RestartGame()
    {
        ResetGame();
        StartGame();
    }
    void CollectNeedFulfillers()
    {
        NeedFulfillers = FindObjectsOfType(typeof(NeedFulfiller)) as NeedFulfiller[];
    }
    public void SpawnNPC(Transform spawnerTransform)
    {
        if (ListOfAssets == null) return;
        DisguiseScriptable disguise = ListOfAssets.GetRandomDisguise();
        SpawnNPC(spawnerTransform, disguise);
    }
    public void SpawnNPC(Transform spawnerTransform, DisguiseScriptable disguise)
    {
        GameObject npc = ListOfAssets.enemies[0];
        OutwardController nakedGuy = Instantiate(npc, spawnerTransform.position, spawnerTransform.rotation, GameplayController.Instance.EntitiesParent).GetComponent<OutwardController>();
        nakedGuy.WearDisguise(disguise);
        EntityController newEntity = nakedGuy.GetComponent<EntityController>();
        newEntity.target = GameplayController.Instance.CurrentPlayer;
        if (disguise.item != null)
            nakedGuy.GetComponent<ToolsController>().EquipItem(new ItemWeaponWrapper(disguise.item));
        EntitiesInGame.Add(newEntity);
    }
    void SpawnEntityFromRandomSpawner()
    {
        if (LegacySpawners.Length == 0) return;
        int spawnerNumber = Random.Range(0, LegacySpawners.Length);
        SpawnNPC(LegacySpawners[spawnerNumber].transform);
    }
    
    public void SelectEntity(EntityController entity)
    {
        if (CurrentEntity != null && CurrentEntity != entity) DeselectEntity();
        CurrentEntity = entity;
        CurrentEntity.Select();
    }
    public void DeselectEntity()
    {
        if (CurrentEntity == null) return;
        CurrentEntity.Deselect();
        CurrentEntity = null;
    }
    public void SetTimeSpeed(float timeSpeed)
    {
        Time.timeScale = timeSpeed;
    }
    public void ResetTimeSpeed()
    {
        SetTimeSpeed(1);
    }
    public void StopTime()
    {
        SetTimeSpeed(0);
    }
}
