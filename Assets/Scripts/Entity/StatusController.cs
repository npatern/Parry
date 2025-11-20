using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

//Main script of almost every dynamic object in the game - from NPCs to breakable rocks. Handles damage and other effects, like fire, electricity etc.
public class StatusController : MonoBehaviour, IHear, IPowerFlowController
{
    [Tooltip("Immortal can't die by any means")]
    public bool Immortal = false;
    [Tooltip("Impervious can't recieve damage, but can be killed by script.")]
    public bool Impervious = false;
    [Tooltip("ID used to avoid friendly fire in some cases")]//change to other kind of value for faster comparing
    public string Team = "";
    [Tooltip("Approximate size - used to scale some animations and define with witch object the entity can interact with.")]//consider moving to other controller
    public Vector3 size = Vector3.one;
    
    public DamageEffects defaultDamageEffects;
    public Stats stats;
    [SerializeField]
    public BoxCollider damageEffectsRangeBoxCollider;
    [SerializeField]
    private StatsArchetypeScriptable statsArchetype;
    public bool IsPlayer = false;
    
    public float CriticalMultiplier = 4;
    public float SlowmoSpeedMultiplier = 1;
    public float Life = 100;
    public float MaxLife = 100;

    public float movementSpeedMultiplier = 1;
    public float movementSpeedMultiplierStun = 1;
    public float attackSpeedMultiplier = 1;

    public bool IsKilled = false;
    public bool IsStunnedOBSOLETE = false;
    public bool DestroyOnKill = false;

    public UnityEvent IsAttackedEvent;

    public UnityEvent StartStunEvent;
    public UnityEvent EndStunEvent;
    public UnityEvent StartFreezeEvent;
    public UnityEvent EndFreezeEvent;

    public UnityEvent OnKillEvent;

    public Transform headTransform;
    public Transform bodyTransform;

    [SerializeField]
    private bool scaleParticles = false;
    [SerializeField]
    private ParticleSystem DamageEffect;
    [SerializeField]
    private ParticleSystem ParryEffect;
    [SerializeField]
    private ParticleSystem PostureEffect;
    [SerializeField]
    private ParticleSystem PostureLongEffect;

    [SerializeField]
    [Tooltip("Debug - set to true if you want to mess with default values in real time")]
    private bool loadValuesInRealTime = false;

    private Rigidbody rb;
    private ToolsController toolsController;
    private SensesController sensesController;

    public DamageEffects effects;
    public float TickTime = 0.5f;
    float TickTimer = 0;
    public event Action RefreshPowerNode;

    public UIOverheadStatus OverheadController;
    public bool UseBoxParticles()
    {
        if (damageEffectsRangeBoxCollider == null) return false;
        else return true;
    }
    void Awake()
    {
        if (Life > MaxLife) Life = MaxLife;
        if (GetComponent<ToolsController>() != null)
            toolsController = GetComponent<ToolsController>();
        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        if (StartStunEvent == null)
            StartStunEvent = new UnityEvent();
        if (StartFreezeEvent == null)
            StartFreezeEvent = new UnityEvent();
        if (EndStunEvent == null)
            EndStunEvent = new UnityEvent();
        if (EndFreezeEvent == null)
            EndFreezeEvent = new UnityEvent();
        if (GetComponent<InputController>() != null)
            IsPlayer = true;
        if (OnKillEvent == null)
            OnKillEvent = new UnityEvent();
        if (GetComponent<SensesController>() != null)
            sensesController = GetComponent<SensesController>();
        //
        
        //
        if (headTransform == null) headTransform = transform;
        if (bodyTransform == null) bodyTransform = transform;
        //onPowerAwake();
    }

    void Start()
    {
        if (statsArchetype == null)
            stats = new Stats(ResourcesManager.Instance.ListOfAssets.DefaultEffectStats, this);
        else
            stats = new Stats(ResourcesManager.Instance.ListOfAssets.DefaultEffectStats, statsArchetype, this);
        if (UIController.Instance == null)
        {
            Debug.LogError("Didnt find UICONTROLLER while starting " + gameObject.name);
            return;
        }
        OverheadController = UIController.Instance.SpawnHealthBar(this).GetComponent<UIOverheadStatus>();
        StartStunEvent.AddListener(StunnedStart);
        EndStunEvent.AddListener(StunnedEnd);
        IsAttackedEvent.AddListener(Attacked);
        StartFreezeEvent.AddListener(FrozenStart);
        EndFreezeEvent.AddListener(FrozenEnd);
        LoadStatsFromScriptable(ResourcesManager.Instance.ListOfAssets.DefaultEntityValues);
        TickTimer = TickTime;
        //onPowerStart();
    }
    
