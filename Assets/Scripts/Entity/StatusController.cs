using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class StatusController : MonoBehaviour, IHear
{
    public string Team = "";
    public Vector3 size = Vector3.one;
    [SerializeField]
    private bool loadValuesInRealTime = false;
    [SerializeField]
    public Stats stats;
    public bool IsPlayer = false;

    public float CriticalMultiplier = 4;
    public float SlowmoSpeedMultiplier = 1;
    public float Life = 100;
    public float MaxLife = 100;
    public float movementSpeedMultiplier = 1;
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

    private Rigidbody rb;
    private ToolsController toolsController;
    private SensesController sensesController;

    private float deafTimer = 0f;
    public float deafTime = 1f;
    /*
    [Space(10), Header("PowerSource")]

    public bool NeedsOutsideSource = false;
    public StatusController powerSource = null;
    public UnityEvent PowerOffEvent;
    public UnityEvent PowerOnEvent;
    public bool HasPower = false;
    public bool PowerSwitch = true;
    public bool IsPowerFlowing()
    {
        if (PowerSwitch == false) return false;
        if (IsStunned()) return false;
        if (IsKilled) return false;
        if (NeedsOutsideSource)
        {
            if (powerSource = null) return false;
            if (powerSource != this) if (powerSource.HasPower == false) return false;
        }
        return true;
    }
    public void RefreshPowerFlow()
    {
        bool _hadPower = HasPower;
        bool _hasPower = IsPowerFlowing();

        HasPower = _hasPower;
        if (HasPower != _hadPower)
        {
            if (HasPower)
                PowerOnEvent.Invoke();
            else
                PowerOffEvent.Invoke();
        }
    }
    public void onPowerAwake()
    {
        if (PowerOffEvent == null)
            PowerOffEvent = new UnityEvent();
        if (PowerOffEvent == null)
            PowerOnEvent = new UnityEvent();
        if (!NeedsOutsideSource) return;
    }
    public void onPowerStart()
    {
        if (!NeedsOutsideSource) return;
        if (powerSource == this) return;
        powerSource.PowerOffEvent.AddListener(RefreshPowerFlow);
        powerSource.PowerOnEvent.AddListener(RefreshPowerFlow);
        RefreshPowerFlow();
    }

    */

    public UIOverheadStatus OverheadController;
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
        stats = new Stats(GameController.Instance.ListOfAssets.DefaultEffectStats, this);
        if (headTransform == null) headTransform = transform;
        if (bodyTransform == null) bodyTransform = transform;
        //onPowerAwake();
    }
    void Start()
    {
        if (UIController.Instance == null)
        {
            Debug.LogError("Didnt find UICONTROLLER while starting " + gameObject.name);
            return;
        } 
        OverheadController = UIController.Instance.SpawnHealthBar(this).GetComponent<UIOverheadStatus>() ;
        StartStunEvent.AddListener(StunnedStart);
        StartStunEvent.AddListener(StunnedEnd);
        IsAttackedEvent.AddListener(Attacked);
        StartFreezeEvent.AddListener(FrozenStart);
        EndFreezeEvent.AddListener(FrozenEnd);
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityValues);

        //onPowerStart();
    }
    
    private void FixedUpdate()
    {
        stats.ApplyTick();
        deafTimer -= Time.fixedDeltaTime;
        
        if (loadValuesInRealTime) LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityValues);
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
        //RefreshPowerFlow();
    }
    public void StunnedEnd()
    {
        //RefreshPowerFlow();
    }
    public void Attacked()
    {
    }
    void FrozenStart()
    {
        attackSpeedMultiplier = 0f;
        movementSpeedMultiplier = 0f;
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
        if (attacker != null) pos = attacker.transform.position;
        return TryTakeDamage(damage, pos, multiplier,  attacker);
    }
    public bool TryTakeDamage(DamageEffects damage, Vector3 damageSource, float multiplier = 1, StatusController attacker=null)
    {
        if (IsKilled) return true;
        bool isFromBullet = true;
        if (attacker != null) isFromBullet = attacker.GetComponent<Bullet>();
        if (toolsController == null)
        {
            TakeDamageEffect(damage, multiplier, attacker, isFromBullet);
            return true;
        }
        if (toolsController.IsDodging) return false;

        

        if (sensesController != null)
            if (isFromBullet ) sensesController.currentTargetLastPosition = damageSource;//&& sensesController.IsAlerted

        if (toolsController.IsDisarming)
        {
            
            Pickable toEquip = attacker.toolsController.DropWeaponFromHands();
            if (toEquip != null)
            {
                UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onParry), transform);
                if (IsPlayer) GameController.Instance.IncreaseSlowmoTimer();
                toolsController.EquipWeaponFromPickable(toEquip);
                //attacker.TakePosture(damage.Damage * CriticalMultiplier, attacker);
                attacker.IsAttackedEvent.Invoke();
                
                return false;
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
                attacker.TakePosture(damage.Damage * CriticalMultiplier*multiplier, attacker);
      
                
                attacker.IsAttackedEvent.Invoke();
            }
            UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onParry), transform);
            
            if (IsPlayer) GameController.Instance.IncreaseSlowmoTimer();
            return false;
        }

        if (toolsController.IsProtected)
        {
            TakePosture(damage.Damage, attacker);
            attacker.TakePosture(damage.Damage, this);
            return true;
        }
        
        TakeDamageEffect(damage, multiplier, attacker, isFromBullet);
        return true;
    }
     
    public void TakeDamageEffect(DamageEffects damageEffect,float multiplier=1, StatusController attacker = null, bool isFromBullet = false)
    {
        float _damage = damageEffect.Damage;
        bool isCritical = false;
        isCritical = MultiplyDamage(attacker);
         
        
        if (isCritical)
            if (attacker != null) multiplier *= attacker.CriticalMultiplier;
           
   
       
        damageEffect.ApplyEffects(stats, multiplier);
        

        if (_damage * multiplier <= 0)
        {
            if (damageEffect.effects.Length > 0 && sensesController != null)
                sensesController.AddAwarenessOnce(35 );

            return;
        }

        TakeDamage(_damage,Color.red, multiplier, attacker, isFromBullet, isCritical);
    }
    public void TakeDamage(float _damage, Color color, float multiplier = 1, StatusController attacker = null, bool isFromBullet = false, bool isCritical = false )
    {

        Life -= _damage * multiplier;
        SpawnParticles(DamageEffect, transform);

        UIController.Instance.SpawnDamageNr("" + _damage * multiplier, transform, color, isCritical);
        if (Life <= 0 && !IsKilled)
        {
            KillSelf(attacker);
            Sound sound = new Sound(this, 2, Sound.TYPES.danger);
            Sounds.MakeSound(sound);
        }
        else
        {
            if (sensesController != null)
            {
                UIController.Instance.SpawnTextBubble(Barks.GetBark(Barks.BarkTypes.onPain), transform);

                sensesController.Awareness = 100;
                if (!isFromBullet)
                    sensesController.SetCurrentTarget(attacker);
                Sound sound = new Sound(this, 10, Sound.TYPES.neutral);
                Sounds.MakeSound(sound);
            }
        }
    }
    bool MultiplyDamage(StatusController attacker)
    {
        
        if (IsPlayer) return false;
        if (IsStunned()) return true;
        if (GetComponent<EntityController>() != null)
            if (!GetComponent<EntityController>().IsCombatReady())
                return true;

        if (sensesController != null) if(!sensesController.IsAlerted) return true;
        return false;
    }
    public void  ApplyEffect(float damage, Stat.Types type) 
    {
        Effect effect = new Effect(damage, type);
        ApplyEffect(effect);
    }
    public void ApplyEffect(Effect effect)
    {
        stats.ApplyEffect(effect);
    }
    public bool TakePosture(float damage, StatusController attacker)
    {
        if (IsStunned()) return false;
        ApplyEffect(damage, Stat.Types.STUN);
        
        
        /*
        if (IsStunnedOBSOLETE) return false;
        postureDelayTimer = postureRegenerationDelayTime;
        if(this != attacker)
            SpawnParticles(PostureEffect, transform);
        Posture -= damage;
        if (Posture <= 0)
        {
            Posture = 0;
            IsStunnedEvent.Invoke();
        }
        */
        return true;
    }
    
    public ParticleSystem SpawnParticles(ParticleSystem particles, Transform position,float destroyLength = 10, float newLifetime = -1)
    {
        if (particles == null) return null;
        ParticleSystem newParticles = Instantiate(particles.gameObject, position).GetComponent<ParticleSystem>();
         
        if (DestroyOnKill) newParticles.transform.parent = null;
        if (newLifetime>=0) 
        {
            newParticles.Stop();
            var main = newParticles.main;
            main.duration = newLifetime;
            newParticles.Play();
        }
        if (scaleParticles)
            newParticles.transform.localScale = position.localScale;
        if (destroyLength>=0)
        Destroy(newParticles.gameObject, destroyLength);
        return newParticles;
    }

    public void Kill()
    {
        KillSelf(null, 0);
    }
    public void KillSelf(StatusController attacker=null, float damage = 10)
    {
        IsKilled = true;
        //RefreshPowerFlow();
        SpawnParticles(DamageEffect, transform, 10, .2f);
        Life = 0;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }
        
        if (attacker != null)
        {
            Vector3 direction = (transform.position - attacker.transform.position).normalized;
            rb.AddForce(direction * damage, ForceMode.VelocityChange);
        }
        
        GetComponent<Collider>().isTrigger = false;
        if (toolsController != null)
        if (toolsController.CurrentWeaponWrapper!=null)
        {
            toolsController.DequipWeaponFromHands();
            toolsController.StopAllCoroutines();
            if (toolsController.inputController != null) toolsController.inputController.enabled = false;
            toolsController.enabled = false;
        }
        
        if (TryGetComponent<EntityController>(out EntityController entityController))
        {
            GameController.Instance.RemoveFromListOfEntities(entityController);
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
            GetComponent<SensesController>().enabled = false;
        }
            

        if (IsPlayer) GameController.Instance.EndGame();
        OnKillEvent.Invoke();
        if (DestroyOnKill)
         Destroy(gameObject,.1f);
    }
}
