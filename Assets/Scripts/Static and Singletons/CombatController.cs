using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class CombatController : MonoBehaviour
{
    public ToolsController currentAttacker = null;

    public static CombatController _instance;
    public static CombatController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CombatController>();
            }

            return _instance;
        }
    }
    private float nextAttackTime = 0;
    public float attackCooldown = 1f;
    public bool CanIAttack()
    {
        return Time.time >= nextAttackTime;
    }

    public void RegisterAttack()
    {
        nextAttackTime = Time.time + attackCooldown;
    }

}
