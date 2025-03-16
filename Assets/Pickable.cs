using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Pickable : MonoBehaviour
{
     
    public ItemWeaponWrapper weaponWrapper;
    [SerializeField]
    ItemWeaponScriptableObject scriptableObject;
    [SerializeField]
    GameObject weaponObject;

    public UnityEvent<StatusController> PickEvent;

    private void Awake()
    {
        if (PickEvent == null)
            PickEvent = new UnityEvent<StatusController>();
        if (weaponWrapper == null) weaponWrapper = new ItemWeaponWrapper(scriptableObject);
        if (weaponObject == null) weaponObject = Instantiate(weaponWrapper.itemType.weaponObject, transform);
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
}
