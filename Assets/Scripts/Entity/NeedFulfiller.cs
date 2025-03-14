using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedFulfiller : MonoBehaviour
{
    public bool Reserved = true;
    public bool Unreservable = false;
    public NeedScriptableObject NeedToFulfill;
    public FulfillmentStepScriptableObject[] StepsToFulfill;
    public EntityController User;
    public Transform UserSpot;
    public GameObject ShowIfUsed;
    public GameObject HideIfUsed;
    public FulfillmentStepScriptableObject currentStep;
    public Vector3 maximumScale = Vector3.zero;
    public Vector3 minimumScale = Vector3.zero;
    private void Awake()
    {
        ChangeGraphics(false);
    }
    public IEnumerator ExecuteSteps(EntityController entity)
    {
        ChangeGraphics(true);
        for (int i = 0; i<StepsToFulfill.Length; i++)
        {
            //Debug.Log("Fulfilling step " + i + "of fulfilling the need of "+NeedToFulfill.Name);
            IEnumerator coroutine = StepsToFulfill[i].CountDown(entity);
            currentStep = StepsToFulfill[i];
            yield return coroutine;
            currentStep = null;
            //Debug.Log("Fulfilled step " + i + "of fulfilling the need of " + NeedToFulfill.Name);
        }
        ChangeGraphics(false);
        //Debug.Log("All steps done. Fulfilling the need");
        entity.NeedFulfilled(true);
        
        yield break;
    }
    private void ChangeGraphics(bool isUsed)
    {
        if (ShowIfUsed!=null)
            ShowIfUsed.SetActive(isUsed);
        if (HideIfUsed != null)
            HideIfUsed.SetActive(!isUsed);
    }
    public bool CanStatusUseIt(StatusController status)
    {
        return IsScaleOk(status.size);
    }
    public bool IsScaleOk(Vector3 scale)
    {
        if (maximumScale!=Vector3.zero)
        if (maximumScale.x < scale.x || maximumScale.y < scale.y || maximumScale.z < scale.z) return false;

        if (minimumScale.x > scale.x || minimumScale.y > scale.y || minimumScale.z > scale.z) return false;
        return true;
    }
    public void ResetFulfiller()
    {
        ChangeGraphics(false);
          
        if (User != null)
        {
            if (currentStep != null)
            {
                currentStep.EndStep(User);
                currentStep = null;
            }
            if (User.CurrentFulfiller == this)
            {
                User.CurrentFulfiller = null;
                Reserved = false;
            }
        }        
        User = null;
    }
}
