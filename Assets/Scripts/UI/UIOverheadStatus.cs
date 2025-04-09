using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIOverheadStatus : MonoBehaviour
{
    [SerializeField]
    private UIBarController HealthBar;
    [SerializeField]
    private UIBarController LightBar;
    [SerializeField]
    private UIBarController AwarenessBar;
    [SerializeField]
    private UIBarController SoundBar;
    [SerializeField]
    private UIBarController[] CircleBars= new UIBarController [0];

    [SerializeField]
    private TMP_Text healthbarText;

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
    [SerializeField]
    private Image enforcerDot;


    [SerializeField]
    private RectTransform CircleBarsParent;
    [SerializeField]
    private GameObject CircleBarTemplate;

    private float InfoTextTimer = 0;
    [SerializeField]
    private TextMeshProUGUI InfoText;
    private float lastHealth;
    private float visibilityTime = 10;
    private float visibilityTimer = 0;
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
        lastHealth = statusController.Life;
        SpawnCircles();
    }
    public void SpawnCircles()
    {
        CircleBars = new UIBarController[statusController.stats.stats.Length];
        for (int i = 0; i < CircleBars.Length; i++)
        {
            //CircleBars[i] = Instantiate(CircleBarTemplate, CircleBarsParent, false).GetComponent<UIBarController>();
            CircleBars[i] = Instantiate(CircleBarTemplate).GetComponent<UIBarController>();
            CircleBars[i].transform.SetParent(CircleBarsParent, false);
            CircleBars[i].transform.localScale = Vector3.one;
            CircleBars[i].stat = statusController.stats.stats[i];
            CircleBars[i].ApplyStatVisuals();
        }
    }
    private void Update()
    {

        if (statusController == null || statusController.IsKilled)
        {
            Destroy(this.gameObject);
            return;
        }
        foreach (UIBarController bar in CircleBars)
        {
            bar.transform.localScale = Vector3.one;
            bar.gameObject.SetActive(bar.SetCircleBar());
        }
        if (sensesController != null && sensesController.IsTargetBurned()) enforcerDot.gameObject.SetActive(true);
        else enforcerDot.gameObject.SetActive(false);

        visibilityTimer -= Time.deltaTime;
        
        ApplyValues(statusController.Life);
        FollowTarget(statusController.headTransform);

        InfoTextTimer -= Time.deltaTime;
        if (InfoText == null) return;
        if (InfoTextTimer < 0) InfoText.gameObject.SetActive(false);
    }
    public void ShowInfoText(string text = "")
    {
        if (InfoText == null) return;
        InfoText.gameObject.SetActive(true);
        InfoTextTimer = .1f;
        InfoText.text = text;
        
    }
    public void ApplyValues(float health)
    {
        HealthBar.gameObject.SetActive(ApplyHealthBar(health));
       // PostureBar.gameObject.SetActive(ApplyStunBar(posture, isStunned));
        LightBar.gameObject.SetActive(ApplyLightBar());
        AwarenessBar.gameObject.SetActive(ApplyAwarenessBar());
        SoundBar.gameObject.SetActive(ApplySoundBar());
    }
    bool ApplyHealthBar(float health)
    {
        if (health != lastHealth)
        {
            visibilityTimer = visibilityTime;
            lastHealth = health;    
        }
        if (visibilityTimer <= 0) return false;
        if (statusController.MaxLife <= 1) return false;
        if (statusController.MaxLife <= 0) return false;
        HealthBar.SetBarValue(health, statusController.MaxLife,0);
        healthbarText.text = statusController.Life +"";
        
        return true;
    }
    void FollowTarget(Transform target)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 3);
        transform.position = screenPosition;
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
        return false;
        /*
        if (!statusController.IsDeaf()) return false;
        //if (outwardController == null) return false;
        //if (!outwardController.AffectedByLight) return false;
        float value = statusController.GetDeafTimerValue();
        SoundBar.SetBarValue(value, 1, 0);
        if (value <= 0) return false;
        return true;
        */
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
