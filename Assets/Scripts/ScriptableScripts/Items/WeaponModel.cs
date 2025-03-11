using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    
    public Transform StartPoint;
    public Transform EndPoint;
    //public Collider collider;
    public TrailRenderer[] trails;
    private void Awake()
    {
        trails = GetComponentsInChildren<TrailRenderer>();
    }
    public void SetWeapon(bool state)
    {
        SetTrail(state);
    }
    public void SetTrail(bool state)
    {
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = state;
        }
    }
     
}
