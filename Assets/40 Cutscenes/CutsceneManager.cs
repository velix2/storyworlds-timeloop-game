using System;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    public bool CutsceneIsPlaying { get; private set; }
    public static CutsceneManager Instance { get; private set; }

    public static event Action CutsceneStarted;
    public static event Action CutsceneEnded;

    private Action onCutsceneFinishedCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Found more than one Cutscene Manager in the scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CutsceneIsPlaying = false;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        director.stopped += OnCutsceneFinished;
    }


    public void PauseCutscene()
    {
        if(director.state == PlayState.Playing)
        {
            director.Pause();
            Debug.Log("Position set");
        }
    }

    public void ContinueCutscene()
    {
        if (director.state == PlayState.Paused)
        {
            director.Play();
        }
    }

    public void StopCutscene()
    {
        director.Stop();
    }

    public void PlayCutscene(PlayableAsset cutscene, Action callback = null)
    {
        CutsceneIsPlaying = true;

        CutsceneStarted?.Invoke();

        onCutsceneFinishedCallback = callback;

        director.playableAsset = cutscene;
        director.Play();
        Debug.Log("Cutscene started playing");
        
    }

    private void OnCutsceneFinished(PlayableDirector director)
    {
        CutsceneIsPlaying = false;

        CutsceneEnded?.Invoke();
        onCutsceneFinishedCallback?.Invoke();
        onCutsceneFinishedCallback = null;
        Debug.Log("Cutscene has finished playing");
    }

    void OnDisable() 
    { 
        director.stopped -= OnCutsceneFinished;
    }


}

    

