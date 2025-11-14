using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITime : MonoBehaviour
{
    public void ResetTime()
    {
        GameplayController.Instance.ResetTimeSpeed();
    }
    public void StopTime()
    {
        GameplayController.Instance.StopTime();
    }
    public void SpeedUpTime()
    {
        GameplayController.Instance.SetTimeSpeed(2);
    }
    public void SpeedUpTime(float time)
    {
        GameplayController.Instance.SetTimeSpeed(time);
    }
    public void SlowDownTime ()
    {
        GameplayController.Instance.SetTimeSpeed(.5f);
    }
}
