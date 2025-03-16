using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interactable : MonoBehaviour
{
    public bool IsInteractable = true;

    public UnityEvent<StatusController> InteractionEvent;
    private void Awake()
    {
        if (InteractionEvent == null)
            InteractionEvent = new UnityEvent<StatusController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<InteractionController>(out InteractionController interaction))
        {
            interaction.AddToInteractions(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<InteractionController>(out InteractionController interaction))
        {
            interaction.RemoveFromInteractions(this);
        }
    }
}
