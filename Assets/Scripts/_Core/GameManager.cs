using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }
   

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
    }
    public void StartGameplay()
    {
        StartCoroutine(StartGameplayRoutine());
    }
    private IEnumerator StartGameplayRoutine()
    {
        yield return SceneManager.LoadSceneAsync(2);
        yield return null;
        ObjectiveController.Instance.Initialize();
        CameraController.Instance.Initialize();
        LevelController.Instance.Initialize();
        //PostProcessController.Instance.Initialize();
    }
}
