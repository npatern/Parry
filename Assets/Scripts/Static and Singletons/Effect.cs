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
    public Stats (StatsScriptable _stats)
    {
        stats = new Stat[_stats.stats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = _stats.stats[i];
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
}
[System.Serializable]
public class Stat
{
    public string name;
    public enum Types {BLEEDING, FIRE, ELECTRIC, FREEZE, STUN, DEAF, POISON }
    public Types type;
    bool isActive = false;
    public float minPoints = 0;
    public float maxPoints = 100;
    public float Points = 100;
    public float defaultSpeed = 0;
    public float defalutPoints = 0;
    public float timer = 0;
    public float time = 10;
    //public List<DamageEffect> effects;
    public void ApplyEffect(Effect effect, float multiplier = 1)
    {
        if (effect.type != type) return;
        Points += effect.points*multiplier;
        RefreshEffect();

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

