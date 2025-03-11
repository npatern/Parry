using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UITextBubbleMovement : MonoBehaviour
{
    public string Speech = "Hello world!";
    public float LerpTime = 1f;
    public AnimationCurve MovementCurve;
    public AnimationCurve AlphaCurve;
    public float MoveDistance = 15f;
    public TextMeshProUGUI Text;
    public Vector3 target;

    Vector3 startPos;
    Vector3 endPos;
    float currentLerpTime;
    protected void Start()
    {
        startPos = transform.position;
        endPos = transform.position + transform.up * MoveDistance;
        Text = GetComponent<TextMeshProUGUI>();
    }

    protected void Update()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > LerpTime)
        {
            currentLerpTime = LerpTime;
        }
        float perc = currentLerpTime / LerpTime;
        if (target != null)
        {
            startPos = FollowTarget(target);
            endPos = startPos + transform.up * MoveDistance;
        }
            
        transform.position = Vector3.Lerp(startPos, endPos, MovementCurve.Evaluate(perc));
        float alpha = Mathf.Lerp(0f, 1f, AlphaCurve.Evaluate(perc));
        Text.text = Speech;
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, alpha);
        
    }
    Vector3 FollowTarget(Vector3 target)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target + Vector3.up * 3);
        return screenPosition;
    }
}