    private void FixedUpdate()
    {
        stats.ApplyTick();

        if (loadValuesInRealTime) LoadStatsFromScriptable(ResourcesManager.Instance.ListOfAssets.DefaultEntityValues);

        ApplyTick();
    }
    void ApplyTick()
    {
        TickTimer += Time.fixedDeltaTime;
        if (TickTimer >= TickTime)
        {
            TickTimer -= TickTime;
            Tick();
        }
    }
    void Tick()
    {
        //reacting to rain - temporary disabled
        /*
        LayerMask mask = LayerMask.GetMask("Weather", "Blockout");
        RaycastHit hit;
        
        if (Physics.Raycast(headTransform.position, transform.up,out hit, 50, mask))
        {
            if (hit.collider.TryGetComponent<Weather>(out Weather weather))
                TakeDamageWithEffects(weather.effects);
        }
        */
            DamageEffects _damageEffects = new DamageEffects();
        foreach (Stat _stat in stats.stats)
        {
            if (_stat.isActive && _stat.contagousSpeed != 0)
            {
                _damageEffects.effectList.Add(new Effect(_stat.contagousSpeed * TickTime, _stat.type));
                if (_stat.emptyOnContangion) _stat.ResetEffect();
            }
                
        }
        if (damageEffectsRangeBoxCollider != null)
        {
            Damages.SendDamage(new DamageField(damageEffectsRangeBoxCollider, _damageEffects, this));
        }
        else
        {
            Damages.SendDamage(new DamageField(2, bodyTransform.position, _damageEffects, this));
        }
        
    }
    bool CanBeStunned()
    {
        if (stats.GetStat(Stat.Types.STUN) != null) return true;
        else return false;
    }
    public bool IsStunned()
    {
        if (CanBeStunned())
            return stats.GetStat(Stat.Types.STUN).IsActive();

        return false;
    }
    public bool IsPowerFlowing()
    {
        if (IsKilled || IsStunned()) return false;
        return true;
    }
    public bool CanBeDeafend()
    {
        if (stats.GetStat(Stat.Types.DEAF) != null) return true;
        else return false;
    }
    public bool IsDeaf()
    {
        if (CanBeDeafend())
            return stats.GetStat(Stat.Types.DEAF).IsActive();

        return false;
    }
    public void MakeDeaf(float _deafTimer = 0)
    {
        if (!CanBeDeafend()) return;
        stats.GetStat(Stat.Types.DEAF).ApplyFullEffect(_deafTimer);
    }
    public void ReactToSound(Sound sound)
    {
        if (sound.type == Sound.TYPES.cover) MakeDeaf();
    }
    void LoadStatsFromScriptable(EntityValuesScriptableObject scriptable)
    {
        if (PostureEffect == null) PostureEffect = scriptable.PostureEffect;
        if (PostureLongEffect == null) PostureLongEffect = scriptable.PostureLongEffect;
    }
    public void StunnedStart()
    {
        if (IsPlayer)
        {
            movementSpeedMultiplierStun = .5f;
        }
        else
        {
            movementSpeedMultiplierStun = 0f;
        }
        CheckPowerFlow();
    }
    public void StunnedEnd()
    {
        //Debug.Log("STUNNED END");
        if (IsPlayer)
        {
            movementSpeedMultiplierStun = 1f;
        }
        else
        {
            movementSpeedMultiplierStun = 1f;
        }
        CheckPowerFlow();
    }
    public void Attacked()
    {
    }
    public void CheckPowerFlow()
    {
        RefreshPowerNode?.Invoke();
    }
    void FrozenStart()
    {
        if (IsPlayer)
        {
            attackSpeedMultiplier = .2f;
            movementSpeedMultiplier = .2f;
        }
        else
        {
            attackSpeedMultiplier = 0f;
            movementSpeedMultiplier = 0f;
        }

    }
    void FrozenEnd()
    {
        attackSpeedMultiplier = 1f;
        movementSpeedMultiplier = 1f;
        if (toolsController != null) toolsController.PerformIdle();
    }

