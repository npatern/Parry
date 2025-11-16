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
    public ItemWeaponWrapper BackWeaponWrapper;
    [SerializeReference]
    public ItemWeaponWrapper EmptyWeaponWrapper;
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
    public bool IsDisarming = false;
    public bool IsIllegal = false;
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
        EmptyWeaponWrapper = new ItemWeaponWrapper(GameplayController.Instance.ListOfAssets.EmptyWeapon);

        //if (DequippedWeaponWrapper == null) DequippedWeaponWrapper = new ItemWeaponWrapper(TESTDequippedWeaponScriptable);
        //if (DequippedWeapon == null) DequippedWeapon = Instantiate(DequippedWeaponWrapper.itemType.weaponObject, transform).transform;
    }
    private void Start()
    {

        statusController.StartStunEvent.AddListener(PerformStunned);
        statusController.IsAttackedEvent.AddListener(PerformAttacked);
        //statusController.IsKilledEvent.AddListener(PerformAttacked);
        if (TESTCurrentWeaponScriptable == null) return;
        EquipItem(new ItemWeaponWrapper(TESTCurrentWeaponScriptable));
        if (CurrentWeaponWrapper == null) EquipItem(EmptyWeaponWrapper);
    }
    private void FixedUpdate()
    {
        if (CurrentWeaponWrapper == null) EquipItem(EmptyWeaponWrapper);
    }
    public float GetCombatSpeedMultiplier()
    {
        float multiplier = 1;
        multiplier *= statusController.SlowmoSpeedMultiplier;
        multiplier *= statusController.attackSpeedMultiplier;
        //dodac mnoznik z broni i roznych efektow jak freeze
        return multiplier;
    }
    public void EquipWeaponFromPickable(Pickable pickable)
    {
        ItemWeaponWrapper weaponWrapper = pickable.weaponWrapper;
        weaponWrapper.RemovePickable(transform, true);
        EquipItem(weaponWrapper);
    }
    public bool EquipItem(ItemWeaponWrapper weaponWrapper)
    {
        if (weaponWrapper == null) return false;
        weaponWrapper.location = ItemLocation.Hands;
        //deal with earlier wrapper
        if (CurrentWeaponWrapper != null)
        {
            if (CurrentWeaponWrapper == weaponWrapper) return true;

            if (TryAddStackToObjectInHands(weaponWrapper)) return true;
            
            DequipOrReplaceWeaponInHands(weaponWrapper);
        }
        //if (CurrentWeaponWrapper.emptyhanded)

        //deal with existing inventory
        if (!weaponWrapper.emptyhanded)
            if (!inventoryController.allItems.Contains(weaponWrapper))
            {
                ItemWeaponWrapper matchingItem = inventoryController.GetItemWithMatchingTag(weaponWrapper);
                if (matchingItem!=null)
                {
                    if (matchingItem.Stackable)
                    {
                        matchingItem.MergeToMe(weaponWrapper);
                        EquipItem(matchingItem);
                        return true;
                    }
                }

               // Debug.Log("Item not found " + weaponWrapper.name);
                inventoryController.AddToInventory(weaponWrapper);
            }

        //
        
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
        //jesli dlonie sa puste
        if (CurrentWeaponWrapper.emptyhanded)
        {
            CurrentWeaponWrapper.DestroyPhysicalPresence();
            CurrentWeaponWrapper = null;
            return;
        }
        //jesli wkladamy do rak obiekt ktory juz tam jest
        
        //wyrzuc z rêki
        DequipWeaponFromHands();
    }
    public bool TryAddStackToObjectInHands(ItemWeaponWrapper itemToReplaceWith)
    {
        if (itemToReplaceWith.ID == CurrentWeaponWrapper.ID)
        {
            if (CurrentWeaponWrapper.Stackable)
            {
                CurrentWeaponWrapper.MergeToMe(itemToReplaceWith);
                return true;
            }
          
            else
            {
                //something to add about picking up copy of the same gun, and taking only ammo

               // itemToReplaceWith.DestroyPhysicalPresence();
                //return true;
            }
            
        }
        return false;
    }
    public void DequipWeaponFromHands()
    {
        if (CurrentWeaponWrapper.emptyhanded) return;
        if (TryHideItemFromHands()) return;
        DropWeaponFromHands();
    }
    public Pickable DropOneWeaponFromHands()
    {
        if (CurrentWeaponWrapper == null) return null;
        if (CurrentWeaponWrapper.emptyhanded) return null;
        Pickable pickableToReturn;
        if (CurrentWeaponWrapper.Stackable && CurrentWeaponWrapper.stack > 1)
        {
            ItemWeaponWrapper _newWeaponWrapper = CurrentWeaponWrapper.TakeOneFromStack();

            pickableToReturn = _newWeaponWrapper.MakePickable();
            if (CurrentWeaponWrapper.CurrentWeaponObject!=null)
            pickableToReturn.transform.position = CurrentWeaponWrapper.CurrentWeaponObject.position;
            //CurrentWeaponWrapper = null;
        }
        else
        {
            pickableToReturn= DropWeaponFromHands();
            
        }
        return pickableToReturn;
    }
    public Pickable DropWeaponFromHands()
    {
        if (CurrentWeaponWrapper == null) return null;
        if (CurrentWeaponWrapper.emptyhanded) return null;
        Pickable pickableToReturn;
        pickableToReturn = inventoryController.RemoveFromInventory(CurrentWeaponWrapper).pickable;
        CurrentWeaponWrapper = null;
        return pickableToReturn;
    }
    public ItemWeaponWrapper GetWeaponOnTheBack()
    {
        return inventoryController.GetWeaponOnTheBack();
    }
    public bool TryHideItemFromHands(bool priority = false)
    {
        if (CurrentWeaponWrapper.emptyhanded) return false;
        if (inventoryController == null) return false;
        // bool isInInventory = inventoryController.AddToInventory(CurrentWeaponWrapper);
        // if (isInInventory)
        if (CurrentWeaponWrapper.Big)
        {
            if (inventoryController.ChangeLocation(CurrentWeaponWrapper, ItemLocation.Back, priority))
            {
                CurrentWeaponWrapper.CurrentWeaponObject.localPosition = CurrentWeaponWrapper.attackPattern.Unequipped.localPosition;
                CurrentWeaponWrapper.CurrentWeaponObject.localRotation = CurrentWeaponWrapper.attackPattern.Unequipped.localRotation;
            }
        }
        else
        {
            CurrentWeaponWrapper.location = ItemLocation.Inventory;
            CurrentWeaponWrapper.DestroyPhysicalPresence();
        }
            CurrentWeaponWrapper = null;
            return true;
    }
    public bool CanPerform()
    {
        if (IsUsingTool) return false;
        if (statusController.IsStunned()) return false;
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
    public void PerformIdle()
    {
        BreakAttackCoroutines();
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Equip));
    }

    public void PerformStunned()
    {
        BreakAttackCoroutines();
        if (CurrentWeaponWrapper!=null)
        attack = StartCoroutine(PlayAttackSteps(CurrentWeaponWrapper.itemType.Stunned));
    }
    public void PerformAttacked()
    {
        BreakAttackCoroutines();
        if (CurrentWeaponWrapper == null) return;
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
        ClearStateEffects();
    }
    private void ClearStateEffects()
    {
        IsParrying = false; 
        IsProtected = false;
        IsDamaging = false;
        IsDisarming = false;
        IsIllegal = false;
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
        Bullet bullet = Instantiate(CurrentWeaponWrapper.itemType.bullet, weaponModel.StartPoint.position, CurrentWeaponWrapper.CurrentWeaponObject.transform.rotation, GameplayController.Instance.GarbageCollector.transform).GetComponent<Bullet>();
        bullet.damage = CurrentWeaponWrapper.bulletEffects;
        bullet.destroyObject = bullet.damage.deathEffectObjectToSpawn;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddRelativeForce(Vector3.forward * 2000, ForceMode.Acceleration);
    }   
    public void Throw()
    {
        ItemWeaponWrapper thrownWrapper;

        if (CurrentWeaponWrapper.Stackable && CurrentWeaponWrapper.stack > 1)
        {
            thrownWrapper = CurrentWeaponWrapper.TakeOneFromStack();
            thrownWrapper.SpawnWeaponObjectAsCurrentObject();
            thrownWrapper.CurrentWeaponObject.position = CurrentWeaponWrapper.CurrentWeaponObject.position;
            thrownWrapper.CurrentWeaponObject.rotation = CurrentWeaponWrapper.CurrentWeaponObject.rotation;
        }
        else
        {
            thrownWrapper = CurrentWeaponWrapper;
            inventoryController.allItems.Remove(CurrentWeaponWrapper);
            CurrentWeaponWrapper = null;
        }
        thrownWrapper.MakePickable();

        //WeaponModel weaponModel = thrownWrapper.CurrentWeaponObject.GetComponent<WeaponModel>();
        Bullet bullet = thrownWrapper.CurrentWeaponObject.gameObject.AddComponent<Bullet>();
        bullet.isDamaging = true;
        bullet.multiplier *= 1;
        bullet.damage = thrownWrapper.Effects;
        bullet.destroyObject = thrownWrapper.Effects.deathEffectObjectToSpawn;
        bullet.soundType = Sound.TYPES.neutral;
        bullet.SoundRange = 15;
        if (thrownWrapper.isPrimeable) bullet.primed = true;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bullet.item = thrownWrapper;
        Vector3 throwDirection = new Vector3(thrownWrapper.CurrentWeaponObject.position.x - transform.position.x, 0, thrownWrapper.CurrentWeaponObject.position.z - transform.position.z);
        bullet.DestroyAfterDamage = false;
        bulletRB.AddForce(throwDirection * 1200, ForceMode.Acceleration);
        if (CurrentWeaponWrapper==null)
            BreakAttackCoroutines();
        //Debug.Log("weapon thrown");
    }
}
