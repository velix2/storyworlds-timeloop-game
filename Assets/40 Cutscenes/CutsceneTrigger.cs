using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneTrigger : MonoBehaviour
{
    private CutsceneManager cutsceneManager;
    [SerializeField] private TimelineAsset cutscene;

    // Replace with ink variable
    private bool cutsceneHasPlayed = false;

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
        //Debug.Log(other.tag);
        if (cutsceneHasPlayed) return;
        if (other.CompareTag("Player") && !cutsceneManager.CutsceneIsPlaying) 
        {
            cutsceneHasPlayed = true;
            cutsceneManager.PlayCutscene(cutscene);
        }
    }
}
