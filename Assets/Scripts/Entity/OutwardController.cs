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
    public LineRenderer[] linerenderers = new LineRenderer[5];
    
    private void Awake()
    {
        gameController = GameController.Instance;
        if (!AffectedByLight) LightValue = 1;
        SpawnLineRenderers();
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
        float radius = CrowdRadius;
        if (IsHiddenInCrowd) radius += .5f;
        Collider[] hits = Physics.OverlapSphere(transform.position+transform.up, radius, layerToCheck);
        DeactivateLineRenderers();
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<SensesController>(out SensesController _senses))
            {
                if (_senses.Awareness <= 0)
                {
                    //linerenderers[foundCrowd] = DrawLine(transform.position + transform.up, _senses.transform.position + transform.up, Color.white);
                    if (foundCrowd<linerenderers.Length)
                        SetLine(linerenderers[foundCrowd], transform.position + transform.up, _senses.transform.position + transform.up);
                    foundCrowd += 1;
                    
                }
                
            } 
        }
        if (foundCrowd >= minCrowd)
        {
            MakeLinesWhite();
            return true;
        }
        return false;
    }
    void SpawnLineRenderers()
    {
        for (int i = 0; i < linerenderers.Length; i++)
        {
            GameObject go = new GameObject("Line");
            linerenderers[i] = go.AddComponent<LineRenderer>();
        }
    }
    void DeactivateLineRenderers()
    {
        foreach (LineRenderer lr in linerenderers)
        {
            lr.gameObject.SetActive(false);
        }
    }
    void SetLine(LineRenderer lr, Vector3 start, Vector3 end)
    {
        lr.gameObject.SetActive(true);
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = 0.03f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // szary z przezroczystoœci¹
    }
    void MakeLinesWhite()
    {
        foreach (LineRenderer lr in linerenderers)
        {
            lr.startWidth = lr.endWidth = 0.06f;
            lr.startColor = lr.endColor = Color.white;
        }
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
