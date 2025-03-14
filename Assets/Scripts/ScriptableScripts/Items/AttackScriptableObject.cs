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
        public CombatController.targets TargetEnum;
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
        public bool SkipIfPlayer = false;
        public Vector3 MovementOffset = Vector3.zero;
        public float SoundRange = 0;
        public Sound.TYPES soundType = Sound.TYPES.neutral;
        public IEnumerator PerformStep(CombatController combat, AttackScriptableObject attackScriptableObject, InputAction.CallbackContext context = new InputAction.CallbackContext())
        {
            combat.CurrentWeapon.GetComponent<WeaponModel>().SetWeapon(IsDamaging);
            combat.MovementLocked = LockMovement;
            combat.RotationLocked = LockRotation;
            Transform GoToTarget = combat.GetTargetFromEnum(TargetEnum);
            Vector3 goToPosition = new Vector3(
                GoToTarget.localPosition.x * combat.statusController.size.x, 
                GoToTarget.localPosition.y * combat.statusController.size.y, 
                GoToTarget.localPosition.z * combat.statusController.size.z);
            Quaternion goToRotation = GoToTarget.localRotation;
            StartStep(combat);
            float elapsedTime = 0;
            Vector3 currentPos = combat.CurrentWeapon.localPosition;
            Quaternion currentRot = combat.CurrentWeapon.localRotation;
            if (IsProtected) combat.IsProtected = true;
            if (IsParrying) combat.IsParrying = true;
            if (IsDamaging) combat.IsDamaging = true;
            if (IsFiring==true) combat.FireBullet();
            float waitTime = WaitTime / combat.GetCombatSpeedMultiplier();
            if (SoundRange > 0) if (!combat.statusController.IsDeaf() || soundType ==Sound.TYPES.cover)
                Sounds.MakeSound(new Sound(combat.statusController, SoundRange, soundType));

            while (elapsedTime < waitTime)
            {
                //combat.CurrentWeapon.localPosition += Vector3.Lerp(Vector3.zero, combat.WeaponVelocity, Time.deltaTime);
                combat.CurrentWeapon.localPosition = Vector3.LerpUnclamped(currentPos+ MovementOffset, goToPosition, MovementCurve.Evaluate(elapsedTime / WaitTime));
                combat.CurrentWeapon.localRotation = Quaternion.LerpUnclamped(currentRot, goToRotation, MovementCurve.Evaluate(elapsedTime / WaitTime));
                elapsedTime += Time.deltaTime;

                if (combat.IsDamaging)
                {
                    combat.CastDamage(combat.CurrentWeaponWrapper.Damage* attackScriptableObject.DamageMultiplayer);
                    
                }
                //combat.WeaponVelocity = goToPosition-combat.CurrentWeapon.localPosition/Time.deltaTime;
                yield return null;
            }
            while (StayTillGrounded && !combat.IsGrounded())
                yield return null;
            while (StayTillContextCanceled && context.performed)
                yield return null;
            // Make sure we got there
            //combat.CurrentWeapon.localPosition = goToPosition;
            //combat.CurrentWeapon.localRotation = goToRotation;
            combat.CurrentWeapon.localPosition = Vector3.LerpUnclamped(currentPos + MovementOffset, goToPosition, MovementCurve.Evaluate(1));
            combat.CurrentWeapon.localRotation = Quaternion.LerpUnclamped(currentRot, goToRotation, MovementCurve.Evaluate(1));
            EndStep(combat);
            combat.MovementLocked = false;
            combat.RotationLocked = false;
        }
        public virtual void StartStep(CombatController combat)
        {
            //Debug.Log("Starting " + WaitTime + " seconds wait.");
        }
        public virtual void EndStep(CombatController combat)
        {
            //Debug.Log("Step executed after " + WaitTime + " seconds.");
        }
    }
}