using Unity.Cinemachine;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Timeline;

public class CutsceneTrigger : MonoBehaviour
{
    private CutsceneManager cutsceneManager;

    [Header("Cutscene Timeline")]
    [SerializeField] private TimelineAsset cutscene;
    [SerializeField] private StateTracker.IntroStates stateToBePlayed;

    [Header("Additional Settings")]
    [SerializeField] private bool SwitchBackToMainCamAfterCutscene = false;
    [SerializeField] private bool SwitchSceneAfterCutscene = false;
    [SerializeField] private string SceneToBeSwitchedTo;

    private void Start()
    {
        cutsceneManager = CutsceneManager.Instance;
        if(cutsceneManager == null)
        {
            Debug.LogError("Cutscene Manager could not be found");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // If we are in last state of Intro or if not in the correct state, return
        if (StateTracker.IntroState == StateTracker.IntroStates.IntroCompleted || StateTracker.IntroState != stateToBePlayed) return;

        // Otherwise cutscene will be played
        if (other.CompareTag("Player") && !cutsceneManager.CutsceneIsPlaying) 
        {
            cutsceneManager.PlayCutscene(cutscene, () =>
            {
                OnCutsceneFinishedInTrigger();
            });
        }
    }

    /// <summary>
    /// Switch back to follow camera
    /// </summary>
    private void SwitchBackToMainCamera()
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

    /// <summary>
    /// Switch the scene after cutscene
    /// </summary>
    /// <param name="sceneName">Scene to be switched</param>
    private void SwitchScene(string sceneName)
    {
        SceneSwitcher.Instance.GoToScene(sceneName);
    }

    /// <summary>
    /// Callback function after cutscene finished playing.
    /// </summary>
    private void OnCutsceneFinishedInTrigger()
    {
        // Transition to new state
        StateTracker.IntroState = stateToBePlayed + 1;
        Debug.Log("Game state is now: " + (stateToBePlayed + 1).ToString());

        // Perform additional steps after cutscene ended
        if (SwitchBackToMainCamAfterCutscene)
        {
            SwitchBackToMainCamera();
        }
        if(SwitchSceneAfterCutscene)
        {
            SwitchScene(SceneToBeSwitchedTo);
        }
        
    }
}
