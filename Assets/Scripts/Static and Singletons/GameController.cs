using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
public class GameController : MonoBehaviour
{
    public bool spawnOnce = false;
    public Transform Level;
    public int MaxEntitiesNr = 10;
    public ListOfAssetsAndValuesScriptableObject ListOfAssets;
    public FollowTarget cameraController;
    public bool StopTimeOnStart = false;
    public float TimeBetweenSpawns = 5;
    [SerializeField]
    float timer = 0;
    public Transform GarbageCollector;
    public static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameController>();
            }

            return _instance;
        }
    }
    public EntityController CurrentEntity = null;
    public GameObject[] LegacySpawners;
    public SpawnerNPC[] Spawners;
    public NeedScriptableObject[] Needs;
    public NeedScriptableObject ExitNeed;
    public NeedFulfiller[] NeedFulfillers;
    public GameObject PostProcess;
    public PostProcessVolume SlowmoPostProcess;
    public PostProcessVolume StunnedPostProcess;
    public PostProcessVolume NoctovisionPostProcess;
    public bool NoctovisionOn = false;
    public GameObject PlayerEntity;
    public Transform EntitiesParent;
    public GameObject EntitySelector;
    public StatusController CurrentPlayer;
    public List<EntityController> EntitiesInGame = new List<EntityController>();
    public List<LightController> lightControllers = new List<LightController>();
    [SerializeField]
    float slowmoTimer = 0;
    bool slowmo = false;
    private void Awake()
    {
        //Instance = this;
        DontDestroyOnLoad(gameObject);
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
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (EntitiesInGame.Count < MaxEntitiesNr)
                SpawnEntity();
            timer = TimeBetweenSpawns;
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
        CurrentPlayer.GetComponent<ToolsController>().EquipWeapon(new ItemWeaponWrapper(ListOfAssets.GetRandomWeapon()));
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
    void SpawnEntity()
    {
        if (LegacySpawners.Length == 0) return;
        int spawnerNumber = Random.Range(0, LegacySpawners.Length);
        if (ListOfAssets == null) return;
        if (ListOfAssets.enemies.Length == 0) return;
        int entityNumber = Random.Range(0, ListOfAssets.enemies.Length);
        GameObject _objToSpawn = ListOfAssets.enemies[entityNumber];
        Vector3 _spawnPosition = LegacySpawners[spawnerNumber].transform.position;
        EntityController newEntity = Instantiate(_objToSpawn, _spawnPosition, Quaternion.identity, EntitiesParent).GetComponent<EntityController>();
        newEntity.target = CurrentPlayer;
        EntitiesInGame.Add(newEntity);
    }
    public void SpawnEntity(Transform spawn,DisguiseScriptable disguise)
    {
        if (ListOfAssets == null) return;
        if (ListOfAssets.enemies.Length == 0) return;
        EntityController newEntity = Instantiate(ListOfAssets.enemies[0], transform.position, transform.rotation, GameController.Instance.EntitiesParent).GetComponent<EntityController>();
        newEntity.target = CurrentPlayer;
        //WearDisguise(DisguiseScriptable _disguise)
        newEntity.GetComponent<ToolsController>().EquipWeapon(new ItemWeaponWrapper(disguise.item));
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
