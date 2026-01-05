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
        
        // // Assert Scene exists
        // var scene = SceneManager.GetSceneByName(sceneName);
        //
        // if (!scene.IsValid())
        // {
        //     Debug.LogError($"Scene {sceneName} doesn't exist or is not valid");
        //     yield break;
        // }
        
        // Show fade to black
        FadeToBlackPanel.Instance.FadeToBlack();
        
        yield return new WaitForSeconds(transitionDelay);
        
        // Will be set true when loading finishes
        _isSceneLoaded = false;
        
        // Invoke transition
        SceneManager.LoadScene(sceneName);
        
        // Await scene loaded
        yield return new WaitUntil(() => _isSceneLoaded);
        
        // Pass 0 time so that we ensure correct scene setup
        TimeHandler.PassTime(0);
    }
}
