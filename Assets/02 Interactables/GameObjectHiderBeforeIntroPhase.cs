using UnityEngine;

public class GameObjectHiderBeforeIntroPhase : MonoBehaviour
{
    [SerializeField] private StateTracker.IntroStates visibleFromPhaseIncl;
    void Start()
    {
        gameObject.SetActive(visibleFromPhaseIncl <= StateTracker.IntroState);
    }
    
}
