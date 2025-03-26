using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageEffects
{
    public float Damage = 10;
    StatusController caster;
    Vector3 origin;
    public Effect[] effects;
    public DamageEffects()
    {
        Damage = 0;
    }
    public DamageEffects(float _damage)
    {
        Damage = _damage;
    }
    public void ApplyEffects(Stats stats, float multiplier = 1)
    {
        foreach (Effect effect in effects)
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
    public Stat.Types target;
}
[System.Serializable]
public class DamageEffectMultiplier
{
    public Stat.Types type;
    public float multiplier = 1;
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
    public StartEffectModifier[] StartModifiers;
    public DamageEffectMultiplier[] DamageModifiers;
    public StatusController status = null;
    public EffectVisuals visuals = null;
   
    //public List<DamageEffect> effects;
    public void ApplyEffect(Effect effect, float multiplier = 1)
    {
        if (effect.type != type) return;
        float _damage = effect.points * multiplier;
        AddPoints(effect.points*multiplier);
         Color color = Color.white;
        if (visuals == null) GetVisuals();
        color = visuals.color;
        UIController.Instance.SpawnDamageNr("" + _damage, status.transform, color, multiplier!=1);
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

        //effects = new List<DamageEffect>();
    }
    public Stat(Types _type)
    {
        type = _type;
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
            OnActiveTick();
      
    }
    public void OnActiveStart()
    {
        isActive = true;
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
        
    }
    public void OnActiveTick()
    {
        if (activeTimer > 0)
        {
            activeTimer -= Time.fixedDeltaTime;
            if (activeTimer <= 0) ResetEffect();
        }
    }
    public void OnActiveEnd()
    {

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

