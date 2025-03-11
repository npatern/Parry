using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    float lerpSpeed = .9f;

    void Update()
    {
        if (target !=null)
        transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed);
    }
    public void ApplyTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
