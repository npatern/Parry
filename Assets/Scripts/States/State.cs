using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
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

    protected float timer = 0;
    protected float time = 10;

    protected float Tick = 0;

    protected float screamDistance = 6;
    protected bool shocked = false;
    protected void ResetTimer(float min = 3, float max = 10)
    {
        timer = 0;
        time = Random.Range(min, max);
    }

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
        entity.Tick = 0.5f;
        stage = EVENT.UPDATE; 
        //Debug.Log(stage + " stage, " + name); 
    }
    public virtual void Exit() 
    { 
        stage = EVENT.EXIT; 
        //Debug.Log(stage + " stage, " + name); 
    }
    public State Process(float _tick)
    {
        Tick = _tick;
        timer += Time.fixedDeltaTime;
        
        entity.SetAvoidanceRadius(0);
        entity.ProcessShockMemory();
        //FLEE
        if (name != STATE.FLEE && sensesController.IsAlerted && sensesController.Awareness > 0 && !entity.IsCombatReady())
        {
            nextState = new Flee(entity);
            stage = EVENT.EXIT;
        }
        //COMBAT
        else if (name!=STATE.COMBAT && sensesController.IsAlerted && sensesController.Awareness > 0 && sensesController.currentTarget!=null && entity.IsCombatReady())
        {
            nextState = new Combat(entity, sensesController.currentTarget);
            stage = EVENT.EXIT;
        }
        
        //SEARCH
        else if (name != STATE.SEARCH && sensesController.IsAlerted && sensesController.Awareness > 0 && sensesController.currentTarget == null && entity.IsCombatReady())
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
        while (entity.TickTimer > Tick)
        {
            if (stage == EVENT.UPDATE) Update();
            entity.TickTimer -= Tick;
        }
        
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
}
[System.Serializable]
public class Idle : State
{
    NeedFulfiller foundFulfiller;
    bool resetOnExit = true;
    public Idle(EntityController _entity) : base(_entity)
    {
        entity.SetAvoidancePriority(Random.Range(80, 90));
        name = STATE.IDLE;
    }
    public override void Enter()
    {
        entity.AddRandomNeeds();
        entity.SetAgentSpeedWalk();
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        entity.SetAgentSpeedWalk();
        if (entity.CurrentNeed == null)
        {
            entity.FindNextNeed();
            return;
            
        }
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
[System.Serializable]
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
        entity.SetAgentSpeedWalk();
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        entity.SetAgentSpeedWalk();
        if (fulfiller.UserSpot != null)
        {
            //entity.transform.position = fulfiller.UserSpot.position;
            //entity.transform.rotation = fulfiller.UserSpot.rotation;
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
            //entity.transform.position = startPosition;
        base.Exit();
    }
}
[System.Serializable]
public class Investigate : State
{
    Vector3 investigatedPosition;
    bool isAtTarget = false;
    float maxTime = 20;
    public Investigate(EntityController _entity, Vector3 _investigatedPosition) : base(_entity)
    {
        name = STATE.INVESTIGATE;
        entity.SetAvoidancePriority(Random.Range(60, 70));
        investigatedPosition = _investigatedPosition;
    }
    public override void Enter()
    {
        sensesController.isInvestigationOpen = true;
        UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onInvestigationStarted), entity.transform);
        entity.SetAgentSpeedWalk();
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        entity.SetAgentSpeedWalk();
        maxTime -= Tick;
        investigatedPosition = sensesController.currentTargetLastPosition;

        entity.GoToTarget(investigatedPosition,2);

        if (entity.IsTargetReached() && !isAtTarget)
        {
            sensesController.isInvestigationOpen = false;
            isAtTarget = true;
            maxTime = 2;
        }

        if (sensesController.Awareness<=0)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onInvestigationEnded), entity.transform);
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
[System.Serializable]
public class Search : State
{
    Vector3 investigatedPosition;
    Vector3 lastSeenPosition;
    bool isAtTarget = false;

