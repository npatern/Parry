using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
public class InteractionController : MonoBehaviour
{
    public List<IInteractable> Interactables = new List<IInteractable>();
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
    public void AddToInteractions(IInteractable interactable)
    {
        if (Interactables.Contains(interactable)) return;
        Interactables.Add(interactable);
    }
    public void RemoveFromInteractions(IInteractable interactable)
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
        if (Interactables.Count <= 0) return;
        GetBestInteraction().Interact(statusController);
    }
    public void Pick(Pickable pick)
    {
        if (pick == null) return;
        //Debug.Log("PICK HAPPENS "+ pick.name);
        RemoveFromPicks(pick);
        GetComponent<ToolsController>().EquipWeaponFromPickable(pick);
    }
    public void Pick()
    {
        Pickable pick = GetBestPick();
        Pick(pick);
    }
    public IInteractable GetBestInteraction()
    {
        if (Interactables.Count <= 0) return null;
        if (GetComponent<InputController>() == null || statusController.IsPlayer == false) return null;
        RemoveEmptyInteractables();
        Interactables.Sort((a, b) => Vector3.Distance(((MonoBehaviour)a).transform.position, transform.position).CompareTo(Vector3.Distance(((MonoBehaviour)b).transform.position, transform.position)));
        IInteractable chosenInteractable = null;
        foreach (IInteractable _interactable in Interactables)
        {
            if (_interactable.CanBeInteracted())
            {
                chosenInteractable = _interactable;
                Debug.Log("Found interaction! "+ ((MonoBehaviour)chosenInteractable).gameObject.name);
                break;
            }
        }
        if (chosenInteractable == null) return null;
        GetComponent<InputController>().ShowBindingsText("Interaction", " to use " + ((MonoBehaviour)chosenInteractable).gameObject.name);
        return chosenInteractable;
    }
    public Pickable GetBestPick()
    {
        if (GetComponent<InputController>() == null || statusController.IsPlayer==false) return null;
        if (Pickables.Count <= 0) return null;
        RemoveEmptyPickables();
        Pickables.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
        GetComponent<InputController>().ShowBindingsText("Pick", " to pick " + Pickables[0].gameObject.name);
        return Pickables[0];
    }
    private void RemoveEmptyInteractables()
    {
        for (int i = 0; i < Interactables.Count; i++)
            if (Interactables[i] == null) Interactables.RemoveAt(i);
    }
    private void RemoveEmptyPickables()
    {
        for (int i = 0; i < Pickables.Count; i++)
            if (Pickables[i] == null) Pickables.RemoveAt(i);
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interaction))
        {
          //  if (other.gameObject==gameObject)
           // if (interaction.IsInteractable)
                AddToInteractions(interaction);
        }
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interaction))
        {
           // if (other.gameObject == gameObject)
                RemoveFromInteractions(interaction);
        }
    }
}
