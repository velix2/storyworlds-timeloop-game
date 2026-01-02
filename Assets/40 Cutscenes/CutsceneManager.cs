using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableAsset[] allCutscenes;

    [SerializeField] private PlayableDirector director;
    public bool CutsceneIsPlaying { get; private set; }
    public static CutsceneManager Instance { get; private set; }

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
    }

    public void PauseCutscene()
    {
        if(director.state == PlayState.Playing)
        {
            director.Pause();
        }
    }

    public void ContinueCutscene()
    {
        if (director.state == PlayState.Paused)
        {
            director.Play();
        }
    }

    


}
