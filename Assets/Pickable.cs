using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Pickable : MonoBehaviour
{
    [SerializeReference]
    public ItemWeaponWrapper weaponWrapper;
    public string wrapperName = "";
    [SerializeField]
    ItemWeaponScriptableObject scriptableObject;
    [SerializeField]
    public GameObject weaponObject;
    public UnityEvent<StatusController> PickEvent;

    private void Awake()
    {
        

        if (PickEvent == null)
            PickEvent = new UnityEvent<StatusController>();
    }
    
    private void Start()
    {
        if (weaponWrapper == null) weaponWrapper = new ItemWeaponWrapper(scriptableObject);
        ApplyWeaponWrapper(weaponWrapper);
    }
    private void Update()
    {
        wrapperName = weaponWrapper.name;
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
                weaponObject = weaponWrapper.SpawnWeaponObject(transform).gameObject;
            else
                weaponObject = weaponWrapper.CurrentWeaponObject.gameObject;
        }
        weaponWrapper.pickable = this;
        gameObject.name = weaponWrapper.itemType.ItemName;
    }
     
    
}
