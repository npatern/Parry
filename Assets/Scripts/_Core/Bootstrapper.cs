using UnityEngine.SceneManagement;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{

    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad()
    {
        if (!initialized)
        {
            SceneManager.LoadScene("Bootstrap");
            initialized = true;
        }
    }

    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
            new GameObject("GameManager").AddComponent<GameManager>();

        if (FindObjectOfType<UIManager>() == null)
            new GameObject("UIManager").AddComponent<UIManager>();

        DontDestroyOnLoad(gameObject);

        SceneManager.LoadSceneAsync("MainMenu");
    }
}
