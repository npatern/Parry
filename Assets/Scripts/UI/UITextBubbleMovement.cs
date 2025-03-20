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
    public AnimationCurve ScaleCurve;
    public float MoveDistance = 15f;
    public TextMeshProUGUI Text;
    public Vector3 target;
    public Vector3 targetOffset;
    Vector3 startPos;
    Vector3 endPos;
    float currentLerpTime;
    [SerializeField]
    Vector3 RandomOffset= Vector3.zero;
    protected void Start()
    {
        startPos = transform.position;
        endPos = transform.position + transform.up * MoveDistance;
        Text = GetComponent<TextMeshProUGUI>();
        RandomOffset = new Vector3(
            RandomOffset.x / 2 - Random.Range(0, RandomOffset.x), 
            RandomOffset.y / 2 - Random.Range(0, RandomOffset.y), 
            RandomOffset.z / 2 - Random.Range(0, RandomOffset.z));
    }

    protected void Update()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > LerpTime)
        {
            Destroy(gameObject);
            currentLerpTime = LerpTime;
        }
        float perc = currentLerpTime / LerpTime;
        if (target != null)
        {
            startPos = FollowTarget(target);
            endPos = startPos + transform.up * MoveDistance;
        }
        float scale = Mathf.Lerp(0f, 1f, ScaleCurve.Evaluate(perc));
        transform.position = Vector3.LerpUnclamped(startPos, endPos, MovementCurve.Evaluate(perc));
        float alpha = Mathf.Lerp(0f, 1f, AlphaCurve.Evaluate(perc));
        transform.localScale = Vector3.one * scale;
        Text.text = Speech;
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, alpha);
        
    }
    Vector3 FollowTarget(Vector3 target)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target + targetOffset+ RandomOffset);
        return screenPosition;
    }
}
