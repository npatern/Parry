using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LightController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textComponent;
    [SerializeField]
    private Light lightComponent;
    [SerializeField]
    private LightType lightType;
    [SerializeField]
    private float range;
    [SerializeField]
    private float brightness;
    [SerializeField]
    private AnimationCurve lightCurve;
    [SerializeField]
    private bool isOn = true;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    StatusController statusController;
    [SerializeField]
    private bool realtime = false;
    private void Awake()
    {
        lightComponent = GetComponent<Light>();
        statusController = GetComponent<StatusController>();
        UpdateLight();
    }
    void UpdateLight()
    {
        lightType = lightComponent.type;
        if (lightType == LightType.Directional)
            range = Mathf.Infinity;
        else
            range = lightComponent.range;
        brightness = lightComponent.intensity;

        layerMask = LayerMask.GetMask("Entity", "Blockout", "Bush");

    }
    
    public void Start()
    {
        if (statusController!=null)
        statusController.OnKillEvent.AddListener(KillLight);
        GameController.Instance.lightControllers.Add(this);
    }
    private void FixedUpdate()
    {
        if (GameController.Instance.CurrentPlayer !=null)
        if(IsInLight(GameController.Instance.CurrentPlayer.transform)&& textComponent!=null)
            textComponent.SetText("" + GetLightValueOnObject(GameController.Instance.CurrentPlayer.transform));

        if (realtime) UpdateLight();
    }
    private bool IsInDistance(Transform target)
    {
        return Vector3.Distance(transform.position, target.position)<=range;
    }

    public bool IsInLight(Transform target)
    {
        if (lightComponent == null) return false;
        if (!isOn) return false;

        switch (lightComponent.type)
        {
            case LightType.Point:
                
                if (!IsInDistance(target)) return false;
                if (IsObscured(target)) return false;
                break;
            case LightType.Directional:
                if (IsObscuredDirection(target)) return false;
                break;
            case LightType.Spot:
                if (!IsInDistance(target)) return false;
                if (!IsAngleRight(target)) return false;
                if (IsObscured(target)) return false;
                break;
        }
        return true;
    }
    public bool IsObscured(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, range, layerMask))
            if (hit.transform == target)
                return false;
        return true;     
    }
    public bool IsAngleRight(Transform target)
    {
        //
        float viewAngle = lightComponent.spotAngle/2;
        Vector3 direction = target.position - transform.position + Vector3.up*1.5f;
        float angle = Vector3.Angle(transform.forward, direction);
        Color rayColor = Color.white;

        //
        if (angle > viewAngle)
            Debug.DrawRay(transform.position, direction * 10, Color.red);
        else
            Debug.DrawRay(transform.position, direction * 10, Color.yellow);
        //

        if (angle > viewAngle) return false;
        return true;
    }
    public bool IsObscuredDirection(Transform target)
    {
        Vector3 direction = -lightComponent.transform.forward;
        RaycastHit hit;
        LayerMask thisLayerMask = LayerMask.GetMask("Blockout", "Bush");
        LayerMask bushLayerMask = LayerMask.GetMask("Bush");
        Debug.DrawRay(target.position + Vector3.up, direction*100,Color.yellow,1f);
        Vector3 point1=target.position + Vector3.up;
        Vector3 point2=target.position + Vector3.up*2;
        if (Physics.CheckSphere(point1, .2f, bushLayerMask) && Physics.CheckSphere(point2, .2f, bushLayerMask))
            return true;
        if (!Physics.Raycast(point1, direction, out hit, 150, thisLayerMask))
            return false;
        if (!Physics.Raycast(point2, direction, out hit, 150, thisLayerMask))
            return false;
        return true;
    }
    public float GetLightValueOnObject(Transform target)
    {
        float lightValue;
        float distance = Vector3.Distance(transform.position, target.position);
        float lerpValue = 1 - distance / range;
        lerpValue = lightCurve.Evaluate(lerpValue);
        lightValue = Mathf.Lerp(0, brightness, lerpValue);
        return lightValue;
    }
    public void SwitchLight (bool onOff)
    {
        isOn = onOff;
        lightComponent.enabled = isOn;
    }
    public void KillLight()
    {
        Debug.Log("KILL LIGHTS!!!!!!!!!!!!!!!!");
        SwitchLight(false);
        GetComponent<MeshRenderer>().enabled = false;
        if (textComponent!=null)
            textComponent.gameObject.SetActive(false);
        
    }
}
