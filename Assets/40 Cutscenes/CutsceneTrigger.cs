using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Timeline;

public class CutsceneTrigger : MonoBehaviour
{
    private CutsceneManager cutsceneManager;

    [Header("Cutscene Timeline")]
    [SerializeField] private TimelineAsset cutscene;
    [SerializeField] private StateTracker.IntroStates whenCompletedState;

    [Header("Additional Settings")]
    [SerializeField] private bool SwitchBackToMainCamAfterCutscene = false;

    private void Start()
    {
        cutsceneManager = CutsceneManager.Instance;
        if(cutsceneManager == null)
        {
            Debug.LogError("Cutscene Manager could not be found");
        }
    }

    private void OnEnable()
    {
        CutsceneManager.CutsceneEnded += OnCutsceneFinishedInTrigger;
    }

    private void OnDisable()
    {
        CutsceneManager.CutsceneEnded -= OnCutsceneFinishedInTrigger;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (StateTracker.IntroState == StateTracker.IntroStates.Init || StateTracker.IntroState != whenCompletedState - 1) return;

        if (other.CompareTag("Player") && !cutsceneManager.CutsceneIsPlaying) 
        {
            cutsceneManager.PlayCutscene(cutscene);
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

    private void OnCutsceneFinishedInTrigger()
    {
        if (SwitchBackToMainCamAfterCutscene)
        {
            SwitchBackToMainCamera();
        }
        StateTracker.IntroState = whenCompletedState;
    }
}
