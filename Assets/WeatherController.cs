using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField]
    private Light SunLight;

    private float dayLengthInHours = 24;
    
    private float dayAsValue = 0;
    
    [SerializeField]
    private float timeOfDay = 0;
    [SerializeField]
    private float hourLength = 1;
    [SerializeField]
    private float sunMaxStrength = 2;
    [SerializeField]
    private AnimationCurve sunlightCurve;
   
    [SerializeField]
    private float sunZeroHourOffsetValue = .25f;

    void Update()
    {
        UpdateTimeOfDay();
    }
    void UpdateSun()
    {
        //SunLight.transform.rotation = Quaternion.Euler(  360 * dayAsValue- 360* sunZeroHourOffsetValue, SunLight.transform.rotation.eulerAngles.y, SunLight.transform.rotation.eulerAngles.z);
        //SunLight.intensity = sunlightCurve.Evaluate(dayAsValue) * sunMaxStrength;
    }
   
    void UpdateTimeOfDay()
    {
        if (hourLength != 0)
        {
            timeOfDay += Time.deltaTime / hourLength;
        }
        if (timeOfDay > dayLengthInHours*hourLength) timeOfDay -= dayLengthInHours * hourLength;
        dayAsValue = timeOfDay / dayLengthInHours;
        UpdateSun();
    }
    public float GetTimeOfDay()
    {
        return timeOfDay;
    }
    public float GetTimeOfDayValue()
    {
        return dayAsValue;
    }
}
