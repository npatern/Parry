using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    MeshRenderer[] renderers;
    [SerializeField]
    Transform buildingParent;
    private void Start()
    {
        if (buildingParent == null) buildingParent = transform;
        renderers = buildingParent.GetComponentsInChildren<MeshRenderer>();
    }
    public void PlayerEntered(bool isIn)
    {
        if (isIn)
            foreach (MeshRenderer renderer in renderers)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        else
            foreach (MeshRenderer renderer in renderers)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<StatusController>( out StatusController entity))
            if (entity.IsPlayer) PlayerEntered(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<StatusController>(out StatusController entity))
            if (entity.IsPlayer) PlayerEntered(false);
    }
}
