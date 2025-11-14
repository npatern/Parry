using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OutwardController : MonoBehaviour,IInteractable
{
    public bool AffectedByLight = false;
    public bool AffectedByCrowd = false;
    public float LightValue = 0;
    private GameplayController gameController;
    public float CrowdDetectionTickTime = .2f;
    float CrowdTick = 0;
    public bool IsHiddenInCrowd = false;
    public float CrowdRadius = 2.5f;
    public LineRenderer[] linerenderers = new LineRenderer[5];
    public StatusController status;
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
        gameController = GameplayController.Instance;
        if (!AffectedByLight) LightValue = 1;
        if (AffectedByCrowd)
            SpawnLineRenderers();
         status = GetComponent<StatusController>();
        if (disguise != null) WearDisguise();
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
            if (hit.isTrigger) continue;
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
        lr.startWidth = lr.endWidth = 0.02f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.grey/2;
        lr.endColor = Color.black;
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
        if (TryGetComponent<ToolsController>(out ToolsController _toolsController))
            return HowMuchActionIllegal(_toolsController);
        return 0;
    }
    public int HowMuchActionIllegal(ToolsController _toolsController)
    {
        int level = 0;
        if 
            (_toolsController.IsIllegal) level = 2;
        else
        level = Mathf.Max( GetWeaponIllegality(_toolsController, _toolsController.CurrentWeaponWrapper), GetWeaponIllegality(_toolsController, _toolsController.GetWeaponOnTheBack()));
            
        return level;
    }
    public int GetWeaponIllegality(ToolsController _toolsController, ItemWeaponWrapper _item)
    {
        int level = 0;
        if (_item != null)
        {
            if (_item.IsIllegal)
                level = 1;
            if (disguise != null)
            {
                bool hasCommonFlags = (_item.tags & disguise.LegalItems) != ItemTypes.None;
                if (hasCommonFlags) level = 0;
            }
        }
        
        return Mathf.Max(level);
    }
    public bool IsActionIllegal()
    {
        if (TryGetComponent<ToolsController>(out ToolsController _toolsController))
            return HowMuchActionIllegal(_toolsController) > 0;
        return false;
    }
    public int HowMuchBeingIllegal()
    {
        int illegality = 0;
        if (disguise != null) illegality = disguise.GetLegality(activeZone);
        return illegality;
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
    public void Undress()
    {
        disguise = null;
        if (torsoGear != null) Destroy(torsoGear);
        if (headGear != null) Destroy(headGear);
    }
    public void WearDisguise(DisguiseScriptable _disguise)
    {
        
        Undress();
        disguise = _disguise;
        if (disguise == null) return;
        if (TryGetComponent<EntityController>(out EntityController _entity))
        {
            if (disguise.randomNeeds!=null)
            _entity.RandomNeeds = disguise.randomNeeds;
        }
        if (TryGetComponent<SensesController>(out SensesController _senses))
        {
            foreach (DisguiseScriptable enforcedDisguise in disguise.enforcesDisguises)
            _senses.AddBurnedDisguise(enforcedDisguise);
        }
        if (disguise.defaultClothes.Count>0)
            torsoGear = Instantiate(disguise.defaultClothes[0], status.bodyTransform.position, status.bodyTransform.rotation, status.bodyTransform);
        if (disguise.defaultHeadgear.Count > 0)
            headGear = Instantiate(disguise.defaultHeadgear[0], status.headTransform.position, status.headTransform.rotation,status.headTransform);
    }
    public void Interact(StatusController _status)
    {
        if (_status == null)
        {
            Debug.Log("Who steals clothes?!");
        }
        StealDisguise(_status);
    }
    public bool CanBeInteracted(StatusController _status)
    {
        if (status != null)
            return status.IsKilled;
        return false;
    }
    public void StealDisguise(StatusController _status)
    {
        if (disguise == null) return;
        _status.GetComponent<OutwardController>().WearDisguise(disguise);
        Undress();
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
            activeZone = Zones.OrderByDescending(z => z.Depth).FirstOrDefault().zone;
        else activeZone = null;
    }
    
}
