using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageEffects
{
    public float Damage = 10;
    StatusController caster;
    Vector3 origin;
    public List<Effect> effectList;
    public DamageEffects()
    {
        Damage = 0;
        effectList = new List<Effect>();
    }
    public DamageEffects(float _damage)
    {
        Damage = _damage;
        effectList = new List<Effect>();
    }
    public void ApplyEffects(Stats stats, float multiplier = 1)
    {
        foreach (Effect effect in effectList)
        {
            stats.ApplyEffect(effect, multiplier);
        }
    }
}
[System.Serializable]
public class Effect
{
    public Stat.Types type;
    public float points;
    //public StatusController attacker;
    public Effect(float _points, Stat.Types _type)
    {
        points = _points;
        type = _type; 
    }
}
[System.Serializable]
public class Stats
{
    [SerializeField]
    public Stat[] stats;
    public StatusController status;
    public Stats (StatsScriptable _stats, StatusController _status = null)
    {
        stats = new Stat[_stats.stats.Length];
        status = _status;
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = new Stat(_stats.stats[i]);
            stats[i].status = _status;
        }
    }
    public void ApplyEffect(Effect effect, float multiplier = 1)
    {
        foreach (Stat stat in stats)
        {
            stat.ApplyEffect(effect, multiplier );
        }
    }
    public void ApplyTick()
    {
        foreach (Stat stat in stats)
        {
            stat.Tick();
        }
    }
    public Stat GetStat(Stat.Types _type)
    {
        foreach (Stat _stat in stats)
            if (_stat.type == _type) return _stat;

        return null;
    }
    public bool IsStatActive(Stat.Types _type)
    {
        Stat _stat = GetStat(_type);
        if (_stat == null) return false;
        return _stat.IsActive();
    }
    public void MakeActive(Stat.Types _type)
    {
        Stat _stat = GetStat(_type);
        if (_stat == null) return;
        _stat.ApplyFullEffect();
    }
}
[System.Serializable]
public class StartEffectModifier
{
    public Stat.Types type;
    public enum StartEffectTypes { KILL, FULLTARGET, EMPTYTARGET, EMPTYME}
    public StartEffectTypes startEffectType;
    public Stat.Types targetType;
    public void ApplyStartEffect(StatusController _status)
    {
        Stat _typeToCheck = _status.stats.GetStat(type);
        if (_typeToCheck == null) return;
        if (!_typeToCheck.IsActive()) return;
        Stat _target = _status.stats.GetStat(targetType);
        switch (startEffectType)
        {
            case StartEffectTypes.KILL:
                _status.Kill();
                break;
            case StartEffectTypes.FULLTARGET:
                if (_target != null) _target.ApplyFullEffect();
                break;
            case StartEffectTypes.EMPTYTARGET:
                if (_target != null) _target.ResetEffect();
                break;
            case StartEffectTypes.EMPTYME:
                if (_target != null) _typeToCheck.ResetEffect();
                break;
        }
    }
}
[System.Serializable]
public class DamageEffectMultiplier
{
    public Stat.Types typeThatIsActive;
    public float multiplier = 1;
    public float GetMultiplayer(Stats _stats)
    {
        if (_stats.IsStatActive(typeThatIsActive)) return multiplier;
        else return 1;
    }
}
[System.Serializable]
public class Stat
{
    public enum Types { BLEEDING, FIRE, ELECTRIC, FREEZE, STUN, DEAF, POISON, WATER }
    public string name;
    public Types type;
    public bool isActive = false;
    public bool canBeAppliedIfActive = false;
    public float minPoints = 0;
    public float maxPoints = 100;
    public float Points = 100;
    public float defaultSpeed = 0;
    public float defalutPoints = 0;
    public float activeTimer = 0;
    public float activeTime = 10;
    public float waitTimer = 0;
    public float waitTime = 1;
    [Tooltip("how many points per second are added to nearby targets")]
    public float contagousSpeed = 0;
    public StartEffectModifier[] StartModifiers;
    public DamageEffectMultiplier[] DamageModifiers;
    public StatusController status = null;
    public EffectVisuals visuals = null;

