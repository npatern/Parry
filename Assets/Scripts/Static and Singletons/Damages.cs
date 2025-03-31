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

        Collider[] colliders = Physics.OverlapSphere(damageField.position, damageField.range);
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
    public DamageEffects damageEffects;
    public StatusController statusController;
    public DamageField(float _range, Vector3 _position, DamageEffects _damageEffects, StatusController _statusController = null)
    {
        range = _range;
        position = _position;
        damageEffects = _damageEffects;
        statusController = _statusController;
    }
}
