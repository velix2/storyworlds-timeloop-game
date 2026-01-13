using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    public bool CutsceneIsPlaying { get; private set; }
    public static CutsceneManager Instance { get; private set; }

    // Events for Cutscenes
    public static event Action CutsceneStarted;
    public static event Action CutsceneEnded;
    public static event Action CutscenePaused;      // When director.paused() is called
    public static event Action CutsceneContinue;

    #region Unity setup
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

    private void OnEnable()
    {
        director.stopped += OnCutsceneFinished;
    }

    void OnDisable()
    {
        director.stopped -= OnCutsceneFinished;
    }
    #endregion


    /// <summary>
    /// Call this funtion to play a cutscene
    /// </summary>
    /// <param name="cutscene"></param>
    public void PlayCutscene(TimelineAsset cutscene)
    {
        Debug.Log("Cutscene started playing");
        // Set flag
        CutsceneIsPlaying = true;

        // Prepare timeline
        director.playableAsset = cutscene;
        BindTimeline(cutscene);

        // Freeze player
        CutsceneStarted?.Invoke();

        // Start the timeline
        director.Play();
    }

    public void PauseCutscene()
    {
        if (director.state == PlayState.Playing)
        {
            // Freeze player animation
            CutscenePaused?.Invoke();

            // Pause cutscene
            director.Pause();
        }
    }

    public void ContinueCutscene()
    {
        if (director.state == PlayState.Paused)
        {
            // Unfreeze player animation
            CutsceneContinue?.Invoke();

            // Resume cutscene
            director.Play();
        }
    }

    public void StopCutscene()
    {
        director.Stop();
    }

    /// <summary>
    /// Since Unity deletes all Bindings when switching the scene, we have to bind them manually
    /// </summary>
    /// <param name="cutscene"></param>
    private void BindTimeline(TimelineAsset cutscene)
    {
        foreach (var track in cutscene.GetOutputTracks())
        {
            // Set bindings for animation track
            if (track is AnimationTrack)
            {
                if (track.name == "Marcus")
                {
                    GameObject player = GameObject.FindWithTag("Player");
                    director.SetGenericBinding(track, player.GetComponent<Animator>());
                }
                else
                {
                    string npcName = track.name;
                    GameObject npc = GameObject.Find(npcName);

                    if(npc == null)
                    {
                        Debug.LogError("No NPC with name \"" +  npcName + "\" could be found on the scene. " +
                            "Make sure track name and object name are the same.");
                        return;
                    }
                    director.SetGenericBinding(track, npc.GetComponent<Animator>());
                }
            }

            // For Signal Track
            if (track is SignalTrack)
            {
                CutsceneManager cutsceneManager = FindAnyObjectByType<CutsceneManager>();

                if (cutsceneManager == null)
                {
                    Debug.LogError("No cutscene manager on the scene");
                    return;
                }

                SignalReceiver receiver = cutsceneManager.GetComponent<SignalReceiver>();

                if (receiver == null)
                {
                    Debug.LogError("There is no SignalReceiver on CutsceneManager-Object");
                    return;
                }

                director.SetGenericBinding(track, receiver);
            }

            // For camera bindings
            if(track is CinemachineTrack)
            {
                GameObject mainCamera = GameObject.FindWithTag("MainCamera");

                if(mainCamera == null)
                {
                    Debug.LogError("No camera with tag MainCamera on the scene");
                    return;
                }

                CinemachineBrain cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();

                if (cinemachineBrain == null)
                {
                    Debug.LogError("No CinemachineBrain on main camera component");
                    return;
                }

                director.SetGenericBinding(track, cinemachineBrain);

                SceneCameraProvider provider = FindFirstObjectByType<SceneCameraProvider>();
                if (provider == null)
                {
                    Debug.LogError("No SceneCameraProvider found on the scene. Please add one to the scene.");
                    return;
                }

                // Set camaeras of all Cinemachine Clips
                foreach (TimelineClip clip in track.GetClips())
                {
                    CinemachineShot shot = clip.asset as CinemachineShot;
                    if (shot == null) continue;

                    string cameraId = clip.displayName;
                    CinemachineCamera camera = provider.GetCamera(cameraId);

                    if (camera == null)
                    {
                        Debug.LogError($"Camera '{cameraId}' not found on SceneCameraProvider script. " +
                            $"Make sure display name of the timeline clip and id on SceneCameraProvider are the same.");
                        continue;
                    }
                    director.SetReferenceValue(shot.VirtualCamera.exposedName, camera);
                }
            }
        }
    }
    
    
    /// <summary>
    /// When cutscene stopped, this function will be called. It switches back to main camera.
    /// </summary>
    /// <param name="director"></param>
    private void OnCutsceneFinished(PlayableDirector director)
    {
        Debug.Log("Cutscene finished playing");

        // Reset flags
        CutsceneIsPlaying = false;
        
        // Unfreeze player
        CutsceneEnded?.Invoke();

    }
}

    

