using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE
    {
        IDLE, USE, INVESTIGATE, SEARCH, COMBAT, FLEE
    }
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }
    public STATE name;
    protected EVENT stage;
    protected Transform target;
    protected State nextState;

    protected EntityController entity;
    protected StatusController statusController;
    protected SensesController sensesController;

    public State(EntityController _entityController)
    {
        entity = _entityController;
        statusController = entity.gameObject.GetComponent<StatusController>();
        sensesController = entity.GetComponent<SensesController>();
        stage = EVENT.ENTER;
        //agent = entity.GetComponent<NavMeshAgent>();
    }
    public virtual void Enter() 
    { 
        stage = EVENT.UPDATE; 
        //Debug.Log(stage + " stage, " + name); 
        if (nextState == null) 
            nextState = new Idle(entity); 
    }
    public virtual void Update() 
    { 
        stage = EVENT.UPDATE; 
        //Debug.Log(stage + " stage, " + name); 
    }
    public virtual void Exit() 
    { 
        stage = EVENT.EXIT; 
        //Debug.Log(stage + " stage, " + name); 
    }

    public State Process()
    {
        //COMBAT
        if (name!=STATE.COMBAT && sensesController.IsAlerted && sensesController.Awareness > 0 && sensesController.currentTarget!=null)
        {
            nextState = new Combat(entity, sensesController.currentTarget);
            stage = EVENT.EXIT;
        }
        
        //SEARCH
        else if (name != STATE.SEARCH && sensesController.IsAlerted && sensesController.Awareness > 0 && sensesController.currentTarget == null)
        {
            nextState = new Search(entity, sensesController.currentTargetLastPosition);
            stage = EVENT.EXIT;
        }
        
        //INVESTIGATE
        else if (name != STATE.INVESTIGATE && !sensesController.IsAlerted && sensesController.Awareness > 20)
        {
            nextState = new Investigate(entity, sensesController.currentTargetLastPosition);
            stage = EVENT.EXIT;
        }
        
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
}
public class Idle : State
{
    NeedFulfiller foundFulfiller;
    bool resetOnExit = true;
    public Idle(EntityController _entity) : base(_entity)
    {
        name = STATE.IDLE;
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log("Is updating");
        //if (entity.isSearchingForNeeds) return;
        if (entity.CurrentNeed == null)
        {
            entity.FindNextNeed();
            return;
            
        }
        //Debug.Log("found need");
        if (entity.CurrentFulfiller == null)
        {
            entity.FindFulfiller();
            return;
        }
        else
        {
            entity.SetAvoidancePriority((int)Vector3.Distance(entity.transform.position, entity.CurrentFulfiller.transform.position));
        }
        
        if (entity.IsTargetReached() && !entity.IsAtTarget)
        {
            entity.IsAtTarget = true;
            entity.AddToLog("Arrived to " + entity.CurrentFulfiller.name + ". Starting fulfilling the need of " + entity.CurrentNeed.Name);
            foundFulfiller = entity.CurrentFulfiller;
            nextState = new UseObject(entity, foundFulfiller);
            resetOnExit = false;
            stage = EVENT.EXIT;
        }
       
        
    }
    public override void Exit()
    {
        if (resetOnExit)
        {
            entity.ResetFulfiller();
            entity.CurrentNeed = null;
        }
        

        base.Exit();
    }
}
public class UseObject : State
{
    NeedFulfiller fulfiller;
    Coroutine currentCoroutine;
    Vector3 startPosition;
    
    public UseObject(EntityController _entity, NeedFulfiller _fulfiller) : base(_entity)
    {
        name = STATE.USE;
        fulfiller = _fulfiller;
        startPosition = _entity.transform.position;
    }
    public override void Enter()
    {
        currentCoroutine = entity.StartCoroutine(fulfiller.ExecuteSteps(entity));
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (fulfiller.UserSpot != null)
        {
            entity.transform.position = fulfiller.UserSpot.position;
            entity.transform.rotation = fulfiller.UserSpot.rotation;
        }
        
        if (entity.CurrentFulfiller == null)
        {
            nextState = new Idle(entity);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        
            entity.ResetFulfiller();
            entity.CurrentNeed = null;
         
        
        entity.transform.position = startPosition;
        base.Exit();
    }
}
public class Investigate : State
{
    Vector3 investigatedPosition;
    bool isAtTarget = false;
    float maxTime = 20;
    public Investigate(EntityController _entity, Vector3 _investigatedPosition) : base(_entity)
    {
        name = STATE.INVESTIGATE;

        investigatedPosition = _investigatedPosition;
    }
    public override void Enter()
    {
        sensesController.isInvestigationOpen = true;
        UIController.Instance.SpawnTextBubble("What was that?", entity.transform);
        
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        maxTime -= Time.deltaTime;
        investigatedPosition = sensesController.currentTargetLastPosition;

        entity.GoToTarget(investigatedPosition);

        if (entity.IsTargetReached() && !isAtTarget)
        {
            sensesController.isInvestigationOpen = false;
            isAtTarget = true;
            maxTime = 2;
        }

        if (sensesController.Awareness<=0)
        {
            UIController.Instance.SpawnTextBubble("Mus've been the wind...", entity.transform);
            nextState = new Idle(entity);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        sensesController.isInvestigationOpen = false;
        base.Exit();
    }
}
public class Search : State
{
    Vector3 investigatedPosition;
    bool isAtTarget = false;
    float timer = 10;
    public Search(EntityController _entity, Vector3 _investigatedPosition) : base(_entity)
    {
        name = STATE.SEARCH;

        investigatedPosition = _investigatedPosition;
    }
    public override void Enter()
    {
        UIController.Instance.SpawnTextBubble("Where are they?", entity.transform);
        investigatedPosition = sensesController.currentTargetLastPosition;
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        

        entity.GoToTarget(investigatedPosition);

        if (entity.IsTargetReached() && !isAtTarget)
        {
            Vector3 randomPositionOffset = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
            isAtTarget = true;
            investigatedPosition += randomPositionOffset;
        }
        if (sensesController.Awareness <= 0)
        {
            UIController.Instance.SpawnTextBubble("They're gone by now...", entity.transform);
            nextState = new Idle(entity);
            stage = EVENT.EXIT;
        }

    }
    public override void Exit()
    {

        base.Exit();
    }
}
public class Combat : State
{
    StatusController combatTarget;
    Vector3 investigatedPosition;
    bool isAtTarget = false;
    public Combat(EntityController _entity, StatusController _combatTarget) : base(_entity)
    {
        name = STATE.COMBAT;

        combatTarget = _combatTarget;
    }
    public override void Enter()
    {
        UIController.Instance.SpawnTextBubble("They're here!", entity.transform);
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        combatTarget = sensesController.currentTarget;
        if (combatTarget == null) return;

 
        entity.agent.avoidancePriority = (int)Vector3.Distance(entity.transform.position, combatTarget.transform.position);
        if (!entity.CanMove()) entity.DisableNavmesh(true);
        else entity.DisableNavmesh(false);
        entity.AttackTarget(combatTarget);
    }
    public override void Exit()
    {

        base.Exit();
    }
}