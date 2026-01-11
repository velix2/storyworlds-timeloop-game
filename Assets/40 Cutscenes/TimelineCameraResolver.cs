using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineCameraResolver : MonoBehaviour
{
    public PlayableDirector director;

    void Start()
    {
        var provider = FindFirstObjectByType<SceneCameraProvider>();
        var timeline = (TimelineAsset)director.playableAsset;

        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is CinemachineTrack cmTrack)
            {
                director.SetGenericBinding(
                    cmTrack,
                    Camera.main.GetComponent<CinemachineBrain>()
                );

                foreach (var clip in cmTrack.GetClips())
                {
                    var shot = clip.asset as CinemachineShot;
                    if (shot == null) continue;

                    var id = shot.VirtualCamera.exposedName.ToString();
                    var cam = provider.GetCamera(id);

                    director.SetReferenceValue(shot.VirtualCamera.exposedName, cam);
                }
            }
        }
    }
}
