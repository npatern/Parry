using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
public class InteractionController : MonoBehaviour
{
    public List<Interactable> Interactables = new List<Interactable>();
    public List<Pickable> Pickables = new List<Pickable>();
    StatusController statusController;
    PlayerInput playerInput;

    private void Awake()
    {
        if (GetComponent<StatusController>()!=null)
        statusController = GetComponent<StatusController>();

        playerInput = GetComponent<PlayerInput>();
        //Debug.Log(playerInput.currentActionMap.FindAction("Jump").GetBindingDisplayString();
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
        GetBestInteraction().InteractionEvent.Invoke(statusController);
    }
    public void Pick(Pickable pick)
    {
        if (pick == null) return;
        Debug.Log("PICK HAPPENS "+ pick.name);
        
        GetComponent<ToolsController>().EquipWeapon(pick);
        RemoveFromPicks(pick);
        Destroy(pick.gameObject);
    }
    public void Pick()
    {
        Pickable pick = GetBestPick();
        Pick(pick);
    }
    public Interactable GetBestInteraction()
    {
        if (Interactables.Count <= 0) return null;
        Interactables.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
        if (GetComponent<InputController>() != null && statusController.IsPlayer) GetComponent<InputController>().ShowBindingsText("Interaction", " to use " + Interactables[0].gameObject.name);
        return Interactables[0];
    }
    public Pickable GetBestPick()
    {
        if (Pickables.Count <= 0) return null;
        Pickables.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
        if (GetComponent<InputController>() != null && statusController.IsPlayer) GetComponent<InputController>().ShowBindingsText("Pick", " to pick " + Pickables[0].gameObject.name);
        return Pickables[0];
    }
}
