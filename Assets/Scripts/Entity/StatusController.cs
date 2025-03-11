using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class StatusController : MonoBehaviour, IHear
{
    
    [SerializeField]
    private bool loadValuesInRealTime = false;

    public bool IsPlayer = false;

    public float CriticalMultiplier = 4;
    public float SpeedMultiplier = 1;
    public float Life = 100;
    public float Posture = 100;

    public bool IsKilled = false;
    public bool IsStunned = false;

    public UnityEvent IsAttackedEvent;
    public UnityEvent IsStunnedEvent;
    public UnityEvent OnKillEvent;

    [SerializeField]
    private ParticleSystem DamageEffect;
    [SerializeField]
    private ParticleSystem ParryEffect;
    [SerializeField]
    private ParticleSystem PostureEffect;
    [SerializeField]
    private ParticleSystem PostureLongEffect;

    private Rigidbody rb;
    private CombatController combatController;
    private SensesController sensesController;

    private float postureRegenerationDelayTime = 1f;
    private float postureDelayTimer = 5f;
    private float postureRegenerationSpeed = 5f;
    private float postureStunnedRegenerationTime = 5f;
    private float stunnedTimer = 0f;

    private float deafTimer = 0f;
    public float deafTime = 1f;

    private GameObject healthBar;
    // Start is called before the first frame update
    void Awake()
    {
        if (GetComponent<CombatController>() != null)
            combatController = GetComponent<CombatController>();
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
    }
    void Start()
    {
        if (UIController.Instance == null) return;
        healthBar = UIController.Instance.SpawnHealthBar(this);
        IsStunnedEvent.AddListener(Stunned);
        IsAttackedEvent.AddListener(Attacked);
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
    }
    private void FixedUpdate()
    {
        deafTimer -= Time.fixedDeltaTime;
        if (IsStunned)
        {
            
            stunnedTimer -= Time.fixedDeltaTime;
            Posture = Mathf.Lerp(0, 100, 1-(stunnedTimer/ postureStunnedRegenerationTime));
            if (stunnedTimer <= 0)
            {
                IsStunned = false;
                Posture = 100;
            }
        }
        else if (Posture<100)
        {
            if (postureDelayTimer > 0)
                postureDelayTimer -= Time.fixedDeltaTime;
            else
                Posture += postureRegenerationSpeed * Time.fixedDeltaTime;
        }
        if (loadValuesInRealTime) LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
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
        IsStunned = true;
        stunnedTimer = postureStunnedRegenerationTime;

        SpawnParticles(PostureLongEffect, transform, 10,postureStunnedRegenerationTime);
    }
    public void Attacked()
    {
    }
    public bool TryTakeDamage(StatusController attacker, float damage = 10)
    {
        if (IsKilled) return true;
        
        if (combatController == null)
        {
            TakeDamage(damage, attacker);
            return true;
        }
        if (combatController.IsDodging) return false;
        bool isFromBullet = attacker.GetComponent<Bullet>();
        if (combatController.IsParrying)
        {
            SpawnParticles(ParryEffect, combatController.CurrentWeapon);
            if (isFromBullet)
            {

            }
            else
            {
                attacker.TakePosture(damage * CriticalMultiplier, attacker);
                attacker.IsAttackedEvent.Invoke();
            }
            UIController.Instance.SpawnTextBubble("Haha!", transform);
            if (IsPlayer) GameController.Instance.IncreaseSlowmoTimer();
            return false;
        }
        if (combatController.IsProtected)
        {
            
            TakePosture( damage, attacker);
            return true;
        }
        
        TakeDamage(damage, attacker);
        return true;
    }
    public void TakeDamage(float damage, StatusController attacker)
    {
        
        UIController.Instance.SpawnTextBubble("Ouch!", transform);
        SpawnParticles(DamageEffect, transform);
        if (MultiplyDamage(attacker)) damage *= attacker.CriticalMultiplier;
        Life -= damage;
        if (Life <= 0 && !IsKilled) Kill(attacker);
    }
    bool MultiplyDamage(StatusController attacker)
    {
        
        if (IsPlayer) return false;
        if (IsStunned) return true;
        if (sensesController != null) if(!sensesController.isAware) return true;
        return false;
    }
    public bool TakePosture(float damage, StatusController attacker)
    {
        if (IsStunned) return false;
        postureDelayTimer = postureRegenerationDelayTime;
        if(this != attacker)
            SpawnParticles(PostureEffect, transform);
        Posture -= damage;
        if (Posture <= 0)
        {
            Posture = 0;
            IsStunnedEvent.Invoke();
        }
        return true;
    }
    void SpawnParticles(ParticleSystem particles, Transform position,float destroyLength = 10, float newLifetime = -1)
    {
        if (particles == null) return;
        ParticleSystem newParticles = Instantiate(particles.gameObject, position).GetComponent<ParticleSystem>();
        if (newLifetime>=0) 
        {
            newParticles.Stop();
            var main = newParticles.main;
            main.duration = newLifetime;
            newParticles.Play();
        }
        
        Destroy(newParticles.gameObject, destroyLength);
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
    public void Kill(StatusController attacker, float damage = 10)
    {
        IsKilled = true;
        
        SpawnParticles(DamageEffect, transform);
        Life = 0;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        if (attacker != null)
        {
            Vector3 direction = (transform.position - attacker.transform.position).normalized;
            rb.AddForce(direction * damage, ForceMode.VelocityChange);
        }
        
        GetComponent<Collider>().isTrigger = false;
        if (combatController != null)
        {
            combatController.CurrentWeapon.GetComponent<Collider>().enabled = true;
            combatController.CurrentWeapon.GetComponent<Collider>().isTrigger = false;
            combatController.CurrentWeapon.GetComponent<Rigidbody>().isKinematic = false;
            combatController.StopAllCoroutines();
            if (combatController.inputController != null) combatController.inputController.enabled = false;
            combatController.enabled = false;
        }
        
        if (GetComponent<EntityController>() != null)
        {
            GameController.Instance.RemoveFromListOfEntities(GetComponent<EntityController>());
            GetComponent<EntityController>().StopAllCoroutines();
            GetComponent<EntityController>().enabled = false;
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
        //Destroy(gameObject,10);
    }

}
