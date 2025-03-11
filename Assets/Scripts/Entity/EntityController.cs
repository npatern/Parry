using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class EntityController : MonoBehaviour
{
    float slowSpeed = 5;
    float fastSpeed = 8;

    public float AwarnessDistance = 7f;
    public float rotationSpeed = 10;
    private NavMeshAgent agent;
    private StatusController statusController;
    private SensesController sensesController;
    private CombatController combatController;
    public StatusController target;

    public List<NeedScriptableObject> ListOfNeeds;
    public int MaxNeeds = 4;

    public NeedScriptableObject CurrentNeed;
    public NeedFulfiller CurrentFulfiller;

    public NeedScriptableObject ExitNeed;
    public Rigidbody rb;

    public bool MarkedForDestruction = false;
    public bool IsAtTarget = false;

    public List<string> Log;
    private GameObject selector;

    public bool isSearchingForNeeds = false;
    State currentState;
    private void Awake()
    {
        if (GetComponent<NavMeshAgent>()!=null)
        agent = GetComponent<NavMeshAgent>();
        if (GetComponent<StatusController>() != null)
            statusController = GetComponent<StatusController>();
        if (GetComponent<CombatController>() != null)
            combatController = GetComponent<CombatController>();
        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        if (GetComponent<SensesController>() != null)
            sensesController = GetComponent<SensesController>();
    }
    void Start()
    {
        if (GameController.Instance.Needs.Length>0)
        while (ListOfNeeds.Count < 4)
            ListOfNeeds.Add(GameController.Instance.Needs[Random.Range(0, GameController.Instance.Needs.Length)]);
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
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
    
    void Update()
    {
        currentState = currentState.Process();
        return;
        if (sensesController.isAware) agent.speed = fastSpeed;
        else agent.speed = slowSpeed;
      
        if (IsAwareOfPlayer())
        {
            CurrentFulfiller = null;
            agent.avoidancePriority = (int)Vector3.Distance(transform.position, target.transform.position);
            if (!CanMove()) DisableNavmesh(true);
            else DisableNavmesh(false);
        }
        else
        {
            
            DisableNavmesh(false);
            StateFulfillingNeeds();
            return;
        }
        

        AttackTarget(target);


        //jesli nie jestes blisko gracza i mozesz chodzic - idz do gracza
        // jesli nie patrzysz na gracza - patrz na gracza
        //jesli nie atakujesz gracza - atakuj gracza
        //if (player != null && Vector3.Distance(transform.position, player.transform.position) < AwarnessDistance)
    }
    void LoadStatsFromScriptable(EntityStatsScriptableObject scriptable)
    {
        slowSpeed = scriptable.NPCspeed;
        fastSpeed = scriptable.NPCspeedChase;
        
    }
    public void AttackTarget(StatusController statusObject)
    {
        isSearchingForNeeds = false;
        

        if (Vector3.Distance(target.transform.position, transform.position) > combatController.attackDistance)
        {
            DisableNavmesh(false);
            GoToTarget(statusObject.transform);
        }
        else if (sensesController.isAware)
        {
            DisableNavmesh(true);
            LookAtTarget(target.transform);
            PlayRandomAttack();
        }

        /*
        //agent.stoppingDistance = 4f;
        GoToTarget(statusObject.transform);
        
        if (IsTargetReached())
        {
            PlayRandomAttack();
        }
        */
    }
    void PlayRandomAttack()
    {
        if (combatController == null) return;
        int randomizer = Random.Range(0, 11);
        if (randomizer < 6 && combatController.CurrentWeaponWrapper.itemType.bullet ==null)
            combatController.PerformParry();
        else if (randomizer < 10)
            combatController.PerformAttack();
        else
            combatController.PerformHeavyAttack();
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
    public void FindFulfiller()
    {
        List<NeedFulfiller> fulfillersList = new List<NeedFulfiller>(GameController.Instance.NeedFulfillers);
        fulfillersList = fulfillersList.OrderBy(x => Random.value).ToList();
        foreach (NeedFulfiller fulfiller in fulfillersList)
            if (fulfiller.NeedToFulfill == CurrentNeed && fulfiller.User == null)
            {
                CurrentFulfiller = fulfiller;
                if (!fulfiller.Unreservable)
                CurrentFulfiller.User = this;
                AddToLog("Found object for fulfilling my " + CurrentNeed + " need - " + CurrentFulfiller.name + "!");
                GoToTarget(CurrentFulfiller.transform);
                break;
            }
        if (CurrentFulfiller == null)
        {
            AddToLog("Didnt find object for fulfilling my " + CurrentNeed+"!", -1);
            NeedFulfilled(false);
        }
        
    }
    void GoToTarget(Transform targetTransform)
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.destination = targetTransform.position;
        //AddToLog("Destination set. Going to " + targetTransform.name);
        IsAtTarget = false;
    }
    void DisableNavmesh( bool isDisabled)
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
        if (combatController == null) return true;
        if (combatController.MovementLocked || combatController.RotationLocked)
            return false;
        else
            return true;


    }
}