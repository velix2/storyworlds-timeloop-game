using System;
using UnityEngine;

public class LarryNoteHider : MonoBehaviour
{
    [SerializeField] private GameObject note;
    private void Start()
    {
        Check();
    }

    private void Check()
    {
        print(StateTracker.IntroState);
        note.SetActive(StateTracker.IntroState >= StateTracker.IntroStates.SonCallCompleted);
    }
}
