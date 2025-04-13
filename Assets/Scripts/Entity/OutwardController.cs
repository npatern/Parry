using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OutwardController : MonoBehaviour
{
    public bool AffectedByLight = false;
    public bool AffectedByCrowd = false;
    public float LightValue = 0;
    private GameController gameController;
    public float CrowdDetectionTickTime = .2f;
    float CrowdTick = 0;
    public bool IsHiddenInCrowd = false;
    public float CrowdRadius = 2.5f;
    public LineRenderer[] linerenderers = new LineRenderer[5];

    public LayerMask zoneMask;
    public List<Zone> Zones;
    public ZoneScriptable activeZone;
    public DisguiseScriptable disguise;
    public GameObject headGear;
    public GameObject torsoGear;
    private bool IsPlayer;
    private void Awake()
    {
        zoneMask = LayerMask.GetMask("Zones");
        gameController = GameController.Instance;
        if (!AffectedByLight) LightValue = 1;
        if (AffectedByCrowd)
            SpawnLineRenderers();
    }
    private void FixedUpdate()
    {
        if (AffectedByLight)
            LightValue = GetLightValue(transform);
        else
            LightValue = 1;
        if (AffectedByCrowd)
        {
            CrowdTick += Time.fixedDeltaTime;
            if (CrowdTick >= CrowdDetectionTickTime)
            {
                IsHiddenInCrowd = CheckIfHiddenInCrowd();
                CrowdTick = 0;
            }
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
    public int HowMuchActionIllegal()
    {
        int level = 0;
        if (TryGetComponent<ToolsController>(out ToolsController toolsController))
        {
            if (toolsController.CurrentWeaponWrapper != null && toolsController.CurrentWeaponWrapper.IsIllegal) level = 1;
            if (toolsController.IsIllegal) level = 2;
        }
        return level;
    }
    public bool IsActionIllegal()
    {
        return HowMuchActionIllegal() > 0;
    }
    public int HowMuchBeingIllegal()
    {
        if (activeZone == null) return 0;
        if (activeZone.DefaultRestriction == RestrictionType.FREE) return 0;
        if (activeZone.DefaultRestriction == RestrictionType.RESTRICTED) return 1;
        if (activeZone.DefaultRestriction == RestrictionType.HOSTILE) return 2;
        return 0;
    }

    public bool IsBeingIllegal()
    {
        return HowMuchBeingIllegal() > 0;
    }
    
    public int HowMuchIllegal()
    {
        return Mathf.Max(HowMuchBeingIllegal(), HowMuchActionIllegal());
    }
    public void WearDisguise()
    {
        WearDisguise(disguise);
    }
    public void WearDisguise(DisguiseScriptable _disguise)
    {
        disguise = _disguise;
        StatusController status = GetComponent<StatusController>();
        if (torsoGear!= null) Destroy(torsoGear);
        torsoGear = Instantiate(disguise.defaultClothes[0],transform);
        if (headGear != null) Destroy(headGear);
        headGear = Instantiate(disguise.defaultHeadgear[0],status.headTransform.position, status.headTransform.rotation,status.headTransform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((zoneMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent<Zone>(out Zone _zone))
            {
                Zones.Add(_zone);
                RefreshZone();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((zoneMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent<Zone>(out Zone _zone))
            {
                Zones.Remove(_zone);
                RefreshZone();
            }
        }
    }
    public void RefreshZone()
    {
        if (Zones.Count > 0)
            activeZone = Zones.OrderByDescending(z => z.zone.Depth).FirstOrDefault().zone;
        else activeZone = null;
    }
}
