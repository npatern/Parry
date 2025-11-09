using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmmiter : PowerReciver
{
    [SerializeField]
    private bool isEmiting = true;
    [SerializeField]
    private float soundRange = 20;
    [SerializeField]
    public ParticleSystem particle;
    protected override void Start()
    {
        base.Start();
        SetEmiting(IsSwitchedOn);
    }
    protected override void OnPowerChanged(bool powered)
    {
        base.OnPowerChanged(powered);
        RefreshEmiting();
    }
    protected override void LateStart()
    {
        base.LateStart();
        RefreshEmiting();
    }
    private void FixedUpdate()
    {

        if (!isEmiting) return;
        Sound sound = new Sound(GetComponent<StatusController>(), soundRange, Sound.TYPES.cover);
        Sounds.MakeSound(sound);
        particle.startLifetime = soundRange / particle.startSpeed;
    }

    public void SetEmiting(bool switchedOn)
    {
        IsSwitchedOn = switchedOn;
        isEmiting = IsSwitchedOn && CheckIfPowered();
        if (isEmiting)
            particle.Play();
        else
            particle.Stop();
    }
    public void SwitchEmiting()
    {
        SetEmiting(!IsSwitchedOn);
    }
    public void RefreshEmiting()
    {
        SetEmiting(IsSwitchedOn);
    }
}