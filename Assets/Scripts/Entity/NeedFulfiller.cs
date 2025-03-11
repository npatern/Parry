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
            yield return coroutine;
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
    public void ResetFulfiller()
    {
        ChangeGraphics(false);
        if (User != null) 
            if (User.CurrentFulfiller == this)
            {
                User.CurrentFulfiller = null;
                Reserved = false;
            }
                
        User = null;
    }
}
