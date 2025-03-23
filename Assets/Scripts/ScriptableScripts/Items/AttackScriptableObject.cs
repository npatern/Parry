using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Attack", menuName = "ScriptableObjects/Attack", order = 1)]
public class AttackScriptableObject : ScriptableObject
{
    public string ID;
    public string Name;
    public string Description;
    public float DamageMultiplayer = 0;
    public AttackStep[] AttackStepss;
    

    [System.Serializable]
    public class AttackStep
    {
        public string StepName = "Step ";
        public bool LockMovement = false;
        public bool LockRotation = false;
        public ToolsController.targets TargetEnum;
        public float WaitTime = 0;
        public AnimationCurve MovementCurve;
        public bool StayTillGrounded = false;
        public bool OnlyIfGrounded = false;
        public bool SkipIfCanceled = false;
        public bool StayTillContextCanceled = false;
        public bool Interruptable = false;
        public bool IsParrying = false;
        public bool IsProtected = false;
        public bool IsDamaging = false;
        public bool IsFiring = false;
        public bool IsThrowing = false;
        public bool IsDisarming = false;
        public bool SkipIfPlayer = false;
        public Vector3 MovementOffset = Vector3.zero;
        public float SoundRange = 0;
        public Sound.TYPES soundType = Sound.TYPES.neutral;
        public IEnumerator PerformStep(ToolsController toolsController, AttackScriptableObject attackScriptableObject, InputAction.CallbackContext context = new InputAction.CallbackContext())
        {
            toolsController.CurrentWeaponWrapper.CurrentWeaponObject.GetComponent<WeaponModel>().SetWeapon(IsDamaging);
            toolsController.MovementLocked = LockMovement;
            toolsController.RotationLocked = LockRotation;
            Transform GoToTarget = toolsController.GetTargetFromEnum(TargetEnum);
            Vector3 goToPosition = new Vector3(
                GoToTarget.localPosition.x * toolsController.statusController.size.x, 
                GoToTarget.localPosition.y * toolsController.statusController.size.y, 
                GoToTarget.localPosition.z * toolsController.statusController.size.z);
            Quaternion goToRotation = GoToTarget.localRotation;
            StartStep(toolsController);
            float elapsedTime = 0;
            Vector3 currentPos = toolsController.CurrentWeaponWrapper.CurrentWeaponObject.localPosition;
            Quaternion currentRot = toolsController.CurrentWeaponWrapper.CurrentWeaponObject.localRotation;
            if (IsProtected) toolsController.IsProtected = true;
            if (IsParrying) toolsController.IsParrying = true;
            if (IsDamaging) toolsController.IsDamaging = true;
            if (IsDisarming) toolsController.IsDisarming = true;
            

            if (IsFiring==true) toolsController.FireBullet();
            float waitTime = WaitTime / toolsController.GetCombatSpeedMultiplier();
            if (SoundRange > 0) if (!toolsController.statusController.IsDeaf() || soundType ==Sound.TYPES.cover)
                Sounds.MakeSound(new Sound(toolsController.statusController, SoundRange, soundType));

            goToPosition += MovementOffset;
            while (elapsedTime < waitTime)
            {
                //combat.CurrentWeapon.localPosition += Vector3.Lerp(Vector3.zero, combat.WeaponVelocity, Time.deltaTime);
                toolsController.CurrentWeaponWrapper.CurrentWeaponObject.localPosition = Vector3.LerpUnclamped(currentPos, goToPosition, MovementCurve.Evaluate(elapsedTime / WaitTime));
                toolsController.CurrentWeaponWrapper.CurrentWeaponObject.localRotation = Quaternion.LerpUnclamped(currentRot, goToRotation, MovementCurve.Evaluate(elapsedTime / WaitTime));
                elapsedTime += Time.deltaTime;

                if (toolsController.IsDamaging)
                {
                    toolsController.CastDamage(toolsController.CurrentWeaponWrapper.Damage* attackScriptableObject.DamageMultiplayer);
                    
                }
                //combat.WeaponVelocity = goToPosition-combat.CurrentWeapon.localPosition/Time.deltaTime;
                yield return null;
            }
            while (StayTillGrounded && !toolsController.IsGrounded())
                yield return null;
            while (StayTillContextCanceled && context.performed)
                yield return null;
            // Make sure we got there
            //combat.CurrentWeapon.localPosition = goToPosition;
            //combat.CurrentWeapon.localRotation = goToRotation;
            toolsController.CurrentWeaponWrapper.CurrentWeaponObject.localPosition = Vector3.LerpUnclamped(currentPos, goToPosition, MovementCurve.Evaluate(1));
            toolsController.CurrentWeaponWrapper.CurrentWeaponObject.localRotation = Quaternion.LerpUnclamped(currentRot, goToRotation, MovementCurve.Evaluate(1));
            if (IsThrowing) toolsController.Throw();
            EndStep(toolsController);
            toolsController.MovementLocked = false;
            toolsController.RotationLocked = false;
        }
        public virtual void StartStep(ToolsController toolsController)
        {
            //Debug.Log("Starting " + WaitTime + " seconds wait.");
        }
        public virtual void EndStep(ToolsController toolsController)
        {
            //Debug.Log("Step executed after " + WaitTime + " seconds.");
        }
    }
}