using System;
using System.Collections;
using FadeToBlack;
using TimeManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private float transitionDelay = 0.25f;

    private bool _isSceneLoaded;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isSceneLoaded = true;
    }

    public void GoToScene(string sceneName)
    {
        StartCoroutine(GoToSceneCoroutine(sceneName));
    }

    private IEnumerator GoToSceneCoroutine(string sceneName)
    {
        bool err = false;
        
        // Will be set true when loading finishes
        _isSceneLoaded = false;

        // Show fade to black
        FadeToBlackPanel.Instance.SceneTransitionIn(() =>
        {
            try
            { 
                // Invoke Scene Loading when Transition finishes
                SceneManager.LoadScene(sceneName);
            }
            catch (Exception e)
            {
                // Fade out again if error occurs
                FadeToBlackPanel.Instance.SceneTransitionOut();
                Debug.LogException(e);
                err = true;
            }


        });
        
        bool timeoutOccured = false;
        // Await scene loaded with a 10 second timeout
        yield return new WaitUntil(() => _isSceneLoaded, TimeSpan.FromSeconds(10), () => timeoutOccured = true);

        // Pass 0 time so that we ensure correct scene setup
        if (!timeoutOccured) TimeHandler.PassTime(0);

        // Fade out from transition
        FadeToBlackPanel.Instance.SceneTransitionOut();
    }
}