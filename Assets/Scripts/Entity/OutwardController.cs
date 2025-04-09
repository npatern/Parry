using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutwardController : MonoBehaviour
{
    public bool AffectedByLight = false;
    public float LightValue = 0;
    private GameController gameController;
    public float CrowdDetectionTickTime = .2f;
    float CrowdTick = 0;
    public bool IsHiddenInCrowd = false;
    public float CrowdRadius = 2.5f;
    public GameObject[] linerenderers = new GameObject[3];
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
        CrowdTick += Time.fixedDeltaTime;
        if (CrowdTick >= CrowdDetectionTickTime)
        {
            IsHiddenInCrowd = CheckIfHiddenInCrowd();
            CrowdTick = 0;
        }
    }
    private void OnDrawGizmos()
    {
        if (IsHiddenInCrowd) Gizmos.DrawWireSphere(transform.position, CrowdRadius);
    }
    bool CheckIfHiddenInCrowd()
    {
        int minCrowd = 3;
        int foundCrowd = 0;
        LayerMask layerToCheck = LayerMask.GetMask("Entity");
        Collider[] hits = Physics.OverlapSphere(transform.position+transform.up, CrowdRadius, layerToCheck);
        Destroy(linerenderers[0]);
        Destroy(linerenderers[1]);
        Destroy(linerenderers[2]);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<SensesController>(out SensesController _senses))
            {
                if (_senses.Awareness <= 0)
                {
                    linerenderers[foundCrowd] = DrawLine(transform.position + transform.up, _senses.transform.position + transform.up, Color.white);
                    foundCrowd += 1;
                    
                }
                if (foundCrowd >= minCrowd) return true;
            } 
        }
        Destroy(linerenderers[0]);
        Destroy(linerenderers[1]);
        Destroy(linerenderers[2]);
        return false;
    }
    GameObject DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject go = new GameObject("Line");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        return go;
    }
    float GetLightValue(Transform target)
    {
        float value = 0;
        //hidden in crowd shouldnt maybe work like that - but for now it does, cos icon of light shows how visible you are.
        //in the future might be problematic - cos you still can be seen from certain distance
        if (IsHiddenInCrowd) return value;
        foreach (LightController light in gameController.lightControllers)
            if (light.IsInLight(target))
                value += light.GetLightValueOnObject(target);

        value = Mathf.Clamp01(value);
        return value;
    }
    public bool IsActionIllegal()
    {
        if (TryGetComponent<ToolsController>(out ToolsController toolsController))
        {
            if (toolsController.IsIllegal) return true;
            if (toolsController.CurrentWeaponWrapper != null && toolsController.CurrentWeaponWrapper.IsIllegal) return true;
                
        }
        return false;
    }
}
