using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAwarenessArrow : MonoBehaviour
{
    public SensesController sensesController;
    public StatusController player;
    [SerializeField]
    private UIBarController bar;
    private void Awake()
    {
        player = GameController.Instance.CurrentPlayer;
    }
    void Update()
    {
        if (!ShouldBeVisible())
        {
            bar.gameObject.SetActive(false);
            return;
        }
        bar.gameObject.SetActive(true);
        
        FollowTarget(player.transform);
        LookAtTarget(sensesController.transform);
        if (sensesController.isAware)
            ApplyValues(sensesController.Awareness);
        else
            ApplyValues(sensesController.Awareness, true);
    }
    void LookAtTarget(Transform target)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.5f);
       // Vector3 relative = transform.InverseTransformPoint(target.position);
       // float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
       // transform.rotation = Quaternion.Euler(0, 0, -angle);
        transform.LookAt(screenPosition,Vector3.forward);
    }
    void FollowTarget(Transform target)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.5f);
        transform.position = screenPosition;
    }
    public void ApplyValues(float value, bool recolor = false)
    {
        bar.SetBarValue(value,100,0,recolor);
    }
    bool ShouldBeVisible()
    {
        if (player == null || sensesController == null) return false;
        if (sensesController.Awareness <=0) return false;
        
        return true;
    }
}
