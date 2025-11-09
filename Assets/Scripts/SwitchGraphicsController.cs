using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGraphicsController : MonoBehaviour
{
    public InteractableSwitch interactable;
    [SerializeField]
    GameObject onGraphics;
    [SerializeField]
    GameObject offGraphics;
    [SerializeField]
    GameObject[] NonInteractableGraphics;
    bool isOn = true;
    public void Awake()
    {
        if (interactable == null) interactable = GetComponent<InteractableSwitch>();
        if (interactable == null) Destroy(this);
        interactable.SwitchOnEvent.AddListener(SwitchStateOn);
        interactable.SwitchOffEvent.AddListener(SwitchStateOff);
    }
    public void SwitchState()
    {
        SwitchState(!isOn); 
    }
    public void SwitchState(bool state)
    {
        isOn = state;
        onGraphics.SetActive(isOn);
        offGraphics.SetActive(!isOn);
    }
    public void SwitchStateOn( )
    {
        SwitchState(true);
    }
    public void SwitchStateOff( )
    {
        SwitchState(false);
    }
}
