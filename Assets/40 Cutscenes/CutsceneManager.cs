using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static CameraManager;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    public bool CutsceneIsPlaying { get; private set; }
    public static CutsceneManager Instance { get; private set; }

    public static event Action CutsceneStarted;
    public static event Action CutsceneEnded;

    public static event Action CutscenePaused;
    public static event Action CutsceneContinue;

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
            CutscenePaused?.Invoke();
            director.Pause();
        }
    }

    public void ContinueCutscene()
    {
        if (director.state == PlayState.Paused)
        {
            CutsceneContinue?.Invoke();
            director.Play();
        }
    }

    public void StopCutscene()
    {
        director.Stop();
    }

    public void PlayCutscene(TimelineAsset cutscene, Action callback = null)
    {
        CutsceneIsPlaying = true;

        onCutsceneFinishedCallback = callback;

        director.playableAsset = cutscene;
        BindTimeline(cutscene);
        CutsceneStarted?.Invoke();
        director.Play();

    }

    private void BindTimeline(TimelineAsset cutscene)
    {
        foreach (var track in cutscene.GetOutputTracks())
        {

            if (track is AnimationTrack)
            {
                if (track.name == "Marcus")
                {
                    var player = GameObject.FindWithTag("Player");
                    director.SetGenericBinding(track, player.GetComponent<Animator>());
                }
                else if (track.name.StartsWith("NPC_"))
                {
                    var npcName = track.name.Replace("NPC_", "");
                    var npc = GameObject.Find(npcName);
                    director.SetGenericBinding(track, npc.GetComponent<Animator>());
                }
            }
            if (track is SignalTrack)
            {
                GameObject signalReceiverGameObject = GameObject.Find("CutsceneManager");
                SignalReceiver receiver = signalReceiverGameObject.GetComponent<SignalReceiver>();
                director.SetGenericBinding(track, receiver);
            }
            if(track is CinemachineTrack)
            {
                GameObject mainCamera = GameObject.Find("Main Camera");
                CinemachineBrain cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
                director.SetGenericBinding(track, cinemachineBrain);

                foreach (TimelineClip clip in track.GetClips())
                {
                    SceneCameraProvider provider = FindFirstObjectByType<SceneCameraProvider>();
                    var shot = clip.asset as CinemachineShot;
                    if (shot == null) continue;

                    string cameraId = clip.displayName;
                    CinemachineCamera cam = provider.GetCamera(cameraId);

                    director.SetReferenceValue(shot.VirtualCamera.exposedName, cam);
                }
            }
        }
    }
    private void OnCutsceneFinished(PlayableDirector director)
    {
        Debug.Log("Cutscene has finished playing");

        CutsceneIsPlaying = false;

        onCutsceneFinishedCallback?.Invoke();
        onCutsceneFinishedCallback = null;
        
        CutsceneEnded?.Invoke();

        SwitchBackToMainCam();

    }

    private void SwitchBackToMainCam()
    {
        CameraTransitionZone cameraTransitionZone = FindAnyObjectByType<CameraTransitionZone>();

        CinemachineCamera mainCam;
        if (cameraTransitionZone != null)
        {
            mainCam = cameraTransitionZone.GetMainCamera();
        }
        else
        {
            Debug.LogError("No Camera Transition Zone to switch back to main camera");
            return;
        }

        CameraManager.FocusCam(mainCam);
    }

    void OnDisable() 
    { 
        director.stopped -= OnCutsceneFinished;
    }


}

    

