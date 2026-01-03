using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    private CutsceneManager cutsceneManager;
    [SerializeField] private int cutsceneIndex;

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
        Debug.Log(other.tag);
        if (other.CompareTag("Player") && !cutsceneManager.CutsceneIsPlaying) 
        {
            cutsceneManager.PlayCutscene(cutsceneIndex);
        }
    }
}
