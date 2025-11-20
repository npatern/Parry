using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    public virtual void Initialize() 
    {
        Debug.Log($"{name} initialized.");
    }
    public virtual void OnLevelLoaded() { }
}