    public bool TryTakeDamage(DamageEffects damage, float multiplier = 1, StatusController attacker = null)
    {
        Vector3 pos = Vector3.zero;
        if (damage == null)
        {
            Debug.LogError("Damage is null!");
        }
        if (attacker != null)
        {
            pos = attacker.transform.position;
            return TryTakeDamage(damage, pos, multiplier, attacker);
        }
        return TryTakeDamage(damage, pos, multiplier);
    }
    public bool TryTakeDamage(DamageEffects damage, Vector3 damageSource, float multiplier = 1, StatusController attacker = null)
    {
        if (IsKilled) return true;
        bool isFromBullet = true;
        if (attacker != null) isFromBullet = attacker.GetComponent<Bullet>();
        if (toolsController == null)
        {
            TakeDamageWithEffects(damage, multiplier, attacker, isFromBullet);
            return true;
        }
        if (toolsController.IsDodging) return false;



        if (sensesController != null)
            if (isFromBullet) sensesController.currentTargetLastPosition = damageSource;//&& sensesController.IsAlerted

        if (toolsController.IsDisarming)
        {
            if (attacker != null)
            {
                Pickable toEquip = attacker.toolsController.DropWeaponFromHands();
                if (toEquip != null)
                {
                    UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onParry), transform);
                    if (IsPlayer) LevelController.Instance.IncreaseSlowmoTimer();
                    toolsController.EquipWeaponFromPickable(toEquip);
                    //attacker.TakePosture(damage.Damage * CriticalMultiplier, attacker);
                    attacker.IsAttackedEvent.Invoke();

                    return false;
                }
            }

        }

        if (toolsController.IsParrying)
        {
            SpawnParticles(ParryEffect, toolsController.CurrentWeaponWrapper.CurrentWeaponObject);
            if (isFromBullet)
            {

            }
            else
            {
                //attacker.TakePostureAndEffects(damage,  CriticalMultiplier * multiplier, attacker);
                attacker.TakePostureOnly(damage.Damage, CriticalMultiplier * multiplier, attacker);
                attacker.IsAttackedEvent.Invoke();
            }
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onParry), transform);

            if (IsPlayer) LevelController.Instance.IncreaseSlowmoTimer();
            return false;
        }

        if (toolsController.IsProtected)
        {
            TakePostureOnly(damage.Damage,  multiplier, attacker);
            attacker.TakePostureOnly(damage.Damage,1,  this);
            return true;
        }

        TakeDamageWithEffects(damage, multiplier, attacker, isFromBullet);
        return true;
    }
    public void TakeDamageWithEffects(DamageEffects damageEffect, float multiplier = 1, StatusController attacker = null, bool isFromBullet = false)
    {
        bool isCritical = false;
        if (attacker != null)
            if (CheckIfIsCritical())
            {
                multiplier *= attacker.CriticalMultiplier;
                isCritical = true;
            }
                
        float _damage = damageEffect.Damage;
        

        ApplyOnlyEffects(damageEffect, multiplier);


        if (_damage * multiplier <= 0 && attacker!=this)
        {
            //if (damageEffect.effectList.Count > 0 && sensesController != null)
            //    sensesController.AddAwarenessOnce(35);

            return;
        }

        TakeDamage(_damage, Color.red, multiplier, attacker, isFromBullet, isCritical);
    }
    public void ApplyOnlyEffects(DamageEffects damageEffect, float multiplier = 1)
    {
        damageEffect.ApplyEffects(stats, multiplier);
    }
    public void TakeDamage(float _damage, Color color, float multiplier = 1, StatusController attacker = null, bool isFromBullet = false, bool isCritical = false)
    {
        if (Impervious) return;
        if (Immortal) return;

        //Push(attacker,_damage);
        Life -= _damage * multiplier;
        SpawnParticles(DamageEffect, transform);

        UIController.Instance.SpawnDamageNr("" + _damage * multiplier, transform, color, isCritical);
        if (Life <= 0 && !IsKilled)
        {
            Kill(attacker);
            //Sound sound = new Sound(this, 2, Sound.TYPES.danger);
            //Sounds.MakeSound(sound);
        }
        else
        {
            if (TryGetComponent<EntityController>(out EntityController _entity))
            {
                _entity.AddVelocity(-transform.forward * 10);
            }
            if (sensesController != null)
            {
                if (attacker != this)
                {
                    sensesController.Awareness = 100;
                    if (!isFromBullet)
                        sensesController.SetCurrentTarget(attacker);

                    UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onPain), transform);
                    Sound sound = new Sound(this, 5, Sound.TYPES.neutral);
                    Sounds.MakeSound(sound);
                }
            }
        }
    }
    void Push(StatusController attacker, float pushForce = 10)
    {
        if (GetComponent<EntityController>() != false)
        {
            GetComponent<EntityController>().agent.velocity += pushForce * 10 * (transform.position - attacker.transform.position);
        }
    }
    bool CheckIfIsCritical(StatusController attacker)
    {
        return CheckIfIsCritical();
    }
    bool CheckIfIsCritical()
    {
        if (IsPlayer) return false;
        if (IsStunned()) return true;
        if (GetComponent<EntityController>() != null)
        {
            if (!GetComponent<EntityController>().IsCombatReady())
                return true;
            if (GetComponent<EntityController>().currentState.IsCurrentlyShocked())
                return true;
        }
            
        if (sensesController != null) if (!sensesController.IsAlerted) return true;
        return false;
    }
    public void ApplyEffect(float damage, Stat.Types type)
    {
        Effect effect = new Effect(damage, type);
        ApplyEffect(effect);
    }
    public void ApplyEffect(Effect effect)
    {
        stats.ApplyEffect(effect);
    }
    public bool TakePostureAndEffects(DamageEffects damageEffect,float multiplier, StatusController attacker)
    {
        ApplyOnlyEffects(damageEffect, multiplier);
        TakePostureOnly(damageEffect.Damage, multiplier, attacker);
        return true;
    }
    public bool TakePostureOnly(float damage, float multiplier, StatusController attacker)
    {
        if (IsStunned()) return false;
        ApplyEffect(damage*multiplier, Stat.Types.STUN);
        return true;
    }

    public ParticleSystem SpawnParticles(ParticleSystem particles, Transform position, float destroyLength = 10, float newLifetime = -1)
    {
        if (particles == null) return null;
        ParticleSystem newParticles = Instantiate(particles.gameObject, position).GetComponent<ParticleSystem>();

        if (DestroyOnKill) newParticles.transform.parent = null;
        if (newLifetime >= 0)
        {
            newParticles.Stop();
            var main = newParticles.main;
            main.duration = newLifetime;
            newParticles.Play();
        }
        if (scaleParticles)
        {
            newParticles.transform.localScale = position.localScale;
            float newVolume = newParticles.transform.lossyScale.x * newParticles.transform.lossyScale.y * newParticles.transform.lossyScale.z;

            float particleRatio = newParticles.emissionRate * newVolume;
            newParticles.emissionRate = particleRatio;
        }

        if (destroyLength >= 0)
            Destroy(newParticles.gameObject, destroyLength);
        return newParticles;
    }
    
    public void Kill()
    {
        Kill(null, 0);
    }
    public void Kill(StatusController attacker = null, float damage = 10)
    {
        if (Immortal) return; 
        IsKilled = true;
        CheckPowerFlow();
        SpawnParticles(DamageEffect, transform, 10, .2f);
        Life = 0;
        if (defaultDamageEffects.deathEffectObjectToSpawn != null) Instantiate(defaultDamageEffects.deathEffectObjectToSpawn, transform.position, Quaternion.identity);
        if (IsStunned()) stats.GetStat(Stat.Types.STUN).ResetEffect();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            Vector3 direction =  transform.forward;
            //direction = Vector3.zero;
            if (attacker != null) direction = (transform.position - attacker.transform.position).normalized;
            rb.AddForce(Vector3.up * 2 + direction * 10, ForceMode.VelocityChange);
        }
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.isTrigger = false;
        }
        if (stats != null && DestroyOnKill)
        {
            stats.EmptyStats();// check for bugs
        }
        if (toolsController != null)
            if (toolsController.CurrentWeaponWrapper != null)
            {
                toolsController.DropWeaponFromHands();
                toolsController.StopAllCoroutines();
                if (toolsController.inputController != null) toolsController.inputController.enabled = false;
                toolsController.enabled = false;
            }

        if (TryGetComponent<EntityController>(out EntityController entityController))
        {
            LevelController.Instance.RemoveFromListOfEntities(entityController);
            entityController.ResetFulfiller();
            entityController.StopAllCoroutines();
            entityController.enabled = false;
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onDeath), transform);
        }

        if (GetComponent<NavMeshAgent>() != null)
            GetComponent<NavMeshAgent>().enabled = false;

        if (GetComponent<PlayerInput>() != null)
            GetComponent<PlayerInput>().enabled = false;

        if (GetComponent<InputController>() != null)
            GetComponent<InputController>().enabled = false;
        if (GetComponent<SensesController>() != null)
        {
            GetComponent<SensesController>().Kill();
           // GetComponent<SensesController>().enabled = false;
            
        }


        if (IsPlayer) LevelController.Instance.EndGame();
        OnKillEvent.Invoke();
        if (DestroyOnKill)
        {
            Debug.Log("Destroying!!!!!! "+name);
            Destroy(gameObject);
            Debug.Log("Destroyed!!!!!! " + name);
        }
           
    }
}
