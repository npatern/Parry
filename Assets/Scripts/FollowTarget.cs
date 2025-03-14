using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    float lerpSpeed = .9f;
    [SerializeField]
    float offsetSize = 1;
    [SerializeField]
    float offsetLerpSpeed = .1f;
    Vector3 offset = Vector3.zero;
    void Update()
    {
         
        if (target == null) return;
        //Rigidbody rb = target.GetComponent<Rigidbody>();
        offset = Vector3.LerpUnclamped(offset, target.transform.forward * offsetSize, offsetLerpSpeed);
        transform.position = Vector3.LerpUnclamped(transform.position, target.position+ offset, lerpSpeed);
         
        //transform.position = Vector3.Lerp(target.position, target.GetComponent<InputController>().target.position, .5f);
    }
    public void ApplyTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
