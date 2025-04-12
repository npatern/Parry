using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InteractableSwitch : Interactable, IPowerFlowController
{
    public bool SignalState = true;
    public UnityEvent SwitchOnEvent;
    public UnityEvent SwitchOffEvent;
    public event Action RefreshPowerNode;

    protected override void Awake()
    {
        base.Awake();
        if (SwitchOnEvent == null)
            SwitchOnEvent = new UnityEvent();
        if (SwitchOffEvent == null)
            SwitchOffEvent = new UnityEvent();
    }
    private void Start()
    {
        Interact(SignalState);
    }
    public override void Interact(StatusController _status = null)
    {
        Interact(!SignalState, _status);
    }
    public void Interact(bool _setSignal, StatusController _status = null)
    {
        
        SwitchSignalToState(_setSignal, _status);
        base.Interact(_status);
    }
    public void SwitchOn()
    {
        Interact(true);
    }
    public void SwitchOff()
    {
        Interact(false);
    }
    public bool IsPowerFlowing()
    {
        return SignalState;
    }
    private void SwitchSignalToState(bool _setSignal, StatusController _status = null)
    {
       //commented cos i want to make sure everything is set properly on awake
        //if (SignalState == _setSignal) return;
        SignalState = _setSignal;
        RefreshPowerNode?.Invoke();
        if (SignalState) SwitchOnEvent.Invoke();
        if (!SignalState) SwitchOffEvent.Invoke();

    }

}
