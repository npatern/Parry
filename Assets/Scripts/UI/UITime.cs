using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITime : MonoBehaviour
{
    public void ResetTime()
    {
        LevelController.Instance.ResetTimeSpeed();
    }
    public void StopTime()
    {
        LevelController.Instance.StopTime();
    }
    public void SpeedUpTime()
    {
        LevelController.Instance.SetTimeSpeed(2);
    }
    public void SpeedUpTime(float time)
    {
        LevelController.Instance.SetTimeSpeed(time);
    }
    public void SlowDownTime ()
    {
        LevelController.Instance.SetTimeSpeed(.5f);
    }
}
