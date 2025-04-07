using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InteractableSwitch : Interactable
{
    public bool SignalState = true;
    public UnityEvent SwitchOnEvent;
    public UnityEvent SwitchOffEvent;
   

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
        Debug.Log("INTERACTION FROM SWITCH");
        Interact(!SignalState, _status);
    }
    public void Interact(bool _setSignal, StatusController _status = null)
    {
        base.Interact(_status);
        SwitchSignalToState(_setSignal, _status);
    }
    public void SwitchOn()
    {
        Interact(true);
    }
    public void SwitchOff()
    {
        Interact(false);
    }
    private void SwitchSignalToState(bool _setSignal, StatusController _status = null)
    {
        Debug.Log("ARE WE GONNA SWITCH SIGNAL?");
        //if (SignalState == _setSignal) return;
        Debug.Log("WE GONNA SWITCH SIGNAL");
        SignalState = _setSignal;
        if (SignalState) SwitchOnEvent.Invoke();
        if (!SignalState) SwitchOffEvent.Invoke();
        Debug.Log("SIGNAL SET TO "+SignalState);

    }

}
