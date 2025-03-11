using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE
    {
        IDLE, USE, INVESTIGATE, PURSUE, COMBAT
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
    private StatusController statusController;
    private SensesController sensesController;

    public State(EntityController _entityController)
    {
        entity = _entityController;
        statusController = entity.gameObject.GetComponent<StatusController>();
        sensesController = entity.GetComponent<SensesController>();
        stage = EVENT.ENTER;
        //agent = entity.GetComponent<NavMeshAgent>();
    }
    public virtual void Enter() { stage = EVENT.UPDATE; Debug.Log(stage + " stage, " + name); }
    public virtual void Update() { stage = EVENT.UPDATE; Debug.Log(stage + " stage, " + name); }
    public virtual void Exit() { stage = EVENT.EXIT; Debug.Log(stage + " stage, " + name); }

    public State Process()
    {
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
        Debug.Log("Is updating");
        //if (entity.isSearchingForNeeds) return;
        if (entity.CurrentNeed == null)
        {
            entity.FindNextNeed();
            return;
            
        }
        Debug.Log("found need");
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
            nextState = new UseObject(entity, entity.CurrentFulfiller);
            stage = EVENT.EXIT;
        }
       
        
    }
    public override void Exit()
    {
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
        entity.StopCoroutine(currentCoroutine);
        entity.transform.position = startPosition;
        base.Exit();
    }
}