    float tickTimer = 0;
    float tickTime = 1;

    private ParticleSystem particleInstance = null;
    //public List<DamageEffect> effects;
    
    public Stat()
    {
        isActive = false;
        canBeAppliedIfActive = false;
        minPoints = 0;
        maxPoints = 100;
        Points = 100;
        defaultSpeed = 0;
        defalutPoints = 0;
        activeTimer = 0;
        activeTime = 10;
        status = null;
        waitTimer = 0;
        waitTime = 1;
        //DamageModifiers = new DamageEffectMultiplier[0];
        particleInstance = null;
        tickTimer = 0;
        tickTime = 1;
        contagousSpeed = 0;
        //effects = new List<DamageEffect>();
    }
    public Stat(Stat cloned)
    {
        name = cloned.name;
        type = cloned.type;
        isActive = cloned.isActive;
        canBeAppliedIfActive = cloned.canBeAppliedIfActive;
        minPoints = cloned.minPoints;
        maxPoints = cloned.maxPoints;
        Points = cloned.Points;
        defaultSpeed = cloned.defaultSpeed;
        defalutPoints = cloned.defalutPoints;
        activeTimer = cloned.activeTimer;
        activeTime = cloned.activeTime;
        status = cloned.status;
        StartModifiers = cloned.StartModifiers;
        DamageModifiers = cloned.DamageModifiers;
        particleInstance = null;
        tickTimer = cloned.tickTimer;
        tickTime = cloned.tickTime;
        contagousSpeed = cloned.contagousSpeed;
        //effects = new List<DamageEffect>();
    }
    public Stat(Types _type)
    {
        type = _type;
    }
    public float GetMultiplierFromModifiers()
    {
        float _multiplier = 1;
        foreach (DamageEffectMultiplier damageEffect in DamageModifiers)
            _multiplier *= damageEffect.GetMultiplayer(status.stats);

        return _multiplier;
    }
    public void ApplyEffect(Effect effect, float multiplier = 1)
    {
        if (effect.type != type) return;
        multiplier *= GetMultiplierFromModifiers();
        float _damage = effect.points * multiplier;
        if (_damage == 0) return;
        if (visuals == null) GetVisuals();
        //status.SpawnParticles(visuals.particles, status.bodyTransform, 2, .1f );
        AddPoints(effect.points * multiplier);
        Color color = Color.white;

        color = visuals.color;
        //UIController.Instance.SpawnDamageNr("" + _damage, status.transform, color, multiplier > 1);
        RefreshEffect();

    }
    void GetVisuals()
    {
        foreach (EffectVisuals _visuals in GameController.Instance.ListOfAssets.EffectVisuals)
        {
            if (type == _visuals.type)
            {
                visuals = _visuals;
            }
        }
    }
    public bool IsActive()
    {
        return isActive;
    }
    public float GetValue()
    {
        return (Points - minPoints) / (maxPoints - minPoints);
    }
    public float GetTimerValue()
    {
        return activeTimer / activeTime;
    }
    public float AddPoints(float _points, bool stopDefaulting = true)
    {   

        Points += _points;
        if (stopDefaulting)
            waitTimer = waitTime;
        return _points;
    }
    public void ApplyFullEffect(float _timer = 0)
    {
        //AddPoints(maxPoints);
        Points = maxPoints;
        if (_timer == 0) _timer = activeTime;
        if (_timer > activeTimer) activeTimer = _timer;
        if (visuals == null) GetVisuals();
        if (particleInstance==null)
            particleInstance = status.SpawnParticles(visuals.particles, status.bodyTransform,-1,-1);
        if (!isActive) OnActiveStart();
    }
    public bool IsWaiting()
    {
        if (waitTimer > 0) return true;
        else return false;
    }
    public void Tick()
    {
        RefreshEffect();
        
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0) waitTimer = 0;
        }
        if (!isActive && !IsWaiting())  
            MakeDefault();

        if (isActive)
            OnActiveUpdate();
      
    }
    public void OnActiveStart()
    {
        isActive = true;
        foreach (StartEffectModifier startEffect in StartModifiers)
        {
            startEffect.ApplyStartEffect(status);
        }

        if (status == null) return;
        //if (status.IsKilled) return;
        switch (type)
        {
            case Types.STUN:
                status.StartStunEvent.Invoke();
                break;
            case Types.FREEZE:
                status.StartFreezeEvent.Invoke();
                break;
        }

        /*
        switch (type)
        {
            case Types.ELECTRIC:
                status.stats.GetStat(Types.STUN).ApplyFullEffect();
                if (status.stats.IsStatActive(Stat.Types.WATER)) status.Kill();
                if (status.stats.IsStatActive(Stat.Types.FREEZE)) ResetEffect();
                break;
            case Types.FIRE:
                if (status.stats.IsStatActive(Stat.Types.WATER)) status.stats.GetStat(Stat.Types.WATER).ResetEffect();
                if (status.stats.IsStatActive(Stat.Types.FREEZE)) status.stats.GetStat(Stat.Types.FREEZE).ResetEffect();
                break;
            case Types.WATER:
                if (status.stats.IsStatActive(Stat.Types.FIRE)) status.stats.GetStat(Stat.Types.FIRE).ResetEffect();
                if (status.stats.IsStatActive(Stat.Types.ELECTRIC)) status.Kill();
                break;
            case Types.FREEZE:
                if (status.stats.IsStatActive(Stat.Types.FIRE)) status.stats.GetStat(Stat.Types.FIRE).ResetEffect();
                if (status.stats.IsStatActive(Stat.Types.WATER)) status.stats.GetStat(Stat.Types.WATER).ResetEffect();
                break;
            case Types.STUN:
                if (status.stats.IsStatActive(Stat.Types.FREEZE)) status.Kill();
                break;
        }
        */

    }
    public void OnActiveUpdate()
    {
        tickTimer += Time.fixedDeltaTime;
        if (tickTimer > tickTime)
        {
            OnActiveTick();
            tickTimer -= tickTime;
        }
        if (activeTimer > 0)
        {
            activeTimer -= Time.fixedDeltaTime;
            if (activeTimer <= 0) ResetEffect();
        }
    }
    public void OnActiveTick()
    {
        float multiplier = GetMultiplierFromModifiers();
        switch (type)
        {
            case Types.FIRE:
                status.TakeDamage(10, visuals.activeColor,multiplier,status);
                break;
            case Types.POISON:
                status.TakeDamage(1, visuals.activeColor, multiplier, status);
                break;
            case Types.BLEEDING:
                status.TakeDamage(5, visuals.activeColor, multiplier, status);
                break;

        }
    }
    public void OnActiveEnd()
    {
        if (particleInstance != null)
        {
            particleInstance.Stop();
            GameObject.Destroy(particleInstance.gameObject, 10);
            particleInstance = null;
        }
           
        if (status == null) return;
        switch (type)
        {
            case Types.STUN:
                status.EndStunEvent.Invoke();
                break;
            case Types.FREEZE:
                status.EndFreezeEvent.Invoke();                     
                break;
        }
    }
    public void MakeDefault()
    {
        if (Points > defalutPoints) 
        { 
            Points -= defaultSpeed * Time.fixedDeltaTime;
            if (Points < defalutPoints)
                Points = defalutPoints;
        }
        if (Points < defalutPoints) 
        {
            Points += defaultSpeed * Time.fixedDeltaTime;
            if (Points > defalutPoints)
                Points = defalutPoints;
        } 
    }
    public void RefreshEffect()
    {

        if (!isActive && Points >= maxPoints || canBeAppliedIfActive && Points > maxPoints)
        {
            ApplyFullEffect();
        }
    }
    public void ResetEffect()
    {
        activeTimer = 0;
        OnActiveEnd();
        isActive = false;
        Points = defalutPoints;
    }
}

