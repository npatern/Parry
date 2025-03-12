using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensesController : MonoBehaviour, IHear
{
    [Space(10), Header("Debug")]
    [SerializeField]
    bool drawRaycasts = false;
    [SerializeField]
    float nrOfTestRays = 100;
    [SerializeField]
    float playerHeight = 1.5f;
    

    public bool IsAlerted = false;
    public StatusController currentTarget;
    public Vector3 currentTargetLastPosition;
    public float currentTargetTimer = 0;
    public bool isInvestigationOpen = false;
    public float Awareness = 0;
    [SerializeField]
    private float maxAwareness = 100;
    [SerializeField]
    float awarenessUpTime = 1f;
    [SerializeField]
    float awarenessDownTime = 1f;
    float alertedAwarenessDownTime = 1f;

    [Space(10), Header("Values")]
    [SerializeField]
    float viewDistance = 10;
    [SerializeField]
    float viewAngle = 45;

    [SerializeField]
    AnimationCurve angleCurve;
    [SerializeField]
    AnimationCurve distanceCurve;
    [SerializeField]
    LayerMask layerMask;

    [Space(10), Header("References")]
    [SerializeField]
    Transform eyesSource;
    [SerializeField]
    Transform target;
    GameObject UIarrow;
    private void Awake()
    {
        if (eyesSource == null) eyesSource = this.transform;
        if (target == null) target = GameController.Instance.CurrentPlayer.transform;
        layerMask = LayerMask.GetMask("Entity", "Blockout");
    }
    private void Start()
    {
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
        if (UIController.Instance != null) UIarrow = UIController.Instance.SpawnAwarenessArrow(this);
    }
    private void Update()
    {
        if (target == null) target = GameController.Instance.CurrentPlayer.transform;
        if (drawRaycasts)
            DrawRayToTarget(eyesSource, target);
    }
    private void FixedUpdate()
    {
        if (currentTargetTimer > 0)
            currentTargetTimer -= Time.fixedDeltaTime;
        else
            ResetTarget();
        if (currentTarget != null) currentTargetLastPosition = currentTarget.transform.position;

        if (currentTargetTimer<=0) 
        if (target == null) return;
        ApplyAwareness(GetAwarenessValue(target), target.GetComponent<StatusController>());
         
    }
    void ResetTarget()
    {
        currentTarget = null;
        currentTargetTimer = 0;
    }
    void SetTarget(StatusController _target)
    {
        if (_target == null) return;
        currentTargetTimer = 1;
        currentTarget = _target;
    }
    void ApplyAwareness(float awarenessValue, StatusController _target=null)
    {

        bool wasAlerted = IsAlerted;
        if (awarenessValue > 0)
        {
            if (_target!=null)
                SetTarget(_target);
            if (IsAlerted)
                Awareness = maxAwareness;
            else
                Awareness += awarenessValue * maxAwareness * Time.fixedDeltaTime/ awarenessUpTime;
        }
        else
        {
            if (isInvestigationOpen)
            {

            }  
            else if (IsAlerted)
            {
                if (currentTargetTimer <= 0) Awareness -= maxAwareness * Time.fixedDeltaTime / alertedAwarenessDownTime;
            }
            else
            {
                Awareness -= maxAwareness * Time.fixedDeltaTime / awarenessDownTime;
            }
                
        }
        if (Awareness >= 100)
        {
            IsAlerted = true;
        }
        if (Awareness <= 0)
        {
            IsAlerted = false;
        }
        Awareness = Mathf.Clamp(Awareness, 0, 100);
    }
    void LoadStatsFromScriptable(EntityStatsScriptableObject scriptable)
    {
        awarenessUpTime = scriptable.AwarenessUpTime;
        awarenessDownTime = scriptable.AwarenessDownTime;
        alertedAwarenessDownTime = scriptable.AlertedAwarenessDownTime;
    }
    float GetAwarenessValue(Transform destination)
    {
        float awarenessValue = 0;
        if (IsTargetSeenTroughEyes(destination))
        {
            awarenessValue = GetSeenValueFromEyes(destination);
        }
        if (destination.GetComponent<OutwardController>() != null)
            awarenessValue *= destination.GetComponent<OutwardController>().LightValue;

        return awarenessValue;
    }
    public bool IsTargetSeenTroughEyes(Transform destination)
    {

        return IsTargetSeen(eyesSource, target);
    }
    public bool IsTargetSeen(Transform source, Transform destination)
    {
        Vector3 direction = destination.position - source.position + Vector3.up * playerHeight;
        float distance = Vector3.Distance(source.position, destination.position + Vector3.up * playerHeight);
        float angle = Vector3.Angle(source.forward, direction);
        Color rayColor = Color.white;

        if (angle > viewAngle) return false;
        else if (distance > viewDistance) return false;

        RaycastHit hit;
        if (Physics.Raycast(source.position, direction, out hit, viewDistance, layerMask))
            if (hit.transform == destination)
            {
                return true;
            }
                

        return false;
    }
    public float GetSeenValueFromEyes(Transform destination)
    {
        return GetSeenValue(eyesSource, destination);
    }
    public float GetSeenValue(Transform source, Transform destination)
    {
        Vector3 direction = destination.position-source.position +Vector3.up* playerHeight;
         
        float distance = Vector3.Distance(source.position, destination.position + Vector3.up * playerHeight);
        float angle = Vector3.Angle(source.forward, direction);
        
        if (angle > viewAngle) return 0;
        else if (distance > viewDistance) return 0;

        float value;

        float angleValue = angle / viewAngle;
        angleValue = 1 - angleValue;
        Mathf.Clamp01(angleValue);
        angleValue = angleCurve.Evaluate(angleValue);


        float distanceValue = distance / (viewDistance*angleValue);
        Mathf.Clamp01(distanceValue);
        distanceValue = 1 - distanceValue;
        distanceValue = distanceCurve.Evaluate(distanceValue);

        value = distanceValue;

        return value;
    }
    void DrawRayToTarget(Transform source, Transform destination)
    {
        float seenValue = GetSeenValue(source, destination);
        Color rayColor = Color.green;
        if (seenValue >= 1 || seenValue <= 0) rayColor = Color.red;
        else rayColor = Color.Lerp(Color.blue, Color.white, seenValue);

        Debug.DrawLine(source.position, destination.position + Vector3.up * playerHeight, rayColor);

        //draw many rays
         
        float angle;
        float anglePercent;
        float totalAngle = viewAngle * 2;

        float delta = totalAngle / nrOfTestRays;
        Vector3 pos = source.position;
        Debug.DrawRay(pos, Quaternion.Euler(0,   -viewAngle, 0) * source.forward * viewDistance, Color.cyan);
        Debug.DrawRay(pos, Quaternion.Euler(0, +viewAngle, 0) * source.forward*viewDistance, Color.cyan);
        for (int i = 0; i <= nrOfTestRays; i++)
        {
            
            var dir = Quaternion.Euler(0, i * delta-viewAngle,0) * source.forward;
            angle = Vector3.Angle(source.forward, dir);
            anglePercent = GetValueFromAngle(angle) * viewDistance*distanceCurve.Evaluate(1);
            if (angle <= viewAngle)
            Debug.DrawRay(pos, dir* anglePercent, Color.green);
        }
         
    }
    float GetValueFromAngle(float angle)
    {
        float anglePercent = 1-angle / viewAngle;
        if (angleCurve != null)
            anglePercent = angleCurve.Evaluate(anglePercent);

        return anglePercent;
    }

    StatusController GetTargetFromSound(Sound sound)
    {
        if (sound.worldInfo != null)
            if (sound.worldInfo.IsPlayer)
                return sound.worldInfo;

        if (sound.statusController != null)
            if (sound.statusController.IsPlayer)
                return sound.statusController;

        return null;
    }
    public void ReactToSound(Sound sound)
    {
        
        StatusController _target = GetTargetFromSound(sound);

        if (Vector3.Distance(eyesSource.position, sound.position) > sound.range) return;
        if (sound.type == Sound.TYPES.cover)
            GetComponent<StatusController>().MakeDeaf();
        if (GetComponent<StatusController>().IsDeaf()) return;

        if (sound.type == Sound.TYPES.danger)
            ApplyAwareness(100, _target);
        if (sound.type == Sound.TYPES.neutral)
            ApplyAwareness((100 * Time.fixedDeltaTime) / 2, _target);


        //GetComponent<EntityController>().currentState.InvestigateSound(GetComponent<EntityController>(),sound.position);
        Debug.Log("senses reacted"+name);
    }
    public void Kill()
    {
        Destroy(UIarrow);
        Destroy(GetComponent<SensesController>());
    }
    
}
