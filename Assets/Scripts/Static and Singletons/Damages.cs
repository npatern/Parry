using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Damages 
{
    public static void SendDamage(DamageField damageField)
    {
        if (damageField.range <= 0) return;
        if (damageField.damageEffects.Damage <= 0 && damageField.damageEffects.effectList.Count<=0) return;

        //Gizmos.DrawSphere(damageField.position, damageField.range);
        Debug.DrawRay(damageField.position , Vector3.up * damageField.range,Color.magenta,.2f);
        Debug.DrawRay(damageField.position , Vector3.right * damageField.range, Color.magenta, .2f);
        Debug.DrawRay(damageField.position, Vector3.left * damageField.range, Color.magenta, .2f);
        Debug.DrawRay(damageField.position, Vector3.forward * damageField.range, Color.magenta, .2f);
        Debug.DrawRay(damageField.position, Vector3.back * damageField.range, Color.magenta, .2f);

        Collider[] colliders; 
        switch (damageField.colliderType)
        {
            case DamageField.ColliderTypes.box:
                colliders = Physics.OverlapBox(damageField.position, damageField.endPosition, damageField.orientation);
                break;
            case DamageField.ColliderTypes.capsule:
                colliders = Physics.OverlapCapsule(damageField.position, damageField.endPosition, damageField.range);
                break;
            default:
                colliders = Physics.OverlapSphere(damageField.position, damageField.range);
                break;
        }
        foreach (Collider col in colliders)
            if (col.TryGetComponent(out StatusController targetStatus))
            {
                if (damageField.statusController != null)
                    if (damageField.statusController == targetStatus) continue;

                targetStatus.TryTakeDamage(damageField.damageEffects, damageField.position, 1, damageField.statusController);
            }
    }
}
public class DamageField
{
    public float range = 0;
    public Vector3 position;
    public Vector3 endPosition;
    public DamageEffects damageEffects;
    public StatusController statusController;
    public Quaternion orientation = Quaternion.identity;
    public enum ColliderTypes { sphere,box,capsule};
    public ColliderTypes colliderType;
    public DamageField(float _range, Vector3 _position, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        colliderType = ColliderTypes.sphere;
        range = _range;
        position = _position;
        damageEffects = _damageEffects;
        statusController = _statusController;
        orientation = Quaternion.identity;
    }
    //capsule
    public DamageField(float _range, Vector3 _position, Vector3 _endPosition, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        colliderType = ColliderTypes.capsule;
        range = _range;
        position = _position;
        endPosition = _endPosition;
        damageEffects = _damageEffects;
        statusController = _statusController;
        orientation = Quaternion.identity;
    }
    public DamageField(Quaternion _orientation,  Vector3 _position, Vector3 _endPosition, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        colliderType = ColliderTypes.box;
        orientation = _orientation;
        range = 0;
        position = _position;
        endPosition = _endPosition;
        damageEffects = _damageEffects;
        statusController = _statusController;
    }
    public DamageField(BoxCollider _collider, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        colliderType = DamageField.ColliderTypes.box;
        position = _collider.transform.position;
        endPosition = 0.5f * new Vector3(
            _collider.size.x * _collider.transform.lossyScale.x,
            _collider.size.y * _collider.transform.lossyScale.y,
            _collider.size.z * _collider.transform.lossyScale.z);

        orientation = _collider.transform.rotation;
        range = 1;
        damageEffects = _damageEffects;
        statusController = _statusController;
    }
    public DamageField(SphereCollider _collider, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        colliderType = ColliderTypes.sphere;
        position = _collider.transform.position;
        orientation = _collider.transform.rotation;
        range = _collider.radius;
        if (_collider.transform.lossyScale.x > _collider.transform.lossyScale.y && _collider.transform.lossyScale.x > _collider.transform.lossyScale.z)
            range *= _collider.transform.lossyScale.x;
        if (_collider.transform.lossyScale.y > _collider.transform.lossyScale.x && _collider.transform.lossyScale.y > _collider.transform.lossyScale.z)
            range *= _collider.transform.lossyScale.y;
        if (_collider.transform.lossyScale.z > _collider.transform.lossyScale.x && _collider.transform.lossyScale.z > _collider.transform.lossyScale.y)
            range *= _collider.transform.lossyScale.z;

        damageEffects = _damageEffects;
        statusController = _statusController;
    }
    /*
    public DamageField(Collider _collider, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        colliderType = GetColliderTypeFromCollider(_collider);
        switch (colliderType)
        {
            case DamageField.ColliderTypes.sphere:
                position = _collider.transform.position;
                
                break;

        }
    }
    */
    ColliderTypes GetColliderTypeFromCollider(Collider _collider)
    {
        if (_collider.GetType() == typeof(CapsuleCollider)) return ColliderTypes.capsule;
        if (_collider.GetType() == typeof(BoxCollider)) return ColliderTypes.box;
        if (_collider.GetType() == typeof(SphereCollider)) return ColliderTypes.sphere;
        Debug.LogError("Wrong type of collider");
        return ColliderTypes.sphere;
    }

}
