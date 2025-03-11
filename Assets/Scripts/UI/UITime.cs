using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITime : MonoBehaviour
{
    public void ResetTime()
    {
        GameController.Instance.ResetTimeSpeed();
    }
    public void StopTime()
    {
        GameController.Instance.StopTime();
    }
    public void SpeedUpTime()
    {
        GameController.Instance.SetTimeSpeed(2);
    }
    public void SpeedUpTime(float time)
    {
        GameController.Instance.SetTimeSpeed(time);
    }
    public void SlowDownTime ()
    {
        GameController.Instance.SetTimeSpeed(.5f);
    }
}
