using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField]
    GameObject onGraphics;
    [SerializeField]
    GameObject offGraphics;
    bool isOn = true;
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
}
