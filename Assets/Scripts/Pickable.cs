using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Pickable : MonoBehaviour
{
    [SerializeReference]
    public ItemWeaponWrapper weaponWrapper;
    public string wrapperName = "";
    [SerializeReference]
    public ItemWeaponScriptableObject scriptableObject;
    public SphereCollider trigger;
    public GameObject weaponObject;
    public UnityEvent<StatusController> PickEvent;

    private void Awake()
    {
        if (PickEvent == null)
            PickEvent = new UnityEvent<StatusController>();
        trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1f;
    }
    
    private void Start()
    {
        if (weaponWrapper == null) weaponWrapper = new ItemWeaponWrapper(scriptableObject);
        ApplyWeaponWrapper(weaponWrapper);
        wrapperName = weaponWrapper.name;
    }
    private void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<InteractionController>(out InteractionController interaction))
        {
            interaction.AddToPicks(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<InteractionController>(out InteractionController interaction))
        {
            interaction.RemoveFromPicks(this);
        }
    }
    public void ApplyWeaponWrapper(ItemWeaponWrapper wrapper)
    {
        if (weaponWrapper == null) weaponWrapper = wrapper;
        if (weaponObject == null)
        {
            if (weaponWrapper.CurrentWeaponObject == null)
                weaponObject = weaponWrapper.SpawnWeaponObjectAsCurrentObject(transform).gameObject;
            else
                weaponObject = weaponWrapper.CurrentWeaponObject.gameObject;
        }
        weaponWrapper.pickable = this;
        gameObject.name = weaponWrapper.itemType.ItemName;
    }
     
    
}
