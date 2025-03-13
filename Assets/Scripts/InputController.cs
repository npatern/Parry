using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputController : MonoBehaviour
{
    StatusController statusController;
    [SerializeField]
    float gamepadDeadzone = .1f;
    [SerializeField]
    public Transform target;
    [SerializeField]
    Transform targetParent;
    [SerializeField]
    ForceMode forceMode;
    [SerializeField]
    bool alignToCamera = false;
    Vector2 movement;
    [SerializeField]
    Vector2 lookAtValue = Vector2.zero;
    private Rigidbody rb;
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float runSpeed = 2f;
    [SerializeField]
    private float jumpForce = 2f;
    [SerializeField]
    private float maxRunForce = 2f;
    [SerializeField]
    private Vector3 groundCheck = new Vector3(0,-.5f,0);
    [SerializeField]
    private float groundDistance = 0.5f;
    [SerializeField]
    private float rotationSpeed = 1f;
    CombatController combatController;
    [SerializeField]
    private bool IsDodging = false;
    [SerializeField]
    private float dodgeTime = .5f;
    [SerializeField]
    private float dodgeTimer = 0;
    [SerializeField]
    private float dodgeForce = 4;
    [SerializeField]
    private float dodgePostureCost = 20;

    bool isSprinting = false;

    float WalkSoundAwarenessTime = 1;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
         
        combatController = GetComponent<CombatController>();
        targetParent.parent = null;
        statusController = GetComponent<StatusController>();
        GetStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityStats);
        
    }
    void GetStatsFromScriptable(EntityStatsScriptableObject scriptable)
    {
        WalkSoundAwarenessTime = scriptable.WalkSoundAwarenessTime;
    }
    private void FixedUpdate()
    {
        if (IsDodging)
        {
            ApplyDodge();
            return;
        }
        if (CanRotate())
            ApplyLookAt();
        if (!CanMove()) return;
        if (movement.x == 0 && movement.y == 0)
        {
            isSprinting = false;
            return;
        }
        ApplyMovementForce();
        targetParent.transform.position = transform.position;
    }
    public void ApplyDodge()
    {
        if (!IsDodging)
        {
            
            //if (statusController.Posture < dodgePostureCost) return;
            if (!statusController.TakePosture(dodgePostureCost, statusController)) return;
            if (movement.magnitude>.2f)
                rb.AddForce(new Vector3(movement.x, 0, movement.y) * dodgeForce, ForceMode.VelocityChange);
            else
                rb.AddForce(transform.forward * -dodgeForce, ForceMode.VelocityChange);
            dodgeTimer = dodgeTime;
        }
        dodgeTimer-=Time.fixedDeltaTime;
        
        if (dodgeTimer <= 0) 
            IsDodging = false;
        else
            IsDodging = true;

        if (IsDodging == false)
            rb.velocity *= .1f;

        combatController.IsDodging = IsDodging;
    }
    public void ApplyMovementForce() 
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(movement.x, 0, movement.y);
        if (isSprinting)
            targetVelocity *= runSpeed;
        else
            targetVelocity *= movementSpeed;

        if (alignToCamera)
            targetVelocity = Camera.main.transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        Vector3.ClampMagnitude(velocityChange, maxRunForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        //if (!isSprinting) return;
        float soundRange = 8f * rb.velocity.magnitude / movementSpeed;
        Sound sound = new Sound(statusController, soundRange);
        Sounds.MakeSound(sound);
    }
    public void ApplyLookAt()
    {
        //target.parent = null;
        /*
        transform.LookAt(target);
        //rb.centerOfMass = Vector3.zero;
        rb.freezeRotation = true;
        */
        float angle;
        
        var localTarget = transform.InverseTransformPoint(target.transform.position);

        angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime* rotationSpeed);
        rb.MoveRotation(rb.rotation * deltaRotation);
        //target.parent = transform;
    }
    public void Movement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<CombatController>().PerformAttack(context);
    }
    public void HeavyAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<CombatController>().PerformHeavyAttack(context);
    }
    public void Parry(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<CombatController>().PerformParry(context);
    }
    public void Sprint(InputAction.CallbackContext context)
    {

        isSprinting = context.performed;
    }
    public void GetLookAtValue(InputAction.CallbackContext context)
    {
        lookAtValue = context.ReadValue<Vector2>();
        if (context.control.device == Gamepad.current)
        {
            //target.parent = transform;
            if (lookAtValue.magnitude> gamepadDeadzone)
                target.transform.position = new Vector3(lookAtValue.x * 10, 2, lookAtValue.y * 10)+transform.position;
        }
        else
        {
            //target.parent = null;
            //TODO: get rid of camera.main later
            Vector3 v3 = new Vector3(lookAtValue.x, lookAtValue.y, 10);
            Ray ray = Camera.main.ScreenPointToRay(v3);
            if (Physics.Raycast(ray, out RaycastHit hitData))
                v3 = hitData.point;
            v3.y += 2;
            target.transform.position = v3;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        if (IsGrounded())
            rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
    }
    public void Dodge(InputAction.CallbackContext context)
    {
        if (IsDodging) return;
        //if (!IsGrounded()) return;
        if(context.started)
            ApplyDodge();
    }
    public bool IsGrounded()
    {
        //if (rb.velocity.y == 0)
        //{
            Collider[] hitcollider;
            
            hitcollider = Physics.OverlapSphere(transform.position + groundCheck, groundDistance, LayerMask.GetMask("Blockout"));
            
            if (hitcollider.Length > 0)
            {
                Gizmos.color = Color.yellow;
                return true;
            }
            else
            {
                Gizmos.color = Color.red;
                return false;
            }
        //}   
        //else
            //return false;
    }
    void OnDrawGizmos()
    { 
        Gizmos.DrawSphere(transform.position + groundCheck, groundDistance);
    }
    public bool CanMove()
    {
        return !combatController.MovementLocked;
    }
    public bool CanRotate()
    {
        return !combatController.RotationLocked;
    }
}
