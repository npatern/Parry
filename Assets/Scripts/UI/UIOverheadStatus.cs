using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIOverheadStatus : MonoBehaviour
{
    [SerializeField]
    private UIBarController HealthBar;
    [SerializeField]
    private UIBarController PostureBar;
    [SerializeField]
    private UIBarController LightBar;
    [SerializeField]
    private UIBarController AwarenessBar;
    [SerializeField]
    private UIBarController SoundBar;

    public StatusController statusController;
    [SerializeField]
    private OutwardController outwardController;
    [SerializeField]
    private SensesController sensesController;

    [SerializeField]
    private Sprite screamerBold;
    [SerializeField]
    private Sprite screamerStroke;
    [SerializeField]
    private Sprite questionBold;
    [SerializeField]
    private Sprite questionStroke;
    [SerializeField]
    private Image punctuationStroke;
    [SerializeField]
    private Image punctuationBold;
    private void Awake()
    {
        //fast fix so that it doesnt spawn in the middle of the screen for 1 frame
        transform.position = new Vector3(1000, 10000, 0);
    }
    private void Start()
    {
        if (statusController.transform.GetComponent<OutwardController>()!=null)
        {
            outwardController = statusController.GetComponent<OutwardController>();
        }
        if (statusController.transform.GetComponent<SensesController>() != null)
        {
            sensesController = statusController.GetComponent<SensesController>();
        }

    }
    
    private void Update()
    {
        if (statusController == null || statusController.IsKilled)
        {
            Destroy(this.gameObject);
            return;
        }
            
         
        ApplyValues(statusController.Life, statusController.Posture, statusController.IsStunned);
        FollowTarget(statusController.transform);
    }
    public void ApplyValues(float health, float posture, bool isStunned = false)
    {
        HealthBar.SetBarValue(health);
        PostureBar.gameObject.SetActive(ApplyStunBar(posture, isStunned));
        LightBar.gameObject.SetActive(ApplyLightBar());
        AwarenessBar.gameObject.SetActive(ApplyAwarenessBar());
        SoundBar.gameObject.SetActive(ApplySoundBar());
    }

    void FollowTarget(Transform target)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 3);
        transform.position = screenPosition;
    }
    bool ApplyStunBar(float posture, bool isStunned)
    {
        if (posture > 99) return false;
        if (isStunned)
            PostureBar.SetBarValue(100 - posture, 100, 0, true);
        else
            PostureBar.SetBarValue(100 - posture);
        return true;
    }
    bool ApplyLightBar()
    {
        if (outwardController == null) return false;
        if (!outwardController.AffectedByLight) return false;
        float value = outwardController.LightValue;
        LightBar.SetBarValue(value, 1, 0);
        if (value <= 0) return false;
        return true;
    }
    bool ApplySoundBar()
    {
        if (!statusController.IsDeaf()) return false;
        //if (outwardController == null) return false;
        //if (!outwardController.AffectedByLight) return false;
        float value = statusController.GetDeafTimerValue();
        SoundBar.SetBarValue(value, 1, 0);
        if (value <= 0) return false;
        return true;
    }
    bool ApplyAwarenessBar()
    {
        if (sensesController == null) return false;
        if (sensesController.Awareness<=0) return false;
        float value = sensesController.Awareness;
        bool recolor = sensesController.IsAlerted;
        bool isKnown = (sensesController.currentTarget != null && recolor);
        if (isKnown)
        {
            punctuationBold.sprite = screamerBold;
            punctuationStroke.sprite = screamerStroke;
        }
        else
        {
            punctuationBold.sprite = questionBold;
            punctuationStroke.sprite = questionStroke;
        }
        AwarenessBar.SetBarValue(value, 100, 0,recolor);
        return true;
    }
}
