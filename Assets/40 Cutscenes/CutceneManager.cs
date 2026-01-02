using UnityEngine;
using UnityEngine.Playables;

public class CutceneManager : MonoBehaviour
{
    [SerializeField] private PlayableAsset[] allCutscenes;

    [SerializeField] private PlayableDirector director;
    public bool CutsceneIsPlaying { get; private set; }
    public static CutceneManager Instance { get; private set; }

    


}
