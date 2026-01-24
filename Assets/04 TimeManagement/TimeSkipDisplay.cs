using System;
using UnityEngine;

public class TimeSkipDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        SetDisplayVisibility(!StateTracker.IsInIntro);
        StateTracker.OnIntroStateChanged.AddListener(OnIntroStateChange);
    }

    private void OnIntroStateChange(StateTracker.IntroStates state)
    {
        SetDisplayVisibility(!StateTracker.IsInIntro);
    }

    private void SetDisplayVisibility(bool value)
    {
        
        canvasGroup.alpha = value? 1 : 0;
        canvasGroup.blocksRaycasts = value;
        canvasGroup.interactable = value;
    }
}
