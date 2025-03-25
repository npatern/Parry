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
}
[System.Serializable]
public class Stat
{
    public enum Types { BLEEDING, FIRE, ELECTRIC, FREEZE, STUN, DEAF, POISON }

    
    public string name;
    public Types type;
    bool isActive = false;
    public float minPoints = 0;
    public float maxPoints = 100;
    public float Points = 100;
    public float defaultSpeed = 0;
    public float defalutPoints = 0;
    public float timer = 0;
    public float time = 10;
    public StatusController status = null;
    public EffectVisuals visuals = null;
    //public List<DamageEffect> effects;
    public void ApplyEffect(Effect effect, float multiplier = 1)
    {
        if (effect.type != type) return;
        float _damage = effect.points * multiplier;
        Points += effect.points*multiplier;
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
        minPoints = 0;
        maxPoints = 100;
        Points = 100;
        defaultSpeed = 0;
        defalutPoints = 0;
        timer = 0;
        time = 10;
        status = null;

        //effects = new List<DamageEffect>();
    }
    public Stat(Stat cloned)
    {
        name = cloned.name;
        type = cloned.type;
        isActive = cloned.isActive;
        minPoints = cloned.minPoints;
        maxPoints = cloned.maxPoints;
        Points = cloned.Points;
        defaultSpeed = cloned.defaultSpeed;
        defalutPoints = cloned.defalutPoints;
        timer = cloned.timer;
        time = cloned.time;
        status = cloned.status;

        //effects = new List<DamageEffect>();
    }
    public Stat(Types _type)
    {
        type = _type;
    }
    public bool IsActive()
    {
        return timer > 0;
    }
    public float GetValue()
    {
        return (Points - minPoints) / (maxPoints - minPoints);
    }
    public float GetTimerValue()
    {
        return timer / time;
    }
    public void ApplyFullEffect(float _timer = 0)
    {
        Points = maxPoints;
        if (_timer == 0) _timer = time;
        if (_timer > timer) timer = _timer;
        isActive = true;
    }
    public void Tick()
    {
        RefreshEffect();
        if (timer > 0) 
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0) ResetEffect();
        }

        if (!isActive)
            MakeDefault();
    }
    public void MakeDefault()
    {
        if (Points > defalutPoints) defalutPoints -= defaultSpeed * Time.fixedDeltaTime;
        if (Points < defalutPoints) defalutPoints += defaultSpeed * Time.fixedDeltaTime;
    }
    public void RefreshEffect()
    {
        if (!isActive && Points >= maxPoints)
        {
            ApplyFullEffect();
        }
    }
    public void ResetEffect()
    {
        timer = 0;
        isActive = false;
        Points = defalutPoints;
    }
}

