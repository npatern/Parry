using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interactable : MonoBehaviour
{
    public bool IsInteractable = true;
    public UnityEvent InteractionEvent;

    protected virtual void Awake()
    {
        if (InteractionEvent == null)
            InteractionEvent = new UnityEvent();
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<InteractionController>(out InteractionController interaction))
        {
            interaction.AddToInteractions(this);
        }
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<InteractionController>(out InteractionController interaction))
        {
            interaction.RemoveFromInteractions(this);
        }
    }
    public virtual void Interact(StatusController _status = null)
    {
        InteractionEvent.Invoke();
    }
}
