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
    public bool justHeardSmthng = false;
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
        LoadStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityValues);
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
            ResetCurrentTarget();
        if (currentTarget != null) currentTargetLastPosition = currentTarget.transform.position;

        if (currentTargetTimer<=0) 
        if (target == null) return;
        ApplyAwareness(GetAwarenessValue(target), target.GetComponent<StatusController>());
         
    }
    void ResetCurrentTarget()
    {
        currentTarget = null;
        currentTargetTimer = 0;
    }
    public void SetCurrentTarget(StatusController _target)
    {
        if (_target == null) return;
        currentTargetTimer = 1;
        currentTarget = _target;
    }
    public StatusController GetCurrentTarget()
    {
        return currentTarget;
    }

    public void AddAwarenessOnce(float awarenessValue, StatusController _target = null)
    {
        if (awarenessValue > 0)
            if (_target != null)
                SetCurrentTarget(_target);

        AddAwarenessOnce(awarenessValue);
    }
    public void AddAwarenessOnce(float awarenessValue, Vector3 targetPosition)
    {
        if (awarenessValue > 0)
            currentTargetLastPosition = targetPosition;

        AddAwarenessOnce(awarenessValue);
    }
    public void AddAwarenessOnce(float awarenessValue )
    {

        justHeardSmthng = true;
        if (IsAlerted)
            Awareness = maxAwareness;
        else
            Awareness += awarenessValue;

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
    void AddAwarenessContinous(float awarenessValue, StatusController _target = null)
    {
        if (awarenessValue > 0)
            if (_target != null)
                SetCurrentTarget(_target);
        justHeardSmthng = true;
        if (IsAlerted)
            Awareness = maxAwareness;
        else
            Awareness += awarenessValue * maxAwareness * Time.fixedDeltaTime / awarenessUpTime;

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
    void ApplyAwareness(float awarenessValue, StatusController _target=null)
    {

        bool wasAlerted = IsAlerted;
        if (awarenessValue > 0)
        {
            if (_target!=null)
                SetCurrentTarget(_target);
            if (IsAlerted)
                Awareness = maxAwareness;
            else
                Awareness += awarenessValue * maxAwareness * Time.fixedDeltaTime/ awarenessUpTime;
        }
        else
        {
            if (isInvestigationOpen || justHeardSmthng)
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
        justHeardSmthng = false;
    }
    void LoadStatsFromScriptable(EntityValuesScriptableObject scriptable)
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
    Vector3 GetTargetPositionFromSound(Sound sound, out StatusController _status)
    {
        _status = null;
        if (sound.hasTargetInfo)
        {
            if (sound.targetEntityInfo != null && sound.targetEntityInfo.IsPlayer)
            {
                _status = sound.targetEntityInfo;
                return _status.transform.position;
            }
            else
            {
                return sound.targetPositionInfo;
            }
        }
        else
        {
            if (sound.callerstatusController != null && sound.callerstatusController.IsPlayer)
            {
                _status = sound.callerstatusController;
                return _status.transform.position;
            }
            else
            {
                return sound.callerPosition;
            }
        }
    }
    StatusController GetTargetFromSound(Sound sound)
    {

        if (sound.targetEntityInfo != null)
            if (sound.targetEntityInfo.IsPlayer)
                return sound.targetEntityInfo;

        if (sound.callerstatusController != null)
            if (sound.callerstatusController.IsPlayer)
                return sound.callerstatusController;

        return null;
    }
    public void ReactToSound(Sound sound)
    {
        StatusController _target;// = GetTargetFromSound(sound);
        Vector3 _targetPosition = GetTargetPositionFromSound(sound, out _target);

        if (sound.callerstatusController == GetComponent<StatusController>()) return;
        if (Vector3.Distance(eyesSource.position, sound.callerPosition) > sound.range) return;
        if (sound.type == Sound.TYPES.cover)
            GetComponent<StatusController>().MakeDeaf();
        if (GetComponent<StatusController>().IsDeaf()) return;

        if (sound.type == Sound.TYPES.danger)
            if (_target != null)
                AddAwarenessOnce(100, _target);
            else
                AddAwarenessOnce(100, _targetPosition);

        if (sound.type == Sound.TYPES.neutral)
        {
            if (_target != null)
                AddAwarenessOnce(30, _target);
            else
                AddAwarenessOnce(30, _targetPosition);
        }
            
        if (sound.type == Sound.TYPES.continous)
            AddAwarenessContinous(1/2, _target);
        


        //GetComponent<EntityController>().currentState.InvestigateSound(GetComponent<EntityController>(),sound.position);
        //Debug.Log("senses reacted"+name);
    }
    public void Kill()
    {
        Destroy(UIarrow);
        Destroy(GetComponent<SensesController>());
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(currentTargetLastPosition, .5f);
    }
}
