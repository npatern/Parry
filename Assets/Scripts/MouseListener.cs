using UnityEngine;
using UnityEngine.AI;

public class MouseListener : MonoBehaviour
{

    RaycastHit m_Hit = new RaycastHit();
    LayerMask layerMask;
    public Camera mainCamera;
    public float raylength = 100f;
    public EntityController HighlightedEntity;
    private void Start()
    {
        layerMask = LayerMask.GetMask("Entity");
        mainCamera = Camera.main;
    }
    void Update()
    {
        HighlightedEntity = null;
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin,ray.direction, out m_Hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(ray.origin, m_Hit.point, Color.yellow);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.green, 1000f);

            Transform hitObject = m_Hit.transform;
            Debug.Log(hitObject.gameObject.name);
            if (hitObject.root.GetComponent<EntityController>() != null)
            {
                HighlightedEntity = hitObject.root.GetComponent<EntityController>();
                Debug.DrawLine(HighlightedEntity.transform.position, HighlightedEntity.transform.position + Vector3.up * 4, Color.green);
            }
        }
    
        if (Input.GetMouseButtonDown(0))
        {
            if (HighlightedEntity != null)
            GameplayController.Instance.SelectEntity(HighlightedEntity);
        }
        if (Input.GetMouseButtonDown(1))
        {
            GameplayController.Instance.DeselectEntity();
        }
    }
}