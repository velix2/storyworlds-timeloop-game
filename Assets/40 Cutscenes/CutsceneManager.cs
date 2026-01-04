using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private CutsceneDatabase cutsceneDatabase;

    [SerializeField] private PlayableDirector director;

    [SerializeField] private GameObject player;
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

    public void PlayCutscene(int index)
    {
        if (index >= 0 && index < cutsceneDatabase.cutscenes.Length)
        {
            CutsceneIsPlaying = true;
            director.playableAsset = cutsceneDatabase.cutscenes[index];
            director.Play();
            player.SetActive(false);
            Debug.Log("Cutscene started playing");
        }
        else
        {
            Debug.LogWarning("Invalid index for cutscene database");
        }
    }

    private void OnCutsceneFinished(PlayableDirector director)
    {
        CutsceneIsPlaying = false;
        player.SetActive(true);
        Debug.Log("Cutscene has finished playing");
    }

    void OnDisable() 
    { 
        director.stopped -= OnCutsceneFinished;
    }

}

    

