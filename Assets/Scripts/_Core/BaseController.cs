using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        if (IsInitialized) return;

        OnInitialize();
        IsInitialized = true;
    }
    protected virtual void OnInitialize()
    {
        Debug.Log($"{GetType().Name} initialized");
    }
}