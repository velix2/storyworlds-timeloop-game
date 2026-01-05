using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    #region Singleton

    // Singleton stuff

    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static SceneSwitcher Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public void GoToScene(string sceneName)
    {
        
    }
}
