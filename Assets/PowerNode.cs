using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PowerNode : PowerReciver
{
    public IPowerFlowController[] powerFlowControllers;
    public event Action<bool> PowerChangedEvent;
    public bool IsOn = true;
    public bool PowerComingOut = false;

    float timer = 1;
    bool refreshed = false;
    protected override void Awake()
    {
        base.Awake();
        powerFlowControllers = GetComponents<MonoBehaviour>().OfType<IPowerFlowController>().ToArray();
    }
    protected override void OnPowerChanged(bool powered)
    {
        base.OnPowerChanged(powered);
        RefreshState();
    }
    protected override void Start()
    {
        if (powerSource != null)
            powerSource.PowerChangedEvent += OnPowerChanged;
        foreach (var powerFlowController in powerFlowControllers)
        {
            //if (powerFlowController.RefreshPowerNode!=null)
               powerFlowController.RefreshPowerNode += RefreshState;
        }
        OnPowerChanged(CheckIfPowered());
        StartCoroutine(DelayedRefresh());
    }
    protected override void LateStart()
    {
        base.LateStart();
        Debug.Log("Power Node Late Start " +name);
        RefreshState();

    }
    public void RefreshState()
    {
        bool _powerOutBefore = PowerComingOut;
        PowerComingOut = IsPowerFlowing();
        Debug.Log(name+" power is flowing in ");
        //if (_powerOutBefore != PowerComingOut)
        PowerChangedEvent?.Invoke(PowerComingOut);
    }
    bool IsPowerFlowing()
    {
        if (!CheckIfPowered()) return false;
        if (!IsSwitchedOn) return false;
        foreach (var powerController in powerFlowControllers)
            if (powerController == null || powerController.IsPowerFlowing()== false) return false;
        return true;
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (PowerComingOut)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.up + transform.right,.5f);
    }
}

public interface IPowerFlowController
{
    public bool IsPowerFlowing();
    public event Action RefreshPowerNode;
    
}
