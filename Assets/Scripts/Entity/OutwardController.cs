using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutwardController : MonoBehaviour
{
    public bool AffectedByLight = false;
    public float LightValue = 0;
    private GameController gameController;
    private void Awake()
    {
        gameController = GameController.Instance;
        if (!AffectedByLight) LightValue = 1;
    }
    private void FixedUpdate()
    {
        if (AffectedByLight)
            LightValue = GetLightValue(transform);
        else
            LightValue = 1;
    }
    float GetLightValue(Transform target)
    {
        float value = 0;
        foreach (LightController light in gameController.lightControllers)
            if (light.IsInLight(target))
                value += light.GetLightValueOnObject(target);

        value = Mathf.Clamp01(value);
        return value;
    }
}
