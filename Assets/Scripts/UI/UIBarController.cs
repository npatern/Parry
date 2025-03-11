using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBarController : MonoBehaviour
{
    [SerializeField]
    private RectTransform bar;
    [SerializeField]
    float Value = 100;
    [SerializeField]
    float minScale = 0;

    Color color;
    [SerializeField]
    Color specialColor;
    private void Start()
    {
        if (bar == null) return;
        if (bar.GetComponent<Image>() == null) return;
        color = bar.GetComponent<Image>().color;
    }
    public void SetBarValue(float value, float maxValue = 100, float minValue = 0, bool recolor = false)
    {
        if (bar == null) return;
        float barTotalLength = maxValue - minValue;
        float barCurrentLength = value - minValue;
        float barPercent = barCurrentLength/ barTotalLength;
        barPercent = minScale + (1f - minScale) * barPercent;
        bar.localScale = new Vector3(barPercent,bar.localScale.y,bar.localScale.y);

        if (bar.GetComponent<Image>() == null) return;
        
        if (recolor)
            bar.GetComponent<Image>().color = specialColor;
        else
            bar.GetComponent<Image>().color = color;
        
    }
}
