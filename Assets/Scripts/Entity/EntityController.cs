using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;
public class EntityController : MonoBehaviour
{
    float slowSpeed = 5;
    float fastSpeed = 8;

    public float AwarnessDistance = 7f;
    public float rotationSpeed = 10;
    public NavMeshAgent agent;
    private StatusController statusController;
    private SensesController sensesController;
    private ToolsController toolsController;
    private TMP_Text textMesh;
    public StatusController target;

    public List<NeedScriptableObject> ListOfNeeds;
    public int MaxNeeds = 4;

    public NeedScriptableObject CurrentNeed;
    public NeedFulfiller CurrentFulfiller;

    public NeedScriptableObject ExitNeed;
    public Rigidbody rb;

    public bool MarkedForDestruction = false;
    public bool IsAtTarget = false;

    public float ShockMemoryTime = 30;
    public float ShockMemoryTimer = 0;
    public float ShockTime = 1;
    public float ShockTimer = 1;
    public List<string> Log;
    private GameObject selector;

    public bool isSearchingForNeeds = false;
    public State currentState;
    public Vector3 HomePosition;
    private void Awake()
    {
        if (GetComponent<NavMeshAgent>()!=null)
        agent = GetComponent<NavMeshAgent>();
        if (GetComponent<StatusController>() != null)
            statusController = GetComponent<StatusController>();
        if (GetComponent<ToolsController>() != null)
            toolsController = GetComponent<ToolsController>();
        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        if (GetComponent<SensesController>() != null)
            sensesController = GetComponent<SensesController>();
        if (GetComponent<TMP_Text>() != null)
            textMesh = GetComponent<TMP_Text>();
    }
    void Start()
    {
        if (GameController.Instance.Needs.Length>0)
        while (ListOfNeeds.Count < 4)
            ListOfNeeds.Add(GameController.Instance.Needs[Random.Range(0, GameController.Instance.Needs.Length)]);
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityValues);
        HomePosition = transform.position;
        currentState = new Idle(this);

    }
    bool IsAwareOfPlayer()
    {
        if (target == null) 
            return false;

        if (sensesController == null)
            return Vector3.Distance(transform.position, target.transform.position) < AwarnessDistance;

        if (sensesController.Awareness > 0) return true; 
        else return false;

        ////
        //return (sensesController.IsTargetSeenTroughEyes(target.transform));
    }
    public void ProcessShockMemory()
    {
        ShockMemoryTimer -= Time.deltaTime;
    }
    public bool CanBeShocked()
    {
        if (ShockMemoryTimer > 0) return false;
        return true;
    }
    public void ResetShockMemory()
    {
        ShockMemoryTimer = ShockMemoryTime;
    }
    public bool IsEnteringShock()
    {
        ShockTimer = ShockTime;
        UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.inCombatNoticeDisbelief), transform);
        return true;
    }
    public bool IsProcessingShock()
    {
        ResetShockMemory();
        if (ShockTimer > 0)
        {
            ShockTimer -= Time.deltaTime;
            SetAgentSpeed(0);
            return true;
        }
        return false;
    }
    void Update()
    {
        ProcessShockMemory();
        currentState = currentState.Process();
         
        return;
    }
    void LoadStatsFromScriptable(EntityValuesScriptableObject scriptable)
    {
        slowSpeed = scriptable.NPCspeed;
        fastSpeed = scriptable.NPCspeedChase;
    }
    public void AttackTarget(StatusController statusObject)
    {
        float sizeDifferential = (1 + statusController.size.z) / 2;
        if (Vector3.Distance(target.transform.position, transform.position) < toolsController.attackDistance*2/3* sizeDifferential)
        {
            DisableNavmesh(false);
            Vector3 direction = Vector3.Normalize(transform.position - statusObject.transform.position);
            GoToTarget(transform.position+ direction);
        }else
        if (Vector3.Distance(target.transform.position, transform.position) > toolsController.attackDistance * sizeDifferential)
        {
            DisableNavmesh(false);
            GoToTarget(statusObject.transform.position);
        }
        else if (sensesController.IsAlerted)
        {
            DisableNavmesh(true);
            LookAtTarget(target.transform);
            PlayRandomAttack();
        }
    }
    void PlayRandomAttack()
    {
        if (toolsController == null) return;
        int randomizer = Random.Range(0, 11);
        if (randomizer < 6 && toolsController.CurrentWeaponWrapper.itemType.bullet ==null)
            toolsController.PerformParry();
        else if (randomizer < 10)
            toolsController.PerformAttack();
        else
            toolsController.PerformHeavyAttack();
    }
    float multiplySpeed()
    {
        float _multiply = 1;
        _multiply *= statusController.movementSpeedMultiplier;
        _multiply *= statusController.movementSpeedMultiplierStun;
        return _multiply;
    }
    public void SetAgentSpeed(float speed)
    {
        agent.speed = speed * multiplySpeed();
    }
    public void SetAgentSpeedChase()
    {
        agent.speed = fastSpeed * multiplySpeed();
    }
    public void SetAgentSpeedWalk()
    {
        agent.speed = slowSpeed* multiplySpeed();
    }
    public void StateFulfillingNeeds()
    {
        
        if (isSearchingForNeeds) return;
        if (CurrentNeed == null)
        {
            StartCoroutine(FindNeed(.1f));
            return;
        }
        if (CurrentFulfiller == null)
        {
            FindFulfiller();
            return;
        }
        agent.avoidancePriority = (int)Vector3.Distance(transform.position, CurrentFulfiller.transform.position);
        if (IsTargetReached() && !IsAtTarget)
        {
            IsAtTarget = true;
            AddToLog("Arrived to " + CurrentFulfiller.name + ". Starting fulfilling the need of " + CurrentNeed.Name);
            StartCoroutine(CurrentFulfiller.ExecuteSteps(this));
        }
    }
    public void AddToLog(string log)
    {
        Log.Add(log);
    }
    public void AddToLog(string log, int positiveness)
    {
        if (positiveness > 0) log = "<color=green>" + log + "</color>";
        if (positiveness < 0) log = "<color=red>" + log + "</color>";
        Log.Add(log);
    }
    public bool IsTargetReached()
    {
        
        if (agent == null) return false;
            //tutaj powinny byc odwrotne statementy i return false
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public IEnumerator FindNeed(float waitTime)
    {
        isSearchingForNeeds = true;
        if (ListOfNeeds.Count > 0) CurrentNeed = ListOfNeeds[0];
        if (ListOfNeeds.Count <= 0) ListOfNeeds.Add(ExitNeed);
        if (CurrentNeed == null)
            AddToLog("Couldn't find need :(",-1);
        else
            AddToLog("Next need: " + CurrentNeed.Name + " (" + CurrentNeed.Description + ")");
        yield return new WaitForSeconds(waitTime);
        isSearchingForNeeds = false;
    }
    public void FindNextNeed()
    {
        if (ListOfNeeds.Count <= 0) ListOfNeeds.Add(ExitNeed);
        if (ListOfNeeds.Count > 0) CurrentNeed = ListOfNeeds[0];
        if (CurrentNeed == null)
            AddToLog("Couldn't find need :(", -1);
        else
            AddToLog("Next need: " + CurrentNeed.Name + " (" + CurrentNeed.Description + ")");
 
 
    }
    public void SetAvoidancePriority(int priority)
    {
        agent.avoidancePriority = priority;
    }
    public void SetAvoidanceRadius(float radius)
    {
        return;
        agent.radius = radius;
        if (radius==0)
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        else
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }
    public void FindFulfiller()
    {
        List<NeedFulfiller> fulfillersList = new List<NeedFulfiller>(GameController.Instance.NeedFulfillers);
        fulfillersList = fulfillersList.OrderBy(x => Random.value).ToList();
        foreach (NeedFulfiller fulfiller in fulfillersList)
            if (fulfiller.NeedToFulfill == CurrentNeed && fulfiller.User == null && fulfiller.CanStatusUseIt(statusController))
            {
                CurrentFulfiller = fulfiller;
                if (!fulfiller.Unreservable)
                CurrentFulfiller.User = this;
                AddToLog("Found object for fulfilling my " + CurrentNeed + " need - " + CurrentFulfiller.name + "!");
                GoToTarget(CurrentFulfiller.transform.position);
                break;
            }
        if (CurrentFulfiller == null)
        {
            AddToLog("Didnt find object for fulfilling my " + CurrentNeed+"!", -1);
            NeedFulfilled(false);
        }
        
    }
    public void GoToTarget(Vector3 targetPosition, float stoppingDistance = 0f)
    {
        if (agent == null) return;
        agent.stoppingDistance = stoppingDistance;
        agent.isStopped = false;
        agent.destination = targetPosition;
        //AddToLog("Destination set. Going to " + targetTransform.name);
        IsAtTarget = false;

    }
    public void DisableNavmesh( bool isDisabled)
    {
        if (agent == null) return;
        agent.isStopped = isDisabled;
        if(isDisabled)
        agent.SetDestination(transform.position);
        //agent.enabled = !isDisabled;
    }
    public void LookAtTarget(Transform target)
    {

        /*
        transform.LookAt(target);
        //rb.centerOfMass = Vector3.zero;
        rb.freezeRotation = true;
        */
        float angle;

        var localTarget = transform.InverseTransformPoint(target.transform.position);

        angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * rotationSpeed);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
    public void ResetFulfiller()
    {
        if (CurrentFulfiller != null)
            CurrentFulfiller.ResetFulfiller();
        CurrentFulfiller = null;
    }
    public void NeedFulfilled(bool success)
    {
        
        if (success)
            AddToLog("Need fulfilled! (" + CurrentNeed + ")", +1);
        else
            AddToLog("Need FAILED! (" + CurrentNeed + ")", -1);
        
        if (MarkedForDestruction)
        {
            GameController.Instance.RemoveFromListOfEntities(this);
            Destroy(gameObject);
            return;
        }
        ListOfNeeds.RemoveAt(0);
        CurrentNeed = null;
        if (CurrentFulfiller != null) CurrentFulfiller.User = null;
        CurrentFulfiller = null;
        

        //AddToLog("Only " + ListOfNeeds.Count + " needs left.");
        
    }
    public void Select()
    {
        if (selector == null) selector = Instantiate(GameController.Instance.EntitySelector, transform);
    }
    public void Deselect()
    {
        if (selector != null)
            Destroy(selector);
    }
    public bool CanMove()
    {
        if (toolsController == null) return true;
        if (toolsController.MovementLocked || toolsController.RotationLocked)
            return false;
        else
            return true;
    }
    public bool IsCombatReady()
    {
        if (toolsController == null) return false;
        if (toolsController.CurrentWeaponWrapper == null) return false;
        if (toolsController.CurrentWeaponWrapper.emptyhanded) return false;
        return true;
    }
}