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
    ToolsController toolsController;
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

    private bool switchWeapon = false;
    bool isSprinting = false;
    PlayerInput playerInput;
    float WalkSoundAwarenessTime = 1;
    private Camera camera;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
         
        toolsController = GetComponent<ToolsController>();
        targetParent.parent = null;
        statusController = GetComponent<StatusController>();
        GetStatsFromScriptable(GameController.Instance.ListOfAssets.DefaultEntityValues);
        playerInput = GetComponent<PlayerInput>();
        camera = Camera.main;
    }
    void GetStatsFromScriptable(EntityValuesScriptableObject scriptable)
    {
        WalkSoundAwarenessTime = scriptable.WalkSoundAwarenessTime;
    }
    public void ShowBindingsText(string actionName, string text)
    {
        statusController.OverheadController.ShowInfoText("<b>"+GetKeyBinding(actionName).ToUpper()+"</b>" + text);
    }
    private void FixedUpdate()
    {
        switchWeapon = true;
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
            if (!statusController.TakePostureOnly(dodgePostureCost,1, statusController)) return;
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

        toolsController.IsDodging = IsDodging;
    }
    public void ApplyMovementForce() 
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(movement.x, 0, movement.y);
        if (isSprinting)
            targetVelocity *= runSpeed;
        else
            targetVelocity *= movementSpeed;
        if (camera == null) camera = Camera.main;
        if (alignToCamera)
            targetVelocity = camera.transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        Vector3.ClampMagnitude(velocityChange, maxRunForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        //if (!isSprinting) return;
        float soundRange = 10f * rb.velocity.magnitude / movementSpeed;
        Sound sound = new Sound(statusController, soundRange,Sound.TYPES.continous);
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
    public void WeaponScroll(InputAction.CallbackContext context)
    {
        if (!switchWeapon) return;
        //if (toolsController.IsUsingTool) return;
        Vector2 weaponScroll = context.ReadValue<Vector2>();

        if (GetComponent<InventoryController>() == null) return;
        
        InventoryController inventoryController = GetComponent<InventoryController>();
        if (inventoryController.allItems.Count == 0) return;
        if (weaponScroll.y < 0) inventoryController.EquipFromInventory(inventoryController.GetNextWeapon());
        else if (weaponScroll.y > 0) inventoryController.EquipFromInventory(inventoryController.GetPreviousWeapon());
        switchWeapon = false;
    }
    public void Attack(InputAction.CallbackContext context)
    {
        
          if (context.performed)
            GetComponent<ToolsController>().PerformAttack(context);
    }
    public void HeavyAttack(InputAction.CallbackContext context)
    {
          if (context.performed)
            GetComponent<ToolsController>().PerformHeavyAttack(context);
    }
    public void ThrowAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<ToolsController>().PerformThrow(context);
    }
    public void Parry(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<ToolsController>().PerformParry(context);
    }
    public void Noctovision(InputAction.CallbackContext context)
    {
        if (context.started)
            GameController.Instance.SwitchNoctovision();
    }
    public void Interaction(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<InteractionController>().Use();
    }
    public void Pick(InputAction.CallbackContext context)
    {
        if (context.started)
            GetComponent<InteractionController>().Pick();
    }
    public void Sprint(InputAction.CallbackContext context)
    {

        isSprinting = context.performed;
    }
    public void HideWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GetComponent<InventoryController>() == null) return;
            InventoryController inventoryController = GetComponent<InventoryController>();
            if (toolsController.CurrentWeaponWrapper.emptyhanded)
            {
                if (inventoryController.allItems.Count == 0) return;
                inventoryController.EquipFromInventory(inventoryController.allItems[inventoryController.currentSlot]);
            }
            else if (toolsController.CurrentWeaponWrapper.Big && inventoryController.GetWeaponOnTheBack()!=null)
            {
                inventoryController.EquipFromInventory(inventoryController.GetWeaponOnTheBack());
            }
            else
            {
                toolsController.DequipWeaponFromHands();
            }
        }     
    }
    public void DropWeapon(InputAction.CallbackContext context)
    {

        if (context.performed)
            GetComponent<ToolsController>().DropOneWeaponFromHands();
    }
    public string GetKeyBinding(string actionName)
    {
        InputAction action;
        action = playerInput.actions.FindAction(actionName);
        if (action == null) return actionName + "NULL";
        int bindingIndex = action.GetBindingIndex(group: playerInput.currentControlScheme);
        string displayString = action.GetBindingDisplayString(bindingIndex, out string deviceLayoutName, out string controlPath);
        return displayString;
    }
    public void GetLookAtValue(InputAction.CallbackContext context)
    {
        lookAtValue = context.ReadValue<Vector2>();
        if (context.control.device == Gamepad.current)
        {
            //target.parent = transform;
            if (lookAtValue.magnitude> gamepadDeadzone)
                target.transform.position = new Vector3(lookAtValue.x * 20, 2, lookAtValue.y * 20)+transform.position;
        }
        else
        {

            /*
          LayerMask mask = LayerMask.GetMask("Blockout");
          //if (Physics.Raycast(ray, out RaycastHit hitData, mask))

          Vector3 v3 = new Vector3(lookAtValue.x, lookAtValue.y, 10);
          if (camera == null) camera = Camera.main;
          Ray ray = camera.ScreenPointToRay(v3);
          if (Physics.Raycast(ray, out RaycastHit hitData, mask))
              v3 = hitData.point;
           target.transform.position = v3;
          */


            if (camera == null) camera = Camera.main;

            Vector2 screenPoint = new Vector2(lookAtValue.x, lookAtValue.y);
            float desiredY = transform.position.y + 2;

            Vector3? worldPoint = ScreenToWorldAtY(screenPoint, desiredY);
            if (worldPoint.HasValue)
            {
                target.transform.position = worldPoint.Value;
            }
        }
    }
    public Vector3? ScreenToWorldAtY(Vector2 screenPos, float yTarget)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Vector3 origin = ray.origin;
        Vector3 direction = ray.direction;

        if (Mathf.Approximately(direction.y, 0f))
            return null;

        float t = (yTarget - origin.y) / direction.y;
        return origin + direction * t;
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
       // Gizmos.DrawSphere(transform.position + groundCheck, groundDistance);
    }
    public bool CanMove()
    {
        return !toolsController.MovementLocked;
    }
    public bool CanRotate()
    {
        return !toolsController.RotationLocked;
    }
}
