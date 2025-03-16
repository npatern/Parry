using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class InteractionController : MonoBehaviour
{
    public List<Interactable> Interactables = new List<Interactable>();
    public List<Pickable> Pickables = new List<Pickable>();
    StatusController statusController;
    // EntitiesInGame = new List<EntityController>();
    private void Awake()
    {
        if (GetComponent<StatusController>()!=null)
        statusController = GetComponent<StatusController>();
    }
    private void Update()
    {
        GetBestInteraction();
        GetBestPick();
        // if (GetBestInteraction()!=null)
        //Debug.Log(GetBestInteraction().gameObject.name);
    }
    public void AddToInteractions(Interactable interactable)
    {
        if (Interactables.Contains(interactable)) return;
        Interactables.Add(interactable);
    }
    public void RemoveFromInteractions(Interactable interactable)
    {
        if (Interactables.Contains(interactable))
            Interactables.Remove(interactable);
    }
    public void AddToPicks(Pickable pickable)
    {
        if (Pickables.Contains(pickable)) return;
        Pickables.Add(pickable);
    }
    public void RemoveFromPicks(Pickable pickable)
    {
        if (Pickables.Contains(pickable))
            Pickables.Remove(pickable);
    }
    public void Use()
    {
        Debug.Log("USE function");
        GetBestInteraction().InteractionEvent.Invoke(statusController);
    }
    public void Pick()
    {
        Debug.Log("PICK function");
        GetBestPick().PickEvent.Invoke(statusController);
    }
    public Interactable GetBestInteraction()
    {
        if (Interactables.Count <= 0) return null;
        Interactables.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
        if (statusController != null && statusController.IsPlayer) statusController.OverheadController.ShowInfoText("Press USE button to interact with " + Interactables[0].gameObject.name);
        return Interactables[0];
    }
    public Pickable GetBestPick()
    {
        if (Pickables.Count <= 0) return null;
        Pickables.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
        if (statusController != null && statusController.IsPlayer) statusController.OverheadController.ShowInfoText("Press PICK button to pick " + Pickables[0].gameObject.name);
        return Pickables[0];
    }
    public ItemWeaponWrapper PickItem()
    {
        return GetBestPick().weaponWrapper;
    }
}
