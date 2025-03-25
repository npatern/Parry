using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
public class GameController : MonoBehaviour
{
    public Transform Level;
    public int MaxEntitiesNr = 10;
    public ListOfAssetsAndValuesScriptableObject ListOfAssets;
    public FollowTarget cameraController;
    public bool StopTimeOnStart = false;
    public float TimeBetweenSpawns = 5;
    [SerializeField]
    float timer = 0;
    public Transform GarbageCollector;
    public static GameController Instance { get; private set; }
    public EntityController CurrentEntity = null;
    public GameObject[] Spawners;
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
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        if (StopTimeOnStart) StopTime();
        CollectLevelElements();
        lightControllers = new List<LightController>();
}
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (EntitiesInGame.Count< MaxEntitiesNr)
                SpawnEntity();
            timer = TimeBetweenSpawns;
        }
    }
    private void FixedUpdate()
    {
        ApplySlowmoPostprocess();
        ApplyStunnedPostprocess();
    }
    void CollectLevelElements()
    {
        Spawners = GameObject.FindGameObjectsWithTag("Spawn");
        CollectNeedFulfillers();
        //Level.GetComponentsInChildren(lightControllers);
    }
    void ApplyStunnedPostprocess()
    {
        if (CurrentPlayer == null) return;
        if (!CurrentPlayer.IsStunned()) StunnedPostProcess.weight = 0;
        else StunnedPostProcess.weight = CurrentPlayer.GetStunndedTimerValue()*3;
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
                    CurrentPlayer.SpeedMultiplier = CurrentPlayer.SpeedMultiplier / slowmospeed;
                }
                slowmo = true;
                slowmoTimer -= Time.fixedDeltaTime;
            }
            else
            {

                if (slowmo)
                {
                    SetTimeSpeed(1);
                    CurrentPlayer.SpeedMultiplier = CurrentPlayer.SpeedMultiplier * slowmospeed;
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
        if (Spawners.Length == 0) return;
        int spawnerNumber = Random.Range(0, Spawners.Length);
        if (ListOfAssets == null) return;
        if (ListOfAssets.enemies.Length == 0) return;
        int entityNumber = Random.Range(0, ListOfAssets.enemies.Length);
        EntityController newEntity = Instantiate(ListOfAssets.enemies[entityNumber], Spawners[spawnerNumber].transform.position, Quaternion.identity, EntitiesParent).GetComponent<EntityController>();
        newEntity.target = CurrentPlayer;
        EntitiesInGame.Add(newEntity);
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
