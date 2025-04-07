using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAwarenessArrow : MonoBehaviour
{
    public SensesController sensesController;
    public StatusController player;
    [SerializeField]
    private UIBarController bar;
    [SerializeField]
    private GameObject eyeContactBar;

    [SerializeField]
    AnimationCurve scaleCurve;
    [SerializeField]
    Vector3 bigScale;
    Vector3 basicScale;
    private bool wasActive = false;
    private float scaleTimer = 0;
    [SerializeField]
    private float scaleTime = 1;

    private bool wasAlerted = false;
    private void Awake()
    {
        player = GameController.Instance.CurrentPlayer;
        basicScale = bar.transform.localScale;
    }
    void Update()
    {
        if (!ShouldBeVisible())
        {
            bar.gameObject.SetActive(false);
            wasActive = false;
            return;
        }
        bar.gameObject.SetActive(true);
        if (wasActive == false)
        {
            scaleTimer = 0;
            wasActive = true;
        }
        if (wasAlerted == false && sensesController.IsAlerted)
        {
            scaleTimer = 0;
            wasAlerted = true;
        }
        wasAlerted = sensesController.IsAlerted;
        NoticeScale();
        if (eyeContactBar !=null)
            eyeContactBar.SetActive(sensesController.currentTarget != null);
        FollowTarget(player.transform);
        LookAtTarget(sensesController.transform);
        if (sensesController.IsAlerted)
            ApplyValues(sensesController.Awareness);
        else
            ApplyValues(sensesController.Awareness, true);
    }
    void NoticeScale()
    {
        if (scaleTimer > scaleTime) 
        {
            bar.transform.localScale = basicScale;
            return;
        }
        scaleTimer += Time.deltaTime;
        bar.transform.localScale = Vector3.LerpUnclamped(bigScale,basicScale,scaleCurve.Evaluate(scaleTimer/ scaleTime));
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
        if (sensesController.currentTarget == null) return false;
        return true;
    }
}
