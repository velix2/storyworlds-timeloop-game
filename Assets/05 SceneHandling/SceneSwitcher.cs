using System;
using System.Collections;
using System.Collections.Generic;
using FadeToBlack;
using SceneHandling;
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


    [SerializeField] private List<SceneMetaData> sceneMetaDatas;
    
    [SerializeField] private float transitionDelay = 0.25f;
    
    private bool _isSceneLoaded;

    private SceneMetaData _currentSceneMetaData;

    private void Start()
    {
        _currentSceneMetaData =
            sceneMetaDatas.Find(data => data.RepresentedSceneName == SceneManager.GetActiveScene().name);
    }
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
        var targetSceneMetaData = sceneMetaDatas.Find(data => data.RepresentedSceneName == sceneName);
        
        
        // Will be set true when loading finishes
        _isSceneLoaded = false;

        // Show fade to black
        FadeToBlackPanel.Instance.SceneTransitionIn(() =>
            // Invoke Scene Loading when Transition finishes
            SceneManager.LoadScene(sceneName)
        );

        bool timeoutOccured = false;
        // Await scene loaded with a 10 second timeout
        yield return new WaitUntil(() => _isSceneLoaded, TimeSpan.FromSeconds(10), () => timeoutOccured = true);

        // Pass 0 time so that we ensure correct scene setup
        if (!timeoutOccured) TimeHandler.PassTime(0);
        
        // Bring Player to correct location if we have the required meta data
        if (!targetSceneMetaData)
        {
            Debug.LogWarning($"Target Scene {sceneName} not in SceneSwitcher Database. Player will not be teleported.");
        }
        else
        {
            // Find out where player is supposed to spawn
            var playerPos = targetSceneMetaData.GetTransitionPositionOfNeighboringScene(_currentSceneMetaData);

            if (playerPos == Vector3.negativeInfinity)
            {
                Debug.LogWarning($"Target Scene {sceneName} does not have neighbor {_currentSceneMetaData.RepresentedSceneName}. Player will not be teleported.");
            }
            else
            {
                // Find player in scene
                Debug.Log(SceneManager.GetActiveScene().name);
                var playerTransform = FindAnyObjectByType<PlayerController>().transform;

                // Teleport
                playerTransform.position = playerPos;
            }
        }

        // Fade out from transition
        FadeToBlackPanel.Instance.SceneTransitionOut();
    }
}