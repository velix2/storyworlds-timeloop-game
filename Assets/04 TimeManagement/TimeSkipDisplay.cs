using System;
using UnityEngine;

public class TimeSkipDisplay : MonoBehaviour
{

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
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }
}
