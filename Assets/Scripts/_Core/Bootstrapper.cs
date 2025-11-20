using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Bootstrapper : MonoBehaviour
{

    private List<BaseManager> managers;

    private static bool initialized = false;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad()
    {
        if (!initialized)
        {
            SceneManager.LoadScene("Bootstrapper");
            initialized = true;
        }
    }
    private void Awake()
    {
        managers = FindObjectsOfType<BaseManager>().ToList();
    }
    private void Start()
    {
        foreach (var manager in managers)
            manager.Initialize();
        Debug.Log("All managers initialized.");
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
