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
    public IEnumerator ExecuteSteps(EntityController entity)
    {
        for (int i = 0; i<StepsToFulfill.Length; i++)
        {
            //Debug.Log("Fulfilling step " + i + "of fulfilling the need of "+NeedToFulfill.Name);
            IEnumerator coroutine = StepsToFulfill[i].CountDown(entity);
            yield return coroutine;
            //Debug.Log("Fulfilled step " + i + "of fulfilling the need of " + NeedToFulfill.Name);
        }
        //Debug.Log("All steps done. Fulfilling the need");
        entity.NeedFulfilled(true);
        yield break;
    }
}
