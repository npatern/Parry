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
    public StatsScriptable statsSource;
    [SerializeField]
    public Stats stats;
    public bool IsPlayer = false;

    public float CriticalMultiplier = 4;
    public float SpeedMultiplier = 1;
    public float Life = 100;
    public Stat HealthStat;
    public Stat StunStat;

    public float MaxLife = 100;
    public float Posture = 100;
    public float movementSpeedMultiplier = 1;
    public bool IsKilled = false;
    public bool IsStunnedOBSOLETE = false;
    public bool DestroyOnKill = false;

    public UnityEvent IsAttackedEvent;
    public UnityEvent IsStunnedEvent;
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

    private float postureRegenerationDelayTime = 1f;
    private float postureDelayTimer = 5f;
    private float postureRegenerationSpeed = 5f;
    private float postureStunnedRegenerationTime = 5f;
    private float stunnedTimer = 0f;

    private float deafTimer = 0f;
    public float deafTime = 1f;

    public UIOverheadStatus OverheadController;
    void Awake()
    {
        if (Life > MaxLife) Life = MaxLife;
        if (GetComponent<ToolsController>() != null)
            toolsController = GetComponent<ToolsController>();
        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        if (IsStunnedEvent == null)
            IsStunnedEvent = new UnityEvent();
        if (GetComponent<InputController>() != null)
            IsPlayer = true;
        if (OnKillEvent == null)
            OnKillEvent = new UnityEvent();
        if (GetComponent<SensesController>() != null)
            sensesController = GetComponent<SensesController>();
        if (statsSource != null) stats = new Stats(statsSource, this);
    }
    void Start()
    {
        if (UIController.Instance == null) return;
        OverheadController = UIController.Instance.SpawnHealthBar(this).GetComponent<UIOverheadStatus>() ;
        IsStunnedEvent.AddListener(Stunned);
        IsAttackedEvent.AddListener(Attacked);
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
    }
    
    private void FixedUpdate()
    {
        stats.ApplyTick();
        deafTimer -= Time.fixedDeltaTime;
        
        if (loadValuesInRealTime) LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
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
    void LoadStatsFromScriptable(EntityStatsScriptableObject scriptable)
    {
        postureRegenerationDelayTime = scriptable.PostureRegenerationDelayTime;
        postureRegenerationSpeed = scriptable.PostureRegenerationSpeed;
        postureStunnedRegenerationTime = scriptable.PostureStunnedRegenerationTime;
        if (PostureEffect == null) PostureEffect = scriptable.PostureEffect;
        if (PostureLongEffect == null) PostureLongEffect = scriptable.PostureLongEffect;
    }
    public void Stunned()
    {
        IsStunnedOBSOLETE = true;
        stunnedTimer = postureStunnedRegenerationTime;

        SpawnParticles(PostureLongEffect, transform, 10, postureStunnedRegenerationTime);
    }
    public void Attacked()
    {
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
        
        if (toolsController == null)
        {
            TakeDamage(damage, multiplier, attacker);
            return true;
        }
        if (toolsController.IsDodging) return false;

        bool isFromBullet = true;
        if (attacker!=null) isFromBullet= attacker.GetComponent<Bullet>();

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
        
        TakeDamage(damage, multiplier, attacker, isFromBullet);
        return true;
    }
    public float GetStunndedTimerValue()
    {
        return stunnedTimer / postureStunnedRegenerationTime;
    }
    public void TakeDamage(DamageEffects damage,float multiplier=1, StatusController attacker = null, bool isFromBullet = false)
    {
        float _damage = damage.Damage;
        bool isCritical = false;
        isCritical = MultiplyDamage(attacker);
         
        
        if (isCritical)
            if (attacker != null) multiplier *= attacker.CriticalMultiplier;
           
   
       
        damage.ApplyEffects(stats, multiplier);
        

        if (_damage * multiplier <= 0)
        {
            if (damage.effects.Length > 0 && sensesController != null)
                sensesController.AddAwarenessOnce(35 );

            return;
        }
            

        Life -= _damage * multiplier;
        SpawnParticles(DamageEffect, transform);

        UIController.Instance.SpawnDamageNr("" + _damage* multiplier, transform,Color.red, isCritical);
        if (Life <= 0 && !IsKilled) 
        {
            Kill(attacker);
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
                    sensesController.SetCurrentTarget( attacker);
                Sound sound = new Sound(this, 10, Sound.TYPES.neutral);
                Sounds.MakeSound(sound);
            }
        }
    }
    bool MultiplyDamage(StatusController attacker)
    {
        
        if (IsPlayer) return false;
        if (IsStunnedOBSOLETE) return true;
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
    public bool IsDeaf()
    {
        return deafTimer > 0;
    }
    public float GetDeafTimerValue()
    {
        return deafTimer/deafTime;
    }
    public void MakeDeaf(float _deafTimer=0)
    {
        if (_deafTimer == 0) _deafTimer = deafTime;
        if (_deafTimer > deafTimer) deafTimer = _deafTimer;
    }
    public void ReactToSound(Sound sound)
    {
        if (sound.type == Sound.TYPES.cover) MakeDeaf();

    }
    public void Kill()
    {
        Kill(null, 0);
    }
    public void Kill(StatusController attacker=null, float damage = 10)
    {
        IsKilled = true;
        
        SpawnParticles(DamageEffect, transform, 10, .2f);
        Life = 0;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
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
