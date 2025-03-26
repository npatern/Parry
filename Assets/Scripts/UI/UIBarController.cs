using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBarController : MonoBehaviour
{
    [SerializeReference]
    public Stat stat;
    [SerializeReference]
    EffectVisuals visuals;
    [SerializeField]
    private RectTransform bar;
    [SerializeField]
    float Value = 100;
    [SerializeField]
    float minScale = 0;
    [SerializeField]
    bool vertical = false;
    Color color;
    [SerializeField]
    Color specialColor;
    [SerializeField]
    float timeValue = 100;
    [SerializeField]
    Image timeBar;
    [SerializeField]
    private bool haveSeparators = false;
    [SerializeField]
    private float separatedValue = 100;
    [SerializeField]
    private RectTransform separatorParent;
    [SerializeField]
    private GameObject separatorObject;
    [SerializeField]
    private Image EffectIcon;

    Vector3 scale;
    private void Start()
    {
        if (bar == null) return;
        if (bar.GetComponent<Image>() == null) return;
        if (stat == null)
        color = bar.GetComponent<Image>().color;
        scale = transform.localScale;
    }
    public void ApplyStatVisuals()
    {
        if (stat == null) return;
        foreach (EffectVisuals _visuals in GameController.Instance.ListOfAssets.EffectVisuals)
        {
            if (stat.type == _visuals.type) 
            {
                visuals = _visuals;
            }
            
        }
        if (visuals == null)
        {
            Debug.LogError("Visuals not found!");
            return;
        }
        color = visuals.color;
        specialColor = visuals.activeColor;
        EffectIcon.sprite = visuals.sprite;
    }
    public bool SetCircleBar()
    {
        if (stat.Points == stat.minPoints) return false;
        bar.localScale = new Vector3(bar.localScale.x, stat.GetValue(), bar.localScale.y);
        timeValue = stat.GetTimerValue();
        timeBar.fillAmount = timeValue;
        transform.localScale = scale + scale *.5f* (stat.waitTimer/stat.waitTime);
        if (stat.IsActive())
            bar.GetComponent<Image>().color = specialColor;
        else
            bar.GetComponent<Image>().color = color;
        return true;
    }
    public void SetBarValue(float value, float maxValue = 100, float minValue = 0, bool recolor = false)
    {
        if (bar == null) return;
        
        if (haveSeparators)
            SetSeparators(maxValue);
        haveSeparators = false;
        float barTotalLength = maxValue - minValue;
        float barCurrentLength = value - minValue;
        float barPercent = barCurrentLength/ barTotalLength;
        barPercent = minScale + (1f - minScale) * barPercent;
        if (vertical)
            bar.localScale = new Vector3(bar.localScale.x, barPercent, bar.localScale.y);
        else
            bar.localScale = new Vector3(barPercent,bar.localScale.y,bar.localScale.y);

        if (bar.GetComponent<Image>() == null) return;
        
        if (recolor)
            bar.GetComponent<Image>().color = specialColor;
        else
            bar.GetComponent<Image>().color = color;
        
    }
    void SetSeparators(float barMaxValue = 100)
    {
        if (separatorParent == null) return;
        int nrOfSeparators = Mathf.CeilToInt(barMaxValue / separatedValue);
        if (barMaxValue<separatedValue) transform.localScale = new Vector3(barMaxValue/separatedValue, transform.localScale.y, transform.localScale.y);
        if (nrOfSeparators <= 0) return;
        while (nrOfSeparators > 0)
        {
            Instantiate(separatorObject, separatorParent);
            nrOfSeparators-=1;
        }

        //healthbarText.text = "";
        //if (nrOfSeparators > 1) healthbarText.text = "" + nrOfSeparators + "x";
    }
}