    float waitInPlaceTime = 1;
    float waitInPlaceTimer = 1;
    public Search(EntityController _entity, Vector3 _investigatedPosition) : base(_entity)
    {
        name = STATE.SEARCH;
        entity.SetAvoidancePriority(Random.Range(30, 60));
        investigatedPosition = _investigatedPosition;
    }
    public override void Enter()
    {
        
        lastSeenPosition = sensesController.currentTargetLastPosition;
        investigatedPosition = lastSeenPosition;
        if (entity.CanBeShocked())
            shocked = entity.IsEnteringShock();
        /*
        if (investigatedPosition != Vector3.zero)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onSearchStart), entity.transform);
            Sound sound = new Sound(statusController, screamDistance, lastSeenPosition, Sound.TYPES.danger);
            Sounds.MakeSound(sound);
        }
        */
        entity.SetAgentSpeedChase();

        
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (entity.IsProcessingShock())
        {
            entity.DisableNavmesh(true);
            //entity.StartLookingAtTarget(target.transform);
            return;
        }
        
        
        if (timer > time)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onSearchStart), entity.transform);
            //Sound sound = new Sound(statusController, 10, Sound.TYPES.danger);
            //Sounds.MakeSound(sound);
            ResetTimer();
        }
        
        if ( lastSeenPosition != sensesController.currentTargetLastPosition)
        {
            lastSeenPosition = sensesController.currentTargetLastPosition;
            investigatedPosition = lastSeenPosition;
            isAtTarget = false;
        }
        if (shocked)
        {
            entity.DisableNavmesh(false);
            //entity.StopLookingAtTarget();
            if (investigatedPosition != Vector3.zero)
            {
                UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onSearchStart), entity.transform);
                Sound sound = new Sound(statusController, screamDistance, Sound.TYPES.danger);
                Sounds.MakeSound(sound);
            }
            shocked = false;
        }
        entity.SetAgentSpeedChase();
        entity.GoToTarget(investigatedPosition,2);
        if (isAtTarget)
        {
            waitInPlaceTimer -= Tick;
            if (waitInPlaceTimer <= 0)
            {
                Vector3 randomPositionOffset = 2 * new Vector3(Random.Range(-3, 4), 0, Random.Range(-3, 4));

                investigatedPosition += randomPositionOffset;
                isAtTarget = false;
            }
            
        }
        else if (entity.IsTargetReached())
        {
            waitInPlaceTimer = waitInPlaceTime + Random.Range(0f, 1f);
            isAtTarget = true;
        }

            
        
        if (sensesController.Awareness <= 0)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onSearchFail), entity.transform);
            nextState = new Idle(entity);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
[System.Serializable]
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
        
        entity.SetAgentSpeedChase();
        if (entity.CanBeShocked())
            shocked = entity.IsEnteringShock();
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        entity.Tick = 0.1f;

        if (entity.IsProcessingShock())
        {
            entity.DisableNavmesh(true);
            if (target!=null)
            entity.StartLookingAtTarget(target.transform);
            return;
        }
        if (shocked)
        {
            entity.DisableNavmesh(false);
            entity.StopLookingAtTarget();
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.inCombatNotice), entity.transform);
            Sound sound = new Sound(statusController, screamDistance, Sound.TYPES.danger, combatTarget);
            Sounds.MakeSound(sound);
            shocked = false;
        }

        
        entity.SetAgentSpeedChase();
        entity.SetAvoidanceRadius(1.5f);
        combatTarget = sensesController.currentTarget;
        if (combatTarget == null) return;
        if (timer > time)
        {
            /*
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.inCombatNotice), entity.transform);
            Sound sound = new Sound(statusController, 10, Sound.TYPES.danger, combatTarget);
            Sounds.MakeSound(sound);
            ResetTimer();
            */
        }

        entity.agent.avoidancePriority = (int)Vector3.Distance(entity.transform.position, combatTarget.transform.position)/5;
        if (!entity.CanMove()) entity.DisableNavmesh(true);
        else entity.DisableNavmesh(false);
        entity.AttackTarget(combatTarget);
    }
    public override void Exit()
    {
        base.Exit();
    }
}
[System.Serializable]
public class Flee : State
{
    Vector3 investigatedPosition;
    bool isAtTarget = false;

    bool shocked = false;
    public Flee(EntityController _entity) : base(_entity)
    {
        investigatedPosition = _entity.HomePosition;
        name = STATE.FLEE;
    }
    public override void Enter()
    {
        shocked = entity.IsEnteringShock();
        entity.SetAgentSpeedChase();
        entity.SetAvoidancePriority(Random.Range(50, 60));
        


        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (entity.IsProcessingShock()) return;
        if (shocked)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.inFleeEnterNotice), entity.transform);
            Sound sound = new Sound(statusController, screamDistance, Sound.TYPES.danger);
            Sounds.MakeSound(sound);
            shocked = false;
        }

        entity.SetAgentSpeedChase();
        statusController.MakeDeaf(1);
        /*
        if (timer > time)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.inFleeEnterNotice), entity.transform);
            Sound sound = new Sound(statusController, 10, Sound.TYPES.danger);
            Sounds.MakeSound(sound);
            ResetTimer();
        }
        */
        entity.GoToTarget(investigatedPosition);

        if (entity.IsTargetReached() && !isAtTarget)
        {
            //Vector3 randomPositionOffset = 2 * new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
            isAtTarget = true;
            //investigatedPosition += randomPositionOffset;
            //isAtTarget = false;
        }
        if (sensesController.Awareness <= 0)
        {
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onFleeEnded), entity.transform);
            nextState = new Idle(entity);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}