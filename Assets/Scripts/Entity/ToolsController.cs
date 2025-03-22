using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class ToolsController : MonoBehaviour
{
    
    public LayerMask layerMask;
    public float ColliderSize = 1f;
    public InputController inputController;
    public StatusController statusController;
    public InventoryController inventoryController;
    [SerializeReference]
    public ItemWeaponWrapper CurrentWeaponWrapper;
    [SerializeReference]
    public ItemWeaponWrapper DefaultWeaponWrapper;
    public ItemWeaponScriptableObject TESTCurrentWeaponScriptable;
    public Transform DequippedWeapon;
    public enum targets { IdlePoint, ParryPoint, HeavyAttack, HeavyAttackEnd, Attack, AttackEnd, Unequipped, Disarmed, Throw };

    public bool MovementLocked = false;
    public bool RotationLocked = false;
    Coroutine attack = null;
    public bool IsUsingTool = false;
    public bool IsProtected = false;
    public bool IsParrying = false;
    public bool IsDamaging = false;
    public bool IsDodging = false;

    public UnityEvent<bool> StopMovingEvent;
    public float attackDistance = 3f;

    
    public void Awake()
    {
        inputController = GetComponent<InputController>();
        statusController = GetComponent<StatusController>();
        inventoryController = GetComponent<InventoryController>();
        if (StopMovingEvent == null)
        {
            StopMovingEvent = new UnityEvent<bool>();
        }


        //if (DequippedWeaponWrapper == null) DequippedWeaponWrapper = new ItemWeaponWrapper(TESTDequippedWeaponScriptable);
        //if (DequippedWeapon == null) DequippedWeapon = Instantiate(DequippedWeaponWrapper.itemType.weaponObject, transform).transform;
    }
    public void Start()
    {

        statusController.IsStunnedEvent.AddListener(PerformStunned);
        statusController.IsAttackedEvent.AddListener(PerformAttacked);
        //statusController.IsKilledEvent.AddListener(PerformAttacked);
        if (TESTCurrentWeaponScriptable == null) return;
        if (CurrentWeaponWrapper==null)
        EquipWeapon(new ItemWeaponWrapper(TESTCurrentWeaponScriptable));
    }
    public float GetCombatSpeedMultiplier()
    {
        float multiplier = 1;
        multiplier *= statusController.SpeedMultiplier;
        //dodac mnoznik z broni i roznych efektow jak freeze
        return multiplier;
    }
    public void EquipWeaponFromPickable(Pickable pickable)
    {
        ItemWeaponWrapper weaponWrapper = pickable.weaponWrapper;
        weaponWrapper.RemovePickable(transform, true);
        EquipWeapon(weaponWrapper);
    }
    
    public bool EquipWeapon(ItemWeaponWrapper weaponWrapper)
    {
        if (weaponWrapper == null) return false;
        if (CurrentWeaponWrapper != null)
            DequipOrReplaceWeaponInHands(weaponWrapper);

        CurrentWeaponWrapper = weaponWrapper;
        if (CurrentWeaponWrapper.CurrentWeaponObject == null)
            CurrentWeaponWrapper.CurrentWeaponObject = CurrentWeaponWrapper.SpawnWeaponObjectAsCurrentObject(transform);
        CurrentWeaponWrapper.RemoveRigidBody();
        attackDistance = CurrentWeaponWrapper.itemType.AttackDistance;
        BreakAttackCoroutines();
        attack =StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Equip));
        return true;
    }
    public void BreakAttackCoroutines()
    {
        if (attack != null)
            StopCoroutine(attack);
        StopAllCoroutines();
        IsUsingTool = false;
        ClearStateEffects();
    }
    public void DequipOrReplaceWeaponInHands(ItemWeaponWrapper itemToReplaceWith)
    {
        if (itemToReplaceWith.ID == CurrentWeaponWrapper.ID)
            if (CurrentWeaponWrapper.Stackable)
            {
                itemToReplaceWith.MergeToMe(CurrentWeaponWrapper);
                return;
            }
            else
            {
                CurrentWeaponWrapper.DestroyPhysicalPresence();
                return;
            }
        DequipWeaponFromHands();
    }
    public void DequipWeaponFromHands()
    {
       
        if (TryHideItemFromHands()) 
        {
            CurrentWeaponWrapper = null;
            return;
        }
        DropWeaponFromHands();
    }
    public void DropWeaponFromHands()
    {
        CurrentWeaponWrapper.MakePickable();
        CurrentWeaponWrapper = null;
    }
    public bool TryHideItemFromHands()
    {
        if (inventoryController == null) return false;
        return inventoryController.AddToInventory(CurrentWeaponWrapper);
    }
    public bool CanPerform()
    {
        if (IsUsingTool) return false;
        if (statusController.IsStunned) return false;
        return true;
    }
    public Transform GetTargetFromEnum(targets target)
    {
        switch (target)
        {
            case targets.IdlePoint:
                return CurrentWeaponWrapper.itemType.attackPattern.IdlePoint;
            case targets.ParryPoint:
                return CurrentWeaponWrapper.itemType.attackPattern.ParryPoint;
            case targets.HeavyAttack:
                return CurrentWeaponWrapper.itemType.attackPattern.HeavyAttack;
            case targets.HeavyAttackEnd:
                return CurrentWeaponWrapper.itemType.attackPattern.HeavyAttackEnd;
            case targets.Attack:
                return CurrentWeaponWrapper.itemType.attackPattern.Attack;
            case targets.AttackEnd:
                return CurrentWeaponWrapper.itemType.attackPattern.AttackEnd;
            case targets.Unequipped:
                return CurrentWeaponWrapper.itemType.attackPattern.Unequipped;
            case targets.Disarmed:
                return CurrentWeaponWrapper.itemType.attackPattern.Disarmed;
            case targets.Throw:
                return CurrentWeaponWrapper.itemType.attackPattern.Throw;
        }
        return CurrentWeaponWrapper.CurrentWeaponObject;
    }
    
    public void PerformAttack(InputAction.CallbackContext context = new InputAction.CallbackContext())
    {
        if (!CanPerform())
        { 
            return;
        }

        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.LightAttack, context));
    }
    public void PerformHeavyAttack(InputAction.CallbackContext context = new InputAction.CallbackContext())
    {
        
        if (!CanPerform())
        {
            return;
        }
        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.HeavyAttack, context));
    }
    public void PerformThrow(InputAction.CallbackContext context = new InputAction.CallbackContext())
    {

        if (!CanPerform())
        {
            return;
        }
        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Throw, context));
    }
    public void PerformParry(InputAction.CallbackContext context = new InputAction.CallbackContext())
    {

        if (!CanPerform())
        {
            return;
        }
        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Parry, context));
    }
    public void PerformStunned()
    {
        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Stunned));
    }
    public void PerformAttacked()
    {
        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Attacked));
    }
    public bool IsGrounded()
    {
        if (inputController != null) return inputController.IsGrounded();

        return true;
    }
    public IEnumerator PlayAttackSteps(AttackScriptableObject attack, InputAction.CallbackContext context = new InputAction.CallbackContext())
    {
        if (attack == null) yield break;
        IsUsingTool = true;
        

        int totalSteps = attack.AttackStepss.Length;
        for (int i = 0; i< totalSteps; i++)
        {
            ClearStateEffects();
            if (attack.AttackStepss[i].OnlyIfGrounded)
                if (!IsGrounded()) continue;
            if (attack.AttackStepss[i].SkipIfCanceled)
                if (context.canceled) continue;
            if (attack.AttackStepss[i].SkipIfPlayer)
                if (statusController.IsPlayer) continue;
            if (attack.AttackStepss[i].Interruptable)
                IsUsingTool = false;
            else
                IsUsingTool = true;

            yield return attack.AttackStepss[i].PerformStep(this, attack, context);
        }
        IsUsingTool = false;
    }
    private void ClearStateEffects()
    {
        IsParrying = false; 
        IsProtected = false;
        IsDamaging = false;
    }
    public void CastDamage(float damage)
    {
        
        if (!IsDamaging) return;
        if (CurrentWeaponWrapper.CurrentWeaponObject == null) return;
        if (CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>() == null) return;

        //Vector3 weaponEnd = CurrentWeapon.transform.position + CurrentWeapon.transform.up * 2;
        WeaponModel weaponModel = CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>();

        Collider[] hitEnemies = Physics.OverlapCapsule(weaponModel.StartPoint.position, weaponModel.EndPoint.position, ColliderSize, layerMask);
        bool hitAnything = false;
        foreach (Collider enemy in hitEnemies)
        {
            StatusController enemyStatus = enemy.GetComponent<StatusController>();
            //if (enemyStatus == null) enemyStatus = enemy.attachedRigidbody.GetComponent<StatusController>();
            if (enemyStatus == null) continue;
            if (enemyStatus == statusController) continue;
            if (statusController.Team == enemyStatus.Team) continue;
            enemyStatus.TryTakeDamage( damage, statusController);
            if (!enemyStatus.IsKilled)
            hitAnything = true;
        }
        if (hitAnything)
            IsDamaging = false;    
    }
    private void OnDrawGizmosSelected()
    {
        if (CurrentWeaponWrapper == null) return;
        if (CurrentWeaponWrapper.CurrentWeaponObject == null) return;
        if (CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>() == null) return;
        WeaponModel weaponModel = CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>();
        Gizmos.DrawWireSphere(weaponModel.StartPoint.position, ColliderSize);
        Gizmos.DrawWireSphere(weaponModel.EndPoint.position, ColliderSize);
    }
    public void FireBullet()
    {
        WeaponModel weaponModel = CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>();
        Bullet bullet = Instantiate(CurrentWeaponWrapper.itemType.bullet, weaponModel.StartPoint.position, CurrentWeaponWrapper.CurrentWeaponObject.transform.rotation, GameController.Instance.GarbageCollector.transform).GetComponent<Bullet>();
        bullet.Damage = CurrentWeaponWrapper.itemType.Damage;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddRelativeForce(Vector3.forward * 1000, ForceMode.Acceleration);
        Debug.Log("bullet fired!");

    }
    public void OBSOLETEThrow()
    {
        WeaponModel weaponModel = CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>();
        Bullet bullet = Instantiate(GameController.Instance.ListOfAssets.BulletThrowTemplate.gameObject, weaponModel.StartPoint.position, CurrentWeaponWrapper.CurrentWeaponObject.transform.rotation, GameController.Instance.GarbageCollector.transform).GetComponent<Bullet>();
        bullet.Damage = CurrentWeaponWrapper.itemType.Damage;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bullet.item = CurrentWeaponWrapper;
        Vector3 throwDirection = new Vector3 (CurrentWeaponWrapper.CurrentWeaponObject.position.x - transform.position.x, 0, CurrentWeaponWrapper.CurrentWeaponObject.position.z - transform.position.z);
        CurrentWeaponWrapper.CurrentWeaponObject.parent = bullet.transform;
        CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<Collider>().enabled = true;
        CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<Collider>().isTrigger = false;
        CurrentWeaponWrapper = null;
        
        bulletRB.AddForce(throwDirection * 1000, ForceMode.Acceleration);
        BreakAttackCoroutines();
        Debug.Log("bullet fired!");

    }
    public void Throw()
    {
        WeaponModel weaponModel = CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>();

        Bullet bullet = CurrentWeaponWrapper.CurrentWeaponObject.gameObject.AddComponent<Bullet>();
        bullet.Damage = CurrentWeaponWrapper.Damage*100;
        bullet.soundType = Sound.TYPES.neutral;
        bullet.SoundRange = 15;

        CurrentWeaponWrapper.MakePickable();
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bullet.item = CurrentWeaponWrapper;
        Vector3 throwDirection = new Vector3(CurrentWeaponWrapper.CurrentWeaponObject.position.x - transform.position.x, 0, CurrentWeaponWrapper.CurrentWeaponObject.position.z - transform.position.z);
        bullet.DestroyAfterDamage = false;
       
        //CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<Collider>().enabled = true;
        //CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<Collider>().isTrigger = false;
        CurrentWeaponWrapper = null;

        bulletRB.AddForce(throwDirection * 1000, ForceMode.Acceleration);
        BreakAttackCoroutines();
        Debug.Log("bullet fired!");

    }
}
